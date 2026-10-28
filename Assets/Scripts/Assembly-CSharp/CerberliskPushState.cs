using System;
using UnityEngine;

public class CerberliskPushState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.LookAtTargetHorizontal();
		enemy.PlayAnimation("push", WrapMode.ClampForever);
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreWindVfx.SetActive(true);
		cerberlisk.mSkill.BreatheOutVfx.SetActive(false);
		cerberlisk.PlaySound(AudioName.CERBERLISK_PRE_WIND);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		if (enemy.CurrentAnimTime >= CerberliskParameter.PUSH_TIME.x && enemy.CurrentAnimTime <= CerberliskParameter.PUSH_TIME.y)
		{
			if (cerberlisk.mSkill.PreWindVfx.activeSelf)
			{
				cerberlisk.mSkill.PreWindVfx.SetActive(false);
			}
			if (!cerberlisk.mSkill.BreatheOutVfx.activeSelf)
			{
				cerberlisk.mSkill.BreatheOutVfx.SetActive(true);
				cerberlisk.PlaySoundLoop(AudioName.CERBERLISK_PUSH);
			}
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			Vector3 vector = enemy.GetTransform().InverseTransformDirection(localPlayer.GetPosition() - enemy.GetPosition());
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= CerberliskParameter.PUSH_RADIUS_SQR && vector.z > 0f && Mathf.Abs(vector.x / vector.z) < Mathf.Tan((float)Math.PI / 180f * CerberliskParameter.PUSH_ANGLE / 2f))
			{
				Vector3 vector2 = localPlayer.GetPosition() - cerberlisk.mSkill.LeftHeadTransform.position;
				vector2.y = 0f;
				localPlayer.MoveDelta(vector2.normalized * Time.deltaTime * cerberlisk.GetPushSpeed());
			}
		}
	}

	public override void End(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.mSkill.PreWindVfx.SetActive(false);
		cerberlisk.mSkill.BreatheOutVfx.SetActive(false);
		cerberlisk.StopSound(AudioName.CERBERLISK_PUSH);
	}
}
