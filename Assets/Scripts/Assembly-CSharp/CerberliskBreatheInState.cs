using System;
using UnityEngine;

public class CerberliskBreatheInState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		enemy.LookAtTargetHorizontal();
		enemy.PlayAnimation("suck", WrapMode.ClampForever);
		enemy.ResetAttack(cerberlisk.mSkill.BreathInAttack);
		enemy.ResetAttack(cerberlisk.mSkill.MouthAttackSkill1);
		cerberlisk.PlaySecondAnimation("suck_point");
		cerberlisk.mSkill.PreWindVfx.SetActive(true);
		cerberlisk.mSkill.BreatheInVfx.SetActive(false);
		cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PRE_WIND);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		if (cerberlisk.IsAnimationTriggered(0.65f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_ATTACK01);
		}
		if (enemy.CurrentAnimTime >= CerberliskParameter.BREATHE_IN_ROTATE_TIME.x && enemy.CurrentAnimTime <= CerberliskParameter.BREATHE_IN_ROTATE_TIME.y)
		{
			LookAtTargetPosition(enemy, CerberliskParameter.BREATHE_IN_ROTATE_SPEED);
		}
		if (enemy.CurrentAnimTime >= CerberliskParameter.BREATHE_IN_TIME.x && enemy.CurrentAnimTime <= CerberliskParameter.BREATHE_IN_TIME.y)
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
		cerberlisk.CheckHit2(cerberlisk.mSkill.BreathInAttack);
		cerberlisk.CheckColliderHit(cerberlisk.mSkill.MouthAttackSkill1);
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.StopSecondAnimation();
		cerberlisk.mSkill.PreWindVfx.SetActive(false);
		cerberlisk.mSkill.BreatheInVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PUSH);
		cerberlisk.StopSound(AudioName.CERBERLISK_PRE_WIND);
	}
}
