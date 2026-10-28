using UnityEngine;

public class Myth370 : Enemy
{
	public static EnemyState TELEPORT_STATE = new TeleportState();

	public static int ShotAttackDamage;

	public static int ShotAreaDamage;

	public static float ShotExplodeRadius;

	private int RangeAttackNum;

	public Myth370()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.MYTH370;
		IsNewState = true;
	}

	public override void StartDead()
	{
		base.StartDead();
		if (entityTransform.Find(EffectName.E5_Shield) != null)
		{
			entityTransform.Find(EffectName.E5_Shield).gameObject.SetActive(false);
		}
		if (Random.value < 50f)
		{
			PlayAnimation("death1", WrapMode.ClampForever);
		}
		else
		{
			PlayAnimation("death2", WrapMode.ClampForever);
		}
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer("_Old/_Audio/enemy/jinjiachong", 5f);
		attackRange = Myth370Parameter.ShotAttackDistance;
		ShotAttackDamage = attackDamage;
		ShotAreaDamage = attackDamage;
		ShotExplodeRadius = Myth370Parameter.ExplodeRadius;
		ConditionStateManager.GetInstance().InitConditionState(this);
		if (entityTransform.Find(EffectName.E5_Shield) != null)
		{
			entityTransform.Find(EffectName.E5_Shield).gameObject.SetActive(true);
		}
	}

	public override void CheckHit()
	{
		if (base.IsMasterPlayer && IsAnimationTriggered(0.6f))
		{
			GetGround();
			Vector3 vector = new Vector3(entityTransform.position.x, mFloorHeight, entityTransform.position.z);
			float num = -2f;
			Vector3 vector2 = new Vector3(mTargetPosition.x, mTargetPosition.y + 1.5f, mTargetPosition.z) - vector;
			float magnitude = vector2.magnitude;
			float num2 = 12f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			Vector3 vector3 = Vector3.up * num4 + vector2.normalized * num2;
			GameObject original = Resources.Load(EffectName.E5_BigBall) as GameObject;
			GameObject gameObject = Object.Instantiate(original, vector + Vector3.up * (0f - num), Quaternion.LookRotation(-vector3)) as GameObject;
			EnemyShotScript component = gameObject.GetComponent<EnemyShotScript>();
			component.trType = TrajectoryType.Parabola;
			component.damageType = EnemyDamageType.Explosion;
			component.speed = vector3;
			component.attackDamage = attackDamage;
			component.areaDamage = attackDamage;
			component.explodeRadius = ShotExplodeRadius;
			component.enemyType = mEnemyType;
			component.attacker = this;
			component.PlayerDamageType = mPlayerDamageType;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				EnemyShotRequest request = new EnemyShotRequest(5, vector + Vector3.up * (0f - num), vector3, base.EnemyID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (GetAttackTimeDuration() > attackFrequency && RangeAttackNum > Myth370Parameter.ShotTimesToTeleport)
		{
			SetState(TELEPORT_STATE);
			SendStateChangeRequest(mConditionState);
		}
		else
		{
			base.MakeDecisionInEnemyIdle();
		}
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		switch (message)
		{
		case EConditionMessage.EIncreaseNum:
			RangeAttackNum++;
			break;
		case EConditionMessage.EResetNum:
			RangeAttackNum = 0;
			break;
		}
		return false;
	}

	public override bool inShield()
	{
		return true;
	}
}
