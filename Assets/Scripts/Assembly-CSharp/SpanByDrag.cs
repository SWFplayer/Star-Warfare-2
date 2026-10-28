using UnityEngine;

public class SpanByDrag : MonoBehaviour
{
	public Transform target;

	public Vector2 degrees = new Vector2(2f, 1f);

	public float rangeX = 1f;

	public float rangeY = 1f;

	public float GyroScope_RangeX = 5f;

	public float GyroScope_RangeY = 5f;

	public float StaticTolerenceX = 0.3f;

	public float StaticTolerenceY = 0.3f;

	private Transform mTrans;

	private Quaternion mStart;

	private Vector2 mRot = Vector2.zero;

	private bool mIsDragging;

	private Vector2 mDelta = Vector2.zero;

	private InterMissionPageType mLastPage;

	private float mWaitForChangePageTime = 0.7f;

	private float mChangePageTime;

	private bool mEnableGravity = true;

	private void Start()
	{
		mTrans = base.transform;
		mStart = mTrans.localRotation;
		mIsDragging = false;
		mLastPage = InterMissionPageType.Shop;
		mEnableGravity = GameApp.GetInstance().GetUserState().GetGravity();
	}

	private void OnDrag(Vector2 delta)
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && mEnableGravity)
		{
			mDelta = delta;
			mRot = Vector2.Lerp(mRot, new Vector2(delta.x, delta.y), RealTime.deltaTime);
			mRot.x = Mathf.Clamp(mRot.x, 0f - rangeX, rangeX);
			mRot.y = Mathf.Clamp(mRot.y, 0f - rangeY, rangeY);
		}
	}

	private void OnPress(bool isPressed)
	{
		mIsDragging = isPressed;
	}

	private void Update()
	{
		if (InterMissionScript.GetInstance() != null && mLastPage != InterMissionScript.GetInstance().CurrentPage)
		{
			mChangePageTime = Time.time;
			mLastPage = InterMissionScript.GetInstance().CurrentPage;
			switch (mLastPage)
			{
			case InterMissionPageType.Mission:
				mStart = InterMissionScript.GetInstance().MissionCameraPosition.localRotation;
				break;
			case InterMissionPageType.Custom:
				mStart = InterMissionScript.GetInstance().CustomCameraPosition.localRotation;
				break;
			case InterMissionPageType.Shop:
				mStart = InterMissionScript.GetInstance().ShopCameraPosition.localRotation;
				break;
			}
			base.transform.localRotation = mStart;
		}
		if (!(Time.time - mChangePageTime > mWaitForChangePageTime))
		{
			return;
		}
		if (!mIsDragging)
		{
			mDelta = Vector2.zero;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer && mEnableGravity)
		{
			float y = Input.gyro.rotationRate.y;
			float x = Input.gyro.rotationRate.x;
			if (Mathf.Abs(y) > StaticTolerenceX || Mathf.Abs(x) > StaticTolerenceY)
			{
				mRot = Vector2.Lerp(mRot, new Vector2(y * 40f, x * 15f), RealTime.deltaTime);
				mRot.x = Mathf.Clamp(mRot.x, 0f - GyroScope_RangeX, GyroScope_RangeX);
				mRot.y = Mathf.Clamp(mRot.y, 0f - GyroScope_RangeY, GyroScope_RangeY);
			}
			else
			{
				mRot = Vector2.Lerp(mRot, Vector2.zero, RealTime.deltaTime);
				mRot.x = Mathf.Clamp(mRot.x, 0f - GyroScope_RangeX, GyroScope_RangeX);
				mRot.y = Mathf.Clamp(mRot.y, 0f - GyroScope_RangeY, GyroScope_RangeY);
			}
		}
		else
		{
			mRot = Vector2.Lerp(mRot, new Vector2(mDelta.x, mDelta.y), RealTime.deltaTime);
			mRot.x = Mathf.Clamp(mRot.x, 0f - rangeX, rangeX);
			mRot.y = Mathf.Clamp(mRot.y, 0f - rangeY, rangeY);
		}
		if (target != null)
		{
			target.localRotation = mStart * Quaternion.Euler((0f - mRot.y) * degrees.y, mRot.x * degrees.x, 0f);
		}
		else
		{
			mTrans.localRotation = mStart * Quaternion.Euler((0f - mRot.y) * degrees.y, mRot.x * degrees.x, 0f);
		}
	}
}
