using UnityEngine;

public class PlayerDeadEye4LaserState : PlayerBossState
{
	private DeadEye4LaserComponent component = new DeadEye4LaserComponent();

	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		player.PlayAnimation(AnimationString.DEAD_EYE_4_RAY, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.DEAD_EYE_4_RAY].speed = DeadEyeParameter.DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED[2];
		component.Start(gameUnit, this);
	}

	public override void Update(GameUnit gameUnit)
	{
		component.Update(gameUnit, this, true);
		StartIdleWhenAnimFinish((Player)gameUnit);
		RestrictRotationSpeed((Player)gameUnit, DeadEyeParameter.PLAYER_BOSS_4_RAY_ROTATE_SPEED);
	}

	public override void End(GameUnit gameUnit)
	{
		component.End(gameUnit, this);
	}
}
