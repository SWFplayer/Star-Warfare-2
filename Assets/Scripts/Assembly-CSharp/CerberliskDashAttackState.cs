using UnityEngine;

public class CerberliskDashAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 DashTarget;

		public GameObject DashSmoke2;

		public Vector3 LastPosition;
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
		MyData data = GetData<MyData>(enemy);
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		sDashAttack dashAttackParameter = cerberlisk.DashAttackParameter;
		if (data.InnerState == 0)
		{
			if (enemy.IsAnimationFinish())
			{
				data.InnerState = 1;
				enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
				enemy.MoveToward(enemy.GetTargetPosition(), dashAttackParameter.DASH_SPEED[(int)cerberlisk.GetMindState()]);
				cerberlisk.ResetAttack(cerberlisk.mSkill.DashAttackSkill);
				cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_DASH02);
			}
			else
			{
				LookAtTargetPosition(enemy, CerberliskParameter.DASH_ROTATE_SPEED);
			}
		}
		else if (data.InnerState == 1)
		{
			enemy.CheckHit2(cerberlisk.mSkill.DashAttackSkill);
			if (enemy.GetHorizontalSqrDistanceFromTarget() >= dashAttackParameter.DASH_LOCK_DISTANCE * dashAttackParameter.DASH_LOCK_DISTANCE)
			{
				enemy.UpdateNavMeshInDash(false, dashAttackParameter.DASH_SPEED[(int)cerberlisk.GetMindState()]);
			}
			else if (enemy.IsMasterPlayer)
			{
				Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
				float magnitude = vector.magnitude;
				vector = vector.normalized;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + dashAttackParameter.DASH_OVER_DISTANCE + CerberliskParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + dashAttackParameter.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - CerberliskParameter.MINIMUM_DISTANCE_BEFORE_WALL));
				data.DashTarget = enemy.GetPosition() + vector * magnitude;
				float speed = dashAttackParameter.DASH_SPEED1[(int)cerberlisk.GetMindState()];
				enemy.MoveToward(data.DashTarget, speed);
				data.InnerState++;
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && enemy.IsMasterPlayer)
				{
					EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 13);
					enemyStateDataRequest.Data.AddByte((byte)data.InnerState);
					enemyStateDataRequest.Data.AddVector3(data.DashTarget);
					GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
				}
			}
		}
		else if (data.InnerState == 2)
		{
			enemy.CheckHit2(cerberlisk.mSkill.DashAttackSkill);
			if ((enemy.GetPosition() - data.DashTarget).sqrMagnitude < dashAttackParameter.DASH_STOP_DISTANCE * dashAttackParameter.DASH_STOP_DISTANCE)
			{
				data.InnerState++;
				enemy.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				enemy.StopSound(AudioName.CERBERLISK_DASH02);
				enemy.PlaySound(AudioName.CERBERLISK_3HIT);
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
		if (data.DashSmoke2 != null)
		{
			Pool.sGetInstance().Deallocate(data.DashSmoke2);
		}
		enemy.StopSound(AudioName.CERBERLISK_DASH02);
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
			Cerberlisk cerberlisk = gameUnit as Cerberlisk;
			sDashAttack dashAttackParameter = cerberlisk.DashAttackParameter;
			float speed = dashAttackParameter.DASH_SPEED1[(int)cerberlisk.GetMindState()];
			cerberlisk.MoveToward(data.DashTarget, speed);
			data.InnerState++;
		}
	}
}
