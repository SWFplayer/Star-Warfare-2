using UnityEngine;

public class DeadEyeRushAttackState2 : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public GameObject Vfx;
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		DeadEye deadEye = (DeadEye)enemy;
		myData.InnerState = 0;
		enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		enemy.MoveToward(enemy.GetTargetPosition(), deadEye.DashAttackParameter.DASH_SPEED[(int)deadEye.GetMindState()]);
		deadEye.ResetAttack(deadEye.mSkill.DashAttackSkill);
		GameObject original = Resources.Load("SW2_Effect/B2_Dash02_smoke") as GameObject;
		myData.Vfx = Object.Instantiate(original, enemy.GetPosition(), Quaternion.identity) as GameObject;
		myData.Vfx.transform.parent = enemy.GetTransform();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		DeadEye deadEye = (DeadEye)enemy;
		sDashAttack dashAttackParameter = deadEye.DashAttackParameter;
		if (myData.InnerState == 0)
		{
			if (Vector3.SqrMagnitude(enemy.GetPosition() - enemy.GetTargetPosition()) >= dashAttackParameter.DASH_LOCK_DISTANCE * dashAttackParameter.DASH_LOCK_DISTANCE)
			{
				enemy.UpdateNavMeshInDash(false, dashAttackParameter.DASH_SPEED[(int)deadEye.GetMindState()]);
			}
			else
			{
				Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
				float magnitude = vector.magnitude;
				vector = vector.normalized;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + dashAttackParameter.DASH_OVER_DISTANCE + DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + dashAttackParameter.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL));
				deadEye.DashTarget = enemy.GetPosition() + vector * magnitude;
				float speed = dashAttackParameter.DASH_SPEED1[(int)deadEye.GetMindState()];
				enemy.MoveToward(deadEye.DashTarget, speed);
				myData.InnerState++;
			}
		}
		else if (myData.InnerState == 1 && (enemy.GetPosition() - deadEye.DashTarget).sqrMagnitude < dashAttackParameter.DASH_STOP_DISTANCE * dashAttackParameter.DASH_STOP_DISTANCE)
		{
			enemy.SetState(DeadEye.DEAD_EYE_DASH03_ATTACK_STATE);
			myData.InnerState++;
		}
		enemy.CheckHit2(((DeadEye)enemy).mSkill.DashAttackSkill);
		if (enemy.IsAnimationTriggered(0f) || enemy.IsAnimationTriggered(0.5f))
		{
			enemy.PlaySound(AudioName.DEADEYE_DASH);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData myData = enemy.StateData[this] as MyData;
		Object.Destroy(myData.Vfx);
		enemy.StopSound(AudioName.SLASHER_DASH02);
	}
}
