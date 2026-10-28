using UnityEngine;

public class BlinkLightScript : MonoBehaviour
{
	public float StartDelay;

	public float StandbyDuration = 3f;

	public float BlinkDuration = 0.1f;

	public float MinIntensity;

	public float MaxIntensity = 8f;

	private bool mStartDelayFinished;

	private float mStartTime;

	private bool mIsBlinking;

	private float mDeltaIntensity;

	private bool mForward;

	private void Start()
	{
		mStartTime = Time.time;
		mIsBlinking = false;
		mStartDelayFinished = false;
		mDeltaIntensity = (MaxIntensity - MinIntensity) / BlinkDuration;
		mForward = true;
	}

	private void Update()
	{
		if (base.GetComponent<Light>() == null)
		{
			return;
		}
		if (mStartDelayFinished)
		{
			if (!mIsBlinking)
			{
				if (StandbyDuration > 0f && Time.time - mStartTime >= StandbyDuration)
				{
					mIsBlinking = true;
				}
			}
			else
			{
				Blink();
			}
		}
		else if (Time.time - mStartTime >= StartDelay)
		{
			mStartDelayFinished = true;
			mStartTime = Time.time;
			mIsBlinking = true;
		}
	}

	public void Fresh()
	{
		Start();
		base.GetComponent<Light>().intensity = MinIntensity;
	}

	private void Blink()
	{
		if (mForward)
		{
			base.GetComponent<Light>().intensity += mDeltaIntensity * Time.deltaTime;
			if (base.GetComponent<Light>().intensity >= MaxIntensity)
			{
				base.GetComponent<Light>().intensity = MaxIntensity;
				mForward = false;
				mIsBlinking = false;
				mStartTime = Time.time;
			}
		}
		else
		{
			base.GetComponent<Light>().intensity -= mDeltaIntensity * Time.deltaTime;
			if (base.GetComponent<Light>().intensity <= MinIntensity)
			{
				base.GetComponent<Light>().intensity = MinIntensity;
				mForward = true;
				mIsBlinking = false;
				mStartTime = Time.time;
			}
		}
	}
}
