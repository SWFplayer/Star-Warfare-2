using System.Collections.Generic;
using UnityEngine;

public class SwVoidScout : SwVoid
{
	public static SwVoidState DASH_STATE = new SwVoidDashState();

	private float skillCoolDown = 10f;

	private float dashTime = 0.2f;

	private float lastActiveSkillTime;

	private Timer skillTimer;

	private Timer dashTimer;

	public Vector3 chargeTargetPosition;

	private float cameraRecoverTime;

	private bool waitCameraRecover;

	protected int damage;

	protected PlayerDamageType mDamageType;

	protected float mDashDistance;

	private List<Enemy> mDashEnemyList = new List<Enemy>();

	private List<RemotePlayer> mDashRemoteplayerList = new List<RemotePlayer>();

	private GameObject rushVFX;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.SCOUT;
		skillTimer = new Timer();
		skillTimer.SetTimer(SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.CD), true);
		dashTimer = new Timer();
		dashTimer.SetTimer(SwVoidParameter.READY_DASH_TIME, false);
		mDashDistance = SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE4);
		Object original = Resources.Load(EffectName.VD5_RUSH);
		rushVFX = Object.Instantiate(original, mPlayer.GetTransform().position + Vector3.up * 1.2f + Vector3.back * 0.3f, mPlayer.GetTransform().rotation) as GameObject;
		rushVFX.transform.parent = mPlayer.GetTransform();
		rushVFX.SetActive(false);
		if (IsHighLevel())
		{
			HighLevelInit();
		}
	}

	public void Dash()
	{
		Vector3 vector = new Vector3(mPlayer.inputController.inputInfo.moveDirection.x, 0f, mPlayer.inputController.inputInfo.moveDirection.z).normalized;
		if (!mPlayer.inputController.inputInfo.IsMoving())
		{
			vector = mPlayer.GetTransform().forward;
			vector.y = 0f;
		}
		chargeTargetPosition = mPlayerTransform.position + vector * mDashDistance;
		if (!GameApp.GetInstance().GetGameMode().IsSingle() && isLocal)
		{
			SwVoidStateRequest request = new SwVoidStateRequest(ESwVoidStateID.SCOUT_DASH, 0, 0, chargeTargetPosition);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		SetState(DASH_STATE);
	}

	public void StartDash()
	{
		StartNavMesh(chargeTargetPosition + Vector3.up * 1.2f, SwVoidParameter.VOID_DASH_SPEED);
		skillTimer.Do();
		dashTimer.Do();
		lastActiveSkillTime = Time.time;
		if (isLocal)
		{
			gameWorld.GetCamera().smoothSpeed = mPlayer.Speed;
		}
		PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.Once);
		rushVFX.SetActive(true);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.VD5_FLASH).CreateObject(mPlayer.GetTransform().position + Vector3.up * 1.2f, mPlayer.GetTransform().forward, mPlayer.GetTransform().rotation);
		gameObject.transform.localScale = Vector3.one;
		mPlayer.PlayerInvisible();
		AudioManager.GetInstance().PlaySoundAt(AudioName.VOID5_SCOUT, entityTransform.position);
		waitCameraRecover = false;
		mDashEnemyList = new List<Enemy>();
		mDashRemoteplayerList = new List<RemotePlayer>();
	}

	public void DoDash()
	{
		if (!waitCameraRecover)
		{
			Vector3 vector = chargeTargetPosition - mPlayerTransform.position;
			if (dashTimer.Ready())
			{
				if (IsHighLevel())
				{
					CheckHit();
				}
				if (mPlayer.IsLocal())
				{
					mPlayer.SimpleMoveWithGravity(vector * SwVoidParameter.PLAYER_DASH_SPEED_RATE, SwVoidParameter.VOID_DASH_GRAVITY);
				}
				else
				{
					mPlayer.SimpleMove(vector * SwVoidParameter.PLAYER_DASH_SPEED_RATE);
				}
			}
			if (Time.time - lastActiveSkillTime > SwVoidParameter.MAX_PLAYER_DASH_TIME || vector.magnitude < 0.5f)
			{
				GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.VD5_FLASH).CreateObject(mPlayer.GetPosition() + Vector3.up * 1.2f, mPlayer.GetTransform().forward, mPlayer.GetTransform().rotation);
				gameObject.transform.localScale = Vector3.one;
				mPlayer.PlayerAppear();
				waitCameraRecover = true;
				cameraRecoverTime = Time.time;
			}
			return;
		}
		DoIdleFloating(Time.deltaTime);
		if (isLocal)
		{
			gameWorld.GetCamera().smoothSpeed += Time.deltaTime * 90f;
		}
		if (Time.time - cameraRecoverTime > 0.5f)
		{
			if (isLocal)
			{
				gameWorld.GetCamera().smoothSpeed = 100f;
			}
			SetState(SwVoid.FLOATING_STATE);
		}
	}

	public void EndDash()
	{
		if (isLocal)
		{
			gameWorld.GetCamera().smoothSpeed = 100f;
		}
		rushVFX.SetActive(false);
		StopNavMesh();
	}

	public override bool IsCoolDown()
	{
		return base.IsCoolDown() && skillTimer.Ready();
	}

	public Timer GetTimer()
	{
		return skillTimer;
	}

	public override void InitSkillButton(StateSkill stateSkill)
	{
		stateSkill.gameObject.SetActive(true);
		stateSkill.SetSwVoid(this);
		stateSkill.SetSkillType(StateSkill.EStateSkillType.SW_VOID);
		stateSkill.SetCoolDownTime(GetTimer(0).GetInterval());
		stateSkill.SetEffectTime(GetTimer(1).GetInterval());
	}

	public override void Destroy()
	{
		if (mState == DASH_STATE && isLocal && gameWorld != null)
		{
			gameWorld.GetCamera().smoothSpeed = 100f;
		}
		base.Destroy();
	}

	public override void ActiveSkill()
	{
		base.ActiveSkill();
		Dash();
		if (isLocal && mPlayer != null)
		{
			mPlayer.GetSkills().SetLastActiveSkill(ESwVoidStateID.SCOUT_DASH);
		}
	}

	public override Timer GetTimer(int id)
	{
		if (id == 0)
		{
			return skillTimer;
		}
		return dashTimer;
	}

	public override void HighLevelInit()
	{
		damage = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2);
		damage += (int)(mPlayer.GetCurrentDps() * SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3));
		mDamageType = (PlayerDamageType)SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
	}

	private void CheckHit()
	{
		if (!isLocal)
		{
			return;
		}
		foreach (Enemy value in GameApp.GetInstance().GetGameWorld().GetEnemies()
			.Values)
		{
			if (value != null && value.InPlayingState() && !mDashEnemyList.Contains(value) && (value.GetTransform().position - mPlayer.GetPosition()).magnitude <= SwVoidParameter.DASH_ATTACK_RADIUS)
			{
				mDashEnemyList.Add(value);
				DamageProperty dp = CreateDamageProperty();
				value.HitEnemy(mPlayer, dp);
			}
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item != null && !mDashRemoteplayerList.Contains(item) && !item.IsSameTeam(mPlayer) && item.InPlayingState() && (item.GetTransform().position - mPlayer.GetPosition()).magnitude <= SwVoidParameter.DASH_ATTACK_RADIUS)
			{
				mDashRemoteplayerList.Add(item);
				DamageProperty dp2 = CreateDamageProperty();
				mPlayer.HitTargetShowCrossHit(true);
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(dp2, item.GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				mPlayer.GetSkills().PlayerHitTheTargetByDamageType(dp2);
			}
		}
	}

	private DamageProperty CreateDamageProperty()
	{
		DamageProperty damageProperty = new DamageProperty(this, damage);
		damageProperty.criticalAttack = false;
		damageProperty.isLocal = isLocal;
		damageProperty.wType = WeaponType.NoGun;
		damageProperty.damageType = mDamageType;
		damageProperty.buff += mPlayer.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
		if (damageProperty.damageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(damageProperty.damageType - 1 + 53);
			damageProperty.buff += mPlayer.GetSkills().GetSkill(sType);
		}
		damageProperty.mAttackerType = AttackerType.SW_VOID;
		damageProperty.isActiveSkill = true;
		return damageProperty;
	}

	public override string GetCenter()
	{
		return SwVoidParameter.ScoutCenterBone;
	}
}
