using UnityEngine;

public class DeadEyeMeleeAttack3State : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public bool IsReach;

		public bool IsStopMove;
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.IsReach = false;
		myData.IsStopMove = false;
		enemy.PlayAnimation(AnimationString.BOSS_ATTACK03, WrapMode.ClampForever);
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.ResetAttack(deadEye.mSkill.MeleeAttackSkill1);
		deadEye.MeleeAttackHasUsedNum++;
		enemy.PlaySound(AudioName.DEADEYE_ATTACK03);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		DeadEye deadEye = (DeadEye)enemy;
		MyData myData = enemy.StateData[this] as MyData;
		deadEye.CheckColliderHit(deadEye.mSkill.MeleeAttackSkill1);
		if (enemy.CurrentAnimTime >= DeadEyeParameter.MELEE_ATTACK_2_MOVE_TIME.x && enemy.CurrentAnimTime <= DeadEyeParameter.MELEE_ATTACK_2_MOVE_TIME.y)
		{
			Vector3 vector = enemy.GetPosition() - enemy.GetTargetPosition();
			Vector3 targetPosition = enemy.GetTargetPosition() + vector.normalized * 2f;
			enemy.SetTargetPosition(targetPosition);
			enemy.UpdateNavMeshInDash(false, DeadEyeParameter.MELEE_ATTACK_2_MOVE_SPEED);
		}
		else if (!myData.IsStopMove && enemy.CurrentAnimTime > DeadEyeParameter.MELEE_ATTACK_2_MOVE_TIME.y)
		{
			myData.IsStopMove = true;
			enemy.StopNavMesh();
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
