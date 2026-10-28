using UnityEngine;

public class FrostStoneDashMoveState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 DashTarget;

		public GameObject DashSmoke2;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.DashSmoke2 = null;
		enemy.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		FrostStone frostStone = enemy as FrostStone;
		sDashAttack dashAttackParameter = frostStone.DashAttackParameter;
		if (myData.InnerState == 0)
		{
			if (enemy.IsAnimationFinish())
			{
				myData.InnerState = 1;
				enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				enemy.MoveToward(enemy.GetTargetPosition(), dashAttackParameter.DASH_SPEED[(int)frostStone.GetMindState()]);
				frostStone.ResetAttack(frostStone.mSkill.DashAttackSkill);
				Pool.sAllocate("SW2_Effect/B3_Dash01_smoke", enemy.GetPosition(), 4f);
				myData.DashSmoke2 = Pool.sGetInstance().Allocate("SW2_Effect/B3_Dash02_smoke");
				myData.DashSmoke2.transform.parent = enemy.GetTransform();
				myData.DashSmoke2.transform.localPosition = Vector3.zero;
				frostStone.PlaySoundLoop(AudioName.FROSTSTONE_DASH);
			}
		}
		else if (myData.InnerState == 1)
		{
			enemy.CheckHit2(frostStone.mSkill.DashAttackSkill);
			if (enemy.GetHorizontalSqrDistanceFromTarget() >= dashAttackParameter.DASH_LOCK_DISTANCE * dashAttackParameter.DASH_LOCK_DISTANCE)
			{
				enemy.UpdateNavMeshInDash(false, dashAttackParameter.DASH_SPEED[(int)frostStone.GetMindState()]);
			}
			else if (enemy.IsMasterPlayer)
			{
				Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
				float num = vector.magnitude;
				vector = vector.normalized;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, num + FrostStoneParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask))
				{
					num = (enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - FrostStoneParameter.MINIMUM_DISTANCE_BEFORE_WALL;
				}
				myData.DashTarget = enemy.GetPosition() + vector * num;
				float speed = dashAttackParameter.DASH_SPEED1[(int)frostStone.GetMindState()];
				enemy.MoveToward(myData.DashTarget, speed);
				myData.InnerState++;
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && enemy.IsMasterPlayer)
				{
					EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 13);
					enemyStateDataRequest.Data.AddByte((byte)myData.InnerState);
					enemyStateDataRequest.Data.AddVector3(myData.DashTarget);
					GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
				}
			}
		}
		else if (myData.InnerState == 2)
		{
			enemy.CheckHit2(frostStone.mSkill.DashAttackSkill);
			Vector3 vector2 = enemy.GetPosition() - myData.DashTarget;
			vector2.y = 0f;
			if (vector2.sqrMagnitude < 0.25f)
			{
				myData.InnerState++;
				enemy.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				enemy.StopSound(AudioName.FROSTSTONE_DASH);
			}
		}
		else if (myData.InnerState != 3)
		{
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData data = GetData<MyData>(enemy);
		if (data.DashSmoke2 != null)
		{
			Pool.sGetInstance().Deallocate(data.DashSmoke2);
		}
		enemy.StopSound(AudioName.FROSTSTONE_DASH);
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
			FrostStone frostStone = gameUnit as FrostStone;
			sDashAttack dashAttackParameter = frostStone.DashAttackParameter;
			float speed = dashAttackParameter.DASH_SPEED1[(int)frostStone.GetMindState()];
			frostStone.MoveToward(data.DashTarget, speed);
			data.InnerState++;
		}
	}
}
