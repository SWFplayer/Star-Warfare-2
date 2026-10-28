using UnityEngine;

public class PlayerDeadEyeShoulderLaserState : PlayerBossState
{
	private DeadEyeShoulderLaserComponent component = new DeadEyeShoulderLaserComponent();

	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.DEAD_EYE_SHOULDER_RAY, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.DEAD_EYE_SHOULDER_RAY].speed = DeadEyeParameter.DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED[3];
		component.Start(gameUnit, this);
	}

	public override void Update(GameUnit gameUnit)
	{
		component.Update(gameUnit, this);
		StartIdleWhenAnimFinish((Player)gameUnit);
		RestrictRotationSpeed((Player)gameUnit, DeadEyeParameter.PLAYER_BOSS_SHOULDER_RAY_ROTATE_SPEED);
	}

	public override void End(GameUnit gameUnit)
	{
		component.End(gameUnit, this);
	}
}
