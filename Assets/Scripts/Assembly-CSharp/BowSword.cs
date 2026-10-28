using System.Collections.Generic;
using UnityEngine;

public class BowSword : Weapon
{
	public enum EInnerState
	{
		EBow = 0,
		ESword = 1
	}

	public enum EAttackType
	{
		Bow = 0,
		Sword_Face_Attack1 = 1,
		Sword_Face_Attack2 = 2,
		Sword_Ass_Attack1 = 3,
		Sword_Ass_Attack2 = 4
	}

	public const float MeleeAttackDamageFactor1 = 1.5f;

	public const float MeleeAttackDamageFactor2 = 1.8f;

	public const float MeleeAttackRadius1 = 3f;

	public const float MeleeHitRadius2 = 4f;

	public const float SwordAttackFrequency = 1.2f;

	public const float SpecialSwordAttackChance = 0.3f;

	public const float BULLET_SPEED = 100f;

	protected const float shootLastingTime = 0.5f;

	public float[] MeleeAttackTriggerTime1 = new float[2] { 0.25f, 0.5f };

	public float[] MeleeAttackTriggerTime2 = new float[2] { 0.19148937f, 0.5531915f };

	public float[] MeleeAttackSoundTimeNormal = new float[2] { 0.25f, 0.5f };

	public float[] MeleeAttackSoundTimeRotate = new float[2] { 0.04255319f, 0.38297874f };

	protected static int sbulletCount;

	private EInnerState mInnerState;

	private Transform gunfire2;

	private GameObject mBow;

	private GameObject mSword;

	private string mCurrentAttackAnimation = string.Empty;

	private bool mLastAnimIsNormalAttack;

	private float mPressTime;

	private List<GameUnit> mTargetList = new List<GameUnit>();

	private CreatureMeleeAttackSkill mSkill1 = new CreatureMeleeAttackSkill();

	private CreatureMeleeAttackSkill mSkill2 = new CreatureMeleeAttackSkill();

	private bool mLastFire;

	private float mLastPressTime;

	private float mLastAttackAnimationTime;

	private MeleeWeaponTrail mTrail;

	protected override float attackFrenquency
	{
		get
		{
			if (mInnerState == EInnerState.ESword)
			{
				return 1.2f;
			}
			return base.attackFrenquency;
		}
		set
		{
			base.attackFrenquency = value;
		}
	}

	public override int BulletCount
	{
		get
		{
			return sbulletCount;
		}
		set
		{
			sbulletCount = value;
		}
	}

	public override SkillsType BoothType
	{
		get
		{
			if (mInnerState == EInnerState.EBow)
			{
				return SkillsType.RPG_BOOTH;
			}
			return SkillsType.SWORD_BOOTH;
		}
	}

	public override SkillsType AccType
	{
		get
		{
			if (mInnerState == EInnerState.EBow)
			{
				return SkillsType.RPG_ACC;
			}
			return SkillsType.SWORD_ACC;
		}
	}

	public BowSword()
	{
		maxCapacity = 9999;
		base.IsSelectedForBattle = false;
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.BowSword;
	}

	public override string GetAnimationSuffix()
	{
		if (mInnerState == EInnerState.EBow)
		{
			return "LaserBow";
		}
		return "LaserSword";
	}

	public override string GetAnimationSuffixAlter()
	{
		if (mInnerState == EInnerState.EBow)
		{
			return "LaserBow";
		}
		return "LaserSword";
	}

