using System;
using UnityEngine;

public class CerberliskCycloneScript : BaseLaserScript
{
	[NonSerialized]
	public float Duration;

	[NonSerialized]
	public float Interval = 0.5f;

	[NonSerialized]
	public GameObject Target;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	private bool mHasHitPlayer;

	public void Reset()
	{
		mHitCheck.Clear();
		mHitCheck.Interval = Interval;
		mHitCheck.mTransform = base.transform;
		mStartTime = Time.time;
		mHasHitPlayer = false;
		AudioManager.GetInstance().PlaySoundLoop(AudioName.CERBERLISK_Cyclone, base.gameObject, base.transform);
	}

	private void Update()
	{
		if (Time.time - mStartTime > Duration)
		{
			Pool.sAllocate("SW2_Effect/B4_Cyclone_end", base.transform.position, 2f);
			Pool.sGetInstance().Deallocate(base.gameObject);
			AudioManager.GetInstance().StopSound(AudioName.CERBERLISK_Cyclone);
		}
		else if (Target != null)
		{
			Vector3 position = Vector3.MoveTowards(maxDistanceDelta: (mHasHitPlayer ? CerberliskParameter.CYCLONE_HIT_MOVE_SPEED : ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CerberliskParameter.CYCLONE_MOVE_SPEED : (CerberliskParameter.CYCLONE_MOVE_SPEED + 1f))) * Time.deltaTime, current: base.transform.position, target: Target.transform.position);
			base.transform.position = position;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, CerberliskParameter.CYCLONE_KNOCK_SPEED))
		{
			mHasHitPlayer = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, CerberliskParameter.CYCLONE_KNOCK_SPEED))
		{
			mHasHitPlayer = true;
		}
	}
}
