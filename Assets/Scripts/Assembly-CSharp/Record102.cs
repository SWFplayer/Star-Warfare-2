using System;
using System.Collections.Generic;
using System.IO;

public class Record102 : IRecordset
{
	private UserState mUserState;

	private RecordData mData;

	public static string mVersion = "102";

	public Record102(UserState userState)
	{
		mUserState = userState;
		mData = new RecordData();
	}

	public void SaveGlobalBuffer(BytesBuffer bb)
	{
		bb.AddInt(mUserState.GetSaveNum());
		bb.AddString("120");
		bb.AddInt(mUserState.GetMithril());
		bb.AddInt(mUserState.GetCash());
		bb.AddBool(mUserState.GetMultiColorUnlocked());
		bb.AddBool(mUserState.GetRookiePackBought());
		bb.AddBool(mUserState.GetExpertPackBought());
		bb.AddByte((byte)mUserState.GetFriendType());
		bb.AddString(mUserState.GetTwitterName());
		bb.AddLong(mUserState.GetFacebookId());
		bb.AddLong(mUserState.GetGameCenterId());
		bb.AddInt(mUserState.GetTotalPlayTime());
		bb.AddBool(mUserState.GetNeedResetFriendTime());
		bb.AddByte((byte)mUserState.GetCurrentTitleID());
		bb.AddByte((byte)mUserState.GetCurrentTilteFrameID());
	}

	public void SaveMissionBuffer(BytesBuffer bb)
	{
		bb.AddByte((byte)mUserState.GetUnlockedDifficultMode());
		MissionStateRec100 missionStateRec = new MissionStateRec100(mUserState.GetNormalMissionState());
		missionStateRec.SaveData(bb);
		MissionStateRec100 missionStateRec2 = new MissionStateRec100(mUserState.GetHeroicMissionState());
		missionStateRec2.SaveData(bb);
	}

