using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
	private const float DISTANCE_SQR_SPAWN_LIMITATION = 225f;

	private const float DISTANCE_SQR_RAYCAST_LIMITATION = 2025f;

	private const float START_HEIGHT = 1f;

	protected Timer sendingTimer = new Timer();

	protected Timer sendingShootDirTimer = new Timer();

	protected NetworkManager networkMgr;

	protected NetworkTransform lastTrans = new NetworkTransform(Vector3.zero, 0f, 0, false);

	protected WeaponType lastWT = WeaponType.AssaultRifle;

	protected Timer hpRecoveryTimer = new Timer();

	protected Timer uploadScoreTimer = new Timer();

	protected Timer attackShiledTimer = new Timer();

	private GameUnitState[] mBossSkillStates = new GameUnitState[7];

	private int pvpNextRebirthLoadoutID;

	private List<BlackHole> blackholeList = new List<BlackHole>();

	private bool mNeedFocusRevengePlayer;

	private float mStartFocusRevengePlayerTime;

	private float mMaxFocusRevengePlayerTime;

	public Transform InstructionObject;

	public Transform InstructionTarget;

	public int DeadWithoutKillNum;

	public int RevengePlayerId;

	public bool InEnemyPoint;

	protected Timer cheatCheckTimer = new Timer();

	private Player mNeighborLongSwordRider;

	public bool IsCoopRebirthTimerBlock { get; set; }

	public LocalPlayer()
	{
		userState = GameApp.GetInstance().GetUserState();
		base.GameUnitType = EGameUnitType.LOCAL_PLAYER;
	}

	public int GetPVPNextRebirthLoadoutID()
	{
		return pvpNextRebirthLoadoutID;
	}

	public void SetPVPNextRebirthLoadoutID(int id)
	{
		pvpNextRebirthLoadoutID = id;
	}

	public override void Init()
	{
		if (!userState.bInit)
		{
			GameApp.GetInstance().LoadGameDataIllegalWay();
		}
		InitSkills();
		base.Init();
		base.inputController.Init();
		mCameraVibrateController.Init();
		sendingTimer.SetTimer(0.2f, false);
		sendingShootDirTimer.SetTimer(0.5f, false);
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		SetChannelID(Lobby.GetInstance().GetChannelID());
		Transform transform = entityTransform.Find(BoneName.Bag).Find("Bag");
		if (transform != null)
		{
			transform.localPosition = Vector3.zero;
		}
		speedBooth = playerSkill.GetSkill(SkillsType.SPEED_BOOTH);
		hpRecoveryTimer.SetTimer(1f, false);
		uploadScoreTimer.SetTimer(30f, false);
		attackShiledTimer.SetTimer(0.25f, false);
		cheatCheckTimer.SetTimer(15f, false);
		if (ShadowCamera.GetInstance() != null)
		{
			ShadowCamera.GetInstance().target = GetTransform();
		}
		SocialPlayer = SocialManager.GetInstance().LocalSocialPlayer;
		if (SocialPlayer != null)
		{
			SocialManager.Log(string.Concat("LocalPlayer::Init: ", SocialPlayer.friendType, " ", SocialPlayer.GetSocialID()));
		}
		foreach (Weapon weapon in weaponList)
		{
			weapon.CheatCheck();
			weapon.CheatCheckParam();
		}
		mNeighborLongSwordRider = null;
	}

	public override void ChangeWeaponInBag(int bagIndex)
	{
		Weapon weaponInBag = userState.GetWeaponInBag(bagIndex);
		Weapon weapon = base.weapon;
		if (weaponInBag != weapon)
		{
			if (weaponInBag != null)
			{
				ChangeWeapon(weaponInBag);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					PlayerChangeWeaponRequest request = new PlayerChangeWeaponRequest((byte)bagIndex);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			ChangeWeapon(weaponInBag);
		}
		else if (weaponInBag != null)
		{
			weaponInBag.GunOn();
		}
	}

	public override void Loop(float deltaTime)
	{
		if (entityObject != null && entityObject.transform.childCount == 0)
		{
			return;
		}
		base.Loop(deltaTime);
		base.inputController.Process();
		mCameraVibrateController.Process();
		if (weapon.GetWeaponType() != lastWT)
		{
			lastWT = weapon.GetWeaponType();
		}
		TrySendInput();
		playerSkill.UpdateEnemyNearPerk();
		if (IsBoss)
		{
			float num = PlayerBossParameter.BossRotateSpeed * Time.deltaTime;
			float x = Mathf.Clamp(base.inputController.CameraRotation.x, 0f - num, num);
			float y = Mathf.Clamp(base.inputController.CameraRotation.y, 0f - num, num);
			base.inputController.CameraRotation = new Vector2(x, y);
			mBossState.Update(this);
			if (mBossState != Player.BOSS_DEAD_STATE && mBossState != Player.BOSS_CHOOSE_SKILL_STATE && mBossTouchTimer.Ready() && !isInBossWinState)
			{
				mBossTouchTimer.Do();
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (item.InPlayingState() && Vector3.SqrMagnitude(GetPosition() - item.GetPosition()) < mBossTouchRadiusSqr && item.canBePlayerBossTouch())
					{
						CheckKnocked(0.1f, item);
						DamageProperty damageProperty = new DamageProperty(this, 0);
						damageProperty.weaponid = 0;
						damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
						if (!item.IsInAnyShields(damageProperty))
						{
							PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, item.GetChannelID());
							GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						}
					}
				}
			}
		}
		else
		{
			base.State.NextState(this, deltaTime);
		}
		if (blackholeList.Count != 0)
		{
			GetAttract();
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && InPlayingState())
		{
			SendTransform();
		}
		if (!GameApp.GetInstance().GetGameMode().IsSingle() && GameApp.GetInstance().GetGameWorld() != null)
		{
			if (mNeighborLongSwordRider == null)
			{
				foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
				{
					if (remotePlayer != null && remotePlayer.mVehicle != null && remotePlayer.mVehicle.VehicleType == EVehicleType.LONG_SWORD)
					{
						VehicleLongSword vehicleLongSword = remotePlayer.mVehicle as VehicleLongSword;
						if (vehicleLongSword.CanGetOn() && vehicleLongSword.GetSubRider() == null && remotePlayer.IsFriendForce(this) && mVehicle == null && Vector3.Distance(remotePlayer.GetPosition(), GetPosition()) < 5f)
						{
							mNeighborLongSwordRider = remotePlayer;
							break;
						}
					}
				}
			}
			else if (mNeighborLongSwordRider.mVehicle == null || mNeighborLongSwordRider.mVehicle.VehicleType != EVehicleType.LONG_SWORD || Vector3.Distance(mNeighborLongSwordRider.GetPosition(), GetPosition()) >= 5f)
			{
				mNeighborLongSwordRider = null;
			}
		}
		playerSkill.PlayerHpRateTrigger((float)Hp / (float)MaxHp);
		CheckHPRecovery();
		if (InPlayingState() && uploadScoreTimer.Ready())
		{
			UploadStatistaic();
			uploadScoreTimer.Do();
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			SendShootDir();
		}
		ThirdPersonStandardCameraScript camera = GameApp.GetInstance().GetGameWorld().GetCamera();
		if (camera != null)
		{
			base.AngleV = camera.AngelV;
		}
		mTotalMoveDistance += (GetTransform().position - mLastPosition).magnitude;
		if (base.isCapturingFlag)
		{
			moveDistanceWithFlag += (GetTransform().position - mLastPosition).magnitude;
		}
		mLastPosition = GetTransform().position;
		if (mNeedFocusRevengePlayer && Time.time - mStartFocusRevengePlayerTime > mMaxFocusRevengePlayerTime)
		{
			mNeedFocusRevengePlayer = false;
		}
		if (cheatCheckTimer.Ready())
		{
			cheatCheckTimer.Do();
			foreach (Weapon weapon in weaponList)
			{
				weapon.CheatCheck();
				weapon.CheatCheckParam();
			}
		}
		if (InstructionObject != null && InstructionTarget != null)
		{
			InstructionObject.LookAt(InstructionTarget);
		}
	}

	public void CheckHPRecovery()
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle() && (playerSkill.HasSkill(SkillsType.HP_AUTO_RECOVERY) || playerSkill.HasSkill(SkillsType.HP_AUTO_PERCENT_RECOVERY)) && hpRecoveryTimer.Ready() && InPlayingState())
		{
			int num = (int)playerSkill.GetSkill(SkillsType.HP_AUTO_RECOVERY);
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				num = (int)VSMath.GetAutoRecoverInVS(num);
			}
			float recoveryAmount = (float)num + playerSkill.GetSkill(SkillsType.HP_AUTO_PERCENT_RECOVERY) * (float)MaxHp;
			RecoverHP(recoveryAmount);
			hpRecoveryTimer.Do();
		}
	}

	public void TrySendInput()
	{
		InputInfo inputInfo = base.inputController.inputInfo;
		InputInfo previousInputInfo = base.inputController.previousInputInfo;
		bool fire = inputInfo.fire;
		if (inputInfo.IsMoving() != previousInputInfo.IsMoving() || fire != previousInputInfo.fire)
		{
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				SendPlayerInputRequest request = new SendPlayerInputRequest(fire, inputInfo.IsMoving());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			if (fire != previousInputInfo.fire)
			{
				int changeType = (fire ? 1 : 0);
				playerSkill.PlayerInputControllerChange(changeType);
			}
			previousInputInfo.moveDirection = inputInfo.moveDirection;
			previousInputInfo.fire = fire;
		}
	}

	public void SendTransform()
	{
		if (!sendingTimer.Ready() || !TimeManager.GetInstance().NetworkTimeSynchronized)
		{
			return;
		}
		if (lastTrans.Pos != entityTransform.position || lastTrans.Angle != entityTransform.eulerAngles.y)
		{
			SendTransformStateRequest request = ((mVehicleState == EPlayerVehicleState.NONE) ? new SendTransformStateRequest(entityTransform.position, entityTransform.eulerAngles) : ((mVehicleState == EPlayerVehicleState.CARRIER) ? new SendTransformStateRequest(mVehicle.GetTransform().position, mVehicle.GetTransform().eulerAngles) : ((mVehicleState != EPlayerVehicleState.CANNON) ? null : ((mVehicle == null) ? null : ((mVehicle.VehicleType != EVehicleType.LONG_SWORD_CANNON) ? new SendTransformStateRequest(mVehicle.GetPosition(), mVehicle.GetRotationTransform().eulerAngles) : new SendTransformStateRequest(mVehicle.GetRotationTransform().position, mVehicle.GetRotationTransform().eulerAngles))))));
			networkMgr.SendRequest(request);
			lastTrans.Pos = entityTransform.position;
			if (mVehicle != null && mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
			{
				lastTrans.Angle = mVehicle.GetRotationTransform().rotation.eulerAngles.y;
			}
			else
			{
				lastTrans.Angle = entityTransform.eulerAngles.y;
			}
		}
		sendingTimer.Do();
	}

	public void SendShootDir()
	{
		if (sendingShootDirTimer.Ready())
		{
			InputInfo inputInfo = base.inputController.inputInfo;
			if (inputInfo.fire)
			{
				short angleV = (short)GameApp.GetInstance().GetGameWorld().GetCamera()
					.AngelV;
				SendPlayerShootAngleVRequest request = new SendPlayerShootAngleVRequest(angleV);
				networkMgr.SendRequest(request);
			}
			sendingShootDirTimer.Do();
		}
	}

	public override bool IsLocal()
	{
		return true;
	}

	public override void OnDead()
	{
		base.OnDead();
		Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
		transform.localScale = new Vector3(3f * (float)Screen.width / (float)Screen.height, 3f, 1f);
		transform.GetComponent<Renderer>().enabled = true;
		userState.Achievement.AddDeathNum(GameApp.GetInstance().GetGameMode().IsVSMode());
	}

	public override bool CheckLose()
	{
		return !IsCoopRebirthTimerBlock && rebirthTimer.Ready();
	}

	public override bool StartWaitRebirth()
	{
		rebirthTimer.SetTimer(5f, false);
		IsCoopRebirthTimerBlock = false;
		return true;
	}

	public override bool WinAnimationCompleted()
	{
		return winAnimationTimer.Ready();
	}

	public void SendWaitVSRebirthRequest()
	{
		if (!base.SendingWaitVSRebirthRequest)
		{
			base.SendingWaitVSRebirthRequest = true;
			PlayerWaitVSRebirtRequest request = new PlayerWaitVSRebirtRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	private int GetRespawnPointIndex()
	{
		if (GetSkills().HasSkill(SkillsType.CHOOSE_SPAWN_POINT) && mNextRebirthIndex != -1)
		{
			int result = mNextRebirthIndex;
			mNextRebirthIndex = -1;
			return result;
		}
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		float num4 = 0f;
		float num5 = 0f;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.RESPAWN);
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		int num6 = Random.Range(0, array.Length);
		List<GameObject> list = new List<GameObject>();
		for (int i = num6; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		for (int j = 0; j < num6; j++)
		{
			list.Add(array[j]);
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!list[k].name.StartsWith(ObjectNamePrefix.PLAYER_SPAWN_POINT))
			{
				continue;
			}
			Vector3 vector = list[k].transform.position + new Vector3(0f, 1f, 0f);
			float num7 = 9999f;
			bool flag = false;
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item == null || !item.InPlayingState() || IsSameTeam(item))
				{
					continue;
				}
				Vector3 vector2 = item.GetTransform().position + new Vector3(0f, 1f, 0f);
				Vector3 direction = vector2 - vector;
				float sqrMagnitude = new Vector3(direction.x, 0f, direction.z).sqrMagnitude;
				if (sqrMagnitude < num7)
				{
					num7 = sqrMagnitude;
				}
				if (!flag && sqrMagnitude < 2025f)
				{
					Ray ray = new Ray(vector, direction);
					float magnitude = direction.magnitude;
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, magnitude, (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.FLOOR)))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				if (num7 > num4)
				{
					num2 = k;
					num4 = num7;
				}
				continue;
			}
			if (num7 < 225f)
			{
				if (num7 > num5)
				{
					num3 = k;
					num5 = num7;
				}
				continue;
			}
			num = k;
			break;
		}
		if (num == -1)
		{
			num = num3;
		}
		if (num == -1)
		{
			num = num2;
		}
		if (num == -1)
		{
			num = 0;
		}
		return (num + num6) % array.Length;
	}

	public void SendRebirthRequest()
	{
		if (base.SendingRebirthRequest)
		{
			return;
		}
		PlayerRebirthRequest request = new PlayerRebirthRequest((byte)GetRespawnPointIndex(), (byte)pvpNextRebirthLoadoutID);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		base.SendingRebirthRequest = true;
		Debug.Log("send rebirth:" + base.State);
		GameObject gameObject = GameObject.Find("GameUI");
		if (gameObject != null)
		{
			InGameUIScript component = gameObject.GetComponent<InGameUIScript>();
			if (component != null)
			{
				component.WaitVSRebirthEnd();
			}
		}
	}

	public void SendOnlineRebirthRequest(int pvpLoadoutID)
	{
		if (!base.SendingRebirthRequest)
		{
			PlayerRebirthRequest playerRebirthRequest = null;
			playerRebirthRequest = ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? new PlayerRebirthRequest(0, (byte)pvpLoadoutID) : new PlayerRebirthRequest((byte)GetRespawnPointIndex(), (byte)pvpLoadoutID));
			GameApp.GetInstance().GetNetworkManager().SendRequest(playerRebirthRequest);
			base.SendingRebirthRequest = true;
		}
	}

	public void SetLastHitTarget(GameUnit target)
	{
		LastHitTarget = target;
		LastHitTargetTime = Time.time;
	}

	public override void RefreshAvatar(EnemyType type)
	{
		base.RefreshAvatar(type);
		entityObject.layer = PhysicsLayer.BOSS;
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		component.Init();
		component.pivotPosition = PlayerBossParameter.CameraPivotPosition[BossIndex];
		if (UIRadar.mInstance != null)
		{
			UIRadar.mInstance.Reset();
		}
		for (int i = 0; i < mBossSkillStates.Length; i++)
		{
			mBossSkillStates[i] = null;
		}
		Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
		if (transform != null)
		{
			transform.GetComponent<Renderer>().enabled = false;
		}
		switch (type)
		{
		case EnemyType.DEAD_EYE:
			mBossSkillStates[0] = Player.DEADEYE_MELEE_ATTACK_STATE;
			mBossSkillStates[1] = Player.DEADEYE_X_LASER_STATE;
			mBossSkillStates[2] = Player.DEADEYE_4_LASER_STATE;
			mBossSkillStates[3] = Player.DEADEYE_SHOULDER_LASER_STATE;
			mBossSkillStates[4] = Player.DEADEYE_RUSH_ATTACK_STATE;
			break;
		case EnemyType.SLASHER:
			mBossSkillStates[0] = Player.SLASHER_WEAPON_ATTACK_STATE01;
			mBossSkillStates[1] = Player.SLASHER_WEAPON_ATTACK_STATE02;
			mBossSkillStates[2] = Player.SLASHER_WEAPON_RANGE_STATE;
			mBossSkillStates[3] = Player.SLASHER_WEAPON_DASH_STATE;
			mBossSkillStates[4] = Player.SLASHER_WEAPON_WAVE_STATE;
			mBossSkillStates[5] = Player.SLASHER_WEAPON_BLADE_STATE;
			break;
		case EnemyType.FROST_STONE:
			mBossSkillStates[0] = Player.FROSTSTONE_MELEE_ATTACK_STATE1;
			mBossSkillStates[1] = Player.FROSTSTONE_MELEE_ATTACK_STATE2;
			mBossSkillStates[2] = Player.FROSTSTONE_SMALL_SNOW_BALL_STATE;
			mBossSkillStates[3] = Player.FROSTSTONE_BIG_SNOW_BALL_STATE;
			mBossSkillStates[4] = Player.FROSTSTONE_JUMP_ATTACK_STATE;
			mBossSkillStates[5] = Player.FROSTSTONE_DASH_ATTACK_STATE;
			break;
		case EnemyType.CERBERLISK:
			mBossSkillStates[0] = Player.CERBERLISK_MELEE_ATTACK_STATE1;
			mBossSkillStates[1] = Player.CERBERLISK_MELEE_ATTACK_STATE2;
			mBossSkillStates[2] = Player.CERBERLISK_THREE_HIT_ATTACK_STATE;
			mBossSkillStates[3] = Player.CERBERLISK_THREE_FIRE_BALL_STATE;
			mBossSkillStates[4] = Player.CERBERLISK_BIG_FIRE_BALL_STATE;
			mBossSkillStates[5] = Player.CERBERLISK_BREATHE_IN_FIRE_BALL_STATE;
			mBossSkillStates[6] = Player.CERBERLISK_DASH_ATTACK_STATE;
			break;
		default:
			Debug.LogError("NoBossSkill");
			break;
		}
	}

	public override void ResetAttack(CreatureSkill skill)
	{
		if (!IsLocal())
		{
			return;
		}
		base.ResetAttack(skill);
		mCheckHitList.Clear();
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			mCheckHitList.Add(item, new CountTimer(skill.Interval));
		}
	}

	public override void OnHitTarget(GameUnit target, CreatureSkill skill, int Count)
	{
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			RemotePlayer remotePlayer = target as RemotePlayer;
			DamageProperty damageProperty = new DamageProperty(this, 0);
			damageProperty.weaponid = skill.DamageType;
			damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
			if (!remotePlayer.IsInAnyShields(damageProperty))
			{
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, remotePlayer.GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public override void CheckKnocked(float speed, GameUnit target)
	{
		if (target.GameUnitType == EGameUnitType.REMOTE_PLAYER)
		{
			RemotePlayer remotePlayer = target as RemotePlayer;
			Vector3 vector = new Vector3(target.GetTransform().forward.x, 0f, target.GetTransform().forward.z);
			Vector3 to = entityTransform.position - target.GetTransform().position;
			to.y = 0f;
			float f = Vector3.Angle(vector, to);
			if (Mathf.Abs(f) < 90f)
			{
				speed = 0f - speed;
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerOnKnockedRequest request = new PlayerOnKnockedRequest(speed, ((Player)target).GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public void OnCastBossActiveBuffer(int skillId)
	{
		if (!IsBoss)
		{
			return;
		}
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		GiantBetrayalModeLogic giantBetrayalModeLogic = gameWorld.VsBattleLogic as GiantBetrayalModeLogic;
		if (giantBetrayalModeLogic == null)
		{
			return;
		}
		giantBetrayalModeLogic.PlayerBossActiveBuff(skillId, playerSkill);
		Joystick[] skillJoysticks = HUDBase.instance.m_BossStickmanager.m_SkillJoysticks;
		Joystick[] array = skillJoysticks;
		foreach (Joystick joystick in array)
		{
			if (joystick != null)
			{
				SkillStick component = joystick.gameObject.GetComponent<SkillStick>();
				if (component.ID == skillId)
				{
					component.startTime = Time.time;
					component.mEffectTime = giantBetrayalModeLogic.getActiveBufferEffectTime(skillId);
				}
			}
		}
	}

	public void TryCastBossActiveBuffer(int skillId)
	{
		if (!isInBossWinState)
		{
			PlayerBossActiveBuffRequest request = new PlayerBossActiveBuffRequest((short)skillId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void OnCastBossSkill(int index)
	{
		if (!isInBossWinState && index <= mBossSkillStates.Length && mBossState == Player.BOSS_IDLE_STATE && mBossSkillStates[index - 1] != null)
		{
			SetState(mBossSkillStates[index - 1]);
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && IsLocal())
			{
				PlayerBossStateRequest request = new PlayerBossStateRequest(mBossState);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public void GetAttract()
	{
		if (!InPlayingState() || (mSwVoid != null && mSwVoid.mType == ESwVoidType.SCOUT && mSwVoid.GetState() == SwVoidScout.DASH_STATE))
		{
			return;
		}
		List<BlackHole> list = new List<BlackHole>();
		foreach (BlackHole blackhole in blackholeList)
		{
			if (Time.time - blackhole.BlackHoleStartTime >= blackhole.BlackHoleDuringTime || (blackhole.owner != null && blackhole.owner.mSwVoid != null && blackhole.owner.mSwVoid.mType == ESwVoidType.HEAVY && blackhole.owner.mSwVoid.GetState() != SwVoidHeavy.SHIELD_STATE))
			{
				list.Add(blackhole);
				continue;
			}
			Vector3 vector = new Vector3(blackhole.x, GetPosition().y, blackhole.z);
			float num = Vector3.Distance(GetPosition(), vector);
			if (blackhole.isAttract)
			{
				if (num <= blackhole.radius && UnitCanBeHitByRay(vector + Vector3.up, num))
				{
					Vector3 vector2 = (1f - num / blackhole.radius) * blackhole.speed * Vector3.Normalize(vector - GetPosition());
					SimpleMove(vector2);
				}
			}
			else if (num <= blackhole.radius && UnitCanBeHitByRay(vector + Vector3.up, num))
			{
				Vector3 vector3 = blackhole.speed * Vector3.Normalize(GetPosition() - vector);
				SimpleMove(vector3);
			}
		}
		foreach (BlackHole item in list)
		{
			blackholeList.Remove(item);
		}
	}

	public void AddBlackHole(BlackHole blackhole)
	{
		blackholeList.Add(blackhole);
	}

	public void ChangeToBoss(EnemyType type)
	{
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		Vector3 vector = component.ReticlePosition;
		RefreshAvatar(type);
		component.ReticlePosition = vector;
	}

	public void TriggerRevengeRadar(float maxTime)
	{
		mNeedFocusRevengePlayer = true;
		mStartFocusRevengePlayerTime = Time.time;
		mMaxFocusRevengePlayerTime = maxTime;
	}

	public void StopRevengeRadar()
	{
		mNeedFocusRevengePlayer = false;
	}

	public int GetRevengePlayerId()
	{
		if (mNeedFocusRevengePlayer)
		{
			return RevengePlayerId;
		}
		return 0;
	}

	public override void OnPickUp(LootType lootType, float param1, float param2)
	{
		base.OnPickUp(lootType, param1, param2);
		IncreasePickNum(lootType);
		IncreasePickNum(LootType.NONE);
		int num = -1;
		switch (lootType)
		{
		case LootType.MONEY:
		{
			AddPickupCash((int)param1);
			int num2 = Random.Range(0, 100);
			if (num2 < 50)
			{
				AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/pickup_money01");
			}
			else
			{
				AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/pickup_money02");
			}
			num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.CASH_LOOT);
			break;
		}
		case LootType.VEHICLE:
		{
			AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/pickup_energy");
			num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.ENERGY_LOOT);
			EVehicleType type = (EVehicleType)param1;
			if (CanSpawnVehicle())
			{
				GameApp.GetInstance().GetGameWorld().SpawnVehicle(this, type);
			}
			break;
		}
		case LootType.PICKUP:
		{
			AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/pickup_energy");
			mBattlePickupManager.PickUp((byte)param1);
			float skill = playerSkill.GetSkill(SkillsType.SHARE_RATE);
			if (skill > 0f)
			{
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (IsFriendForce(item) && item.InPlayingState() && BattlePickup.GetPickupType((int)param1) == BattlePickup.EPickupType.PICKUP)
					{
						item.mBattlePickupManager.SharePickUp((byte)param1, skill);
					}
				}
			}
			if (param1 <= 2f)
			{
				num = GameApp.GetInstance().GetGameWorld().GetLootEffectIndex(EffectName.HP_LOOT);
			}
			GetUserState().Achievement.CheckAchievement_PickUp((int)param1);
			break;
		}
		}
		if (num != -1)
		{
			GameApp.GetInstance().GetGameWorld().SpawnLootEffect(num, entityTransform.position, entityTransform);
		}
		if (base.VSStatistics != null && UserStateHUD.GetInstance() != null && lootType == LootType.PICKUP && GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
			base.VSStatistics.ShowPickUpInfo(infoBox, (int)param1);
		}
		else if (lootType == LootType.PICKUP)
		{
			UserStateHUD.InfoBoxHUD infoBox2 = UserStateHUD.GetInstance().InfoBox;
			infoBox2.SetPrivateAchieveWithoutNum(LocalizationManager.GetInstance().GetString("LOC_PICKUP_DESC_" + param1));
		}
	}

	public Player GetNeighborLongSwordRider()
	{
		return mNeighborLongSwordRider;
	}
}
