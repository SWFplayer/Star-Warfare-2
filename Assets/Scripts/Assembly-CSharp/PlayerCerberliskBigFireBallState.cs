using UnityEngine;

public class PlayerCerberliskBigFireBallState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform FireTransform;
	}

	public const float Distance = 100f;

	public GameUnitStateComponent component = new GameUnitStateComponent();

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		Player player = gameUnit as Player;
		data.InnerState = 0;
		data.FireTransform = player.CerberliskSkill.FirePosition;
		player.PlayAnimation("bigfireball", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["bigfireball"].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[5];
		component.UpdateTargetPosition(gameUnit, 100f, true);
		player.CerberliskSkill.PreFireVfx.SetActive(true);
		player.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		if (player.IsAnimationTriggered(0.1f))
		{
			player.PlaySound(AudioName.CERBERLISK_BIG_FIREBALL);
		}
		RestrictRotationSpeed(player, CerberliskParameter.BIG_FIRE_BALL_ROTATE_SPEED);
		component.UpdateTargetPosition(gameUnit, 100f, false);
		if (player.IsAnimationTriggered(CerberliskParameter.BIG_FIRE_BALL_TRIGGER_TIME))
		{
			if (player.CerberliskSkill.PreFireVfx.activeSelf)
			{
				player.CerberliskSkill.PreFireVfx.SetActive(false);
				player.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			}
			component.UpdateTargetPosition(gameUnit, 100f, true);
			Vector3 forward = player.GetTargetPosition() - data.FireTransform.position;
			GameObject go = Pool.sAllocate("SW2_Effect/B4_bigfireball", data.FireTransform.position, forward);
			CerberliskBigFireBallScript cerberliskBigFireBallScript = GameWorld.GetComponent<CerberliskBigFireBallScript>(go);
			cerberliskBigFireBallScript.Speed = forward.normalized * CerberliskParameter.BIG_FIRE_BALL_SPEED;
			cerberliskBigFireBallScript.SetDamageInfo(CerberliskParameter.GetInstance(), 5);
			cerberliskBigFireBallScript.ExplodeRadius = CerberliskParameter.BIG_FIRE_BALL_EXPLODE_RADIUS;
			cerberliskBigFireBallScript.Vfx = "SW2_Effect/B4_bigfireballhit";
			cerberliskBigFireBallScript.VfxSound = AudioName.FROSTSTONE_SNOWBALL2_END;
			cerberliskBigFireBallScript.VfxDuration = CerberliskParameter.BIG_FIRE_BALL_DOT_DURATION;
			cerberliskBigFireBallScript.Attacker = gameUnit;
			cerberliskBigFireBallScript.Reset();
		}
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.CerberliskSkill.PreFireVfx.SetActive(false);
		player.StopSound(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}
}
