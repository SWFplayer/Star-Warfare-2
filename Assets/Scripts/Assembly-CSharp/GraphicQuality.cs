using UnityEngine;

public class GraphicQuality
{
	public DeviceCaps.Capbility mCapbility;

	public bool mSwitchBloom;

	public bool mSwitchColorCorrection;

	public void Init()
	{
		mCapbility = DeviceCaps.GetCapbility();
		mSwitchBloom = mCapbility == DeviceCaps.Capbility.High;
		mSwitchColorCorrection = mSwitchBloom;
	}

	public void Check()
	{
	}

	public void SetCapbility(DeviceCaps.Capbility cap)
	{
		mCapbility = cap;
		ApplayQuality();
		if (mCapbility < DeviceCaps.Capbility.Middle)
		{
			mSwitchBloom = false;
			mSwitchColorCorrection = false;
		}
	}

	public void ApplayQuality()
	{
		int num = ((mCapbility >= DeviceCaps.Capbility.Middle) ? 1 : 0);
		if (QualitySettings.GetQualityLevel() != num)
		{
			QualitySettings.SetQualityLevel(num);
		}
		if (mCapbility != DeviceCaps.Capbility.Low)
		{
			Shader.globalMaximumLOD = 1000;
		}
		else
		{
			Shader.globalMaximumLOD = 200;
		}
	}
}
