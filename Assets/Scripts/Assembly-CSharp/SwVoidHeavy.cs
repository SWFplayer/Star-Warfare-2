using System.Collections.Generic;
using UnityEngine;

public class SwVoidHeavy : SwVoid
{
	public static SwVoidState SHIELD_STATE = new SwVoidShieldState();

	private Timer skillTimer;

	private Timer shieldTimer;

	public Vector3 shieldPosition;

	private GameObject shieldEntity;

	private int damageToDefend;

	private float currentShieldRadius;

	private float shieldRadius;

	private ShieldScript shieldScript;

	private int explodeDamage;

	protected PlayerDamageType mDamageType;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.HEAVY;
		skillTimer = new Timer(SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.CD), true);
		shieldTimer = new Timer(SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE4), false);
		shieldTimer.Pause();
		shieldRadius = SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3);
		Object original = Resources.Load(EffectName.VD6_SHIELD);
		shieldEntity = Object.Instantiate(original) as GameObject;
		shieldEntity.name = "Shield" + mPlayer.GetChannelID();
		shieldScript = shieldEntity.GetComponent<ShieldScript>();
		shieldScript.ownedPlayer = player;
		shieldScript.ownedSwVoid = this;
		GameApp.GetInstance().GetGameWorld().GetCurrentShields()
			.Add(shieldScript);
		shieldEntity.SetActive(false);
		shieldScript.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
	}

	public void ActiveShieldSkill()
	{
		shieldPosition = mPlayerTransform.position;
		if (mPlayer.IsLocal() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			SwVoidStateRequest request = new SwVoidStateRequest(ESwVoidStateID.HEAVY_SHIELD, 0, 0, shieldPosition);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		SetState(SHIELD_STATE);
	}

	public void StartShield()
	{
		PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.Loop);
		mNavMeshAgent.enabled = false;
		entityObject.GetComponent<Collider>().enabled = false;
		skillTimer.Do();
		shieldTimer.Pause();
		currentShieldRadius = 0f;
		if (IsHighLevel())
		{
			mDamageType = (PlayerDamageType)SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
			explodeDamage = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1) * mPlayer.MaxHp / 100;
		}
		damageToDefend = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2) * mPlayer.MaxHp / 100;
		shieldScript.shieldInited = false;
		shieldScript.shieldStarted = false;
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			shieldScript.InitShield(damageToDefend, shieldRadius, shieldTimer.GetInterval());
		}
		else
		{
			shieldScript.InitShield(0, shieldRadius, shieldTimer.GetInterval());
		}
	}

	public void DoShield()
	{
		if (shieldTimer.Ready() || (shieldScript.shieldStarted && shieldScript.shieldCurrentHP <= 0) || !mPlayer.InPlayingState())
		{
			SetState(SwVoid.FLOATING_STATE);
			return;
		}
		float num = Vector3.Distance(shieldPosition + Vector3.up * shieldRadius, entityTransform.position) * SwVoidParameter.HEAVE_VOID_DASH_RATE;
		entityTransform.position += (shieldPosition + Vector3.up * shieldRadius - entityTransform.position).normalized * num * Time.deltaTime;
		if (Vector3.Distance(entityTransform.position, shieldPosition + Vector3.up * shieldRadius) <= 3f && shieldScript.shieldInited)
		{
			if (!shieldScript.shieldStarted)
			{
				CreateShield();
				shieldTimer.Do();
				shieldScript.StartShield();
			}
			if (currentShieldRadius < shieldRadius)
			{
				currentShieldRadius += Time.deltaTime * SwVoidParameter.SHIELD_ENLARGE_RATE;
				shieldEntity.transform.localScale = Vector3.one * (currentShieldRadius / SwVoidParameter.SHIELD_DEFAULT_RADIUS);
			}
		}
	}

	public void EndShield()
	{
		if (IsHighLevel())
		{
			ExplodeAttack();
			Vector3 position = entityTransform.position;
			position.y -= shieldRadius;
			gameWorld.GetEffectPool(EffectPoolType.VOID_SHIELD_EXP).CreateObject(position + Vector3.up * 0.5f, Vector3.zero, Quaternion.identity);
			AudioManager.GetInstance().PlaySoundAt(AudioName.VOID1_EXPLODE_END, position);
		}
		mNavMeshAgent.enabled = true;
		entityObject.GetComponent<Collider>().enabled = true;
		shieldScript.shieldStarted = false;
		shieldEntity.SetActive(false);
	}

	public void CreateShield()
	{
		shieldEntity.transform.position = shieldPosition;
		shieldEntity.transform.localScale = Vector3.zero;
		shieldEntity.SetActive(true);
	}

	public override void InitSkillButton(StateSkill stateSkill)
	{
		stateSkill.gameObject.SetActive(true);
		stateSkill.SetSwVoid(this);
		stateSkill.SetSkillType(StateSkill.EStateSkillType.SW_VOID);
		stateSkill.SetCoolDownTime(GetTimer(0).GetInterval());
		stateSkill.SetEffectTime(GetTimer(1).GetInterval());
	}

	public override void ActiveSkill()
	{
		base.ActiveSkill();
		ActiveShieldSkill();
		if (isLocal && mPlayer != null)
		{
			mPlayer.GetSkills().SetLastActiveSkill(ESwVoidStateID.HEAVY_SHIELD);
		}
	}

	public override bool IsCoolDown()
	{
		return base.IsCoolDown() && skillTimer.Ready();
	}

	public override Timer GetTimer(int id)
	{
		if (id == 0)
		{
			return skillTimer;
		}
		return shieldTimer;
	}

	public bool IsShieldOn()
	{
		if (mState == SHIELD_STATE && currentShieldRadius >= shieldRadius)
		{
			return true;
		}
		return false;
	}

	public override string GetCenter()
	{
		return SwVoidParameter.HeavyCenterBone;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (GameApp.GetInstance().GetGameWorld() != null && shieldEntity != null)
		{
			GameApp.GetInstance().GetGameWorld().GetCurrentShields()
				.Remove(shieldEntity.GetComponent<ShieldScript>());
			Object.Destroy(shieldEntity);
		}
	}

	private void ExplodeAttack()
	{
		if (isLocal)
		{
			DamageProperty damageProperty = new DamageProperty(this, explodeDamage);
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
			Vector3 position = entityTransform.position;
			position.y -= shieldRadius;
			if (bulletProjectile != null)
			{
				bulletProjectile.ExplosionDetect(damageProperty, null, false, 80, new HashSet<PenerateHitInfo>(), position, SwVoidParameter.SHIELD_EXPLODE_RADIUS, false, true);
			}
		}
	}
}
