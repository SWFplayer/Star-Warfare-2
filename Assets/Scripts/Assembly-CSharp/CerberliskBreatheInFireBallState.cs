using System;
using UnityEngine;

public class CerberliskBreatheInFireBallState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform FireTransform;
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		enemy.LookAtTargetHorizontal();
		enemy.PlayAnimation("suckfireball", WrapMode.ClampForever);
		data.FireTransform = cerberlisk.mSkill.FirePosition;
		enemy.ResetAttack(cerberlisk.mSkill.MouthAttackSkill2);
		cerberlisk.mSkill.PreFireVfx.SetActive(true);
		cerberlisk.mSkill.PreWindVfx.SetActive(true);
		cerberlisk.mSkill.BreatheInVfx.SetActive(false);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_WIND);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_FIRE);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		MyData data = GetData<MyData>(enemy);
		if (enemy.IsAnimationTriggered(0.15f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_3FIREBALL);
		}
		if (enemy.CurrentAnimTime < CerberliskParameter.BREATHE_FIRE_BALL_FIRE_TIME[0])
		{
			LookAtTargetPosition(enemy, CerberliskParameter.BREATHE_FIRE_BALL_ROTATE_SPEED);
		}
		if (enemy.CurrentAnimTime >= CerberliskParameter.BREATHE_FIRE_BALL_TIME.x && enemy.CurrentAnimTime <= CerberliskParameter.BREATHE_FIRE_BALL_TIME.y)
		{
			if (cerberlisk.mSkill.PreWindVfx.activeSelf)
			{
				cerberlisk.mSkill.PreWindVfx.SetActive(false);
				cerberlisk.StopSound(AudioName.CERBERLISK_PRE_WIND);
			}
			if (!cerberlisk.mSkill.BreatheInVfx.activeSelf)
			{
				cerberlisk.mSkill.BreatheInVfx.SetActive(true);
				cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PUSH);
			}
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			Vector3 vector = enemy.GetTransform().InverseTransformDirection(localPlayer.GetPosition() - enemy.GetPosition());
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= CerberliskParameter.BREATHE_IN_RADIUS_SQR && vector.z > 0f && Mathf.Abs(vector.x / vector.z) < Mathf.Tan((float)Math.PI / 180f * CerberliskParameter.BREATHE_IN_ANGLE / 2f))
			{
				Vector3 vector2 = cerberlisk.mSkill.LeftHeadTransform.position - localPlayer.GetPosition();
				vector2.y = 0f;
				localPlayer.MoveDelta(vector2.normalized * Time.deltaTime * cerberlisk.GetBreathInSpeed());
			}
		}
		else if (cerberlisk.mSkill.BreatheInVfx.activeSelf)
		{
			cerberlisk.mSkill.BreatheInVfx.SetActive(false);
			cerberlisk.StopSound(AudioName.CERBERLISK_PUSH);
		}
		cerberlisk.CheckColliderHit(cerberlisk.mSkill.MouthAttackSkill2);
		if (data.InnerState < CerberliskParameter.THREE_FIRE_BALL_TIMES && enemy.IsAnimationTriggered(CerberliskParameter.BREATHE_FIRE_BALL_FIRE_TIME[data.InnerState]))
		{
			if (cerberlisk.mSkill.PreFireVfx.activeSelf)
			{
				cerberlisk.mSkill.PreFireVfx.SetActive(false);
				cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
			}
			Vector3 forward = enemy.GetTransform().forward;
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
				GameObject go = Pool.sAllocate("SW2_Effect/B4_fireball", data.FireTransform.position, vector3);
				ParaHitCheckScript component = GameWorld.GetComponent<ParaHitCheckScript>(go);
				component.Speed = CerberliskParameter.THREE_FIRE_BALL_SPEED * vector3;
				component.SetDamageInfo(CerberliskParameter.GetInstance(), 4);
				component.Attacker = enemy;
				component.Reset();
			}
			data.InnerState++;
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreFireVfx.SetActive(false);
		cerberlisk.mSkill.PreWindVfx.SetActive(false);
		cerberlisk.mSkill.BreatheInVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PUSH);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_FIRE);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_WIND);
	}
}