	public override void AttackZoom(float deltaTime)
	{
		if (mInnerState == EInnerState.EBow)
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ZoomToRPG(deltaTime);
		}
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return false;
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.NONE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.NONE;
	}

	public override void Init(Player player)
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		cameraTransform = gameCamera.CameraTransform;
		base.player = player;
		hitForce = 0f;
		weaponBoneTrans = player.GetTransform().Find(BoneName.WeaponL);
		CreateGun();
		gun.transform.parent = weaponBoneTrans;
		gun.transform.localPosition = Vector3.zero;
		gun.transform.localRotation = Quaternion.identity;
		shootAudio = gun.GetComponent<AudioSource>();
		if (shootAudio == null)
		{
		}
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
		GunOff();
		gunfire = mBow.transform.Find("Point01");
		gunfire2 = mBow.transform.Find("Point02");
		SetWeaponShootSpeed();
		mSkill1.Damage = (int)(damage * 1.5f);
		mSkill1.Radius = 3f + player.GetSkills().GetSkill(SkillsType.SWORD_RADIUS_INCREASE);
		mSkill1.Angle = 180f;
		mSkill2.Damage = (int)(damage * 1.8f);
		mSkill2.HitRadius = 4f + player.GetSkills().GetSkill(SkillsType.SWORD_RADIUS_INCREASE);
		mInnerState = EInnerState.EBow;
		if (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.High)
		{
			MeleeWeaponTrail[] componentsInChildren = mSword.GetComponentsInChildren<MeleeWeaponTrail>(true);
			if (componentsInChildren.Length > 0)
			{
				mTrail = componentsInChildren[0];
				mTrail.Emit = false;
			}
		}
		else
		{
			Transform transform = mSword.transform.Find("W26_slash");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	public override void GunOn()
	{
		Renderer[] componentsInChildren = gun.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = true;
		}
		SetWeaponShootSpeed();
	}

	public override void GunOff()
	{
		if (gun != null)
		{
			Renderer[] componentsInChildren = gun.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
		StopFire();
		if ((bool)mTrail)
		{
			mTrail.Emit = false;
		}
	}

	public override void Loop(float deltaTime)
	{
		if (!player.IsLocal() || player.GameUnitType != EGameUnitType.LOCAL_PLAYER)
		{
			return;
		}
		InputController inputController = player.inputController;
		bool fire = inputController.inputInfo.fire;
		if (mInnerState == EInnerState.ESword)
		{
			if (player.IsPlayingAnimation(mCurrentAttackAnimation))
			{
				float legacyAnimationTime = player.GetLegacyAnimationTime(mCurrentAttackAnimation);
				if (mLastAnimIsNormalAttack)
				{
					float[] meleeAttackTriggerTime = MeleeAttackTriggerTime1;
					foreach (float num in meleeAttackTriggerTime)
					{
						if (mLastAttackAnimationTime <= num && num <= legacyAnimationTime)
						{
							CheckHit(mSkill1, 1);
						}
					}
					if (mLastAttackAnimationTime <= MeleeAttackSoundTimeNormal[0] && MeleeAttackSoundTimeNormal[0] <= legacyAnimationTime)
					{
						AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_LEFT, player.GetTransform().position);
					}
					if (mLastAttackAnimationTime <= MeleeAttackSoundTimeNormal[1] && MeleeAttackSoundTimeNormal[1] <= legacyAnimationTime)
					{
						AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_RIGHT, player.GetTransform().position);
					}
				}
				else
				{
					if (mLastAttackAnimationTime <= MeleeAttackTriggerTime2[0] && MeleeAttackTriggerTime2[0] <= legacyAnimationTime)
					{
						CheckHit(mSkill2, 1);
					}
					else if (mLastAttackAnimationTime <= MeleeAttackTriggerTime2[1] && MeleeAttackTriggerTime2[1] <= legacyAnimationTime)
					{
						CheckHit(mSkill2, 2);
					}
					if ((mLastAttackAnimationTime <= MeleeAttackSoundTimeRotate[0] || mLastAttackAnimationTime > legacyAnimationTime) && MeleeAttackSoundTimeRotate[0] <= legacyAnimationTime)
					{
						AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_ROTATE, player.GetTransform().position);
					}
					if (mLastAttackAnimationTime <= MeleeAttackSoundTimeRotate[1] && MeleeAttackSoundTimeRotate[1] <= legacyAnimationTime)
					{
						AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_ROTATE, player.GetTransform().position);
					}
				}
				mLastAttackAnimationTime = legacyAnimationTime;
			}
			else if ((bool)mTrail)
			{
				mTrail.Emit = false;
			}
			if (fire)
			{
				mPressTime += Time.deltaTime;
			}
			else
			{
				mPressTime = 0f;
			}
			if (mPressTime > 0.5f)
			{
				ChangeToBow();
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BowSwordChangeRequest request = new BowSwordChangeRequest(true);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (!mLastFire && fire)
		{
			if (Time.time - mLastPressTime <= 0.5f)
			{
				ChangeToSword();
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BowSwordChangeRequest request2 = new BowSwordChangeRequest(false);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
			}
			mLastPressTime = Time.time;
		}
		mLastFire = fire;
	}

	public override void StopFire()
	{
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		attacked = false;
		if (mInnerState == EInnerState.EBow)
		{
			GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
			mBow.transform.Find(BoneName.BowString).gameObject.SetActive(false);
			mBow.transform.Find(BoneName.BowString).gameObject.SetActive(true);
			Vector3 direction;
			Ray weaponRay = GetWeaponRay(gunfire, out direction);
			RaycastHit hitInfo;
			if (Physics.Raycast(weaponRay, out hitInfo, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER)))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
			Fire(gunfire);
			Fire(gunfire2);
		}
		else
		{
			attacked = false;
			if ((bool)mTrail)
			{
				mTrail.SendMessage("Start");
				mTrail.Emit = true;
			}
		}
	}

	private void Fire(Transform point)
	{
		Vector3 normalized = (aimTarget - point.position).normalized;
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.BOW_ROCKET).CreateObject(point.position, normalized, Quaternion.identity);
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.isLocal = true;
		component.dir = normalized;
		component.flySpeed = bulletSpeed;
		component.explodeRadius = GetBombRange();
		component.hitForce = hitForce;
		component.life = 10f;
		component.damage = (int)damage;
		component.GunType = WeaponType.BowSword;
		component.targetPos = aimTarget;
		component.bagIndex = (byte)GameApp.GetInstance().GetUserState().GetWeaponBagIndex(this);
		component.weapon = this;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, point.position, normalized);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
		gunfire = weaponObj.transform.Find("Point01");
		gunfire2 = weaponObj.transform.Find("Point02");
		if (!(weaponObjL != null))
		{
			return;
		}
		if (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.High)
		{
			MeleeWeaponTrail[] componentsInChildren = weaponObjL.GetComponentsInChildren<MeleeWeaponTrail>(true);
			if (componentsInChildren.Length > 0)
			{
				mTrail = componentsInChildren[0];
			}
		}
		else
		{
			Transform transform = weaponObjL.transform.Find("W26_slash");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, avatar.transform.position);
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.BOW_ROCKET).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		gameObject.GetComponent<ProjectileScript>().enabled = false;
		gameObject.transform.localScale = Vector3.one * 0.5f;
		FireLineScript component = GameWorld.GetComponent<FireLineScript>(gameObject);
		component.enabled = true;
		component.beginPos = gunfire.position;
		component.endPos = gunfire.position + normalized * 100f;
		AutoDestroyScript component2 = GameWorld.GetComponent<AutoDestroyScript>(gameObject);
		component2.life = 1f;
		component2.enabled = true;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
		normalized = gunfire2.transform.forward.normalized;
		gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.BOW_ROCKET).CreateObject(gunfire2.position, Vector3.zero, gunfire2.rotation);
		gameObject.GetComponent<ProjectileScript>().enabled = false;
		gameObject.transform.localScale = Vector3.one * 0.5f;
		component = GameWorld.GetComponent<FireLineScript>(gameObject);
		component.enabled = true;
		component.beginPos = gunfire2.position;
		component.endPos = gunfire2.position + normalized * 100f;
		component2 = GameWorld.GetComponent<AutoDestroyScript>(gameObject);
		component2.life = 1f;
		component2.enabled = true;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
		gunObj.transform.Find(BoneName.BowString).gameObject.SetActive(false);
		gunObj.transform.Find(BoneName.BowString).gameObject.SetActive(true);
	}

	protected override void CreateGun()
	{
		string path = Global.WEAPON_PATH + "/" + GetPrefabLevel() + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", gunID);
		Object original = Resources.Load(path);
		mBow = (GameObject)Object.Instantiate(original);
		Object original2 = Resources.Load("Character/Weapon/" + GetPrefabLevel() + "/Sword");
		mSword = Object.Instantiate(original2) as GameObject;
		mSword.SetActive(false);
		gun = new GameObject();
		gun.name = "gun";
		mBow.transform.parent = gun.transform;
		mBow.transform.localPosition = Vector3.zero;
		mBow.transform.localRotation = Quaternion.identity;
		mSword.transform.parent = gun.transform;
		mSword.transform.localPosition = Vector3.zero;
		mSword.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
	}

	public void ChangeToBow()
	{
		if (!(mBow == null))
		{
			mBow.SetActive(true);
			mSword.SetActive(false);
			mInnerState = EInnerState.EBow;
			weaponBoneTrans = player.GetTransform().Find(BoneName.WeaponL);
			gun.transform.parent = weaponBoneTrans;
			gun.transform.localPosition = Vector3.zero;
			gun.transform.localRotation = Quaternion.identity;
			SetWeaponShootSpeed();
			if ((bool)mTrail)
			{
				mTrail.Emit = false;
			}
		}
	}

	public void ChangeToSword()
	{
		if ((bool)mTrail)
		{
			mTrail.Emit = true;
		}
		mPressTime = 0f;
		mBow.SetActive(false);
		mSword.SetActive(true);
		if ((bool)mTrail)
		{
			mTrail.SendMessage("Start");
		}
		mInnerState = EInnerState.ESword;
		weaponBoneTrans = player.GetTransform().Find(BoneName.Weapon);
		gun.transform.parent = weaponBoneTrans;
		gun.transform.localPosition = Vector3.zero;
		gun.transform.localRotation = Quaternion.identity;
		SetWeaponShootSpeed();
		InputController inputController = player.inputController;
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				player.PlayAnimationAllLayers(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), WrapMode.ClampForever);
				return;
			}
			player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
			player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), WrapMode.ClampForever);
		}
		else if (flag)
		{
			player.PlayAnimationAllLayers(AnimationString.FlyIdle, WrapMode.Loop);
			player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), WrapMode.ClampForever);
		}
		else
		{
			player.PlayAnimationAllLayers(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), WrapMode.ClampForever);
		}
	}

	public override void UpdateAttackState(float deltaTime)
	{
		if (mInnerState == EInnerState.EBow)
		{
			base.UpdateAttackState(deltaTime);
			return;
		}
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode mode = WrapMode.Loop;
		if (!IsTypeOfLoopShootingWeapon())
		{
			mode = WrapMode.ClampForever;
		}
		if (CoolDown())
		{
			if (player.IsLocal())
			{
				player.Attack();
			}
			else
			{
				CreateTrajectory();
			}
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
			}
			else
			{
				player.PlayAnimationAllLayers(GetAttackAnim(flag, inputController.inputInfo.IsMoving()), mode);
			}
		}
		else if (!IsTypeOfLoopShootingWeapon())
		{
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) || player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack + "01"))
					{
						player.ChangeToIdleState();
					}
					else
					{
						player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					}
					if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
					{
						player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
					{
						player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
				}
			}
			else if (flag)
			{
				if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
				{
					player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
			}
			else if (player.IsPlayingAnimation(mCurrentAttackAnimation) && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
		if (!inputController.inputInfo.fire && GetWaitForAttackAnimationStop() && player.AnimationPlayed(mCurrentAttackAnimation, 1f))
		{
			player.ChangeToIdleState();
			if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
	}

	public override void StartAttackState()
	{
		base.StartAttackState();
		mLastAnimIsNormalAttack = false;
		mLastFire = false;
	}

	private string GetAttackAnim(bool enableFly, bool isMoving)
	{
		string text = ((!enableFly) ? AnimationString.Stand : AnimationString.Fly);
		string text2 = ((!isMoving) ? AnimationString.Attack : AnimationString.RunAttack);
		if (mInnerState == EInnerState.ESword)
		{
			if (Random.value < 0.3f)
			{
				mCurrentAttackAnimation = text + "_" + GetAnimationSuffix() + "_" + text2 + "01";
				mLastAnimIsNormalAttack = false;
			}
			else
			{
				mCurrentAttackAnimation = text + "_" + GetAnimationSuffix() + "_" + text2;
				mLastAnimIsNormalAttack = true;
			}
		}
		else
		{
			mCurrentAttackAnimation = text + "_" + GetAnimationSuffix() + "_" + text2;
		}
		return mCurrentAttackAnimation;
	}

	private void CheckHit(CreatureMeleeAttackSkill skill, byte factor)
	{
		foreach (Enemy value in GameApp.GetInstance().GetGameWorld().GetEnemies()
			.Values)
		{
			if (value != null && value.InPlayingState())
			{
				Vector3 vector = player.GetTransform().InverseTransformDirection(value.GetTransform().position - player.GetPosition());
				float num = vector.magnitude - value.GetHitColliderRadius();
				if (num <= skill.HitRadius || (num <= skill.Radius && vector.z > 0f && Mathf.Abs(vector.z / vector.x) > Mathf.Tan(skill.AngleRad)))
				{
					DamageProperty damageProperty = CreateDamageProperty();
					damageProperty.damage = skill.Damage;
					value.HitEnemy(player, damageProperty);
					((LocalPlayer)player).SetLastHitTarget(value);
				}
			}
		}
		foreach (DestructibleObjectScript value2 in GameApp.GetInstance().GetGameWorld().GetDestructibleDic()
			.Values)
		{
			if (null != value2)
			{
				Vector3 vector2 = player.GetTransform().InverseTransformDirection(value2.transform.position - player.GetPosition());
				vector2.y = 0f;
				float magnitude = vector2.magnitude;
				if (magnitude <= skill.HitRadius || (magnitude <= skill.Radius && vector2.z > 0f && Mathf.Abs(vector2.z / vector2.x) > Mathf.Tan(skill.AngleRad)))
				{
					value2.OnHit(skill.Damage);
					AudioManager.GetInstance().PlaySoundAt(AudioName.SWORD_HIT, value2.transform.position);
				}
			}
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item == null || item.IsSameTeam(player) || !item.InPlayingState())
			{
				continue;
			}
			Vector3 vector3 = player.GetTransform().InverseTransformDirection(item.GetTransform().position - player.GetPosition());
			float num2 = vector3.magnitude - item.GetHitColliderRadius();
			if (!(num2 <= skill.HitRadius) && (!(num2 <= skill.Radius) || !(vector3.z > 0f) || !(Mathf.Abs(vector3.z / vector3.x) > Mathf.Tan(skill.AngleRad))))
			{
				continue;
			}
			DamageProperty damageProperty2 = CreateDamageProperty();
			float num3 = Vector3.Angle(-item.GetTransform().forward, player.GetPosition() - item.GetPosition());
			if (num3 >= 60f)
			{
				damageProperty2.damageFactor = factor;
			}
			else
			{
				damageProperty2.damageFactor = (byte)(factor + 2);
			}
			if (!item.IsInAnyShields(damageProperty2))
			{
				player.HitTargetShowCrossHit();
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty2, item.GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				if (item.InPlayingState())
				{
					player.GetSkills().PlayerHitTheTargetByDamageType(damageProperty2);
				}
				((LocalPlayer)player).SetLastHitTarget(item);
			}
		}
		List<ShieldScript> currentShields = gameWorld.GetCurrentShields();
		foreach (ShieldScript item2 in currentShields)
		{
			if (item2 != null && item2.shieldStarted && (!GameApp.GetInstance().GetGameMode().IsTeamMode() || !item2.ownedPlayer.IsSameTeam(player)))
			{
				Vector3 vector4 = player.GetTransform().InverseTransformDirection(item2.transform.position - player.GetPosition());
				vector4.y -= item2.shieldRadius;
				float num4 = vector4.magnitude - item2.shieldRadius;
				if (num4 <= skill.HitRadius || (num4 <= skill.Radius && vector4.z > 0f && Mathf.Abs(vector4.z / vector4.x) > Mathf.Tan(skill.AngleRad)))
				{
					DamageProperty damageProperty3 = CreateDamageProperty();
					damageProperty3.damageFactor = factor;
					item2.PlayerHitShield(damageProperty3);
				}
			}
		}
	}

	public override PlayerDamageType GetDamageType()
	{
		if (mInnerState == EInnerState.ESword)
		{
			return PlayerDamageType.MELEE;
		}
		return PlayerDamageType.ENERGY;
	}

	public void SetInnerState(EInnerState state)
	{
		mInnerState = state;
	}

	public EInnerState GetInnerState()
	{
		return mInnerState;
	}

	public override void EnableGunObject(bool bEnable)
	{
		if (!bEnable)
		{
			if (mInnerState == EInnerState.EBow)
			{
				mBow.SetActiveRecursively(false);
			}
			else
			{
				mSword.SetActiveRecursively(false);
			}
		}
		else if (mInnerState == EInnerState.EBow)
		{
			mBow.SetActiveRecursively(true);
		}
		else
		{
			mSword.SetActiveRecursively(true);
		}
	}

	public override void CheatCheck()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		float num = gameDataTable.GetFloat(rowID, 3);
		byte b = 0;
		if (base.AttackFrenquencyInit < num)
		{
			b++;
		}
		if (player != null && player.GetSkills() != null)
		{
			float accBuff = GetAccBuff();
			if (accBuff > 2.25f)
			{
				b += 2;
			}
		}
		if (player != null)
		{
			if (mInnerState == EInnerState.ESword)
			{
				if (attackFrenquency * 1.05f < 1.2f / (GetAccBuff() + 1f))
				{
					b += 4;
				}
			}
			else if (attackFrenquency * 1.05f < num / (GetAccBuff() + 1f))
			{
				b += 4;
			}
		}
		else if (mInnerState == EInnerState.ESword)
		{
			if (attackFrenquency < 1.2f)
			{
				b += 4;
			}
		}
		else if (attackFrenquency < num)
		{
			b += 4;
		}
		if (b != 0 && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request = new CheatCheckRequest(CheatType.WEAPON_SPEED_RATE, attackFrenquency, b, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		float obj = gameDataTable.GetFloat(rowID, 11);
		if (!GetSpeedDrag().Equals(obj) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request2 = new CheatCheckRequest(CheatType.PLAYER_SPEED, GetSpeedDrag(), 0, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}

	public override void StartIdleState()
	{
		base.StartIdleState();
		if ((bool)mTrail)
		{
			mTrail.Emit = false;
		}
	}

	public void UIShowTail()
	{
		if ((bool)mTrail)
		{
			mTrail.SendMessage("Start");
		}
	}
}