	public void SaveEquipBuffer(BytesBuffer bb)
	{
		bb.AddByte((byte)mUserState.GetWeapons().Count);
		for (int i = 0; i < mUserState.GetWeapons().Count; i++)
		{
			Weapon weapon = mUserState.GetWeapons()[i + 1];
			bb.AddByte(weapon.Level);
		}
		List<List<Armor>> armor = mUserState.GetArmor();
		for (int j = 0; j < armor.Count; j++)
		{
			List<Armor> list = armor[j];
			bb.AddByte((byte)list.Count);
			for (int k = 0; k < list.Count; k++)
			{
				Armor armor2 = list[k];
				bb.AddByte(armor2.Level);
			}
		}
		bb.AddByte((byte)mUserState.GetItems().Count);
		for (int l = 0; l < mUserState.GetItems().Count; l++)
		{
			Item item = mUserState.GetItems()[l + 1];
			bb.AddByte((byte)item.GetCount());
		}
		OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mUserState.GetCoopCustomize());
		onlineCustomizeRec.WriteData(bb);
	}

	public BytesBuffer SaveBytesBuffer()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(8192);
		SaveGlobalBuffer(bytesBuffer);
		SaveMissionBuffer(bytesBuffer);
		SaveEquipBuffer(bytesBuffer);
		BytesBuffer bb = new BytesBuffer(512);
		OperRec100 operRec = new OperRec100(mUserState.OperInfo);
		operRec.SaveData(bb);
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(4096);
		AchievementRec100 achievementRec = new AchievementRec100(mUserState.Achievement);
		achievementRec.SaveData(bb);
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(8192);
		GameAnalysisRec100 gameAnalysisRec = new GameAnalysisRec100(mUserState.AnalysisManager);
		gameAnalysisRec.SaveData(bb);
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(64);
		bb.AddString("120");
		bb.AddByte(18);
		for (int i = 0; i < 18; i++)
		{
			bb.AddBool(mUserState.GetTutorialWatched(i));
		}
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(64);
		bb.AddString("120");
		bb.AddBool(mUserState.GetFollowTwitter());
		bb.AddBool(mUserState.GetFollowFacebook());
		bb.AddBool(mUserState.GetFirstIapSuccess());
		bb.AddLong(mUserState.GetSyncRecordTime().Ticks);
		bb.AddByte(mUserState.GetSelectedStageIndex());
		bb.AddByte(mUserState.GetSelectedSubStageIndex());
		bb.AddByte((byte)mUserState.GetSelectedDifficultMode());
		bb.AddBool(mUserState.GetHasGetFirstBossVoidMaterials());
		bb.AddInt(mUserState.GetPlayTimesAfterIgnoreRating());
		bb.AddInt(mUserState.lastAnnouncementId);
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(64);
		bb.AddString("120");
		bb.AddBool(mUserState.GetPlaySound());
		bb.AddBool(mUserState.GetPlayMusic());
		bb.AddInt((int)(mUserState.GetSoundVolume() * 100f));
		bb.AddInt((int)(mUserState.GetMusicVolume() * 100f));
		bb.AddByte(mUserState.TouchInputSensitivity);
		bb.AddByte((byte)mUserState.GetUserLanguage());
		bb.AddBool(mUserState.GetGravity());
		bb.AddBool(mUserState.GetItemHide());
		bb.AddBool(mUserState.GetSkipDialog());
		bb.AddBool(mUserState.GetAutoHide());
		bb.AddBool(mUserState.GetChangeSide());
		bb.AddBool(mUserState.GetSkipTutorial());
		bb.AddByte((byte)mUserState.mGraphicQuality.mCapbility);
		bb.AddBool(mUserState.mGraphicQuality.mSwitchBloom);
		bb.AddBool(mUserState.mGraphicQuality.mSwitchColorCorrection);
		bytesBuffer.AddBytesBuffer(bb);
		bb = new BytesBuffer(1024);
		bb.AddString("120");
		Dictionary<int, int> friendRemainDictionary = mUserState.GetFriendRemainDictionary();
		bb.AddInt(friendRemainDictionary.Count);
		foreach (int key in friendRemainDictionary.Keys)
		{
			bb.AddInt(key);
			bb.AddByte((byte)friendRemainDictionary[key]);
		}
		bytesBuffer.AddBytesBuffer(bb);
		return bytesBuffer;
	}

	public void SaveData(BinaryWriter bw)
	{
		BytesBuffer bytesBuffer = SaveBytesBuffer();
		int offset = bytesBuffer.GetOffset();
		bw.Write(offset);
		bw.Write(bytesBuffer.GetBytes(), 0, offset);
		bw.Write(mUserState.GetBonusMithril());
		bw.Write(mUserState.GetCompensateMithril());
		bw.Write(mUserState.GetBonusGold());
		bw.Write(mUserState.GetCompensateGold());
		bw.Write(mUserState.GetLastLoginTicks());
		bw.Write(mUserState.GetNextLoginInterval());
		bw.Write(mUserState.GetOnlineRegisted());
		bw.Write(mUserState.GetOnlineEmail());
		bw.Write(mUserState.GetOnlineNickname());
		bw.Write(mUserState.GetOnlinePassword());
		bw.Write(mUserState.GetOnlineUserId());
		bw.Write(mUserState.GetServerAddress());
		bw.Write(mUserState.GetVoidServerPort());
		bw.Write(mUserState.GetFriendServerPort());
		Dictionary<int, OnlineFriend> friendDictionary = mUserState.GetFriendDictionary();
		bw.Write((short)friendDictionary.Count);
		foreach (OnlineFriend value2 in friendDictionary.Values)
		{
			bw.Write(value2.GetUserId());
			bw.Write(value2.GetName());
			bw.Write(value2.GetExp());
			bw.Write((byte)value2.GetPrestige());
			bw.Write((byte)value2.GetPortraitType());
			bw.Write(value2.GetPortraitId());
			bw.Write(value2.GetTwitterName());
			bw.Write((byte)value2.GetTitleId());
			bw.Write((byte)value2.GetFrameType());
			bw.Write((byte)value2.GetMaxCount());
			bw.Write((short)value2.GetMissionNum());
			bw.Write((byte)value2.GetWeaponNum());
			bw.Write((short)value2.GetEquipNum());
			bw.Write((byte)value2.GetSwVoidNum());
			bw.Write(value2.GetTotalPlayTime());
			bw.Write(value2.GetLastLoginTime().Ticks);
			bytesBuffer = new BytesBuffer(256);
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(value2.GetCustomize());
			onlineCustomizeRec.WriteData(bytesBuffer);
			offset = bytesBuffer.GetOffset();
			bw.Write(offset);
			bw.Write(bytesBuffer.GetBytes(), 0, offset);
			if (value2.GetCustomize().mSwVoidId != 0)
			{
				SwVoidSavedataRec100 swVoidSavedataRec = new SwVoidSavedataRec100(value2.GetSwVoidData());
				swVoidSavedataRec.Save(bw);
			}
			bytesBuffer = new BytesBuffer(2048);
			AchievementRec100 achievementRec = new AchievementRec100(value2.GetAchievement());
			achievementRec.SaveData(bytesBuffer);
			offset = bytesBuffer.GetOffset();
			bw.Write(offset);
			bw.Write(bytesBuffer.GetBytes(), 0, offset);
		}
		bw.Write(mUserState.swVoidMaterialList.Count);
		foreach (KeyValuePair<short, short> swVoidMaterial in mUserState.swVoidMaterialList)
		{
			bw.Write(swVoidMaterial.Key);
			bw.Write(swVoidMaterial.Value);
		}
		bw.Write(mUserState.swVoidList.Count);
		foreach (KeyValuePair<int, SwVoidSavedata> swVoid in mUserState.swVoidList)
		{
			bw.Write(swVoid.Key);
			SwVoidSavedataRec100 swVoidSavedataRec2 = new SwVoidSavedataRec100(swVoid.Value);
			swVoidSavedataRec2.Save(bw);
		}
		byte value = 0;
		bw.Write(value);
	}

	public void LoadBytesBuffer(BytesBuffer bb)
	{
		mData.mSaveNum = bb.ReadInt();
		bb.ReadString();
		mData.mMithril = bb.ReadInt();
		mData.mCash = bb.ReadInt();
		mData.mMultiColorBought = bb.ReadBool();
		mData.mRookiePackBought = bb.ReadBool();
		mData.mExpertPackBought = bb.ReadBool();
		mData.mFriendType = (FriendType)bb.ReadByte();
		mData.mTwitterName = bb.ReadString();
		mData.mFacebookId = bb.ReadLong();
		mData.mGameCenterId = bb.ReadLong();
		mData.mTotalPlayTime = bb.ReadInt();
		mData.mNeedResetFriendTime = bb.ReadBool();
		mData.mTitleId = bb.ReadByte();
		mData.mTitleFrameId = bb.ReadByte();
		mData.mUnlockedDifficultMode = (EDifficultMode)bb.ReadByte();
		MissionStateRec100 missionStateRec = new MissionStateRec100(mData.mNormalMissionState);
		missionStateRec.LoadData(bb);
		MissionStateRec100 missionStateRec2 = new MissionStateRec100(mData.mHeroicMissionState);
		missionStateRec2.LoadData(bb);
		mData.mOwnedWeapons = new byte[33];
		byte b = bb.ReadByte();
		for (int i = 0; i < b; i++)
		{
			mData.mOwnedWeapons[i] = bb.ReadByte();
		}
		mData.mOwnedArmorsHeads = new byte[Global.TOTAL_ARMOR_HEAD_NUM];
		mData.mOwnedArmorsBodies = new byte[Global.TOTAL_ARMOR_BODY_NUM];
		mData.mOwnedArmorsArms = new byte[Global.TOTAL_ARMOR_ARM_NUM];
		mData.mOwnedArmorsFeet = new byte[Global.TOTAL_ARMOR_FOOT_NUM];
		mData.mOwnedArmorsBags = new byte[Global.TOTAL_ARMOR_BAG_NUM];
		mData.mOwnedItems = new byte[26];
		byte b2 = bb.ReadByte();
		for (int j = 0; j < b2; j++)
		{
			mData.mOwnedArmorsHeads[j] = bb.ReadByte();
		}
		b2 = bb.ReadByte();
		for (int k = 0; k < b2; k++)
		{
			mData.mOwnedArmorsBodies[k] = bb.ReadByte();
		}
		b2 = bb.ReadByte();
		for (int l = 0; l < b2; l++)
		{
			mData.mOwnedArmorsArms[l] = bb.ReadByte();
		}
		b2 = bb.ReadByte();
		for (int m = 0; m < b2; m++)
		{
			mData.mOwnedArmorsFeet[m] = bb.ReadByte();
		}
		b2 = bb.ReadByte();
		for (int n = 0; n < b2; n++)
		{
			mData.mOwnedArmorsBags[n] = bb.ReadByte();
		}
		b2 = bb.ReadByte();
		for (int num = 0; num < b2; num++)
		{
			mData.mOwnedItems[num] = bb.ReadByte();
		}
		OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mData.mCustomizeData);
		onlineCustomizeRec.ReadData(bb);
		BytesBuffer bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			OperRec100 operRec = new OperRec100(mData.mOperatingInfo);
			operRec.LoadData(bytesBuffer);
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			AchievementRec100 achievementRec = new AchievementRec100(mData.mAchievement);
			achievementRec.LoadData(bytesBuffer);
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			GameAnalysisRec100 gameAnalysisRec = new GameAnalysisRec100(mData.mAnalysis);
			gameAnalysisRec.LoadData(bytesBuffer);
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			byte b3 = bytesBuffer.ReadByte();
			for (int num2 = 0; num2 < b3; num2++)
			{
				mData.mTutorials[num2] = bytesBuffer.ReadBool();
			}
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			mData.mFollowTwitter = bytesBuffer.ReadBool();
			mData.mFollowFacebook = bytesBuffer.ReadBool();
			mData.mFirstIapSuccess = bytesBuffer.ReadBool();
			mData.mSyncRecordTime = new DateTime(bytesBuffer.ReadLong());
			mData.mSelectedStageIndex = bytesBuffer.ReadByte();
			mData.mSelectedSubStageIndex = bytesBuffer.ReadByte();
			mData.mSelectedDifficultMode = (EDifficultMode)bytesBuffer.ReadByte();
			mData.mHasGetFirstBossVoidMaterials = bytesBuffer.ReadBool();
			mData.mPlayTimesAfterIgnoreRating = bytesBuffer.ReadInt();
			mData.mLastAnnouncementId = bytesBuffer.ReadInt();
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			mData.mPlaySound = bytesBuffer.ReadBool();
			mData.mPlayMusic = bytesBuffer.ReadBool();
			mData.mSoundVolume = (float)bytesBuffer.ReadInt() / 100f;
			mData.mMusicVolume = (float)bytesBuffer.ReadInt() / 100f;
			mData.mSensitivity = bytesBuffer.ReadByte();
			mData.mLanguage = bytesBuffer.ReadByte();
			mData.mGravity = bytesBuffer.ReadBool();
			mData.mEnableHide = bytesBuffer.ReadBool();
			mData.mSkipDialog = bytesBuffer.ReadBool();
			mData.mAutoHide = bytesBuffer.ReadBool();
			mData.mChangeSide = bytesBuffer.ReadBool();
			mData.mSkipTutorial = bytesBuffer.ReadBool();
			mData.mGraphicQuality.mCapbility = (DeviceCaps.Capbility)bytesBuffer.ReadByte();
			mData.mGraphicQuality.mSwitchBloom = bytesBuffer.ReadBool();
			mData.mGraphicQuality.mSwitchColorCorrection = bytesBuffer.ReadBool();
			mData.mGraphicQuality.Check();
		}
		bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			bytesBuffer.ReadString();
			mData.mFriendRemainDictionary = new Dictionary<int, int>();
			int num3 = bytesBuffer.ReadInt();
			for (int num4 = 0; num4 < num3; num4++)
			{
				int key = bytesBuffer.ReadInt();
				int value = bytesBuffer.ReadByte();
				mData.mFriendRemainDictionary.Add(key, value);
			}
		}
	}

	public void LoadData(BinaryReader br)
	{
		int count = br.ReadInt32();
		byte[] fromBytes = br.ReadBytes(count);
		BytesBuffer bb = new BytesBuffer(fromBytes);
		LoadBytesBuffer(bb);
		int bonusMithril = br.ReadInt32();
		int compensateMithril = br.ReadInt32();
		int bonusGold = br.ReadInt32();
		int compensateGold = br.ReadInt32();
		long lastLoginTicks = br.ReadInt64();
		long nextLoginInterval = br.ReadInt64();
		bool onlineRegisted = br.ReadBoolean();
		string onlineEmail = br.ReadString();
		string onlineNickname = br.ReadString();
		string onlinePassword = br.ReadString();
		int onlineUserId = br.ReadInt32();
		string serverAddress = br.ReadString();
		int voidServerPort = br.ReadInt32();
		int friendServerPort = br.ReadInt32();
		List<OnlineFriend> list = new List<OnlineFriend>();
		short num = br.ReadInt16();
		for (int i = 0; i < num; i++)
		{
			OnlineFriend onlineFriend = new OnlineFriend();
			onlineFriend.SetUserId(br.ReadInt32());
			onlineFriend.SetName(br.ReadString());
			onlineFriend.SetExp(br.ReadInt32());
			onlineFriend.SetPrestige(br.ReadByte());
			onlineFriend.SetProtraitType((FriendType)br.ReadByte());
			onlineFriend.SetPortraitId(br.ReadInt64());
			onlineFriend.SetTwitterName(br.ReadString());
			onlineFriend.SetTitleId(br.ReadByte());
			onlineFriend.SetFrameType(br.ReadByte());
			onlineFriend.SetMaxCount(br.ReadByte());
			onlineFriend.SetMissionNum(br.ReadInt16());
			onlineFriend.SetWeaponNum(br.ReadByte());
			onlineFriend.SetEquipNum(br.ReadInt16());
			onlineFriend.SetSwVoidNum(br.ReadByte());
			onlineFriend.SetTotalPlayTime(br.ReadInt32());
			onlineFriend.SetLastLoginTime(new DateTime(br.ReadInt64()));
			count = br.ReadInt32();
			fromBytes = br.ReadBytes(count);
			bb = new BytesBuffer(fromBytes);
			OnlineCustomizeData onlineCustomizeData = new OnlineCustomizeData();
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(onlineCustomizeData);
			onlineCustomizeRec.ReadData(bb);
			onlineFriend.SetCustomize(onlineCustomizeData);
			if (onlineFriend.GetPortraitType() != FriendType.NA && onlineFriend.GetPortraitId() != 0L)
			{
				onlineFriend.SocialPlayer = SocialManager.GetInstance().GetSocialPlayer(onlineFriend.GetPortraitType(), onlineFriend.GetPortraitId(), onlineFriend.GetTwitterName());
			}
			if (onlineCustomizeData.mSwVoidId != 0)
			{
				SwVoidSavedata swVoidSavedata = new SwVoidSavedata();
				SwVoidSavedataRec100 swVoidSavedataRec = new SwVoidSavedataRec100(swVoidSavedata);
				swVoidSavedata.SetId(onlineCustomizeData.mSwVoidId);
				swVoidSavedataRec.Load(br);
				onlineFriend.SetSwVoidData(swVoidSavedata);
			}
			count = br.ReadInt32();
			fromBytes = br.ReadBytes(count);
			bb = new BytesBuffer(fromBytes);
			bb.ReadString();
			AchievementState achievementState = new AchievementState();
			AchievementRec100 achievementRec = new AchievementRec100(achievementState);
			achievementRec.LoadData(bb);
			onlineFriend.SetAchievement(achievementState);
			list.Add(onlineFriend);
		}
		int num2 = br.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			short key = br.ReadInt16();
			short value = br.ReadInt16();
			mUserState.swVoidMaterialList.Add(key, value);
		}
		int num3 = br.ReadInt32();
		for (int k = 0; k < num3; k++)
		{
			SwVoidSavedata swVoidSavedata2 = new SwVoidSavedata();
			SwVoidSavedataRec100 swVoidSavedataRec2 = new SwVoidSavedataRec100(swVoidSavedata2);
			int num4 = br.ReadInt32();
			swVoidSavedata2.SetId(num4);
			swVoidSavedataRec2.Load(br);
			mUserState.swVoidList.Add(num4, swVoidSavedata2);
		}
		byte b = br.ReadByte();
		ResetUserState();
		mUserState.SetBonusMithril(bonusMithril);
		mUserState.SetCompensateMithril(compensateMithril);
		mUserState.SetBonusGold(bonusGold);
		mUserState.SetCompensateGold(compensateGold);
		mUserState.SetLastLoginTicks(lastLoginTicks);
		mUserState.SetNextLoginInterval(nextLoginInterval);
		mUserState.SetOnlineRegisted(onlineRegisted);
		mUserState.SetOnlineEmail(onlineEmail);
		mUserState.SetOnlineNickname(onlineNickname);
		mUserState.SetOnlinePassword(onlinePassword);
		mUserState.SetOnlineUserId(onlineUserId);
		mUserState.SetServerAddress(serverAddress);
		mUserState.SetVoidServerPort(voidServerPort);
		mUserState.SetFriendServerPort(friendServerPort);
		mUserState.SetFriendDictionary(list);
	}

	public void ResetUserState()
	{
		mUserState.SetSaveNum(mData.mSaveNum);
		mUserState.SetMithril(mData.mMithril);
		mUserState.SetCash(mData.mCash);
		mUserState.SetMultiColorUnlocked(mData.mMultiColorBought);
		mUserState.SetRookiePackBought(mData.mRookiePackBought);
		mUserState.SetExpertPackBought(mData.mExpertPackBought);
		mUserState.SetFriendType(mData.mFriendType);
		mUserState.SetTwitterName(mData.mTwitterName);
		mUserState.SetFacebookId(mData.mFacebookId);
		mUserState.SetGameCenterId(mData.mGameCenterId);
		mUserState.SetTotalPlayTime(mData.mTotalPlayTime);
		mUserState.SetNeedResetFriendTime(mData.mNeedResetFriendTime);
		mUserState.SetCurrentTitleId(mData.mTitleId);
		mUserState.SetCurrentTitleFrameId(mData.mTitleFrameId);
		mUserState.SetUnlockedDifficultMode(mData.mUnlockedDifficultMode);
		mUserState.SetNormalMissionState(mData.mNormalMissionState);
		mUserState.SetHeroicMissionState(mData.mHeroicMissionState);
		if (mUserState.GetUnlockedStageId(EDifficultMode.NORMAL) <= Global.IRREGULAR_STAGE_ID && mUserState.GetUnlockedSubStageId(EDifficultMode.NORMAL) < Global.BOSS_FINAL_SUB_STAGE_ID)
		{
			mUserState.SetUnlockedDifficultMode(EDifficultMode.NORMAL);
			mUserState.GetHeroicMissionState().SetUnlockedStageId(0);
			mUserState.GetHeroicMissionState().SetUnlockedSubStageId(0);
		}
		if (mUserState.GetUnlockedStageId(EDifficultMode.NORMAL) == Global.IRREGULAR_STAGE_ID && mUserState.GetUnlockedSubStageId(EDifficultMode.NORMAL) == Global.TOTAL_SUB_STAGE)
		{
			mUserState.GetNormalMissionState().SetUnlockedStageId((byte)(Global.IRREGULAR_STAGE_ID + 1));
			mUserState.GetNormalMissionState().SetUnlockedSubStageId(1);
		}
		if (mUserState.GetUnlockedStageId(EDifficultMode.HEROIC) == Global.IRREGULAR_STAGE_ID && mUserState.GetUnlockedSubStageId(EDifficultMode.HEROIC) == Global.TOTAL_SUB_STAGE)
		{
			mUserState.GetHeroicMissionState().SetUnlockedStageId((byte)(Global.IRREGULAR_STAGE_ID + 1));
			mUserState.GetHeroicMissionState().SetUnlockedSubStageId(1);
		}
		if (mUserState.GetUnlockedDifficultMode() == EDifficultMode.HEROIC && mUserState.GetUnlockedStageId(EDifficultMode.HEROIC) == 0 && mUserState.GetUnlockedSubStageId(EDifficultMode.HEROIC) <= 1)
		{
			mUserState.GetHeroicMissionState().SetUnlockedStageId(0);
			mUserState.GetHeroicMissionState().SetUnlockedSubStageId(1);
		}
		MissionManager.GetInstance().UpdateMissionState();
		mUserState.Achievement = mData.mAchievement;
		mUserState.AnalysisManager = mData.mAnalysis;
		mUserState.InitWeapons(mData.mOwnedWeapons);
		mUserState.InitArmors(mData.mOwnedArmorsHeads, mData.mOwnedArmorsBodies, mData.mOwnedArmorsArms, mData.mOwnedArmorsFeet, mData.mOwnedArmorsBags);
		mUserState.InitCurrentCustomize(mData.mCustomizeData);
		mUserState.InitItems(mData.mOwnedItems);
		mUserState.Achievement.LateInit();
		mUserState.OperInfo = mData.mOperatingInfo;
		for (int i = 0; i < mData.mTutorials.Length; i++)
		{
			mUserState.SetTutorialWatched(mData.mTutorials[i], i);
		}
		mUserState.SetFollowTwitter(mData.mFollowTwitter);
		mUserState.SetFollowFacebook(mData.mFollowFacebook);
		mUserState.SetFirstIapSuccess(mData.mFirstIapSuccess);
		mUserState.SetSyncRecordTime(mData.mSyncRecordTime);
		mUserState.SetSelectedStageIndex(mData.mSelectedStageIndex);
		if (mData.mSelectedSubStageIndex == Global.BOSS_FINAL_SUB_STAGE_ID)
		{
			mData.mSelectedSubStageIndex = (byte)Global.BOSS_SUB_STAGE_ID;
		}
		mUserState.SetSelectedSubStageIndex(mData.mSelectedSubStageIndex);
		mUserState.SetSelectedDifficultMode(mData.mSelectedDifficultMode);
		mUserState.SetHasGetFirstBossVoidMaterials(mData.mHasGetFirstBossVoidMaterials);
		mUserState.lastAnnouncementId = mData.mLastAnnouncementId;
		mUserState.SetPlaySound(mData.mPlaySound);
		mUserState.SetPlayMusic(mData.mPlayMusic);
		mUserState.SetSoundVolume(mData.mSoundVolume);
		mUserState.SetMusicVolume(mData.mMusicVolume);
		mUserState.SetUserLanguage((EUserLanguage)mData.mLanguage);
		mUserState.SetGravity(mData.mGravity);
		mUserState.SetItemHide(mData.mEnableHide);
		mUserState.SetSkipDialog(mData.mSkipDialog);
		mUserState.SetAutoHide(mData.mAutoHide);
		mUserState.SetChangeSide(mData.mChangeSide);
		mUserState.SetSkipTutorial(mData.mSkipTutorial);
		mUserState.TouchInputSensitivity = mData.mSensitivity;
		mUserState.SetGraphicQuality(mData.mGraphicQuality);
		mUserState.SetFriendRemainDictionary(mData.mFriendRemainDictionary);
	}

	public RecordData GetRecordData()
	{
		return mData;
	}
}
