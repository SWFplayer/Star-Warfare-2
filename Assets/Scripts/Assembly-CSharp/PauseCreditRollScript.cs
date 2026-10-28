using UnityEngine;

public class PauseCreditRollScript : MonoBehaviour
{
	public TweenPosition StaffRolling;

	public Transform AnchorMiddle;

	public float PauseTime = 5f;

	private float mStartPauseTime;

	private bool mReadyToPause;

	private void Update()
	{
		if (base.transform.position.y < AnchorMiddle.position.y)
		{
			mReadyToPause = false;
			return;
		}
		if (!mReadyToPause)
		{
			StaffRolling.enabled = false;
			mStartPauseTime = Time.realtimeSinceStartup;
		}
		else if (!StaffRolling.enabled && Time.realtimeSinceStartup - mStartPauseTime >= PauseTime)
		{
			StaffRolling.enabled = true;
		}
		mReadyToPause = true;
	}
}
