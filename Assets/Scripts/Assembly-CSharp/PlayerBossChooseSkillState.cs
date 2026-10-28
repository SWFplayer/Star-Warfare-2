using UnityEngine;

public class PlayerBossChooseSkillState : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		if (player.mBossType == EnemyType.SLASHER)
		{
			player.PlayAnimation(AnimationString.SLASHER_WEAPON_IDLE, WrapMode.Loop);
		}
		else
		{
			player.PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		}
	}

	public override void Update(GameUnit gameUnit)
	{
	}
}
