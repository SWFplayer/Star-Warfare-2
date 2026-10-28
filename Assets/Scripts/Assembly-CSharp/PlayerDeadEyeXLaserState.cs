using UnityEngine;

public class PlayerDeadEyeXLaserState : PlayerBossState
{
	private DeadEyeXLaserComponent component = new DeadEyeXLaserComponent();

	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		player.PlayAnimation(AnimationString.DEAD_EYE_X_RAY, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.DEAD_EYE_X_RAY].speed = DeadEyeParameter.DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED[1];
		component.Start(gameUnit, this, true);
	}

	public override void Update(GameUnit gameUnit)
	{
		component.Update(gameUnit, this);
		StartIdleWhenAnimFinish((Player)gameUnit);
	}

	public override void End(GameUnit gameUnit)
	{
		component.End(gameUnit, this);
	}
}
