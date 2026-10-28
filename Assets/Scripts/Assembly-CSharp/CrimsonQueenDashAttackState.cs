using UnityEngine;

public class CrimsonQueenDashAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 DashTarget;

		public GameObject DashVFX;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		enemy.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		if (data.DashVFX == null)
		{
			data.DashVFX = Pool.sAllocate("SW2_Effect/B5_rush", enemy.GetPosition(), enemy.GetTransform().forward);
			data.DashVFX.transform.parent = enemy.GetTransform();
			data.DashVFX.transform.localEulerAngles = Vector3.zero;
			data.DashVFX.SetActive(false);
		}
		enemy.PlaySound(AudioName.CRIMSONQUEEN_DASH01);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		sDashAttack dashAttackParameter = crimsonQueen.DashAttackParameter;
		if (data.InnerState == 0)
		{
			if (enemy.IsAnimationFinish())
			{
				data.InnerState = 1;
				enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				enemy.MoveToward(enemy.GetTargetPosition(), dashAttackParameter.DASH_SPEED[crimsonQueen.GetMindStateInt()]);
				enemy.ResetAttack(crimsonQueen.mSkill.DashAttackSkill);
				data.DashVFX.SetActive(true);
				enemy.PlaySound(AudioName.CRIMSONQUEEN_DASH02);
			}
			else
			{
				LookAtTargetPosition(enemy, CrimsonQueenParameter.DASH_ROTATE_SPEED);
			}
		}
		else if (data.InnerState == 1)
		{
			enemy.CheckHit2(crimsonQueen.mSkill.DashAttackSkill);
			if (enemy.GetHorizontalSqrDistanceFromTarget() >= dashAttackParameter.DASH_LOCK_DISTANCE * dashAttackParameter.DASH_LOCK_DISTANCE)
			{
				enemy.UpdateNavMeshInDash(false, dashAttackParameter.DASH_SPEED[crimsonQueen.GetMindStateInt()]);
			}
			else if (enemy.IsMasterPlayer)
			{
				data.InnerState++;
				data.DashTarget = ChooseDashOverPoint(crimsonQueen, dashAttackParameter, data.InnerState, CrimsonQueenParameter.MINIMUM_DISTANCE_BEFORE_WALL);
			}
		}
		else if (data.InnerState == 2)
		{
			enemy.CheckHit2(crimsonQueen.mSkill.DashAttackSkill);
			if ((enemy.GetPosition() - data.DashTarget).sqrMagnitude < dashAttackParameter.DASH_STOP_DISTANCE * dashAttackParameter.DASH_STOP_DISTANCE)
			{
				data.InnerState++;
				enemy.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
			}
		}
		else if (data.InnerState != 3)
		{
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData data = GetData<MyData>(enemy);
		if (data.DashVFX != null)
		{
			data.DashVFX.SetActive(false);
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InnerState == 3 && gameUnit.IsAnimationFinish();
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		byte b = buffer.ReadByte();
		if (b > data.InnerState)
		{
			data.DashTarget = buffer.ReadVector3();
			CrimsonQueen crimsonQueen = gameUnit as CrimsonQueen;
			sDashAttack dashAttackParameter = crimsonQueen.DashAttackParameter;
			float speed = dashAttackParameter.DASH_SPEED1[crimsonQueen.GetMindStateInt()];
			crimsonQueen.MoveToward(data.DashTarget, speed);
			data.InnerState++;
		}
	}
}
