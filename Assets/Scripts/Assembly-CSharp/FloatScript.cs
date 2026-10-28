using UnityEngine;

public class FloatScript : MonoBehaviour
{
	public Vector3 TargetPosition;

	public float StartSpeed;

	public float StayTime;

	private Vector3 mOriginalPosition;

	private float mMaxDistance;

	private Vector3 mDirection;

	private float mAcceleration;

	private float mSpeed;

	private float mMoveDistance;

	private bool mIsStay;

	private float mStartStayTime;

	private void Start()
	{
		mIsStay = false;
		mOriginalPosition = base.transform.position;
		mMaxDistance = TargetPosition.magnitude;
		mAcceleration = 0f - StartSpeed * StartSpeed / (2f * mMaxDistance);
		mSpeed = StartSpeed;
		mMoveDistance = 0f;
		mDirection = TargetPosition.normalized;
	}

	private void Update()
	{
		if (mIsStay)
		{
			if (Time.time - mStartStayTime > StayTime)
			{
				mIsStay = false;
			}
			return;
		}
		mMoveDistance += mSpeed * Time.deltaTime;
		mSpeed += mAcceleration * Time.deltaTime;
		if (Mathf.Abs(mMoveDistance) > mMaxDistance)
		{
			mIsStay = true;
			mStartStayTime = Time.time;
			mMoveDistance = Mathf.Sign(mMoveDistance) * mMaxDistance;
			mSpeed = 0f;
		}
		if (mMoveDistance * mAcceleration > 0f)
		{
			mAcceleration = 0f - mAcceleration;
		}
		base.transform.position = mOriginalPosition + mDirection * mMoveDistance;
	}
}
