using System;
using UnityEngine;

public class FrostStoneIceGroundScript : BaseLaserScript
{
	[NonSerialized]
	public float Interval = 1f;

	[NonSerialized]
	public float Duration;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	public void Reset()
	{
		mHitCheck.Clear();
		mHitCheck.Interval = Interval;
		mHitCheck.mTransform = base.transform;
		base.transform.Translate(0f, 0.1f, 0f);
		mStartTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - mStartTime > Duration)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (!(localPlayer.GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0)
			{
				mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, 0f);
			}
		}
	}
}
