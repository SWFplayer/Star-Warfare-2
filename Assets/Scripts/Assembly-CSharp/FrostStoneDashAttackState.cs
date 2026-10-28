using UnityEngine;

public class FrostStoneDashAttackState : EnemyState
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
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.DashSmoke2 = null;
		myData.LastPosition = enemy.GetPosition();
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
				float magnitude = vector.magnitude;
				vector = vector.normalized;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + dashAttackParameter.DASH_OVER_DISTANCE + FrostStoneParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + dashAttackParameter.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - FrostStoneParameter.MINIMUM_DISTANCE_BEFORE_WALL));
				myData.DashTarget = enemy.GetPosition() + vector * magnitude;
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
			if (frostStone.HasIceArmor && Vector3.SqrMagnitude(enemy.GetPosition() - myData.LastPosition) >= FrostStoneParameter.ICE_GROUND_VFX_DISTANCE_SQR)
			{
				myData.LastPosition = enemy.GetPosition();
				GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_IceGround", enemy.GetPosition() + new Vector3(0f, 0.4f, 0f));
				FrostStoneIceGroundScript component = gameObject.GetComponent<FrostStoneIceGroundScript>();
				component.Duration = 5f;
				component.SetDamageInfo(FrostStoneParameter.GetInstance(), 8);
				component.Attacker = enemy;
				component.Reset();
			}
		}
		else if (myData.InnerState == 2)
		{
			enemy.CheckHit2(frostStone.mSkill.DashAttackSkill);
			if ((enemy.GetPosition() - myData.DashTarget).sqrMagnitude < dashAttackParameter.DASH_STOP_DISTANCE * dashAttackParameter.DASH_STOP_DISTANCE)
			{
				myData.InnerState++;
				enemy.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
				enemy.StopSound(AudioName.FROSTSTONE_DASH);
			}
			if (frostStone.HasIceArmor && Vector3.SqrMagnitude(enemy.GetPosition() - myData.LastPosition) >= FrostStoneParameter.ICE_GROUND_VFX_DISTANCE_SQR)
			{
				myData.LastPosition = enemy.GetPosition();
				GameObject gameObject2 = Pool.sAllocate("SW2_Effect/B3_IceGround", enemy.GetPosition() + new Vector3(0f, 0.4f, 0f));
				FrostStoneIceGroundScript component2 = gameObject2.GetComponent<FrostStoneIceGroundScript>();
				component2.Duration = 5f;
				component2.SetDamageInfo(FrostStoneParameter.GetInstance(), 8);
				component2.Attacker = enemy;
				component2.Reset();
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
