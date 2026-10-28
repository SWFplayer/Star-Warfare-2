using UnityEngine;

public class AvatarColorData
{
	public byte mMode;

	public byte mSliderValue;

	public Color32 mColor = default(Color32);

	public byte mCurrentEquipID;

	public AvatarColorData()
	{
	}

	public AvatarColorData(AvatarColorData data)
	{
		mMode = data.mMode;
		mSliderValue = data.mSliderValue;
		mColor.r = data.mColor.r;
		mColor.g = data.mColor.g;
		mColor.b = data.mColor.b;
		mColor.a = data.mColor.a;
		mCurrentEquipID = data.mCurrentEquipID;
	}
}
