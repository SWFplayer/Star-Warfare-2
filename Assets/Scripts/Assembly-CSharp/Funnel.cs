using UnityEngine;

public class Funnel : GameUnit
{
	public enum EState
	{
		None = 0,
		Animation = 1,
		Active = 2,
		Dead = 3
	}

	public bool Enable;

	public Vector3 SpawnFinishPosition;

	public Vector3 SpawnFinishRotation;

	public byte ID;

	public float DeathTime;

	public EState State;

	private bool mIsAssistFunnel;

	private Transform mFollowTransform;

	private CrimsonQueen mQueen;

	private static string[] mAnimNames = new string[6] { "Base.attack", "Base.idle", "Base.back", "Base.attack02", "Base.attack03", "Base.down" };

	private GameObject mLaser;

	private new float LastAnimTime;

	private float CurrentTotalAnimTime;

	private Animator mAnimator;

	private AnimatorStateInfo[] mAnimStateInfo = new AnimatorStateInfo[2];

	private int mAnimIndex;

	public static int mTrackingRPGNum = 0;

	public override void SetObject(GameObject obj)
	{
		base.SetObject(obj);
		if (obj.GetComponentsInChildren<Animator>(true).Length != 0)
		{
			mAnimator = obj.GetComponentsInChildren<Animator>(true)[0];
		}
	}

