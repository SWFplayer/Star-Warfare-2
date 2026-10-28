using UnityEngine;

public class EnemySpecial : Enemy
{
	public static EnemyState ENEMY_SPECIAL_BORN_START_STATE = new EnemySpecialBornStartState();

	public static EnemyState ENEMY_SPECIAL_BORN_END_STATE = new EnemySpecialBornEndState();

	public static EnemyState ENEMY_SPECIAL_LEAVE_START_STATE = new EnemySpecialLeaveStartState();

	public static EnemyState ENEMY_SPECIAL_LEAVE_END_STATE = new EnemySpecialLeaveEndState();

	public static EnemyState ENEMY_SPECIAL_ATTACK1_STATE = new EnemySpecialAttack1State();

	public static EnemyState ENEMY_SPECIAL_ATTACK2_STATE = new EnemySpecialAttack2State();

	public static EnemyState ENEMY_SPECIAL_ATTACK3_STATE = new EnemySpecialAttack3State();

	public static EnemyState ENEMY_SPECIAL_TAUNT_STATE = new EnemySpecialTauntState();

	protected float mStartTime;

	protected float mLeaveTime;

	protected Vector3 mRushDirection;

	protected float mLeaveSpeed;

	protected float mAttackRadius;

	protected int probability1;

	protected int probability2;

	protected float mHpPercentagePerHit;

	protected int mCurrentHitTime = 1;

	protected bool mHasFire;

	public EnemySpecialPositionScript PositionScript { get; set; }

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		loadParameters();
		maxHp = hp;
		mStartTime = Time.time;
		mLastGotHitTime = Time.time;
	}

	public virtual void StartEnemySpecialBornStart()
	{
	}

	public virtual void DoEnemySpecialBornStart()
	{
	}

	public virtual void EndEnemySpecialBornStart()
	{
	}

	public virtual void StartEnemySpecialBornEnd()
	{
	}

	public virtual void DoEnemySpecialBornEnd()
	{
	}

	public virtual void EndEnemySpecialBornEnd()
	{
	}

	public virtual void StartEnemySpecialLeaveStart()
	{
	}

	public virtual void DoEnemySpecialLeaveStart()
	{
	}

	public virtual void EndEnemySpecialLeaveStart()
	{
		StopNavMesh();
	}

	public virtual void StartEnemySpecialLeaveEnd()
	{
	}

	public virtual void DoEnemySpecialLeaveEnd()
	{
	}

	public virtual void EndEnemySpecialLeaveEnd()
	{
	}

	public virtual void StartEnemySpecialAttack1()
	{
	}

	public virtual void DoEnemySpecialAttack1()
	{
	}

	public virtual void EndEnemySpecialAttack1()
	{
	}

	public virtual void StartEnemySpecialAttack2()
	{
	}

	public virtual void DoEnemySpecialAttack2()
	{
	}

	public virtual void EndEnemySpecialAttack2()
	{
	}

	public virtual void StartEnemySpecialAttack3()
	{
	}

	public virtual void DoEnemySpecialAttack3()
	{
	}

	public virtual void EndEnemySpecialAttack3()
	{
	}

	protected virtual void PlayTauntSound()
	{
	}

	public virtual void StartEnemySpecialTaunt()
	{
		PlayAnimation(AnimationString.ENEMY_RAGE, WrapMode.ClampForever);
		PlayTauntSound();
	}

	public virtual void DoEnemySpecialTaunt()
	{
		PlayAnimation(AnimationString.ENEMY_RAGE, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.ENEMY_RAGE, 1f))
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public virtual void EndEnemySpecialTaunt()
	{
	}
}
