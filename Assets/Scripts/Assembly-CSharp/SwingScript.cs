using UnityEngine;

public class SwingScript : MonoBehaviour
{
	public Vector3 rotationSpeed = new Vector3(0f, 0f, 50f);

	public float maxAngle = 45f;

	public float maxStayTime = 2f;

	private float mCurrentAngle;

	private int mDirectionFactor;

	private Vector3 mAngleDirection;

	private float mAngleSpeed;

	private Quaternion mStartRotation;

	private float mStartStayTime;

	private float mStartSwingTime;

	private bool mIsStay;

	private void Start()
	{
		mStartRotation = base.transform.rotation;
		mIsStay = false;
		mDirectionFactor = 1;
		mAngleSpeed = Quaternion.Angle(Quaternion.identity, Quaternion.Euler(rotationSpeed));
		if (mAngleSpeed > 0f)
		{
			mAngleDirection = rotationSpeed / mAngleSpeed;
		}
		else
		{
			mAngleDirection = Vector3.zero;
		}
	}

	private void Update()
	{
		if (mIsStay)
		{
			if (Time.time - mStartStayTime > maxStayTime)
			{
				mIsStay = false;
				mDirectionFactor = -mDirectionFactor;
			}
			return;
		}
		float num = mCurrentAngle + (float)mDirectionFactor * mAngleSpeed * Time.deltaTime;
		if (Mathf.Abs(num) > maxAngle)
		{
			mIsStay = true;
			mStartStayTime = Time.time;
			num = Mathf.Clamp(num, 0f - maxAngle, maxAngle);
		}
		base.transform.rotation = mStartRotation * Quaternion.Euler(mAngleDirection * num);
		mCurrentAngle = num;
	}
}
