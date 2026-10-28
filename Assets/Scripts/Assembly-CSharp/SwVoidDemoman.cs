using System.Collections.Generic;
using UnityEngine;

public class SwVoidDemoman : SwVoid
{
	public enum EExplodeState
	{
		DASH_TO_EXPLODE = 0,
		WAIT_EXPLODE = 1,
		EXPLODE_END = 2
	}

	public static SwVoidState Explode_STATE = new SwVoidExplodeState();

	private Timer skillTimer;

	private Timer explodeTimer;

	private Timer readyExplodeTimer;

	private Vector3 explodePosition;

	private int damage;

	protected ThirdPersonStandardCameraScript gameCamera;

	protected Camera cameraComponent;

	protected Vector3 beginPosition;

	protected EExplodeState mExplodeState;

	protected Vector3 dashSpeed;

	protected Vector3 baseDashSpeed;

	protected float explodeRadius;

	protected HashSet<PenerateHitInfo> penetratingTargets = new HashSet<PenerateHitInfo>();

	protected SwVoidExplodeCollider mExplodeCollider;

	protected PlayerDamageType mDamageType;

	private bool isFloatUp;

	private AttractiveForce mforce;

	private GameObject explodeLine;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.DEMOMAN;
		skillTimer = new Timer();
		skillTimer.SetTimer(SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.CD), true);
		readyExplodeTimer = new Timer();
		readyExplodeTimer.SetTimer(SwVoidParameter.EXPLODE_READY_TIME, false);
		explodeTimer = new Timer();
		explodeTimer.SetTimer(SwVoidParameter.EXPLODE_TIME, false);
		explodeRadius = 7f;
		mDamageType = (PlayerDamageType)SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
		float currentDps = mPlayer.GetCurrentDps();
		damage = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2);
		damage += (int)(currentDps * SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3));
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		mExplodeState = EExplodeState.EXPLODE_END;
		if (entityObject.GetComponent<SwVoidExplodeCollider>() != null)
		{
			mExplodeCollider = entityObject.GetComponent<SwVoidExplodeCollider>();
			mExplodeCollider.enabled = false;
			mExplodeCollider.mSwVoid = this;
			mExplodeCollider.isLocal = isLocal;
		}
		Object original = Resources.Load(EffectName.WB_EXPLODELINE);
		explodeLine = Object.Instantiate(original) as GameObject;
		explodeLine.transform.parent = entityTransform.Find("VoidEntity");
		explodeLine.transform.localPosition = Vector3.zero;
		explodeLine.SetActive(false);
		if (IsHighLevel())
		{
			HighLevelInit();
		}
	}

	public void StartExplode()
	{
		if (isLocal)
		{
			explodePosition = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 15f));
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				SwVoidStateRequest request = new SwVoidStateRequest(ESwVoidStateID.DEMOMAN_EXPLODE, 0, 0, explodePosition);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			skillTimer.Do();
		}
		beginPosition = entityTransform.position;
		baseDashSpeed = explodePosition - beginPosition;
		dashSpeed = baseDashSpeed * 2f;
		explodeLine.SetActive(true);
		StopNavMesh();
		mExplodeState = EExplodeState.DASH_TO_EXPLODE;
		entityTransform.LookAt(explodePosition);
		PlayAnimation(AnimationString.SWVOID_DASH, WrapMode.Loop);
		readyExplodeTimer.Do();
		mExplodeCollider.enabled = true;
		if (mforce != null)
		{
			mforce.enabled = true;
		}
	}

	public void DoExplode()
	{
		switch (mExplodeState)
		{
		case EExplodeState.DASH_TO_EXPLODE:
			entityObject.transform.position += dashSpeed * Time.deltaTime;
			dashSpeed -= baseDashSpeed * Time.deltaTime * 2f;
			if (readyExplodeTimer.Ready())
			{
				ActiveExplode();
				PlayExplodeAnimation();
				mExplodeState = EExplodeState.WAIT_EXPLODE;
				explodeTimer.Do();
				PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.Once);
				AudioManager.GetInstance().PlaySoundAt(AudioName.VOID1_EXPLODE, entityObject, entityTransform.position);
			}
			break;
		case EExplodeState.WAIT_EXPLODE:
			if (mforce != null)
			{
				mforce.Attract();
			}
			if (explodeTimer.Ready())
			{
				mExplodeState = EExplodeState.EXPLODE_END;
				AudioManager.GetInstance().PlaySoundAt(AudioName.VOID1_EXPLODE_END, entityObject, entityTransform.position);
				ExplodeAttack();
			}
			break;
		case EExplodeState.EXPLODE_END:
			SetState(SwVoid.FLOATING_STATE);
			break;
		}
	}

	public void EndExplode()
	{
		if (mforce != null)
		{
			mforce.enabled = false;
		}
		explodeLine.SetActive(false);
	}

	private void ActiveExplode()
	{
	}

	private void Explode()
	{
		SetState(Explode_STATE);
	}

	private void ExplodeAttack()
	{
		if (isLocal)
		{
			DamageProperty damageProperty = new DamageProperty(this, damage);
			damageProperty.criticalAttack = true;
			damageProperty.isLocal = isLocal;
			damageProperty.wType = WeaponType.NoGun;
			damageProperty.damageType = mDamageType;
			damageProperty.mAttackerType = AttackerType.SW_VOID;
			damageProperty.isActiveSkill = true;
			damageProperty.buff += mPlayer.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
			if (damageProperty.damageType != PlayerDamageType.NORMAL)
			{
				SkillsType sType = (SkillsType)(damageProperty.damageType - 1 + 53);
				damageProperty.buff += mPlayer.GetSkills().GetSkill(sType);
			}
			BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile, this) as BulletProjectile;
			if (bulletProjectile != null)
			{
				bulletProjectile.ExplosionDetect(damageProperty, null, false, 80, penetratingTargets, entityTransform.position, explodeRadius, false, true);
			}
		}
	}

	public override void ActiveSkill()
	{
		base.ActiveSkill();
		Explode();
		if (isLocal && mPlayer != null)
		{
			mPlayer.GetSkills().SetLastActiveSkill(ESwVoidStateID.DEMOMAN_EXPLODE);
		}
	}

	public void PlayExplodeAnimation()
	{
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.WB_VOIDGRENADE).CreateObject(entityTransform.position, Vector3.zero, Quaternion.identity);
		if (gameObject != null && isFloatUp)
		{
			VoidExplodeScript component = entityTransform.GetComponent<VoidExplodeScript>();
			component.beginPos = gameObject.transform.position;
			component.endPos = component.beginPos + new Vector3(0f, 2.5f, 0f);
			component.StartFloat();
			component = gameObject.GetComponent<VoidExplodeScript>();
			component.beginPos = gameObject.transform.position;
			component.endPos = component.beginPos + new Vector3(0f, 2.5f, 0f);
			component.StartFloat();
		}
	}

	public override Timer GetTimer(int id)
	{
		if (id == 0)
		{
			return skillTimer;
		}
		return explodeTimer;
	}

	public override void InitSkillButton(StateSkill stateSkill)
	{
		stateSkill.gameObject.SetActive(true);
		stateSkill.SetSwVoid(this);
		stateSkill.SetSkillType(StateSkill.EStateSkillType.SW_VOID);
		stateSkill.SetCoolDownTime(GetTimer(0).GetInterval());
		stateSkill.SetEffectTime(GetTimer(1).GetInterval());
	}

	public override bool IsCoolDown()
	{
		return base.IsCoolDown() && skillTimer.Ready();
	}

	public void ColliderTouched(GameObject go)
	{
		if (go.layer == PhysicsLayer.FLOOR)
		{
			isFloatUp = true;
		}
		else
		{
			isFloatUp = false;
		}
		if (mExplodeState == EExplodeState.DASH_TO_EXPLODE && go.layer != PhysicsLayer.VOID_SHIELD)
		{
			ActiveExplode();
			PlayExplodeAnimation();
			mExplodeState = EExplodeState.WAIT_EXPLODE;
			PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.Once);
			AudioManager.GetInstance().PlaySoundAt(AudioName.VOID1_EXPLODE, entityObject, entityTransform.position);
			explodeTimer.Do();
		}
	}

	public void GetExplodeResponse(Vector3 position)
	{
		explodePosition = position;
		SetState(Explode_STATE);
	}

	public override void HighLevelInit()
	{
		if (entityObject.GetComponent<AttractiveForce>() == null)
		{
			mforce = entityObject.AddComponent<AttractiveForce>();
			mforce.radius = 10f;
			mforce.enemySpeed = 10f;
			mforce.playerSpeed = 6f;
			mforce.owner = mPlayer;
		}
	}

	public override string GetCenter()
	{
		return SwVoidParameter.DemomanCenterBone;
	}
}
