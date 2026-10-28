using UnityEngine;

public class DeadEyeLeapAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 TargetPosition;

		public float Speed;

		public float MinSpeed;

		public float Deceleration;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_LEAP_ATTACK, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		enemy.ResetAttack(((DeadEye)enemy).mSkill.LeapAttackSkill);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(DeadEyeParameter.LEAP_SOUND_TIME))
		{
			enemy.PlaySound(AudioName.DEADEYE_LEAP_ATTACK);
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0)
		{
			enemy.LookAtTargetHorizontal();
			if (enemy.CurrentAnimTime >= DeadEyeParameter.LEAP_START_JUMP_TIME)
			{
				myData.InnerState++;
				Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
				float magnitude = vector.magnitude;
				vector = vector.normalized;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + DeadEyeParameter.LEAP_OVER_DISTANCE + DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + DeadEyeParameter.DASH_ATTACK.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - DeadEyeParameter.MINIMUM_DISTANCE_BEFORE_WALL));
				myData.TargetPosition = enemy.GetPosition() + vector * magnitude;
				myData.Speed = DeadEyeParameter.LEAP_MAXIMUM_SPEED * magnitude;
				myData.MinSpeed = DeadEyeParameter.LEAP_MINIMUM_SPEED * magnitude;
				myData.Deceleration = (DeadEyeParameter.LEAP_MAXIMUM_SPEED - DeadEyeParameter.LEAP_MINIMUM_SPEED) / DeadEyeParameter.LEAP_TIME * magnitude;
				enemy.MoveToward(myData.TargetPosition, myData.Speed);
			}
		}
		else if (myData.InnerState == 1)
		{
			if (myData.Speed > myData.MinSpeed)
			{
				myData.Speed = Mathf.Max(myData.MinSpeed, myData.Speed - myData.Deceleration * Time.deltaTime);
				enemy.ChangeNavigationSpeed(myData.Speed);
			}
			if (enemy.CurrentAnimTime >= DeadEyeParameter.LEAP_STOP_MOVE_TIME)
			{
				myData.InnerState++;
				enemy.StopNavMesh();
			}
		}
		enemy.CheckHit2(((DeadEye)enemy).mSkill.LeapAttackSkill);
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
