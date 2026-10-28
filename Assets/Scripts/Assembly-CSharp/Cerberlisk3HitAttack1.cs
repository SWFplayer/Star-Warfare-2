using UnityEngine;

public class Cerberlisk3HitAttack1 : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		enemy.IsStateRotateFinish = true;
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.PlayAnimation("3hit01", WrapMode.ClampForever);
		cerberlisk.PlaySecondAnimation("3hit01_point");
		cerberlisk.ResetAttack(cerberlisk.mSkill.ThreeHitAttack1);
		cerberlisk.SetCanTurn(false);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.CheckHit2(cerberlisk.mSkill.ThreeHitAttack1);
		if (cerberlisk.CurrentAnimTime < CerberliskParameter.THREE_ATTACK_ROTATE_TIME1)
		{
			LookAtTargetPosition(enemy, CerberliskParameter.THREE_ATTACK_ROTATE_SPEED1);
		}
		if (cerberlisk.IsAnimationTriggered(0.21f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_3HIT);
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.StopSecondAnimation();
	}
}
