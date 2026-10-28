using System.Collections.Generic;
using UnityEngine;

public class SwVoidSoldier : SwVoid
{
	public static SwVoidState ATTACK_STATE = new SwVoidAttackState();

	protected float attackRange;

	protected float attackAngle;

	protected float checkRangeForward;

	protected float checkAngleForward;

	protected float totalAttackTime;

	protected float mAttackAnimationTime;

	protected float tempAnimationTime;

	protected float changeTargetTime;

	protected float playerMaxHitLastTime;

	protected float attackInterval;

	protected float lastAttackEndTime;

	protected float lastAttackStartTime;

	private GameObject gunFireObj;

	private GameObject fireLineObj;

	protected Transform gunfire;

	public bool isFirstRoundAttack = true;

	protected Vector3 attackPosition;

	protected PlayerDamageType mDamageType;

	protected int mDamage;

	protected float mDamageFactor;

	public override void init(GameObject gObject, Player player)
	{
		base.init(gObject, player);
		mType = ESwVoidType.SOLDIER;
		LoadAttackInfo();
		gunfire = GetTransform();
		Object original = Resources.Load(EffectName.VD1_SPARK);
		gunFireObj = (GameObject)Object.Instantiate(original, gunfire.position, Quaternion.identity);
		gunFireObj.transform.parent = gunfire;
		Object original2 = Resources.Load(EffectName.VD1_TRAJECTORY);
		fireLineObj = (GameObject)Object.Instantiate(original2, gunfire.position, Quaternion.identity);
		fireLineObj.transform.parent = gunfire;
		mDamageType = (PlayerDamageType)SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE1);
		mDamage = SwVoidDataManager.GetSwVoidParameterInt(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE2);
		mDamageFactor = SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE3);
		mDamage += (int)(mPlayer.GetCurrentDps() * mDamageFactor);
		mAttackAnimationTime = SwVoidDataManager.GetSwVoidParameterFloat(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.VALUE4);
		StopFire();
	}

	private void LoadAttackInfo()
	{
		attackRange = SwVoidParameter.ATTACK_RANGE;
		attackAngle = SwVoidParameter.ATTACK_ANGLE;
		checkRangeForward = SwVoidParameter.CHECK_RANGE_WHEN_PLAYER_ATTACK;
		checkAngleForward = SwVoidParameter.CHECK_ANGLE_WHEN_PLAYER_ATTACK;
		attackInterval = SwVoidParameter.ATTACK_INTERVAL;
		changeTargetTime = SwVoidParameter.CHANGE_TARGET_INTERVAL;
		totalAttackTime = SwVoidParameter.TOTAL_ATTACK_TIME;
		playerMaxHitLastTime = SwVoidParameter.PLAYER_HAS_HIT_TARGET_MAX_TIME;
	}

	public void EndAttack()
	{
		lastAttackEndTime = Time.time;
		StopFire();
		isDash = false;
	}

	public virtual void TryToAttack()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			if (gameWorld.GetRemotePlayers().Count == 0)
			{
				return;
			}
		}
		else if (gameWorld.GetEnemies().Count == 0)
		{
			return;
		}
		FindNewTarget();
		if (attackTarget == null)
		{
			return;
		}
		SetState(ATTACK_STATE);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			ESwVoidStateID state = ((!isFirstRoundAttack) ? ESwVoidStateID.SOLDIER_CHANGE_TARGET : ESwVoidStateID.SOLDIER_ATTACK);
			if (attackTarget.GameUnitType == EGameUnitType.REMOTE_PLAYER)
			{
				SwVoidStateRequest request = new SwVoidStateRequest(state, 0, ((RemotePlayer)attackTarget).GetChannelID(), Vector3.zero);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else if (attackTarget.GameUnitType == EGameUnitType.ENEMY)
			{
				SwVoidStateRequest request2 = new SwVoidStateRequest(state, ((Enemy)attackTarget).EnemyID, 0, Vector3.zero);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
	}

	public virtual void FindNewTarget()
	{
		if (attackTarget == null || !attackTarget.InPlayingState())
		{
			bool flag = false;
			attackTarget = GameWorld.FindEnemyTarget(mPlayer, false, attackRange);
			if (attackTarget == null || (attackTarget != null && (!attackTarget.InPlayingState() || !attackTarget.UnitCanBeHitByRay(entityTransform.position + new Vector3(0f, 1.6f, 0f), (!flag) ? attackRange : checkRangeForward))))
			{
				attackTarget = GameWorld.FindEnemyTarget(mPlayer, true, checkRangeForward, checkAngleForward);
				flag = true;
			}
			if ((attackTarget != null && (!attackTarget.UnitCanBeHitByRay(entityTransform.position + new Vector3(0f, 1.6f, 0f), (!flag) ? attackRange : checkRangeForward) || !attackTarget.InPlayingState())) || Time.time - mPlayer.LastHitTargetTime >= playerMaxHitLastTime || !mPlayer.InPlayingState())
			{
				attackTarget = null;
			}
			if (attackTarget == null)
			{
				SetState(SwVoid.FLOATING_STATE);
			}
		}
	}

	public void StartAttack()
	{
		PlayAnimation(AnimationString.SWVOID_ATTACK, WrapMode.ClampForever);
		if (isFirstRoundAttack)
		{
			lastAttackStartTime = Time.time;
		}
		if (attackTarget != null && attackTarget.InPlayingState())
		{
			AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.VOID3_ATTACK, entityObject, entityTransform);
		}
		else
		{
			SetState(SwVoid.FLOATING_STATE);
			attackTarget = null;
			AudioManager.GetInstance().StopSound(AudioName.VOID3_ATTACK, entityObject);
		}
		isDash = true;
	}

	public void DoAttack(float deltaTime)
	{
		Floating();
		if (Time.time - lastAttackStartTime >= totalAttackTime || !mPlayer.InPlayingState())
		{
			SetState(SwVoid.FLOATING_STATE);
			attackTarget = null;
			AudioManager.GetInstance().StopSound(AudioName.VOID3_ATTACK, entityObject);
			return;
		}
		if (attackTarget != null && attackTarget.InPlayingState())
		{
			attackPosition = attackTarget.GetTransform().position + new Vector3(0f, 1.6f, 0f);
		}
		Vector3 vector;
		if (mPlayerSwVoidTransform == null)
		{
			vector = GetTransform().position - attackPosition;
		}
		else
		{
			vector = mPlayerSwVoidTransform.position - attackPosition;
			vector.y = 0f;
		}
		if (vector.magnitude > 10f && (mPlayerSwVoidTransform.position - GetTransform().position).magnitude < DISTANCE_FAR)
		{
			StartNavMesh(attackPosition, Speed);
		}
		if (attackTarget != null)
		{
			entityTransform.LookAt(attackPosition);
			if (gunFireObj != null && fireLineObj != null)
			{
				gunFireObj.SetActive(true);
				fireLineObj.SetActive(true);
			}
		}
		if (fireLineObj.activeSelf)
		{
			float magnitude = (attackPosition - GetTransform().position).magnitude;
			fireLineObj.transform.localScale = new Vector3(1f, 1f, magnitude / 5.4f);
		}
		tempAnimationTime += deltaTime;
		if (!(tempAnimationTime >= mAttackAnimationTime))
		{
			return;
		}
		if (attackTarget.InPlayingState())
		{
			if (isLocal)
			{
				Attack();
			}
			tempAnimationTime = 0f;
		}
		else
		{
			SetState(SwVoid.FLOATING_STATE);
			attackTarget = null;
			AudioManager.GetInstance().StopSound(AudioName.VOID3_ATTACK, entityObject);
		}
	}

	public virtual void Attack()
	{
		Ray ray = default(Ray);
		Vector3 normalized = (attackPosition - GetTransform().position).normalized;
		ray = new Ray(GetTransform().position, normalized);
		List<Ray> list = new List<Ray>();
		list.Add(ray);
		RaycastHit raycastHit = new RaycastHit
		{
			point = GetTransform().position + (attackPosition - GetTransform().position).normalized * 80f
		};
		Vector3 normalized2 = (attackPosition - gunfire.position).normalized;
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, this) as BulletRay;
		float range = 1000f;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = new DamageProperty(this, mDamage);
			damageProperty.hitForce = ray.direction * 2f;
			damageProperty.criticalAttack = false;
			damageProperty.hitpoint = raycastHit.point;
			damageProperty.isLocal = true;
			damageProperty.mAttackerType = AttackerType.SW_VOID;
			damageProperty.wType = WeaponType.NoGun;
			damageProperty.damageType = mDamageType;
			damageProperty.buff += mPlayer.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
			if (damageProperty.damageType != PlayerDamageType.NORMAL)
			{
				SkillsType sType = (SkillsType)(damageProperty.damageType - 1 + 53);
				damageProperty.buff += mPlayer.GetSkills().GetSkill(sType);
			}
			bulletRay.ShootDetect(damageProperty, list, 10, null, range, 0f, 1f, IsHighLevel());
		}
	}

	private void StopFire()
	{
		if (gunFireObj != null && fireLineObj != null)
		{
			gunFireObj.SetActive(false);
			fireLineObj.SetActive(false);
		}
	}

	public override void DoIdleFloating(float deltaTime)
	{
		base.DoIdleFloating(deltaTime);
		if (isLocal && Time.time - lastAttackStartTime <= totalAttackTime && Time.time - lastAttackEndTime >= changeTargetTime)
		{
			isFirstRoundAttack = false;
			TryToAttack();
		}
		else if (isLocal && Time.time - lastAttackEndTime >= attackInterval)
		{
			isFirstRoundAttack = true;
			TryToAttack();
		}
	}

	public override void Destroy()
	{
		StopFire();
		base.Destroy();
	}

	public override string GetCenter()
	{
		return SwVoidParameter.SoldierCenterBone;
	}
}
