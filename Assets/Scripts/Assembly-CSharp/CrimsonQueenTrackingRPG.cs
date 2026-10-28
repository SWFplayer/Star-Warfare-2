using System;
using UnityEngine;

public class CrimsonQueenTrackingRPG : EnemyShotScript
{
	[NonSerialized]
	public Player mTrackingPlayer;

	[NonSerialized]
	public Vector3 mDirection;

	[NonSerialized]
	public float mSpeed;

	[NonSerialized]
	public float mStartTime;

	[NonSerialized]
	public Vector3 mOffset;

	private Vector3 trackingPos;

	private Vector3 mUp;

	public void SetTransform(Transform t)
	{
		base.transform.position = t.position;
		base.transform.eulerAngles = t.eulerAngles;
		mUp = base.transform.up;
		mDirection = t.forward;
		mStartTime = Time.time;
	}

	public void Update()
	{
		if (mTrackingPlayer != null && mTrackingPlayer.InPlayingState())
		{
			trackingPos = mTrackingPlayer.GetTransform().position + Vector3.up + mTrackingPlayer.GetTransform().TransformDirection(mOffset);
			Vector3 normalized = (trackingPos - base.transform.position).normalized;
			mDirection = Vector3.RotateTowards(mDirection, normalized, CrimsonQueenParameter.TRACKING_RPG_ROTATE_SPEED * Time.deltaTime, 0f);
			base.transform.LookAt(base.transform.position + mDirection, mUp);
		}
		base.transform.Translate(mSpeed * mDirection * Time.deltaTime, Space.World);
		if (Time.time - mStartTime > CrimsonQueenParameter.TRACKING_RPG_DURATION)
		{
			base.gameObject.SetActive(false);
			GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.W3_ROCKET_BOOM_5)
				.CreateObject(base.gameObject.transform.position, Vector3.zero, Quaternion.identity);
			if (onDead != null)
			{
				onDead();
			}
		}
	}
}