	public void InitAssistFunnel(Transform followTransform, CrimsonQueen queen)
	{
		mFollowTransform = followTransform;
		mIsAssistFunnel = true;
		Enable = true;
		entityObject.SetActive(true);
		mQueen = queen;
		entityTransform.position = mQueen.GetPosition() + Vector3.up;
		entityTransform.rotation = mFollowTransform.rotation;
		Hp = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.ASSIST_FUNNEL_HP : CrimsonQueenParameter.ASSIST_FUNNEL_HP_HERO);
		collider.enabled = true;
		State = EState.Animation;
		if (ID == 0)
		{
			PlayAnim(4);
		}
		else
		{
			PlayAnim(3);
		}
	}

	public void InitTrackingRPG()
	{
		State = EState.Active;
		Hp = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.TRACKING_RPG_HP : CrimsonQueenParameter.TRACKING_RPG_HP_HERO);
		Enable = false;
		entityObject.name = "TrackingRPG_" + ID;
		if (mTrackingRPGNum++ == 0)
		{
			AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.CRIMSONQUEEN_ROCKET);
		}
		CrimsonQueenTrackingRPG component = entityObject.GetComponent<CrimsonQueenTrackingRPG>();
		component.onDead = OnDead;
	}

	public void Respawn()
	{
		Enable = true;
		entityObject.SetActive(true);
		entityTransform.position = mQueen.GetPosition() + Vector3.up;
		entityTransform.rotation = mFollowTransform.rotation;
		Hp = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.ASSIST_FUNNEL_HP : CrimsonQueenParameter.ASSIST_FUNNEL_HP_HERO);
		State = EState.Animation;
		if (ID == 0)
		{
			PlayAnim(4);
		}
		else
		{
			PlayAnim(3);
		}
	}

	public void SetActive(bool active)
	{
		entityObject.SetActive(active);
		Enable = active;
	}

	public void Update()
	{
		if (Enable)
		{
			UpdateCurrentAnimTime();
			if (mIsAssistFunnel)
			{
				UpdateAssistFunnel();
			}
		}
	}

	private void UpdateAssistFunnel()
	{
		if (State == EState.Animation)
		{
			if (CurrentAnimTime >= 1f)
			{
				State = EState.Active;
				PlayIdleAnimation();
			}
		}
		else if (State == EState.Active)
		{
			float num = Vector3.Distance(entityTransform.position, mFollowTransform.position);
			float t = num / CrimsonQueenParameter.ASSIST_FUNNEL_MOVE_MAX_DISTANCE;
			float num2 = Mathf.Lerp(CrimsonQueenParameter.ASSIST_FUNNEL_MOVE_SPEED[0], CrimsonQueenParameter.ASSIST_FUNNEL_MOVE_SPEED[0], t);
			Vector3 position = Vector3.MoveTowards(entityTransform.position, mFollowTransform.position, num2 * Time.deltaTime);
			entityTransform.position = position;
			if (mQueen.GetTarget() != null)
			{
				entityTransform.LookAt(mQueen.GetTarget().GetPosition());
			}
		}
	}

	public void PlaySpawnAnimation()
	{
		PlayAnim(0);
	}

	public void PlayIdleAnimation()
	{
		PlayAnim(1);
	}

	public void PlayBackAnimation()
	{
		entityTransform.eulerAngles = SpawnFinishRotation;
		PlayAnim(2);
	}

	public void PlayNoneAnimation()
	{
		PlayAnim(5);
	}

	public bool MoveToSpawnPosition()
	{
		Vector3 vector = SpawnFinishPosition - GetPosition();
		float sqrMagnitude = vector.sqrMagnitude;
		vector = vector.normalized;
		if (sqrMagnitude > 1f)
		{
			vector = Vector3.RotateTowards(entityTransform.forward, vector, 20f * Time.deltaTime, 0f);
		}
		entityTransform.position = Vector3.MoveTowards(entityTransform.position, SpawnFinishPosition, CrimsonQueenParameter.FUNNEL_LASER_MOVE_BACK_SPEED * Time.deltaTime);
		return sqrMagnitude < 0.1f;
	}

	public void ShowLaser()
	{
		PlayNoneAnimation();
		mLaser = Pool.sGetInstance().Allocate("SW2_Effect/B5_Range02_Laser");
		LaserScript component = GameWorld.GetComponent<LaserScript>(mLaser);
		component.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 4);
		component.Attacker = this;
		component.KnockSpeed = CrimsonQueenParameter.FUNNEL_LASER_KNOCK_SPEED;
		mLaser.transform.LookAt(mLaser.transform.position + Vector3.down);
		mLaser.transform.parent = entityObject.transform;
		mLaser.transform.localPosition = Vector3.zero;
	}

	public void HideLaser()
	{
		if (mLaser != null)
		{
			Pool.sGetInstance().Deallocate(mLaser);
			mLaser = null;
		}
	}

	public void UpdateCurrentAnimTime()
	{
		if (mAnimator != null)
		{
			mAnimStateInfo[0] = mAnimator.GetCurrentAnimatorStateInfo(0);
			mAnimStateInfo[1] = mAnimator.GetNextAnimatorStateInfo(0);
			LastAnimTime = CurrentAnimTime;
			CurrentAnimTime = GetCurrentAnimPercent();
			if (mAnimStateInfo[1].IsName(mAnimNames[mAnimIndex]))
			{
				mAnimator.SetInteger("AnimState", -1);
			}
		}
	}

	public Animator GetAnimator()
	{
		return mAnimator;
	}

	public virtual void PlayAnim(int index)
	{
		mAnimIndex = index;
		mAnimator.SetInteger("AnimState", index);
		CurrentAnimTime = -1f;
		LastAnimTime = -1f;
	}

	public float GetAnimTime(string name)
	{
		if (mAnimStateInfo[0].IsName(name))
		{
			if (mAnimStateInfo[0].loop)
			{
				return (!(mAnimStateInfo[0].normalizedTime > 1f)) ? mAnimStateInfo[0].normalizedTime : (mAnimStateInfo[0].normalizedTime % 1f);
			}
			return mAnimStateInfo[0].normalizedTime;
		}
		return 0f;
	}

	private float GetCurrentAnimPercent()
	{
		if (null != mAnimator)
		{
			if (mAnimator.IsInTransition(0))
			{
				if (mAnimStateInfo[1].IsName(mAnimNames[mAnimIndex]))
				{
					CurrentTotalAnimTime = mAnimStateInfo[1].normalizedTime;
					if (mAnimStateInfo[1].loop)
					{
						return (!(mAnimStateInfo[1].normalizedTime > 1f)) ? mAnimStateInfo[1].normalizedTime : (mAnimStateInfo[1].normalizedTime % 1f);
					}
					return mAnimStateInfo[1].normalizedTime;
				}
				CurrentTotalAnimTime = 0f;
				return 0f;
			}
			if (mAnimStateInfo[0].IsName(mAnimNames[mAnimIndex]))
			{
				CurrentTotalAnimTime = mAnimStateInfo[0].normalizedTime;
				if (mAnimStateInfo[0].loop)
				{
					return (!(mAnimStateInfo[0].normalizedTime > 1f)) ? mAnimStateInfo[0].normalizedTime : (mAnimStateInfo[0].normalizedTime % 1f);
				}
				return mAnimStateInfo[0].normalizedTime;
			}
			CurrentTotalAnimTime = 0f;
			return 0f;
		}
		return 0f;
	}

	public bool IsAssistFunnel()
	{
		return mIsAssistFunnel;
	}

	public void Attack()
	{
		if (mQueen.GetTarget() != null)
		{
			GameObject gameObject = Pool.sGetInstance().Allocate("SW2_Effect/B5_Small_Laser", GetPosition(), mQueen.GetTarget().GetPosition() - GetPosition() + Vector3.up);
			FunnelSmallLaser component = gameObject.GetComponent<FunnelSmallLaser>();
			component.Reset();
			component.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 8);
			component.Speed = CrimsonQueenParameter.ASSIST_FUNNEL_LASER_SPEED;
			component.Attacker = mQueen;
		}
	}

	public void OnGeneralHit(Player attacker, DamageProperty dp)
	{
		float num = Mathf.Max(0f, 1f + attacker.GetSkills().GetSkill(SkillsType.FINAL_DAMAGE));
		dp.damage = (int)((float)dp.damage * (1f + dp.buff) / (1f + dp.debuff) * num);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			if (dp.isLocal)
			{
				FunnelRequest funnelRequest = new FunnelRequest(ID, EFunnelActionType.EOnhit);
				funnelRequest.Data.AddInt(dp.damage);
				GameApp.GetInstance().GetNetworkManager().SendRequest(funnelRequest);
			}
		}
		else
		{
			OnHit(dp);
		}
	}

	private void OnDead()
	{
		Enable = false;
		State = EState.Dead;
		DeathTime = Time.time;
		GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.W3_ROCKET_BOOM_5)
			.CreateObject(GetPosition(), Vector3.zero, Quaternion.identity);
		if (!mIsAssistFunnel)
		{
			if (--mTrackingRPGNum == 0)
			{
				AudioManager.GetInstance().StopSound(AudioName.CRIMSONQUEEN_ROCKET);
			}
			AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/rpg/rpg-21_boom", entityObject.transform.position);
		}
	}

	public new void OnHit(DamageProperty dp)
	{
		Hp -= dp.damage;
		if (Hp <= 0)
		{
			SetActive(false);
			OnDead();
		}
	}

	public void OnHitResponse(int newHp)
	{
		Hp = newHp;
		if (Hp <= 0)
		{
			SetActive(false);
			OnDead();
		}
	}

	public override bool InPlayingState()
	{
		return State == EState.Animation || State == EState.Active;
	}
}
