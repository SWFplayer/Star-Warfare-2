using System;
using UnityEngine;

public class PlayerCerberliskBreathInFireBallState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform FireTransform;
	}

	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		data.FireTransform = player.CerberliskSkill.FirePosition;
		player.PlayAnimation("suckfireball", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["suckfireball"].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[6];
		player.ResetAttack(player.CerberliskSkill.MouthAttackSkill2);
		player.CerberliskSkill.PreFireVfx.SetActive(true);
		player.CerberliskSkill.PreWindVfx.SetActive(true);
		player.CerberliskSkill.BreatheInVfx.SetActive(false);
		player.PlaySoundLoop(AudioName.CERBERLISK_PRE_WIND);
		player.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		RestrictRotationSpeed(player, CerberliskParameter.BREATHE_FIRE_BALL_ROTATE_SPEED);
		if (player.IsAnimationTriggered(0.15f))
		{
			player.PlaySound(AudioName.CERBERLISK_3FIREBALL);
		}
		if (player.CurrentAnimTime >= CerberliskParameter.BREATHE_FIRE_BALL_TIME.x && player.CurrentAnimTime <= CerberliskParameter.BREATHE_FIRE_BALL_TIME.y)
		{
			if (player.CerberliskSkill.PreWindVfx.activeSelf)
			{
				player.CerberliskSkill.PreWindVfx.SetActive(false);
				player.StopSound(AudioName.CERBERLISK_PRE_WIND);
			}
			if (!player.CerberliskSkill.BreatheInVfx.activeSelf)
			{
				player.CerberliskSkill.BreatheInVfx.SetActive(true);
				player.PlaySoundLoop(AudioName.CERBERLISK_PUSH);
			}
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (!localPlayer.IsBoss)
			{
				Vector3 vector = player.GetTransform().InverseTransformDirection(localPlayer.GetPosition() - player.GetPosition());
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude <= CerberliskParameter.BREATHE_IN_RADIUS_SQR && vector.z > 0f && Mathf.Abs(vector.x / vector.z) < Mathf.Tan((float)Math.PI / 180f * CerberliskParameter.BREATHE_IN_ANGLE / 2f))
				{
					Vector3 vector2 = player.CerberliskSkill.LeftHeadTransform.position - localPlayer.GetPosition();
					vector2.y = 0f;
					localPlayer.MoveDelta(vector2.normalized * Time.deltaTime * CerberliskParameter.BREATHE_IN_SPEED);
				}
			}
		}
		else if (player.CerberliskSkill.BreatheInVfx.activeSelf)
		{
			player.CerberliskSkill.BreatheInVfx.SetActive(false);
			player.StopSound(AudioName.CERBERLISK_PUSH);
		}
		player.CheckColliderHit(player.CerberliskSkill.MouthAttackSkill2);
		if (data.InnerState < CerberliskParameter.THREE_FIRE_BALL_TIMES && player.IsAnimationTriggered(CerberliskParameter.BREATHE_FIRE_BALL_FIRE_TIME[data.InnerState]))
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
			forward.y = 0f;
			Vector3[] array = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				Vector3 vector3 = (array[i] = Quaternion.Euler(0f, (float)i * CerberliskParameter.THREE_FIRE_BALL_ANGLE, 0f) * forward);
				GameObject go = Pool.sAllocate("SW2_Effect/B4_fireball", data.FireTransform.position);
				ParaHitCheckScript component = GameWorld.GetComponent<ParaHitCheckScript>(go);
				component.Speed = CerberliskParameter.THREE_FIRE_BALL_SPEED * vector3;
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
		player.CerberliskSkill.PreWindVfx.SetActive(false);
		player.CerberliskSkill.BreatheInVfx.SetActive(false);
		player.StopSound(AudioName.CERBERLISK_PUSH);
		player.StopSound(AudioName.CERBERLISK_PRE_FIRE);
		player.StopSound(AudioName.CERBERLISK_PRE_WIND);
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}
}
