using UnityEngine;

public class DeadEyeDashUpState : EnemyState
{
	public class MyData : GameUnitStateData
	{
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
		sDashAttack dashAttackParameter = deadEye.DashAttackParameter;
		enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		deadEye.ResetAttack(deadEye.mSkill.DashAttackSkill);
		Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		RaycastHit hitInfo = default(RaycastHit);
		magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + dashAttackParameter.DASH_OVER_DISTANCE + DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + dashAttackParameter.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL));
		deadEye.DashTarget = enemy.GetPosition() + vector * magnitude;
		enemy.MoveToward(deadEye.DashTarget, dashAttackParameter.DASH_SPEED[0]);
		GameObject original = Resources.Load("SW2_Effect/B2_Dash02_smoke") as GameObject;
		myData.Vfx = Object.Instantiate(original, enemy.GetPosition(), Quaternion.identity) as GameObject;
		myData.Vfx.transform.parent = enemy.GetTransform();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.CheckHit2(((DeadEye)enemy).mSkill.DashAttackSkill);
		if (enemy.IsAnimationTriggered(0f) || enemy.IsAnimationTriggered(0.5f))
		{
			enemy.PlaySound(AudioName.DEADEYE_DASH);
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		DeadEye deadEye = (DeadEye)gameUnit;
		return (deadEye.GetPosition() - deadEye.DashTarget).sqrMagnitude <= 0.1f || Time.time - gameUnit.StateStartTime > 3f;
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData myData = enemy.StateData[this] as MyData;
		Object.Destroy(myData.Vfx);
	}
}
