using UnityEngine;

public class DemsterRotateAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public bool HasHitEnemy;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.HasHitEnemy = false;
		enemy.PlayAnimation("attack_sp2_01", WrapMode.ClampForever);
		enemy.ResetAttack(enemy.MeleeAttackSkill);
		Demster demster = enemy as Demster;
		demster.mBossDefend.SetDefendTimeNow();
		if (enemy.GetTransform().Find("E6_Wind") != null)
		{
			enemy.GetTransform().Find("E6_Wind").gameObject.SetActive(true);
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		Demster demster = enemy as Demster;
		if (myData.InnerState == 0 && enemy.IsAnimationFinish())
		{
			myData.InnerState = 1;
			enemy.PlayAnimation("attack_sp2_02", WrapMode.Loop);
			enemy.UpdateNavMeshInDash(true, 6f);
		}
		else if (myData.InnerState == 1)
		{
			enemy.CheckHit2(demster.DashAttackSkill);
			enemy.UpdateNavMeshInDash(false, 6f);
			if (enemy.HasHitEnemy())
			{
				myData.InnerState++;
				myData.HasHitEnemy = true;
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && !enemy.IsMasterPlayer)
				{
					EnemyStateDataRequest request = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 0);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		else if (myData.InnerState == 2)
		{
			enemy.CheckHit2(demster.DashAttackSkill);
			enemy.UpdateNavMeshInDash(false, 6f);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		if (enemy.GetTransform().Find("E6_Wind") != null)
		{
			enemy.GetTransform().Find("E6_Wind").gameObject.SetActive(false);
		}
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		if (gameUnit.IsMasterPlayer)
		{
			MyData myData = gameUnit.StateData[this] as MyData;
			myData.HasHitEnemy = true;
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.HasHitEnemy;
	}
}
