using UnityEngine;

public class SpecialSlasher : EnemySpecial
{
	public static EnemyState SPECIAL_SLASHER_STEPBACK_STATE = new SpecialSlasherStepbackState();

	private Transform mMouthTransform;

	protected Animation mEyeAnimation;

	private float mLeapDeceleration;

	private float mLeapMinSpeed;

	private Timer mRainTimer = new Timer();

	private bool mIsLeap;

	private bool mIsWaveEffect;

	protected EnemyBoss.BossTouch mBossTouch = new EnemyBoss.BossTouch();

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		bipObject = entityTransform.Find(BoneName.ENEMY_BIP01).gameObject;
		mMouthTransform = entityTransform.Find(BoneName.SLASHER_MOUTH);
		mEyeAnimation = entityTransform.Find(BoneName.SLASHER_EYE).gameObject.GetComponent<Animation>();
		mRainTimer.SetTimer(SlasherParameter.RAIN_INTERVAL, false);
		mBossTouch.Init(this, mLocalPlayer, SlasherParameter.GetInstance(), 0, SlasherParameter.TOUCH_RADIUS, SlasherParameter.TOUCH_KNOCK_SPEED);
		mBossParameter = SlasherParameter.GetInstance();
	}

	protected override void loadParameters()
	{
		mTurnSpeed = SlasherParameter.TURN_SPEED;
		mRunSpeed = SlasherParameter.CATCHING_SPEED;
		mHpPercentagePerHit = SlasherParameter.HP_PERCENTAGE_PER_HIT;
	}

	protected override void PlayGotHitSound()
	{
		PlaySound(AudioName.SLASHER_ATTACKED);
	}

	protected override void PlayTauntSound()
	{
		PlaySound(AudioName.SLASHER_RAGE);
	}

	public override void OnHit(DamageProperty dp)
	{
		if (InPlayingState() && mState != SPECIAL_SLASHER_STEPBACK_STATE && mState != EnemySpecial.ENEMY_SPECIAL_LEAVE_START_STATE && mState != EnemySpecial.ENEMY_SPECIAL_LEAVE_END_STATE)
		{
			hp -= dp.damage;
			MissionManager.GetInstance().DamageToSpecialEnemy += dp.damage;
			Vector3 hitPoint = Vector3.zero;
			if (GetHitPoint(out hitPoint))
			{
				GameObject original = Resources.Load("SW2_Effect/dragon_blood") as GameObject;
				Object.Instantiate(original, hitPoint, Quaternion.identity);
			}
			if (Time.time - mLastGotHitTime > SlasherParameter.SPECIAL_GOTHIT_INTERVAL)
			{
				mCurrentHitTime++;
				SetState(Enemy.GOTHIT_STATE);
				GameObject original2 = Resources.Load("_Old/_Effect/Dragon/dragon_dead_blood") as GameObject;
				Object.Instantiate(original2, bipObject.transform.position, Quaternion.identity);
			}
		}
	}

	public override void DoLogic(float deltaTime)
	{
		base.DoLogic(deltaTime);
		mBossTouch.Update();
	}

	private void CheckDashHit()
	{
		if (!mIsShoot)
		{
			float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
			if (sqrMagnitude < SlasherParameter.DASH_CHECK_RADIUS * SlasherParameter.DASH_CHECK_RADIUS)
			{
				mIsShoot = true;
			}
			else if (sqrMagnitude < SlasherParameter.DASH_SECTOR_RADIUS * SlasherParameter.DASH_SECTOR_RADIUS && GetLocalPlayerHorizontalAbsAngle() < SlasherParameter.DASH_SECTOR_ANGLE)
			{
				mIsShoot = true;
			}
			if (mIsShoot)
			{
				mLocalPlayer.OnHit(CreateDamageProperty(5));
				CheckKnocked(SlasherParameter.DASH_KNOCK_SPEED);
			}
		}
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (!(GetIdleTimeDuration() > SlasherParameter.SPECIAL_MAX_IDLE_TIME))
		{
			return;
		}
		if (Time.time - mStartTime > SlasherParameter.SPECIAL_LEAVE_TIME)
		{
			SetState(SPECIAL_SLASHER_STEPBACK_STATE);
			return;
		}
		if (base.SqrDistanceFromPlayer > SlasherParameter.SPECIAL_ATTACK_RADIUS * SlasherParameter.SPECIAL_ATTACK_RADIUS)
		{
			SetState(Enemy.CATCHING_STATE);
			return;
		}
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.SPECIAL_PROBABILITY_BLADE)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK1_STATE);
		}
		else if ((float)num < SlasherParameter.SPECIAL_PROBABILITY_WAVE)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK2_STATE);
		}
		else
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK3_STATE);
		}
	}

	protected override void MakeDecisionInCatching()
	{
		if (!(GetCatchingTimeDuration() > SlasherParameter.SPECIAL_MAX_CATCHING_TIME) && !(base.SqrDistanceFromPlayer < SlasherParameter.SPECIAL_ATTACK_RADIUS * SlasherParameter.SPECIAL_ATTACK_RADIUS))
		{
			return;
		}
		if (Time.time - mStartTime > SlasherParameter.SPECIAL_LEAVE_TIME)
		{
			SetState(SPECIAL_SLASHER_STEPBACK_STATE);
			return;
		}
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.SPECIAL_PROBABILITY_BLADE)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK1_STATE);
		}
		else if ((float)num < SlasherParameter.SPECIAL_PROBABILITY_WAVE)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK2_STATE);
		}
		else
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_ATTACK3_STATE);
		}
	}

	private void PlayEyeEffect()
	{
		mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].wrapMode = WrapMode.ClampForever;
		mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].time = 0f;
		mEyeAnimation.CrossFade(AnimationString.SLASHER_EYE_EFFECT, 0f, PlayMode.StopAll);
	}

	private void SetNavMeshAgentForStepBack()
	{
		Vector3 vector = entityTransform.position - entityTransform.forward * SlasherParameter.BACK_DISTANCE;
		Vector3 vector2 = -entityTransform.forward;
		float num = SlasherParameter.BACK_DISTANCE;
		mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), vector2);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		if (Physics.Raycast(mRay, out mRaycastHit, num + SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask))
		{
			num = (entityTransform.position + new Vector3(0f, 0.5f, 0f) - mRaycastHit.point).magnitude - SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL;
		}
		num = Mathf.Max(0f, num);
		vector = entityTransform.position + vector2 * num;
		mNavMeshAgent.enabled = true;
		mNavMeshAgent.Resume();
		mNavMeshAgent.SetDestination(vector);
		mNavMeshAgent.speed = SlasherParameter.BACK_MAXIMUM_SPEED * num;
		mLeapMinSpeed = SlasherParameter.BACK_MINIMUM_SPEED * num;
		mLeapDeceleration = (SlasherParameter.BACK_MAXIMUM_SPEED - SlasherParameter.BACK_MINIMUM_SPEED) / SlasherParameter.BACK_TIME * num;
	}

	public void StartSpecialSlasherStepback()
	{
		LookAtTargetHorizontal();
		mIsShoot = false;
		mIsLeap = false;
		PlaySound(AudioName.SLASHER_STEPBACK);
	}

	public void DoSpecialSlasherStepback()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.BOSS_STEP_BACK, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_STEP_BACK].time / animation[AnimationString.BOSS_STEP_BACK].clip.length;
		if (num > 0.37f)
		{
			if (null != mNavMeshAgent)
			{
				if (!mIsLeap)
				{
					mIsLeap = true;
					SetNavMeshAgentForStepBack();
				}
				else if (mNavMeshAgent.speed > mLeapMinSpeed)
				{
					mNavMeshAgent.speed = Mathf.Max(mLeapMinSpeed, mNavMeshAgent.speed - mLeapDeceleration * Time.deltaTime);
				}
			}
			if (!mIsShoot)
			{
				float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
				if (sqrMagnitude < SlasherParameter.BACK_CHECK_RADIUS * SlasherParameter.BACK_CHECK_RADIUS)
				{
					mIsShoot = true;
					mLocalPlayer.OnHit(CreateDamageProperty(14));
					CheckKnocked(SlasherParameter.BACK_KNOCK_SPEED);
				}
			}
		}
		if (num > 1f)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_LEAVE_START_STATE);
		}
	}

	public void EndSpecialSlasherStepback()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_STEPBACK);
	}

	public override void StartEnemySpecialBornStart()
	{
		LookAtPointHorizontalAtOnce(base.PositionScript.BornEnd.position);
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		mRushDirection = (base.PositionScript.BornEnd.position - entityTransform.position).normalized;
		mIsShoot = false;
	}

	public override void DoEnemySpecialBornStart()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		cc.Move(mRushDirection * SlasherParameter.SPECIAL_APPEAR_SPEED * Time.deltaTime);
		CheckDashHit();
		SelfDestructionObject fence = GetFence();
		if (fence != null && (entityTransform.position - fence.gameObject.transform.position).sqrMagnitude < 20f)
		{
			fence.SelfDestroy();
		}
		if ((entityTransform.position - base.PositionScript.BornEnd.position).sqrMagnitude < 25f)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_BORN_END_STATE);
		}
	}

	public override void EndEnemySpecialBornStart()
	{
	}

	public override void StartEnemySpecialBornEnd()
	{
		CreateNavMeshAgent();
		mIsShoot = false;
		mIsLeap = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_RANGE02);
	}

	public override void DoEnemySpecialBornEnd()
	{
		PlayAnimation(AnimationString.SLASHER_RANGE02, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_RANGE02].time / animation[AnimationString.SLASHER_RANGE02].clip.length;
		if (num > 0.1f)
		{
			if (num < 0.3f)
			{
				LookAtPointHorizontal(base.PositionScript.BornEnd.position);
			}
			else
			{
				if (null != mNavMeshAgent)
				{
					if (!mIsLeap)
					{
						mIsLeap = true;
						Vector3 position = base.PositionScript.BornEnd.position;
						Vector3 vector = position - entityTransform.position;
						vector.y = 0f;
						Vector3 normalized = vector.normalized;
						float magnitude = vector.magnitude;
						mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
						int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
						magnitude = ((!Physics.Raycast(mRay, out mRaycastHit, magnitude + SlasherParameter.LEAP_OVER_DISTANCE + SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + SlasherParameter.LEAP_OVER_DISTANCE) : ((entityTransform.position + new Vector3(0f, 0.5f, 0f) - mRaycastHit.point).magnitude - SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL));
						position = entityTransform.position + normalized * magnitude;
						mNavMeshAgent.enabled = true;
						mNavMeshAgent.Resume();
						mNavMeshAgent.SetDestination(position);
						mNavMeshAgent.speed = SlasherParameter.LEAP_MAXIMUM_SPEED * magnitude;
						SetCanTurn(false);
						mLeapMinSpeed = SlasherParameter.LEAP_MINIMUM_SPEED * magnitude;
						mLeapDeceleration = (SlasherParameter.LEAP_MAXIMUM_SPEED - SlasherParameter.LEAP_MINIMUM_SPEED) / SlasherParameter.LEAP_TIME * magnitude;
					}
					else if (mNavMeshAgent.speed > mLeapMinSpeed)
					{
						mNavMeshAgent.speed = Mathf.Max(mLeapMinSpeed, mNavMeshAgent.speed - mLeapDeceleration * Time.deltaTime);
					}
				}
				if (!mIsShoot)
				{
					float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
					if (sqrMagnitude < SlasherParameter.LEAP_CHECK_RADIUS * SlasherParameter.LEAP_CHECK_RADIUS)
					{
						mIsShoot = true;
					}
					else if (sqrMagnitude < SlasherParameter.LEAP_SECTOR_RADIUS * SlasherParameter.LEAP_SECTOR_RADIUS && GetLocalPlayerHorizontalAbsAngle() < SlasherParameter.LEAP_SECTOR_ANGLE)
					{
						mIsShoot = true;
					}
					if (mIsShoot)
					{
						mLocalPlayer.OnHit(CreateDamageProperty(8));
						CheckKnocked(SlasherParameter.LEAP_KNOCK_SPEED);
					}
				}
			}
		}
		if (num > 1f)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_TAUNT_STATE);
		}
	}

	public override void EndEnemySpecialBornEnd()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_RANGE02);
	}

	public override void StartEnemySpecialLeaveStart()
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(base.PositionScript.LeaveStart.position);
			mNavMeshAgent.speed = SlasherParameter.SPECIAL_LEAVE_SPEED;
			SetCanTurn(false);
		}
		mIsShoot = false;
	}

	public override void DoEnemySpecialLeaveStart()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		CheckDashHit();
		if ((entityTransform.position - base.PositionScript.LeaveStart.position).sqrMagnitude < 1f)
		{
			SetState(EnemySpecial.ENEMY_SPECIAL_LEAVE_END_STATE);
		}
	}

	public override void EndEnemySpecialLeaveStart()
	{
		StopNavMesh();
	}

	public override void StartEnemySpecialLeaveEnd()
	{
		LookAtPointHorizontalAtOnce(base.PositionScript.LeaveEnd.position);
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		mRushDirection = (base.PositionScript.LeaveEnd.position - entityTransform.position).normalized;
		entityObject.layer = PhysicsLayer.DEFAULT;
	}

	public override void DoEnemySpecialLeaveEnd()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		cc.Move(mRushDirection * SlasherParameter.SPECIAL_LEAVE_SPEED * Time.deltaTime);
		SelfDestructionObject rock = GetRock();
		if (rock != null && (entityTransform.position - rock.gameObject.transform.position).sqrMagnitude < 80f)
		{
			rock.SelfDestroy();
		}
		if ((entityTransform.position - base.PositionScript.LeaveEnd.position).sqrMagnitude < 1f)
		{
			MissionManager.GetInstance().OnSpecialBossLeave();
			Destroy();
		}
	}

	public override void EndEnemySpecialLeaveEnd()
	{
	}

	private void ShotBlade()
	{
		GameObject original = Resources.Load(EffectName.SLASHER_SWORD_WIND) as GameObject;
		GameObject gameObject = Object.Instantiate(original, entityTransform.position + entityTransform.forward * 3f + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
		gameObject.transform.LookAt(new Vector3(mTargetPosition.x, gameObject.transform.position.y, mTargetPosition.z));
		SlasherBladeScript component = gameObject.GetComponent<SlasherBladeScript>();
		component.SpeedValue = SlasherParameter.WEAPON_BLADE_SPEED;
		component.Speed = gameObject.transform.forward * SlasherParameter.WEAPON_BLADE_SPEED;
		component.SetDamageInfo(SlasherParameter.GetInstance(), 12);
		component.Attacker = this;
	}

	public override void StartEnemySpecialAttack1()
	{
		LookAtTargetHorizontal();
		mIsShoot = false;
		PlaySound(AudioName.SLASHER_ATTACK01);
	}

	public override void DoEnemySpecialAttack1()
	{
		PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_ATTACK02].time / animation[AnimationString.BOSS_ATTACK02].clip.length;
		if (!mIsShoot)
		{
			LookAtTargetHorizontal();
		}
		if (!mIsShoot && num > 0.5f)
		{
			mIsShoot = true;
			ShotBlade();
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public override void EndEnemySpecialAttack1()
	{
		StopSound(AudioName.SLASHER_ATTACK01);
	}

	public override void StartEnemySpecialAttack2()
	{
		LookAtTargetHorizontal();
		mIsShoot = false;
		mIsWaveEffect = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_ATTACK03);
	}

	public override void DoEnemySpecialAttack2()
	{
		PlayAnimation(AnimationString.BOSS_ATTACK03, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_ATTACK03].time / animation[AnimationString.BOSS_ATTACK03].clip.length;
		if (!mIsWaveEffect)
		{
			LookAtTargetHorizontal();
		}
		if (!mIsWaveEffect && num > 0.36f)
		{
			mIsWaveEffect = true;
			GameObject original = Resources.Load(EffectName.SLASHER_WAVE_ROUND) as GameObject;
			Object.Instantiate(original, entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET, Quaternion.identity);
			GameObject original2 = Resources.Load(EffectName.SLASHER_WAVE_DIRECT) as GameObject;
			GameObject gameObject = Object.Instantiate(original2, entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET, Quaternion.LookRotation(entityTransform.forward)) as GameObject;
			SlasherWaveRootScript component = gameObject.GetComponent<SlasherWaveRootScript>();
			if (null != component)
			{
				component.SetDamageInfo(SlasherParameter.GetInstance(), 10);
				component.KnockSpeed = SlasherParameter.WAVE_LIGHT_KNOCK_SPEED;
				component.Attacker = this;
			}
		}
		if (!mIsShoot && num > 0.5f && num < 0.8f && (entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET - mLocalPlayer.GetTransform().position).sqrMagnitude < SlasherParameter.WAVE_RADIUS * SlasherParameter.WAVE_RADIUS)
		{
			mIsShoot = true;
			mLocalPlayer.OnHit(CreateDamageProperty(9));
			CheckKnocked(SlasherParameter.WAVE_KNOCK_SPEED);
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public override void EndEnemySpecialAttack2()
	{
		StopSound(AudioName.SLASHER_ATTACK03);
	}

	private void ShotThornRain(float angleH, float angleV)
	{
		GameObject original = Resources.Load(EffectName.SLASHER_ARROW) as GameObject;
		GameObject gameObject = Object.Instantiate(original, mMouthTransform.position, Quaternion.identity) as GameObject;
		Vector3 vector = mTargetPosition - mMouthTransform.position;
		vector.y = 0f;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = magnitude / SlasherParameter.RAIN_SPEED;
		float num2 = mMouthTransform.position.y - mFloorHeight;
		float num3 = num2 / num + 0.5f * Physics.gravity.y * num;
		Vector3 vector2 = num3 * Vector3.down;
		Vector3 vector3 = vector2 + SlasherParameter.RAIN_SPEED * normalized;
		gameObject.transform.LookAt(mMouthTransform.position + vector3 * 10f);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.up, angleH);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.right, 0f - angleV);
		SlasherThornScript component = gameObject.GetComponent<SlasherThornScript>();
		component.Speed = gameObject.transform.forward * vector3.magnitude;
		component.SetDamageInfo(SlasherParameter.GetInstance(), 7);
		component.ExplodeRadius = SlasherParameter.RAIN_EXPLODE_RADIUS;
		component.ExplosionTime = SlasherParameter.RAIN_EXPLODE_TIME;
		component.SetSlasher(this);
	}

	public override void StartEnemySpecialAttack3()
	{
		LookAtTargetHorizontal();
		GetGround();
		PlaySound(AudioName.SLASHER_RANGE01);
	}

	public override void DoEnemySpecialAttack3()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.SLASHER_RANGE01, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_RANGE01].time / animation[AnimationString.SLASHER_RANGE01].clip.length;
		if (mRainTimer.Ready() && num > 0.45f && num < 0.66f)
		{
			mRainTimer.Do();
			float angleH = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_HORIZONTAL_RANGE / 100f;
			float angleV = (float)Random.Range(-100, 100) * (SlasherParameter.RAIN_VERTICAL_UP_RANGE + SlasherParameter.RAIN_VERTICAL_DOWN_RANGE) / 100f - SlasherParameter.RAIN_VERTICAL_DOWN_RANGE;
			ShotThornRain(angleH, angleV);
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public override void EndEnemySpecialAttack3()
	{
		StopSound(AudioName.SLASHER_RANGE01);
	}

	private SelfDestructionObject GetRock()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.SELF_DESTRUCTION);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			SelfDestructionObject component = gameObject.GetComponent<SelfDestructionObject>();
			if (component.type == SelfDestructionObjectType.STONE)
			{
				return component;
			}
		}
		return null;
	}

	private SelfDestructionObject GetFence()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.SELF_DESTRUCTION);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			SelfDestructionObject component = gameObject.GetComponent<SelfDestructionObject>();
			if (component.type == SelfDestructionObjectType.FENCE)
			{
				return component;
			}
		}
		return null;
	}

	public override DamageProperty CreateDamageProperty(int damage)
	{
		return new DamageProperty(this, mBossParameter.GetDamage(damage), mBossParameter.GetDamageType(damage));
	}
}
