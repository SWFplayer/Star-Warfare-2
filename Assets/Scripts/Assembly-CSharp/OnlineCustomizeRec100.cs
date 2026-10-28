public class OnlineCustomizeRec100
{
	private OnlineCustomizeData mData;

	public OnlineCustomizeRec100(OnlineCustomizeData data)
	{
		mData = data;
	}

	public void ReadData(BytesBuffer bb)
	{
		byte b = 0;
		b = bb.ReadByte();
		for (int i = 0; i < b; i++)
		{
			mData.mAvatarIdArray[i] = bb.ReadShort();
		}
		mData.mSwVoidId = bb.ReadInt();
		b = bb.ReadByte();
		for (int j = 0; j < b; j++)
		{
			mData.mWeaponIdArray[j] = bb.ReadShort();
		}
		b = bb.ReadByte();
		for (int k = 0; k < b; k++)
		{
			mData.mItemIdArray[k] = bb.ReadShort();
		}
		b = bb.ReadByte();
		mData.mPvpSkillIdArray = new short[b];
		for (int l = 0; l < mData.mPvpSkillIdArray.Length; l++)
		{
			mData.mPvpSkillIdArray[l] = bb.ReadShort();
		}
		b = bb.ReadByte();
		mData.mAwardSkillIdArray = new short[b];
		for (int m = 0; m < mData.mAwardSkillIdArray.Length; m++)
		{
			mData.mAwardSkillIdArray[m] = bb.ReadShort();
		}
		b = bb.ReadByte();
		for (int n = 0; n < b; n++)
		{
			mData.mAvatarColorArray[n] = new AvatarColorData();
			mData.mAvatarColorArray[n].mMode = bb.ReadByte();
			mData.mAvatarColorArray[n].mSliderValue = bb.ReadByte();
			mData.mAvatarColorArray[n].mColor.r = (byte)bb.ReadShort();
			mData.mAvatarColorArray[n].mColor.g = (byte)bb.ReadShort();
			mData.mAvatarColorArray[n].mColor.b = (byte)bb.ReadShort();
			mData.mAvatarColorArray[n].mColor.a = (byte)bb.ReadShort();
		}
	}

	public void WriteData(BytesBuffer bb)
	{
		bb.AddByte((byte)mData.mAvatarIdArray.Length);
		short[] mAvatarIdArray = mData.mAvatarIdArray;
		foreach (short s in mAvatarIdArray)
		{
			bb.AddShort(s);
		}
		bb.AddInt(mData.mSwVoidId);
		bb.AddByte((byte)mData.mWeaponIdArray.Length);
		short[] mWeaponIdArray = mData.mWeaponIdArray;
		foreach (short s2 in mWeaponIdArray)
		{
			bb.AddShort(s2);
		}
		bb.AddByte((byte)mData.mItemIdArray.Length);
		short[] mItemIdArray = mData.mItemIdArray;
		foreach (short s3 in mItemIdArray)
		{
			bb.AddShort(s3);
		}
		bb.AddByte((byte)mData.mPvpSkillIdArray.Length);
		short[] mPvpSkillIdArray = mData.mPvpSkillIdArray;
		foreach (short s4 in mPvpSkillIdArray)
		{
			bb.AddShort(s4);
		}
		bb.AddByte((byte)mData.mAwardSkillIdArray.Length);
		short[] mAwardSkillIdArray = mData.mAwardSkillIdArray;
		foreach (short s5 in mAwardSkillIdArray)
		{
			bb.AddShort(s5);
		}
		bb.AddByte((byte)mData.mAvatarColorArray.Length);
		for (int n = 0; n < mData.mAvatarColorArray.Length; n++)
		{
			bb.AddByte(mData.mAvatarColorArray[n].mMode);
			bb.AddByte(mData.mAvatarColorArray[n].mSliderValue);
			bb.AddShort(mData.mAvatarColorArray[n].mColor.r);
			bb.AddShort(mData.mAvatarColorArray[n].mColor.g);
			bb.AddShort(mData.mAvatarColorArray[n].mColor.b);
			bb.AddShort(mData.mAvatarColorArray[n].mColor.a);
		}
	}
}
