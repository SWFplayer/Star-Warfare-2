using UnityEngine;

public class CameraVibrateController
{
	public class VibrateProcess
	{
		private CameraVibrateConfig mConfig;

		private bool mIsChangeOffset;

		private float mStartTime;

		private float mOutTime;

		private float mInTime;

		private Vector3 mStartOffset;

		private Vector3 mTargetOffset;

		private int mSwingFactor;

		public VibrateProcess(CameraVibrateConfig config)
		{
			mConfig = config;
		}

		public void Start(Vector3 offset)
		{
			mStartOffset = offset;
			mStartTime = Time.time;
			mTargetOffset = mConfig.TargetOffset;
			mOutTime = mConfig.TotalTime * mConfig.OutRatio;
			mInTime = mConfig.TotalTime * (1f - mConfig.OutRatio);
			mSwingFactor = 1;
			mIsChangeOffset = mConfig.ChangeOffsetTime > 0f && mConfig.ChangeOffsetFactor > 0f;
		}

		public Vector3 Update()
		{
			if (mConfig.Type == CameraVibrateType.NORMAL)
			{
				mTargetOffset = mConfig.TargetOffset;
			}
			else if (mConfig.Type == CameraVibrateType.SWING)
			{
				mSwingFactor = -mSwingFactor;
				mTargetOffset = mConfig.TargetOffset * mSwingFactor;
			}
			else if (mConfig.Type == CameraVibrateType.RANDOM)
			{
				mTargetOffset = new Vector3(Random.Range(0f - mConfig.TargetOffset.x, mConfig.TargetOffset.x), Random.Range(0f - mConfig.TargetOffset.y, mConfig.TargetOffset.y), Random.Range(0f - mConfig.TargetOffset.z, mConfig.TargetOffset.z));
			}
			if (mIsChangeOffset && Time.time - mStartTime > mConfig.ChangeOffsetTime)
			{
				mTargetOffset *= mConfig.ChangeOffsetFactor;
			}
			if (Time.time - mStartTime <= mOutTime)
			{
				return OnUpdateOut((Time.time - mStartTime) / mOutTime);
			}
			if (Time.time - mStartTime <= mInTime + mOutTime)
			{
				return OnUpdateIn((Time.time - mStartTime - mOutTime) / mInTime);
			}
			return Vector3.zero;
		}

		public bool IsEnd()
		{
			return Time.time - mStartTime > mConfig.TotalTime;
		}

		private Vector3 OnUpdateOut(float factor)
		{
			return mStartOffset * (1f - factor) + mTargetOffset * factor;
		}

		private Vector3 OnUpdateIn(float factor)
		{
			return mTargetOffset * (1f - factor) + Vector3.zero * factor;
		}
	}

	private Vector3 mCameraRotation = Vector3.zero;

	private VibrateProcess mVibrateProcess;

	private bool mCanVibrate = true;

	public Vector3 CameraRotation
	{
		get
		{
			return mCameraRotation;
		}
		set
		{
			mCameraRotation = value;
		}
	}

	public void Init()
	{
		mCanVibrate = true;
		mVibrateProcess = null;
		mCameraRotation = Vector3.zero;
	}

	public void Process()
	{
		if (mVibrateProcess != null)
		{
			mCameraRotation = mVibrateProcess.Update();
			if (!mCanVibrate && mVibrateProcess.IsEnd())
			{
				mCanVibrate = true;
				mVibrateProcess = null;
				mCameraRotation = Vector3.zero;
			}
		}
	}

	public void Vibrate(CameraVibrateConfig config)
	{
		mVibrateProcess = new VibrateProcess(config);
		mVibrateProcess.Start(mCameraRotation);
	}
}
