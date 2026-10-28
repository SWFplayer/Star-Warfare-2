using UnityEngine;

public class PlayerCerberlisk3FireBallState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform FireTransform;
	}

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		Player player = gameUnit as Player;
		data.InnerState = 0;
		data.FireTransform = player.CerberliskSkill.FirePosition;
		player.PlayAnimation("3fireball", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["3fireball"].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[3];
		player.CerberliskSkill.PreFireVfx.SetActive(true);
		player.PlaySound(AudioName.CERBERLISK_3FIREBALL);
		player.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		RestrictRotationSpeed(player, CerberliskParameter.THREE_FIRE_BALL_ROTATE_SPEED);
		if (data.InnerState < CerberliskParameter.THREE_FIRE_BALL_TIMES && player.IsAnimationTriggered(CerberliskParameter.THREE_FIRE_BALL_TIME[data.InnerState]))
		{
			if (player.CerberliskSkill.PreFireVfx.activeSelf)
			{
				player.CerberliskSkill.PreFireVfx.SetActive(false);
				player.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			}
			Vector3 forward = player.GetTransform().forward;
			int num;
			if (data.InnerState % 2 == 0)
			{
				num = CerberliskParameter.THREE_FIRE_BALL_NUM;
				forward = Quaternion.Euler(0f, (float)(-((CerberliskParameter.THREE_FIRE_BALL_NUM - 1) / 2)) * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward;
			}
			else
			{
				num = CerberliskParameter.THREE_FIRE_BALL_NUM + 1;
				forward = Quaternion.Euler(0f, (0f - ((float)(num / 2) - 0.5f)) * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward;
			}
			Vector3[] array = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = (array[i] = Quaternion.Euler(0f, (float)i * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward);
				GameObject go = Pool.sAllocate("SW2_Effect/B4_fireball", data.FireTransform.position, vector);
				ParaHitCheckScript component = GameWorld.GetComponent<ParaHitCheckScript>(go);
				component.Speed = CerberliskParameter.THREE_FIRE_BALL_SPEED * vector;
				component.SetDamageInfo(CerberliskParameter.GetInstance(), 4);
				component.Attacker = gameUnit;
				component.Reset();
			}
			data.InnerState++;
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
