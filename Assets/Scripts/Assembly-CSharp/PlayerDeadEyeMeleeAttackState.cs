using UnityEngine;

public class PlayerDeadEyeMeleeAttackState : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		player.PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_ATTACK01].speed = DeadEyeParameter.DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED[0];
		player.ResetAttack(player.DeadEyeSkill.MeleeAttackSkill);
		GameObject gameObject = Pool.sAllocate("SW2_Effect/B2_Attack01_Swing", player.GetPosition(), player.GetTransform().forward);
		gameObject.transform.parent = player.GetTransform();
		gameObject = Pool.sAllocate("SW2_Effect/B2_Attack01_smoke", player.GetPosition() + player.GetTransform().TransformDirection(-0.7f, 0f, 9f));
		gameObject.transform.parent = player.GetTransform();
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		player.CheckColliderHit(player.DeadEyeSkill.MeleeAttackSkill);
		StartIdleWhenAnimFinish(player);
	}
}
