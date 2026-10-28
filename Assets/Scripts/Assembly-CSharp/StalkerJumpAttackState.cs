using UnityEngine;

public class StalkerJumpAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Timer Timer = new Timer(0.1f);

		public float StartTime;
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.Timer.Do();
		myData.StartTime = Time.time;
		enemy.PlayAnimation(AnimationString.STALKER_JUMP_ATTACK, WrapMode.ClampForever);
		enemy.ResetAttack(((Stalker)enemy).JumpAttackSkill);
		Move(enemy);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.EnemyType == EnemyType.STALKER)
		{
			enemy.CheckHit2(((Stalker)enemy).JumpAttackSkill);
			if (myData.Timer.Ready2() && !enemy.IsAnimationPlayed(StalkerParameter.JUMP_ATTACK_STOP_AUTO_TARGET_TIME))
			{
				Move(enemy);
			}
		}
	}

	private void Move(Enemy enemy)
	{
		MyData myData = enemy.StateData[this] as MyData;
		Vector3 targetPosition = enemy.GetTargetPosition();
		Vector3 vector = enemy.GetPosition() - enemy.GetTargetPosition();
		targetPosition += vector.normalized * StalkerParameter.JUMP_ATTACK_OFFSET;
		float num = StalkerParameter.JUMP_ATTACK_DURATION * StalkerParameter.JUMP_ATTACK_STOP_MOVE_TIME - (Time.time - myData.StartTime);
		float speed = (vector.magnitude - StalkerParameter.JUMP_ATTACK_OFFSET) / num * 1.1f;
		enemy.MoveToward(targetPosition, speed);
	}
}
