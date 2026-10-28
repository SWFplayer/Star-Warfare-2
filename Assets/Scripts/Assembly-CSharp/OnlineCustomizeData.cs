public class OnlineCustomizeData
{
	public short[] mAvatarIdArray;

	public int mSwVoidId;

	public short[] mWeaponIdArray;

	public short[] mItemIdArray;

	public short[] mPvpSkillIdArray;

	public short[] mAwardSkillIdArray;

	public AvatarColorData[] mAvatarColorArray;

	public SwVoidResponseData swVoidData;

	public bool IsPvp { get; set; }

	public OnlineCustomizeData()
	{
		mAvatarIdArray = new short[Global.AVATAR_PART_NUM];
		for (int i = 0; i < Global.AVATAR_PART_NUM; i++)
		{
			mAvatarIdArray[i] = 1;
		}
		mSwVoidId = 0;
		mWeaponIdArray = new short[3];
		mWeaponIdArray[0] = 1;
		for (int j = 1; j < 3; j++)
		{
			mWeaponIdArray[j] = 0;
		}
		mItemIdArray = new short[3];
		for (int k = 0; k < 3; k++)
		{
			mItemIdArray[k] = 0;
		}
		mPvpSkillIdArray = new short[0];
		mAwardSkillIdArray = new short[0];
		mAvatarColorArray = new AvatarColorData[Global.AVATAR_PART_NUM - 1];
		for (int l = 0; l < Global.AVATAR_PART_NUM - 1; l++)
		{
			mAvatarColorArray[l] = new AvatarColorData();
		}
	}

	public OnlineCustomizeData(OnlineCustomizeData data)
	{
		mAvatarIdArray = new short[data.mAvatarIdArray.Length];
		for (int i = 0; i < mAvatarIdArray.Length; i++)
		{
			mAvatarIdArray[i] = data.mAvatarIdArray[i];
		}
		mSwVoidId = data.mSwVoidId;
		mWeaponIdArray = new short[data.mWeaponIdArray.Length];
		for (int j = 0; j < mWeaponIdArray.Length; j++)
		{
			mWeaponIdArray[j] = data.mWeaponIdArray[j];
		}
		mItemIdArray = new short[data.mItemIdArray.Length];
		for (int k = 0; k < mItemIdArray.Length; k++)
		{
			mItemIdArray[k] = data.mItemIdArray[k];
		}
		mPvpSkillIdArray = new short[data.mPvpSkillIdArray.Length];
		for (int l = 0; l < mPvpSkillIdArray.Length; l++)
		{
			mPvpSkillIdArray[l] = data.mPvpSkillIdArray[l];
		}
		mAwardSkillIdArray = new short[data.mAwardSkillIdArray.Length];
		for (int m = 0; m < mAwardSkillIdArray.Length; m++)
		{
			mAwardSkillIdArray[m] = data.mAwardSkillIdArray[m];
		}
		mAvatarColorArray = new AvatarColorData[data.mAvatarColorArray.Length];
		for (int n = 0; n < mAvatarColorArray.Length; n++)
		{
			mAvatarColorArray[n] = new AvatarColorData(data.mAvatarColorArray[n]);
		}
	}

	public short GetFirstWeaponID()
	{
		short result = 1;
		for (int i = 0; i < mWeaponIdArray.Length; i++)
		{
			if (mWeaponIdArray[i] > 0)
			{
				result = mWeaponIdArray[i];
				break;
			}
		}
		return result;
	}
}
