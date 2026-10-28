using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class UserState
{
	public const string version = "120";

	public const string compatibleServerVersion = "124";

	public const string realVersion = "127";

	public const string displayVersion = "Version 1.27";

	public string readVersion;

	public string key = "Please quit the app immediately";

	public string md5Key = "1e re sis 02a";

	public bool bInit;

	protected Dictionary<int, Weapon> weaponList;

	protected Dictionary<int, Item> itemList;

	protected string mithril;

	protected string cash;

	protected string mBonusMithril;

	protected string mCompensateMithril;

	protected string mBonusGold;

	protected string mCompensateGold;

	protected string mCompensateExp;

	protected bool mCompensateMaterial;

	protected Dictionary<short, short> mCompensateMaterialList;

	protected FriendType mFriendType;

	protected string mTwitterName = string.Empty;

	protected long mFacebookId;

	protected long mGameCenterId;

	protected int mPlayTimesAfterIgnoreRating = -1;

	protected int saveNum;

	protected string saveIdentity = string.Empty;

	protected int totalPlayTime;

	protected DateTime mSyncRecordTime = DateTime.Now;

	protected DateTime mRemindSyncRecordTime = DateTime.Now;

	protected bool needResetFriendTime;

	protected static List<Rank> rankList;

	protected List<int> mPerkSizeList;

	protected long mLastLoginTicks;

	protected long mNextLoginInterval;

	protected LevelConfig[,] mLevelConfigArray = new LevelConfig[Global.TOTAL_STAGE, Global.TOTAL_SUB_STAGE];

	protected LevelConfig[,] mVSLevelConfigArray = new LevelConfig[1, 100];

	protected EDifficultMode mSelectedDifficultMode;

	protected byte mSelectedStageIndex;

	protected byte mSelectedSubStageIndex = 1;

	protected byte mSelectNetStageIdx = 1;

	protected EDifficultMode mNetDifficultMode;

	protected bool hasGetFirstBossVoidMaterials;

	protected EDifficultMode mUnlockedDifficultMode;

	protected MissionState mNormalMissionState = new MissionState(EDifficultMode.NORMAL);

	protected MissionState mHeroicMissionState = new MissionState(EDifficultMode.HEROIC);

	public List<List<Armor>> armorList;

	public ArmorSet mCurrentArmorSet;

	public WeaponUpgrade weaponUpgrade;

	protected bool bPlayMusic = true;

	protected bool bPlaySound = true;

	protected float musicVolume = 1f;

	protected float soundVolume = 1f;

	protected EUserLanguage userLanguage;

	protected bool mEnableGaravity = true;

	protected bool mEnableItemHide = true;

	protected bool mSkipDialog;

	protected bool mAutoHideButton;

	protected bool mChangeJoystickSide;

	protected bool mSkipTutorial;

	protected bool mLeftJoystickRevert;

	protected bool mRightJoystickRevert;

	protected bool mGuestRegisted;

	protected bool mOnlineRegisted;

	protected string mOnlineEmail = string.Empty;

	protected string mOnlineNickname = string.Empty;

	protected string mOnlinePassword = string.Empty;

	protected int mOnlineUserId;

	protected string mServerAddress = string.Empty;

	protected int mVoidServerPort;

	protected int mFriendServerPort;

	protected int mMissionNum;

	protected int mWeaponNum;

	protected int mEquipNum;

	protected int mSwVoidNum;

	protected byte deadTimer = 2;

	public bool mFollowTwitter;

	public bool mFollowFacebook;

	public bool mFirstIapSuccess;

	protected int mCurrentTitleId;

	protected int mCurrentTitleFrameId;

	protected readonly object syncMithrilLock = new object();

	public string rewardAdsName = string.Empty;

	public int rewardNumber;

	public bool showRewardMsg;

	public Dictionary<short, short> swVoidMaterialList = new Dictionary<short, short>();

	public Dictionary<int, SwVoidSavedata> swVoidList = new Dictionary<int, SwVoidSavedata>();

	public Dictionary<short, bool> swVoidMaterialIsNewList = new Dictionary<short, bool>();

	public Dictionary<short, bool> swVoidMaterialWishList = new Dictionary<short, bool>();

	public int swvoidCountLimit;

	public PlayerSkill playerSkill = new PlayerSkill();

	private OnlineCustomizeData mCurrentCustomizeData = new OnlineCustomizeData();

	private int mCurrentCustomizeDataID;

	private OnlineCustomizeData mCoopCustomizeData = new OnlineCustomizeData();

	private OnlinePlayer mOnlinePlayer;

	private Dictionary<int, OnlineFriend> mFriendDictionary = new Dictionary<int, OnlineFriend>();

	private Dictionary<int, int> mFriendRemainDictionary = new Dictionary<int, int>();

	private List<OnlineFriend> mRequestList = new List<OnlineFriend>();

	private bool[] mTutorialWatched = new bool[18];

	private List<Item> mFlaskItemList = new List<Item>();

	private PvpSkillManager mPvpSkillManager;

	public EnemyType Boss1Type = EnemyType.NA;

	public EnemyType Boss2Type = EnemyType.NA;

	public EnemyType CoopBossType = EnemyType.NA;

	public GraphicQuality mGraphicQuality = new GraphicQuality();

	public bool mReadyToShowRate;

	public Dictionary<int, string> TitleDictionary = new Dictionary<int, string>();

	private bool mMultiColorUnlocked;

	private bool mRookiePackBought;

	private bool mExpertPackBought;

	private bool mUnlockPackBought;

	private bool mPVEAutoAimOn = true;

	private bool mPVPAutoAimOn = true;

	private bool mSteadyModeOn;

	public int lastAnnouncementId;

	public bool isV100To104MithrilHandled;

	private DateTime mPackagePushResetTime = default(DateTime);

	private int mLoseRookiePushTimes;

	private bool mIsWinRookiePush;

	private int mLoseExpertPushTimes;

	private bool mIsWinExpertPush;

	private short mTrialWeaponID = -1;

	private short mTrialWeaponBuyID = -1;

	private float mTrialWeaponBuySecondsLeft;

	private DateTime mTrialWeaponTimeLimit = default(DateTime);

	private DateTime mTrialWeaponPushResetTime = default(DateTime);

	private int mTrialWeaponPushedTimes;

	private float mSecondCount;

	private bool mTodayCanTrial;

	private bool mTodayTrialed;

	private List<short> mPushedWeaponOrderList = new List<short>();

	private short mWeaponUpgradePushID = -1;

	private short mLastestBuyWeaponID = -1;

	private DateTime mWeaponUpgradePushResetTime = default(DateTime);

	private bool mTodayCanPushUpgrade;

	private bool mTodayUpgradePushed;

	private int mUpgradePushIntervalHours;

	private int mPushItemIntervalMinutes = 10;

	private DateTime mLastPushItemTime;

	private bool mSmoothMove;

	public int MainMissionRewardGold { get; set; }

	public int MainMissionRewardMithril { get; set; }

	public AchievementState Achievement { get; set; }

	public byte TouchInputSensitivity { get; set; }

	public OperatingInfo OperInfo { get; set; }

	public int SwVoidIdInUsing
	{
		get
		{
			return mCurrentCustomizeData.mSwVoidId;
		}
		set
		{
			mCurrentCustomizeData.mSwVoidId = value;
		}
	}

	public GameAnalysisManager AnalysisManager { get; set; }

	public UserState()
	{
		weaponList = new Dictionary<int, Weapon>();
		armorList = new List<List<Armor>>();
		weaponUpgrade = new WeaponUpgrade();
		mCurrentArmorSet = new ArmorSet();
		itemList = new Dictionary<int, Item>();
		OperInfo = new OperatingInfo();
		mPvpSkillManager = new PvpSkillManager();
		mGraphicQuality.Init();
		mTrialWeaponID = -1;
	}

	public void Init()
	{
		mSelectedStageIndex = 0;
		mSelectedSubStageIndex = 1;
		SetMithril(0);
		SetCash(0);
		saveNum = 0;
		totalPlayTime = 0;
		mSyncRecordTime = DateTime.Now;
		mRemindSyncRecordTime = DateTime.Now;
		mLastLoginTicks = 0L;
		mNextLoginInterval = 0L;
		byte[] array = new byte[33];
		byte[] array2 = new byte[Global.TOTAL_ARMOR_HEAD_NUM];
		byte[] array3 = new byte[Global.TOTAL_ARMOR_BODY_NUM];
		byte[] array4 = new byte[Global.TOTAL_ARMOR_ARM_NUM];
		byte[] array5 = new byte[Global.TOTAL_ARMOR_FOOT_NUM];
		byte[] array6 = new byte[Global.TOTAL_ARMOR_BAG_NUM];
		string[][] array7 = new string[33][];
		for (int i = 0; i < array7.Length; i++)
		{
			array7[i] = new string[Global.MAX_LEVEL_WEAPONW];
		}
		string[] mHeadBuyVer = new string[Global.TOTAL_ARMOR_NUM];
		string[] mBodyBuyVer = new string[Global.TOTAL_ARMOR_NUM];
		string[] mHandBuyVer = new string[Global.TOTAL_ARMOR_NUM];
		string[] mFootBuyVer = new string[Global.TOTAL_ARMOR_NUM];
		string[] mBagBuyVer = new string[Global.TOTAL_ARMOR_BAG_NUM];
		byte[] ownedItems = new byte[26]
		{
			0, 0, 0, 0, 4, 3, 0, 0, 0, 1,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};
		array[0] = 1;
		array2[0] = 1;
		array3[0] = 1;
		array4[0] = 1;
		array5[0] = 1;
		array6[0] = 1;
		mNormalMissionState.Init();
		mHeroicMissionState.Init();
		Achievement = new AchievementState();
		AnalysisManager = new GameAnalysisManager();
		InitWeapons(array, array7);
		InitArmors(array2, array3, array4, array5, array6, mHeadBuyVer, mBodyBuyVer, mHandBuyVer, mFootBuyVer, mBagBuyVer);
		InitItems(ownedItems);
		InitCoopCustomize();
		TouchInputSensitivity = Global.NORMAL_SENSITIVITY_LEVEL;
		mFollowTwitter = false;
		mFollowFacebook = false;
		mFirstIapSuccess = false;
		mPackagePushResetTime = default(DateTime);
		mLoseRookiePushTimes = 0;
		mIsWinRookiePush = false;
		mLoseExpertPushTimes = 0;
		mIsWinExpertPush = false;
		mTrialWeaponID = -1;
		mTrialWeaponBuyID = -1;
		mTrialWeaponBuySecondsLeft = 0f;
		mTrialWeaponPushResetTime = default(DateTime);
		mTrialWeaponPushedTimes = 0;
		mTodayCanTrial = false;
		mPushedWeaponOrderList = new List<short>();
		mWeaponUpgradePushID = -1;
		mWeaponUpgradePushResetTime = default(DateTime);
		mTodayCanPushUpgrade = false;
		bInit = true;
		for (int j = 0; j < 18; j++)
		{
			mTutorialWatched[j] = false;
		}
		TutorialManager.GetInstance().Init();
		UnlockEquipsByLevel(EDifficultMode.NORMAL, 0, 0, 0, 0);
	}

	public void InitWeapons(byte[] ownedWeapons, string[][] buyVer)
	{
		InitWeaponUpgrade();
		weaponList.Clear();
		for (int i = 0; i < ownedWeapons.Length; i++)
		{
			Weapon weapon = WeaponFactory.GetInstance().CreateWeapon((byte)i);
			weapon.LoadConfig(ownedWeapons[i]);
			weapon.SetLevel(ownedWeapons[i]);
			weapon.SetBuyVer(buyVer[i]);
			weaponList.Add(weapon.GunID, weapon);
		}
	}

	public void InitWeapons(byte[] ownedWeapons)
	{
		InitWeaponUpgrade();
		weaponList.Clear();
		for (int i = 0; i < ownedWeapons.Length; i++)
		{
			Weapon weapon = WeaponFactory.GetInstance().CreateWeapon((byte)i);
			weapon.LoadConfig(ownedWeapons[i]);
			weapon.SetLevel(ownedWeapons[i]);
			weaponList.Add(weapon.GunID, weapon);
		}
	}

	public void EquippedWeaponUnlockPatch()
	{
		bool flag = false;
		short[] mWeaponIdArray = mCoopCustomizeData.mWeaponIdArray;
		foreach (short num in mWeaponIdArray)
		{
			if (weaponList.ContainsKey(num) && weaponList[num] != null && (weaponList[num].Level == 0 || weaponList[num].Level == 15) && !weaponList[num].EquippedUnlockPatch())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			mCoopCustomizeData.mWeaponIdArray = new short[3];
			mCoopCustomizeData.mWeaponIdArray[0] = 1;
			for (int j = 1; j < 3; j++)
			{
				mCoopCustomizeData.mWeaponIdArray[j] = 0;
			}
		}
	}

	public void InitWeaponUpgrade()
	{
		weaponUpgrade.LoadConfig();
	}

	public void InitItems(byte[] ownedItems)
	{
		itemList.Clear();
		for (int i = 0; i < ownedItems.Length; i++)
		{
			Item item = new Item();
			item.LoadConfig(i);
			item.SetCount(ownedItems[i]);
			itemList.Add(item.GetItemID(), item);
		}
	}

	public void InitLevelName()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.LEVEL, DataSheetName.LEVEL_NAME);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int num = gameDataTable.GetInt(i, 0);
			int num2 = gameDataTable.GetInt(i, 1);
			LevelConfig levelConfig = new LevelConfig();
			levelConfig.levelName = gameDataTable.GetString(i, 2);
			levelConfig.spawnPointId = gameDataTable.GetInt(i, 3);
			levelConfig.enemySpawnDoorId = gameDataTable.GetInt(i, 4);
			levelConfig.enemySpawnGraveId = gameDataTable.GetInt(i, 5);
			levelConfig.mapName = gameDataTable.GetString(i, 6);
			float x = gameDataTable.GetFloat(i, 7);
			float z = gameDataTable.GetFloat(i, 8);
			levelConfig.topLeft = new Vector3(x, 0f, z);
			float x2 = gameDataTable.GetFloat(i, 9);
			float z2 = gameDataTable.GetFloat(i, 10);
			levelConfig.bottomRight = new Vector3(x2, 0f, z2);
			levelConfig.locId = gameDataTable.GetString(i, 11);
			mLevelConfigArray[num - 1, num2] = levelConfig;
		}
		gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.LEVEL, DataSheetName.MULTI_LEVEL_NAME);
		for (int j = 0; j < gameDataTable.Count; j++)
		{
			int num3 = gameDataTable.GetInt(j, 1);
			LevelConfig levelConfig2 = new LevelConfig();
			levelConfig2.levelName = gameDataTable.GetString(j, 2);
			levelConfig2.spawnPointId = gameDataTable.GetInt(j, 3);
			levelConfig2.enemySpawnDoorId = gameDataTable.GetInt(j, 4);
			levelConfig2.enemySpawnGraveId = gameDataTable.GetInt(j, 5);
			levelConfig2.mapName = gameDataTable.GetString(j, 6);
			float x3 = gameDataTable.GetFloat(j, 7);
			float z3 = gameDataTable.GetFloat(j, 8);
			levelConfig2.topLeft = new Vector3(x3, 0f, z3);
			float x4 = gameDataTable.GetFloat(j, 9);
			float z4 = gameDataTable.GetFloat(j, 10);
			levelConfig2.bottomRight = new Vector3(x4, 0f, z4);
			levelConfig2.locId = gameDataTable.GetString(j, 11);
			mVSLevelConfigArray[0, num3] = levelConfig2;
		}
	}

	public LevelConfig GetLevelConfig(int stage, int subStage)
	{
		return mLevelConfigArray[stage, subStage];
	}

	public LevelConfig GetVSLevelConfig(int stage, int subStage)
	{
		return mVSLevelConfigArray[stage, subStage];
	}

	public LevelConfig GetCurrentLevelConfig()
	{
		return mLevelConfigArray[mSelectedStageIndex, mSelectedSubStageIndex];
	}

	public void InitArmors(byte[] ownedHeads, byte[] ownedBodies, byte[] ownedArms, byte[] ownedFeet, byte[] ownedBag, string[] mHeadBuyVer, string[] mBodyBuyVer, string[] mHandBuyVer, string[] mFootBuyVer, string[] mBagBuyVer)
	{
		ClearArmors();
		List<Armor> list = new List<Armor>();
		for (int i = 0; i < ownedHeads.Length; i++)
		{
			Armor armor = new Armor(BodyType.Head, i + 1);
			int resID = i * 4;
			armor.LoadConfig(ownedHeads[i], resID);
			armor.SetLevel(ownedHeads[i]);
			armor.SetBuyVer(mHeadBuyVer[i]);
			list.Add(armor);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int j = 0; j < ownedBodies.Length; j++)
		{
			Armor armor2 = new Armor(BodyType.Body, j + 1);
			int resID2 = j * 4 + 1;
			armor2.LoadConfig(ownedBodies[j], resID2);
			armor2.SetLevel(ownedBodies[j]);
			armor2.SetBuyVer(mBodyBuyVer[j]);
			list.Add(armor2);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int k = 0; k < ownedArms.Length; k++)
		{
			Armor armor3 = new Armor(BodyType.Arm, k + 1);
			int resID3 = k * 4 + 2;
			armor3.LoadConfig(ownedArms[k], resID3);
			armor3.SetLevel(ownedArms[k]);
			armor3.SetBuyVer(mHandBuyVer[k]);
			list.Add(armor3);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int l = 0; l < ownedFeet.Length; l++)
		{
			Armor armor4 = new Armor(BodyType.Foot, l + 1);
			int resID4 = l * 4 + 3;
			armor4.LoadConfig(ownedFeet[l], resID4);
			armor4.SetLevel(ownedFeet[l]);
			armor4.SetBuyVer(mFootBuyVer[l]);
			list.Add(armor4);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int m = 0; m < ownedBag.Length; m++)
		{
			Armor armor5 = new Armor(BodyType.Bag, m + 1);
			int resID5 = m + 4 * Global.TOTAL_ARMOR_NUM_IN_EXCEL;
			armor5.LoadConfig(ownedBag[m], resID5);
			armor5.SetLevel(ownedBag[m]);
			armor5.SetBuyVer(mBagBuyVer[m]);
			list.Add(armor5);
		}
		armorList.Add(list);
		UnlockEquipsByLevel();
	}

	public void InitArmors(byte[] ownedHeads, byte[] ownedBodies, byte[] ownedArms, byte[] ownedFeet, byte[] ownedBag)
	{
		ClearArmors();
		List<Armor> list = new List<Armor>();
		for (int i = 0; i < ownedHeads.Length; i++)
		{
			Armor armor = new Armor(BodyType.Head, i + 1);
			int resID = i * 4;
			armor.LoadConfig(ownedHeads[i], resID);
			armor.SetLevel(ownedHeads[i]);
			list.Add(armor);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int j = 0; j < ownedBodies.Length; j++)
		{
			Armor armor2 = new Armor(BodyType.Body, j + 1);
			int resID2 = j * 4 + 1;
			armor2.LoadConfig(ownedBodies[j], resID2);
			armor2.SetLevel(ownedBodies[j]);
			list.Add(armor2);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int k = 0; k < ownedArms.Length; k++)
		{
			Armor armor3 = new Armor(BodyType.Arm, k + 1);
			int resID3 = k * 4 + 2;
			armor3.LoadConfig(ownedArms[k], resID3);
			armor3.SetLevel(ownedArms[k]);
			list.Add(armor3);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int l = 0; l < ownedFeet.Length; l++)
		{
			Armor armor4 = new Armor(BodyType.Foot, l + 1);
			int resID4 = l * 4 + 3;
			armor4.LoadConfig(ownedFeet[l], resID4);
			armor4.SetLevel(ownedFeet[l]);
			list.Add(armor4);
		}
		armorList.Add(list);
		list = new List<Armor>();
		for (int m = 0; m < ownedBag.Length; m++)
		{
			Armor armor5 = new Armor(BodyType.Bag, m + 1);
			int resID5 = m + 4 * Global.TOTAL_ARMOR_NUM_IN_EXCEL;
			armor5.LoadConfig(ownedBag[m], resID5);
			armor5.SetLevel(ownedBag[m]);
			list.Add(armor5);
		}
		armorList.Add(list);
	}

	public void InitRank()
	{
		if (rankList == null)
		{
			rankList = new List<Rank>();
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.PVP_RANK);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			Rank rank = new Rank(i);
			rank.LoadConfig();
			rankList.Add(rank);
		}
	}

	public void CheckPackagePushData()
	{
		if ((DateTime.Now - mPackagePushResetTime).Days > 0)
		{
			mPackagePushResetTime = DateTime.Today;
			mLoseRookiePushTimes = 0;
			mIsWinRookiePush = false;
			mLoseExpertPushTimes = 0;
			mIsWinExpertPush = false;
		}
	}

	public void UseEnegy(int count)
	{
	}

	public void EnterGodMode()
	{
		if (GetMithril() <= 0)
		{
			SetMithril(100000000);
		}
		if (GetCash() <= 0)
		{
			SetCash(100000000);
		}
	}

	public Weapon GetWeaponInBag(int bagIndex)
	{
		int num = mCurrentCustomizeData.mWeaponIdArray[bagIndex];
		if (mTrialWeaponID > 0)
		{
			num = mTrialWeaponID;
		}
		if (num != 0 && num < 80)
		{
			return weaponList[num];
		}
		return null;
	}

	public int GetWeaponBagIndex(Weapon w)
	{
		if (mTrialWeaponID > 0)
		{
			return 0;
		}
		for (int i = 0; i < mCurrentCustomizeData.mWeaponIdArray.Length; i++)
		{
			if (mCurrentCustomizeData.mWeaponIdArray[i] == w.GetGunID())
			{
				return i;
			}
		}
		return 0;
	}

	public List<Weapon> GetBattleWeapons()
	{
		List<Weapon> list = new List<Weapon>();
		if (mTrialWeaponID > 0)
		{
			list.Add(weaponList[mTrialWeaponID]);
		}
		else
		{
			for (int i = 0; i < mCurrentCustomizeData.mWeaponIdArray.Length; i++)
			{
				if (mCurrentCustomizeData.mWeaponIdArray[i] > 0)
				{
					list.Add(weaponList[mCurrentCustomizeData.mWeaponIdArray[i]]);
				}
			}
		}
		return list;
	}

	public short[] GetBattleWeaponIDs()
	{
		if (mTrialWeaponID > 0)
		{
			return new short[1] { mTrialWeaponID };
		}
		return mCurrentCustomizeData.mWeaponIdArray;
	}

	public void SetBattleWeaponID(int index, int id)
	{
		if (index < mCurrentCustomizeData.mWeaponIdArray.Length)
		{
			mCurrentCustomizeData.mWeaponIdArray[index] = (short)id;
		}
	}

	public void SetBattleWeaponID(byte[] weaponIDs)
	{
		for (int i = 0; i < weaponIDs.Length; i++)
		{
			mCurrentCustomizeData.mWeaponIdArray[i] = weaponIDs[i];
		}
	}

	public short[] GetBattleItemIDs()
	{
		return mCurrentCustomizeData.mItemIdArray;
	}

	public void SetBattleItemID(int index, int id)
	{
		if (index < mCurrentCustomizeData.mItemIdArray.Length)
		{
			mCurrentCustomizeData.mItemIdArray[index] = (short)id;
		}
	}

	public void SetBattleItemID(byte[] itemIDs)
	{
		for (int i = 0; i < itemIDs.Length; i++)
		{
			mCurrentCustomizeData.mItemIdArray[i] = itemIDs[i];
		}
	}

	private void AddExpendDollar(int dols)
	{
		if (OperInfo != null)
		{
		}
	}

	public void DeliverIAPItem(IAPName iapName)
	{
		Debug.Log("DeliverIAPItem " + iapName);
		switch (iapName)
		{
		case IAPName.M10:
			AddMithril(10);
			break;
		case IAPName.M24:
			AddMithril(24);
			break;
		case IAPName.M72:
			AddMithril(72);
			break;
		case IAPName.M168:
			AddMithril(168);
			break;
		case IAPName.M666:
			AddMithril(666);
			break;
		case IAPName.M1430:
			AddMithril(1430);
			break;
		case IAPName.M4000:
			AddMithril(4000);
			break;
		}
		GameApp.GetInstance().Save();
	}

	public int OwnedSuitCount()
	{
		int num = 0;
		for (int i = 0; i < Global.TOTAL_ARMOR_NUM; i++)
		{
			bool flag = true;
			for (int j = 0; j < Global.AVATAR_PART_NUM - 1; j++)
			{
				if (armorList[j][i].Level == 0 || armorList[j][i].Level == 15)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				num++;
			}
		}
		return num;
	}

	public bool OwnSuit(int suitID)
	{
		bool result = true;
		for (int i = 0; i < Global.AVATAR_PART_NUM - 1; i++)
		{
			if (armorList[i][suitID].Level == 0 || armorList[i][suitID].Level == 15)
			{
				return false;
			}
		}
		return result;
	}

	public void UseFlaskItems(bool isCoop)
	{
		mFlaskItemList = new List<Item>();
		foreach (Item value in itemList.Values)
		{
			if (value.GetItemType() == Item.ItemType.Flask && value.GetCount() > 0 && ((isCoop && value.GetFlaskSkillsType() != SkillsType.EXP_BOOTH) || (!isCoop && value.GetFlaskSkillsType() != SkillsType.MONEY_BOOTH)))
			{
				mFlaskItemList.Add(value);
				value.Use(1);
				Achievement.AddFlaskUsed(value.GetItemID());
			}
		}
	}

	public List<Item> GetFlaskItemList()
	{
		return mFlaskItemList;
	}

	public bool GetTutorialWatched(int i)
	{
		if (i < 18)
		{
			return mTutorialWatched[i];
		}
		Debug.Log("Out of Index!");
		return true;
	}

	public void SetTutorialWatched(bool value, int i)
	{
		if (i < 18)
		{
			mTutorialWatched[i] = value;
		}
		else
		{
			Debug.Log("Out of Index!");
		}
	}

	public bool GetItemHide()
	{
		return mEnableItemHide;
	}

	public void SetItemHide(bool value)
	{
		mEnableItemHide = value;
	}

	public bool GetSkipDialog()
	{
		return mSkipDialog;
	}

	public void SetSkipDialog(bool value)
	{
		mSkipDialog = value;
	}

	public bool GetAutoHide()
	{
		return mAutoHideButton;
	}

	public void SetAutoHide(bool value)
	{
		mAutoHideButton = value;
	}

	public bool GetChangeSide()
	{
		return mChangeJoystickSide;
	}

	public void SetChangeSide(bool value)
	{
		mChangeJoystickSide = value;
	}

	public bool GetSkipTutorial()
	{
		return mSkipTutorial;
	}

	public void SetSkipTutorial(bool value)
	{
		mSkipTutorial = value;
	}

	public bool GetHorizontalJoyStickRevert()
	{
		return mLeftJoystickRevert;
	}

	public void SetHorizontalJoyStickRevert(bool value)
	{
		mLeftJoystickRevert = value;
	}

	public bool GetVerticalJoyStickRevert()
	{
		return mRightJoystickRevert;
	}

	public void SetVerticalJoyStickRevert(bool value)
	{
		mRightJoystickRevert = value;
	}

	public bool GetPlayMusic()
	{
		return bPlayMusic;
	}

	public void SetPlayMusic(bool isPlay)
	{
		bPlayMusic = isPlay;
		if (AudioManager.GetInstance() != null)
		{
			AudioManager.GetInstance().SetMusicMute(!bPlayMusic);
		}
	}

	public bool GetPlaySound()
	{
		return bPlaySound;
	}

	public void SetPlaySound(bool isPlay)
	{
		bPlaySound = isPlay;
		if (AudioManager.GetInstance() != null)
		{
			AudioManager.GetInstance().SetSoundMute(!bPlaySound);
		}
	}

	public bool GetGravity()
	{
		return mEnableGaravity;
	}

	public void SetGravity(bool value)
	{
		mEnableGaravity = value;
	}

	public float GetMusicVolume()
	{
		return musicVolume;
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		if (AudioManager.GetInstance() != null)
		{
			AudioManager.GetInstance().SetMusicVolume(volume);
		}
	}

	public float GetSoundVolume()
	{
		return soundVolume;
	}

	public void SetSoundVolume(float volume)
	{
		soundVolume = volume;
		if (AudioManager.GetInstance() != null)
		{
			AudioManager.GetInstance().SetSoundVolume(volume);
		}
	}

	public EUserLanguage GetUserLanguage()
	{
		return userLanguage;
	}

	public void SetUserLanguage(EUserLanguage language)
	{
		userLanguage = language;
	}

	public void SetAvatar(BodyType bodyType, int avatarID)
	{
		ChangeAvatar(bodyType, avatarID);
		mCurrentCustomizeData.mAvatarIdArray[(int)bodyType] = (short)avatarID;
	}

	public void SetAvatar(short[] armorIDs)
	{
		for (int i = 0; i < armorIDs.Length; i++)
		{
			ChangeAvatar((BodyType)i, armorIDs[i]);
			mCurrentCustomizeData.mAvatarIdArray[i] = armorIDs[i];
		}
	}

	public void ChangeAvatar(BodyType bodyType, int avatarID)
	{
		int num = mCurrentCustomizeData.mAvatarIdArray[(int)bodyType];
		if (avatarID != num)
		{
			Armor armor = GetArmor((int)bodyType, num);
			if (armor != null)
			{
				armor.TakeOff();
			}
			Armor armor2 = GetArmor((int)bodyType, avatarID);
			armor2.TakeOn(playerSkill);
			mCurrentArmorSet.SetArmorElement(bodyType, armor2.SetElement, armor2.SuitId, playerSkill);
			playerSkill.PlayerEquipmentTrigger(this);
		}
	}

	public short[] GetAvatar()
	{
		return mCurrentCustomizeData.mAvatarIdArray;
	}

	public Armor GetArmor(int part)
	{
		List<Armor> list = GetArmor()[part];
		foreach (Armor item in list)
		{
			if (item.GetArmorID() == mCurrentCustomizeData.mAvatarIdArray[part])
			{
				return item;
			}
		}
		return null;
	}

	public Armor GetArmor(int part, int id)
	{
		List<Armor> list = GetArmor()[part];
		foreach (Armor item in list)
		{
			if (item.GetArmorID() == id)
			{
				return item;
			}
		}
		return null;
	}

	public Armor GetArmorByUniqueID(int id)
	{
		foreach (List<Armor> armor in armorList)
		{
			foreach (Armor item in armor)
			{
				if (item.GetUniqueID() == id)
				{
					return item;
				}
			}
		}
		Debug.LogError("GetArmorByUniqueID could not find with : " + id);
		return null;
	}

	public WeaponUpgrade GetWeaponUpgrade()
	{
		return weaponUpgrade;
	}

	public void ClearArmors()
	{
		for (int i = 0; i < armorList.Count; i++)
		{
			armorList[i].Clear();
		}
		armorList.Clear();
	}

	public Rank GetRank(int exp)
	{
		if (rankList == null)
		{
			InitRank();
		}
		for (int num = rankList.Count - 1; num >= 0; num--)
		{
			if (rankList[num].exp <= exp)
			{
				return rankList[num];
			}
		}
		return rankList[0];
	}

	public Rank GetRank()
	{
		return GetRank(GetExp());
	}

	public List<Rank> GetRankList()
	{
		return rankList;
	}

	public int GetLowestPrestigeForPerk(int index)
	{
		if (mPerkSizeList == null)
		{
			mPerkSizeList = new List<int>();
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PERK_SIZE);
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				mPerkSizeList.Add(gameDataTable.GetInt(i, 1));
			}
		}
		for (int j = 0; j < mPerkSizeList.Count; j++)
		{
			if (mPerkSizeList[j] > index)
			{
				return j;
			}
		}
		return 0;
	}

	public List<Weapon> GetWeaponListForUnLock()
	{
		return new List<Weapon>();
	}

	public bool HaveWeaponToUpgrade()
	{
		bool result = false;
		foreach (Weapon value in weaponList.Values)
		{
			if (value.Level > 0 && value.Level < Global.MAX_LEVEL_WEAPONW)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public PvpSkillManager GetPvpSkillManager()
	{
		return mPvpSkillManager;
	}

	public void ChangePerk(int oldId, int newId)
	{
		if (newId != oldId)
		{
			PvpSkill perkById = mPvpSkillManager.GetPerkById(oldId);
			if (perkById != null)
			{
				perkById.TakeOff();
			}
			PvpSkill perkById2 = mPvpSkillManager.GetPerkById(newId);
			if (perkById2 != null)
			{
				perkById2.TakeOn(playerSkill);
			}
		}
	}

	public void ChangeKillStreak(int oldId, int newId)
	{
		if (newId != oldId)
		{
			PvpSkill killStreakById = mPvpSkillManager.GetKillStreakById(oldId);
			if (killStreakById != null)
			{
				killStreakById.TakeOff();
			}
			PvpSkill killStreakById2 = mPvpSkillManager.GetKillStreakById(newId);
			if (killStreakById2 != null)
			{
				killStreakById2.TakeOn(playerSkill);
			}
		}
	}

	public MissionState GetNormalMissionState()
	{
		return mNormalMissionState;
	}

	public void SetNormalMissionState(MissionState state)
	{
		mNormalMissionState = state;
	}

	public MissionState GetHeroicMissionState()
	{
		return mHeroicMissionState;
	}

	public void SetHeroicMissionState(MissionState state)
	{
		mHeroicMissionState = state;
	}

	public bool GetMissionStatus(int i, int j, int k)
	{
		return GetMissionStatus(i, j, k, GetSelectedDifficultMode());
	}

	public bool GetMissionStatus(int i, int j, int k, EDifficultMode diffMode)
	{
		switch (diffMode)
		{
		case EDifficultMode.NORMAL:
			return mNormalMissionState.GetMissionStatus(i, j, k);
		case EDifficultMode.HEROIC:
			return mHeroicMissionState.GetMissionStatus(i, j, k);
		default:
			return false;
		}
	}

	public void SetMissionStatus(int i, int j, int k, bool status)
	{
		if (GetSelectedDifficultMode() == EDifficultMode.NORMAL)
		{
			mNormalMissionState.SetMissionStatus(i, j, k, status);
		}
		else if (GetSelectedDifficultMode() == EDifficultMode.HEROIC)
		{
			mHeroicMissionState.SetMissionStatus(i, j, k, status);
		}
	}

	public void SetUnlockedStageId(byte stage)
	{
		if (GameApp.GetInstance().GetUserState().AnalysisManager.GetPackageBuy(Global.IAP_UNLOCK_LEVEL_WEAPON_INDEX) == 0)
		{
			if (GetSelectedDifficultMode() == EDifficultMode.NORMAL)
			{
				mNormalMissionState.SetUnlockedStageId(stage);
			}
			else if (GetSelectedDifficultMode() == EDifficultMode.HEROIC)
			{
				mHeroicMissionState.SetUnlockedStageId(stage);
			}
		}
	}

	public byte GetUnlockedStageId()
	{
		return GetUnlockedStageId(GetSelectedDifficultMode());
	}

	public byte GetUnlockedStageId(EDifficultMode diffMode)
	{
		switch (diffMode)
		{
		case EDifficultMode.NORMAL:
			return mNormalMissionState.GetUnlockedStageId();
		case EDifficultMode.HEROIC:
			return mHeroicMissionState.GetUnlockedStageId();
		default:
			return 0;
		}
	}

	public void SetUnlockedSubStageId(byte subStage)
	{
		if (GameApp.GetInstance().GetUserState().AnalysisManager.GetPackageBuy(Global.IAP_UNLOCK_LEVEL_WEAPON_INDEX) == 0)
		{
			if (GetSelectedDifficultMode() == EDifficultMode.NORMAL)
			{
				mNormalMissionState.SetUnlockedSubStageId(subStage);
			}
			else if (GetSelectedDifficultMode() == EDifficultMode.HEROIC)
			{
				mHeroicMissionState.SetUnlockedSubStageId(subStage);
			}
		}
	}

	public byte GetUnlockedSubStageId()
	{
		return GetUnlockedSubStageId(GetSelectedDifficultMode());
	}

	public byte GetUnlockedSubStageId(EDifficultMode diffMode)
	{
		switch (diffMode)
		{
		case EDifficultMode.NORMAL:
			return mNormalMissionState.GetUnlockedSubStageId();
		case EDifficultMode.HEROIC:
			return mHeroicMissionState.GetUnlockedSubStageId();
		default:
			return 0;
		}
	}

	public void UnlockAllLevel_Weapons_Equips()
	{
		if (mNormalMissionState.GetUnlockedStageId() < Global.TOTAL_ACTIVATED_STAGE)
		{
			mNormalMissionState.SetUnlockedStageId((byte)Global.TOTAL_ACTIVATED_STAGE);
		}
		if (mHeroicMissionState.GetUnlockedStageId() < Global.TOTAL_ACTIVATED_STAGE)
		{
			mHeroicMissionState.SetUnlockedStageId((byte)Global.TOTAL_ACTIVATED_STAGE);
		}
		mNormalMissionState.SetUnlockedSubStageId(1);
		mHeroicMissionState.SetUnlockedSubStageId(1);
		SetUnlockedDifficultMode(EDifficultMode.HEROIC);
		UnlockEquipsByLevel(EDifficultMode.DIFFICULTY_COUNT, (byte)Global.TOTAL_STAGE, (byte)Global.TOTAL_SUB_STAGE, (byte)Global.TOTAL_STAGE, (byte)Global.TOTAL_SUB_STAGE);
	}

	public void SetFollowTwitter(bool twitter)
	{
		mFollowTwitter = twitter;
	}

	public bool GetFollowTwitter()
	{
		return mFollowTwitter;
	}

	public void SetFollowFacebook(bool facebook)
	{
		mFollowFacebook = facebook;
	}

	public bool GetFollowFacebook()
	{
		return mFollowFacebook;
	}

	public void SetFirstIapSuccess(bool success)
	{
		mFirstIapSuccess = success;
	}

	public bool GetFirstIapSuccess()
	{
		return mFirstIapSuccess;
	}

	public int GetCurrentTitleID()
	{
		return mCurrentTitleId;
	}

	public void SetCurrentTitleId(int num)
	{
		mCurrentTitleId = num;
	}

	public int GetCurrentTilteFrameID()
	{
		return mCurrentTitleFrameId;
	}

	public void SetCurrentTitleFrameId(int num)
	{
		mCurrentTitleFrameId = num;
	}

	public byte GetDeadTimer()
	{
		return deadTimer;
	}

	public void SetDeadTimer(byte timer)
	{
		deadTimer = timer;
	}

	public byte AtomicDeadTimer()
	{
		deadTimer++;
		deadTimer %= 3;
		return deadTimer;
	}

	public bool GetGuestRegisted()
	{
		return mGuestRegisted;
	}

	public void SetGuestRegisted(bool registed)
	{
		mGuestRegisted = registed;
	}

	public bool GetOnlineRegisted()
	{
		return mOnlineRegisted;
	}

	public void SetOnlineRegisted(bool registed)
	{
		mOnlineRegisted = registed;
	}

	public bool HasLoginOnline()
	{
		return mOnlineRegisted || mGuestRegisted;
	}

	public bool DontMindIfRegisted()
	{
		return true;
	}

	public string GetOnlineEmail()
	{
		return mOnlineEmail;
	}

	public void SetOnlineEmail(string email)
	{
		mOnlineEmail = email;
	}

	public string GetOnlineNickname()
	{
		return mOnlineNickname;
	}

	public void SetOnlineNickname(string name)
	{
		mOnlineNickname = name;
	}

	public string GetOnlinePassword()
	{
		return mOnlinePassword;
	}

	public void SetOnlinePassword(string password)
	{
		mOnlinePassword = password;
	}

	public int GetOnlineUserId()
	{
		return mOnlineUserId;
	}

	public void SetOnlineUserId(int id)
	{
		mOnlineUserId = id;
	}

	public string GetServerAddress()
	{
		return mServerAddress;
	}

	public void SetServerAddress(string address)
	{
		mServerAddress = address;
	}

	public int GetVoidServerPort()
	{
		return mVoidServerPort;
	}

	public void SetVoidServerPort(int port)
	{
		mVoidServerPort = port;
	}

	public int GetFriendServerPort()
	{
		return mFriendServerPort;
	}

	public void SetFriendServerPort(int port)
	{
		mFriendServerPort = port;
	}

	public EDifficultMode GetUnlockedDifficultMode()
	{
		return mUnlockedDifficultMode;
	}

	public void SetUnlockedDifficultMode(EDifficultMode difficultMode)
	{
		mUnlockedDifficultMode = difficultMode;
	}

	public EDifficultMode GetSelectedDifficultMode()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			return mNetDifficultMode;
		}
		return mSelectedDifficultMode;
	}

	public void SetSelectedDifficultMode(EDifficultMode difficultMode)
	{
		mSelectedDifficultMode = difficultMode;
	}

	public byte GetSelectedStageIndex()
	{
		return mSelectedStageIndex;
	}

	public void SetSelectedStageIndex(byte stage)
	{
		mSelectedStageIndex = stage;
	}

	public byte GetSelectedSubStageIndex()
	{
		return mSelectedSubStageIndex;
	}

	public void SetSelectedSubStageIndex(byte subStage)
	{
		mSelectedSubStageIndex = subStage;
	}

	public byte GetSelectedNetIndex()
	{
		return mSelectNetStageIdx;
	}

	public void SetSelectedNetIndex(byte netStage)
	{
		mSelectNetStageIdx = netStage;
	}

	public bool GetHasGetFirstBossVoidMaterials()
	{
		return hasGetFirstBossVoidMaterials;
	}

	public void SetHasGetFirstBossVoidMaterials(bool hasGetFirstBossVoidMaterials)
	{
		this.hasGetFirstBossVoidMaterials = hasGetFirstBossVoidMaterials;
	}

	public Dictionary<int, Weapon> GetWeapons()
	{
		return weaponList;
	}

	public Dictionary<int, Item> GetItems()
	{
		return itemList;
	}

	public Item GetItemById(int id)
	{
		if (itemList.ContainsKey(id))
		{
			return itemList[id];
		}
		return null;
	}

	public Item GetItemBySkill(int mExternalSkillId)
	{
		foreach (Item value in itemList.Values)
		{
			if (value.GetExternalId() == mExternalSkillId)
			{
				return value;
			}
		}
		return null;
	}

	public List<List<Armor>> GetArmor()
	{
		return armorList;
	}

	public int GetMithril()
	{
		lock (syncMithrilLock)
		{
			return AntiCracking.DecryptBufferStr(mithril, "sw_mod");
		}
	}

	public void SetMithril(int _mithril)
	{
		lock (syncMithrilLock)
		{
			mithril = AntiCracking.CryptBufferStr(Mathf.Clamp(_mithril, 0, Global.MAX_MITHRIL), "sw_mod");
		}
	}

	public void AddMithril(int _mithrilGot)
	{
		lock (syncMithrilLock)
		{
			SetMithril(Mathf.Clamp(GetMithril() + _mithrilGot, 0, Global.MAX_MITHRIL));
		}
	}

	public int GetBonusMithril()
	{
		return AntiCracking.DecryptBufferStr(mBonusMithril, "jia_miyin");
	}

	public void SetBonusMithril(int _mithril)
	{
		mBonusMithril = AntiCracking.CryptBufferStr(Mathf.Min(Global.MAX_MITHRIL, _mithril), "jia_miyin");
	}

	public int GetCompensateMithril()
	{
		return AntiCracking.DecryptBufferStr(mCompensateMithril, "jia_miyin");
	}

	public void SetCompensateMithril(int _mithril)
	{
		mCompensateMithril = AntiCracking.CryptBufferStr(Mathf.Min(Global.MAX_MITHRIL, _mithril), "jia_miyin");
	}

	public int GetBonusGold()
	{
		return AntiCracking.DecryptBufferStr(mBonusGold, "jia_qian");
	}

	public void SetBonusGold(int _gold)
	{
		mBonusGold = AntiCracking.CryptBufferStr(Mathf.Min(Global.MAX_CASH, _gold), "jia_qian");
	}

	public int GetCompensateGold()
	{
		return AntiCracking.DecryptBufferStr(mCompensateGold, "jia_qian");
	}

	public void SetCompensateGold(int _gold)
	{
		mCompensateGold = AntiCracking.CryptBufferStr(Mathf.Min(Global.MAX_CASH, _gold), "jia_qian");
	}

	public int GetCompensateExp()
	{
		return AntiCracking.DecryptBufferStr(mCompensateExp, "jia_jingyan");
	}

	public void SetCompensateExp(int _exp)
	{
		mCompensateExp = AntiCracking.CryptBufferStr(Mathf.Min(Global.MAX_EXP, _exp), "jia_jingyan");
	}

	public bool GetCompensateMaterial()
	{
		return mCompensateMaterial;
	}

	public void SetCompensateMaterial(bool _material)
	{
		mCompensateMaterial = _material;
	}

	public Dictionary<short, short> GetCompensateMaterialList()
	{
		return mCompensateMaterialList;
	}

	public void SetCompensateMaterialList(Dictionary<short, short> list)
	{
		mCompensateMaterialList = list;
	}

	public int GetExp()
	{
		return GetOnlinePlayer().GetExp();
	}

	public void SetExp(int exp)
	{
		GetOnlinePlayer().SetExp(exp);
	}

	public void AddExp(int exp)
	{
		GetOnlinePlayer().AddExp(exp);
		if (Achievement != null)
		{
			Achievement.CheckAchivement_PVPLEVEL(GetRank().rankID);
		}
	}

	public void SetPrestige(byte prestige)
	{
		GetOnlinePlayer().SetPrestige(prestige);
	}

	public byte GetPrestige()
	{
		return GetOnlinePlayer().GetPrestige();
	}

	public int GetCash()
	{
		return AntiCracking.DecryptBufferStr(cash, "sw_acc");
	}

	public void SetCash(int _cash)
	{
		cash = AntiCracking.CryptBufferStr(Mathf.Clamp(_cash, 0, Global.MAX_CASH), "sw_acc");
	}

	public void AddCash(int _cash)
	{
		SetCash(Mathf.Clamp(GetCash() + _cash, 0, Global.MAX_CASH));
	}

	public void Buy(int price)
	{
		SetCash(Mathf.Clamp(GetCash() - price, 0, Global.MAX_CASH));
	}

	public void BuyWithMithril(int price)
	{
		lock (syncMithrilLock)
		{
			SetMithril(Mathf.Clamp(GetMithril() - price, 0, Global.MAX_MITHRIL));
		}
	}

	public FriendType GetFriendType()
	{
		return mFriendType;
	}

	public void SetFriendType(FriendType type)
	{
		mFriendType = type;
	}

	public string GetTwitterName()
	{
		return mTwitterName;
	}

	public void SetTwitterName(string name)
	{
		mTwitterName = name;
	}

	public long GetFacebookId()
	{
		return mFacebookId;
	}

	public void SetFacebookId(long id)
	{
		mFacebookId = id;
	}

	public long GetGameCenterId()
	{
		return mGameCenterId;
	}

	public void SetGameCenterId(long id)
	{
		mGameCenterId = id;
	}

	public DateTime GetSyncRecordTime()
	{
		return mSyncRecordTime;
	}

	public void SetSyncRecordTime(DateTime time)
	{
		mSyncRecordTime = time;
	}

	public DateTime GetRemindSyncRecordTime()
	{
		return mRemindSyncRecordTime;
	}

	public void SetRemindSyncRecordTime(DateTime time)
	{
		mRemindSyncRecordTime = time;
	}

	public long GetLastLoginTicks()
	{
		return mLastLoginTicks;
	}

	public void SetLastLoginTicks(long ticks)
	{
		mLastLoginTicks = ticks;
	}

	public long GetNextLoginInterval()
	{
		return mNextLoginInterval;
	}

	public void SetNextLoginInterval(long interval)
	{
		mNextLoginInterval = interval;
	}

	public int GetPlayTimesAfterIgnoreRating()
	{
		return mPlayTimesAfterIgnoreRating;
	}

	public void AddPlayTimesAfterIgnoreRating()
	{
		mPlayTimesAfterIgnoreRating++;
	}

	public void ResetPlayTimesAfterIgnoreRating(bool showAgain)
	{
		if (showAgain)
		{
			mPlayTimesAfterIgnoreRating = 0;
		}
		else
		{
			mPlayTimesAfterIgnoreRating = -1;
		}
	}

	public int GetSaveNum()
	{
		return saveNum;
	}

	public void SetSaveNum(int saveNum)
	{
		this.saveNum = saveNum;
	}

	public void SetSaveIdentity(string saveIdentity)
	{
		this.saveIdentity = saveIdentity;
	}

	public string GetSaveIdentity()
	{
		return saveIdentity;
	}

	public void SetTotalPlayTime(int time)
	{
		totalPlayTime = time;
	}

	public int GetTotalPlayTime()
	{
		return totalPlayTime;
	}

	public void AddTotalPlayTime(int deltaTime)
	{
		totalPlayTime += deltaTime;
	}

	public void SetNeedResetFriendTime(bool need)
	{
		needResetFriendTime = need;
	}

	public bool GetNeedResetFriendTime()
	{
		return needResetFriendTime;
	}

	public void CheckNeedResetFriendTime(bool needReset)
	{
		if (!needResetFriendTime)
		{
			needResetFriendTime = needReset;
		}
	}

	public bool NeedShowExtraGoldAndMithril()
	{
		return GetBonusMithril() != 0 || GetCompensateMithril() != 0 || GetBonusGold() != 0 || GetCompensateGold() != 0 || GetCompensateExp() != 0;
	}

	public void ApplyExtraGoldAndMithril()
	{
		bool flag = GetBonusMithril() != 0 || GetCompensateMithril() != 0;
		bool flag2 = GetBonusGold() != 0 || GetCompensateGold() != 0;
		AnalysisManager.mithrilInfo.OfficialMithril += GetBonusMithril();
		AnalysisManager.mithrilInfo.OfficialMithril += GetCompensateMithril();
		AnalysisManager.goldInfo.OfficialGold += GetBonusGold();
		AnalysisManager.goldInfo.OfficialGold += GetCompensateGold();
		AddMithril(GetBonusMithril());
		SetBonusMithril(0);
		AddMithril(GetCompensateMithril());
		SetCompensateMithril(0);
		AddCash(GetBonusGold());
		SetBonusGold(0);
		AddCash(GetCompensateGold());
		SetCompensateGold(0);
		if (GetCompensateExp() > 0)
		{
			AddCompensateExpRequest request = new AddCompensateExpRequest(GetCompensateExp());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		SetCompensateExp(0);
		GameApp.GetInstance().Save();
		if (flag)
		{
			SyncMithrilRequest request2 = new SyncMithrilRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
		if (flag2)
		{
			SyncCashRequest request3 = new SyncCashRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
		}
	}

	public OnlineCustomizeData GetCoopCustomize()
	{
		return mCoopCustomizeData;
	}

	public void SetCoopCustomize(OnlineCustomizeData data)
	{
		mCoopCustomizeData = new OnlineCustomizeData(data);
	}

	public void InitCurrentCustomize(OnlineCustomizeData data)
	{
		mCurrentCustomizeData = new OnlineCustomizeData(data);
		playerSkill.init(mCurrentCustomizeData, this);
		mCoopCustomizeData = new OnlineCustomizeData(mCurrentCustomizeData);
	}

	public void LoadCurrentCustomize(bool isCoop, int vsId)
	{
		if (isCoop)
		{
			mCurrentCustomizeData = new OnlineCustomizeData(mCoopCustomizeData);
		}
		else
		{
			mCurrentCustomizeData = new OnlineCustomizeData(mOnlinePlayer.GetOnlineCustomizeById(vsId));
			mCurrentCustomizeData.IsPvp = true;
		}
		mCurrentCustomizeDataID = vsId;
		playerSkill.init(mCurrentCustomizeData, this);
	}

	public void SaveCurrentCustomize(bool isCoop, int vsId)
	{
		if (isCoop)
		{
			mCoopCustomizeData = new OnlineCustomizeData(mCurrentCustomizeData);
		}
		else
		{
			mOnlinePlayer.SetOnlineCustomizeArray(vsId, mCurrentCustomizeData);
		}
	}

	public OnlineCustomizeData GetCurrentCustomize()
	{
		return mCurrentCustomizeData;
	}

	public int GetCurrentCustomizeDataID()
	{
		int result = -1;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			result = mCurrentCustomizeDataID;
		}
		return result;
	}

	public OnlinePlayer GetOnlinePlayer()
	{
		if (mOnlinePlayer == null)
		{
			mOnlinePlayer = new OnlinePlayer();
		}
		return mOnlinePlayer;
	}

	public void SetOnlinePlayer(OnlinePlayer onlinePlayer)
	{
		mOnlinePlayer = onlinePlayer;
	}

	public void SetFriendRemainDictionary(Dictionary<int, int> dic)
	{
		mFriendRemainDictionary = dic;
	}

	public Dictionary<int, int> GetFriendRemainDictionary()
	{
		return mFriendRemainDictionary;
	}

	public Dictionary<int, OnlineFriend> GetFriendDictionary()
	{
		return mFriendDictionary;
	}

	public void SetFriendDictionary(Dictionary<int, OnlineFriend> dic)
	{
		mFriendDictionary = dic;
	}

	public void AddFriend(OnlineFriend friend)
	{
		if (!mFriendDictionary.ContainsKey(friend.GetUserId()))
		{
			mFriendDictionary.Add(friend.GetUserId(), friend);
		}
	}

	public void RemoveFriend(OnlineFriend friend)
	{
		if (mFriendDictionary.ContainsKey(friend.GetUserId()))
		{
			mFriendDictionary.Remove(friend.GetUserId());
		}
	}

	public void RemoveFriendByUserId(int userId)
	{
		if (mFriendDictionary.ContainsKey(userId))
		{
			mFriendDictionary.Remove(userId);
		}
	}

	public void SetFriendDictionary(List<OnlineFriend> list)
	{
		mFriendDictionary = new Dictionary<int, OnlineFriend>();
		foreach (OnlineFriend item in list)
		{
			mFriendDictionary.Add(item.GetUserId(), item);
		}
	}

	public List<OnlineFriend> GetRequestList()
	{
		return mRequestList;
	}

	public void SetRequestList(List<OnlineFriend> list)
	{
		mRequestList = list;
	}

	public void RemoveRequest(OnlineFriend f)
	{
		if (mRequestList.Contains(f))
		{
			mRequestList.Remove(f);
		}
	}

	public void RemoveRequestByUserId(int userId)
	{
		OnlineFriend onlineFriend = null;
		foreach (OnlineFriend mRequest in mRequestList)
		{
			if (mRequest.GetUserId() == userId)
			{
				onlineFriend = mRequest;
				break;
			}
		}
		if (onlineFriend != null)
		{
			mRequestList.Remove(onlineFriend);
		}
	}

	public void WriteWeaponOwnState(BytesBuffer buffer)
	{
		for (int i = 1; i <= 33; i++)
		{
			buffer.AddByte(weaponList[i].Level);
		}
	}

	public void WriteArmorOwnStateHead(BytesBuffer buffer)
	{
		for (int i = 0; i < Global.TOTAL_ARMOR_HEAD_NUM; i++)
		{
			buffer.AddByte(armorList[0][i].Level);
		}
	}

	public void WriteArmorOwnStateBody(BytesBuffer buffer)
	{
		for (int i = 0; i < Global.TOTAL_ARMOR_BODY_NUM; i++)
		{
			buffer.AddByte(armorList[1][i].Level);
		}
	}

	public void WriteArmorOwnStateArm(BytesBuffer buffer)
	{
		for (int i = 0; i < Global.TOTAL_ARMOR_ARM_NUM; i++)
		{
			buffer.AddByte(armorList[2][i].Level);
		}
	}

	public void WriteArmorOwnStateFoot(BytesBuffer buffer)
	{
		for (int i = 0; i < Global.TOTAL_ARMOR_FOOT_NUM; i++)
		{
			buffer.AddByte(armorList[3][i].Level);
		}
	}

	public void WriteArmorOwnStateBag(BytesBuffer buffer)
	{
		for (int i = 0; i < Global.TOTAL_ARMOR_BAG_NUM; i++)
		{
			buffer.AddByte(armorList[4][i].Level);
		}
	}

	public void WriteCash(BytesBuffer buffer)
	{
		buffer.AddInt(GetCash());
	}

	public void WriteMithril(BytesBuffer buffer)
	{
		buffer.AddInt(GetMithril());
	}

	public void WriteSaveNum(BytesBuffer buffer)
	{
		buffer.AddInt(saveNum);
	}

	public void SaveSwVoid_Material(BinaryWriter bw)
	{
		bw.Write(swVoidMaterialList.Count);
		foreach (KeyValuePair<short, short> swVoidMaterial in swVoidMaterialList)
		{
			bw.Write(swVoidMaterial.Key);
			bw.Write(swVoidMaterial.Value);
		}
	}

	public void LoadSwVoid_Material(BinaryReader br)
	{
		int num = br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			short num2 = br.ReadInt16();
			short value = br.ReadInt16();
			swVoidMaterialList.Add(num2, value);
		}
	}

	public short GetMaterialNumber(short materialId)
	{
		if (swVoidMaterialList.ContainsKey(materialId))
		{
			return swVoidMaterialList[materialId];
		}
		return 0;
	}

	public void SetMaterialNumber(short materialId, short materialNumber)
	{
		if (swVoidMaterialList.ContainsKey(materialId))
		{
			swVoidMaterialList[materialId] = materialNumber;
		}
		else
		{
			swVoidMaterialList.Add(materialId, materialNumber);
		}
	}

	public bool GetMaterialIsNew(short materialId)
	{
		if (swVoidMaterialIsNewList.ContainsKey(materialId))
		{
			return swVoidMaterialIsNewList[materialId];
		}
		return false;
	}

	public void SetMaterialIsNew(short materialId, bool isNew)
	{
		if (swVoidMaterialIsNewList.ContainsKey(materialId))
		{
			swVoidMaterialIsNewList[materialId] = isNew;
		}
		else
		{
			swVoidMaterialIsNewList.Add(materialId, isNew);
		}
	}

	public bool GetMaterialWish(short materialId)
	{
		if (swVoidMaterialWishList.ContainsKey(materialId))
		{
			return swVoidMaterialWishList[materialId];
		}
		return false;
	}

	public void SetMaterialWish(short materialId, bool isWish)
	{
		if (swVoidMaterialWishList.ContainsKey(materialId))
		{
			swVoidMaterialWishList[materialId] = isWish;
		}
		else
		{
			swVoidMaterialWishList.Add(materialId, isWish);
		}
	}

	public void InitCoopCustomize()
	{
		mCurrentCustomizeData = new OnlineCustomizeData();
		mCurrentCustomizeData.mItemIdArray = new short[3] { 5, 6, 10 };
		playerSkill.init(mCurrentCustomizeData, this);
		mCoopCustomizeData = new OnlineCustomizeData(mCurrentCustomizeData);
	}

	public void AddNewSwVoid(SwVoidSavedata newSwVoid)
	{
		if (newSwVoid != null)
		{
			int i;
			for (i = 0; swVoidList.ContainsKey(i); i++)
			{
			}
			swVoidList.Add(i, newSwVoid);
		}
	}

	public void AddNewSwVoid(int swVoidId, SwVoidSavedata newSwVoid)
	{
		if (newSwVoid != null)
		{
			Debug.Log(swVoidId);
			if (swVoidList.ContainsKey(swVoidId))
			{
				Debug.Log("BIG ERROR");
				return;
			}
			swVoidList.Add(swVoidId, newSwVoid);
			Achievement.CheckAchievement_OneVoid();
			Achievement.CheckAchievement_GoodEquipment();
		}
	}

	public void DeleteSwVoid(int swVoidId)
	{
		swVoidList.Remove(swVoidId);
	}

	public void ResolveSwVoid(int swVoidId)
	{
		swVoidList.Remove(swVoidId);
	}

	public void ClearAllSwVoidData()
	{
		ChangeEquipedSwVoid(0);
		swVoidMaterialList = new Dictionary<short, short>();
		swVoidList = new Dictionary<int, SwVoidSavedata>();
	}

	public void ChangeEquipedSwVoid(int swVoidId)
	{
		Debug.Log("old swVoid : " + SwVoidIdInUsing);
		Debug.Log("new swVoid : " + swVoidId);
		if (swVoidId != SwVoidIdInUsing)
		{
			if (SwVoidIdInUsing != 0 && swVoidList.ContainsKey(SwVoidIdInUsing))
			{
				swVoidList[SwVoidIdInUsing].TakeOff();
			}
			if (swVoidId != 0 && swVoidList.ContainsKey(swVoidId))
			{
				swVoidList[swVoidId].TakeOn(playerSkill);
			}
			SwVoidIdInUsing = swVoidId;
		}
	}

	public void CheckSaveIdentity()
	{
		if (string.IsNullOrEmpty(saveIdentity))
		{
			saveIdentity = GameApp.GetInstance().MacAddress;
		}
	}

	public void SaveData(BinaryWriter bw)
	{
		bw.Write("120");
		IRecordset recordset = RecordFactory.CreateRecord("120", this);
		SetSaveNum(saveNum + 1);
		if (recordset != null)
		{
			recordset.SaveData(bw);
		}
	}

	public void LoadData(BinaryReader br, out string version)
	{
		Debug.Log("Load");
		version = br.ReadString();
		readVersion = version;
		IRecordset recordset = RecordFactory.CreateRecord(version, this);
		if (recordset != null)
		{
			recordset.LoadData(br);
			bInit = true;
		}
	}

	public byte[] CryptMD5Buffer(byte[] data)
	{
		string s = md5Key + GameApp.GetInstance().UUID;
		byte[] bytes = Encoding.ASCII.GetBytes(s);
		byte[] array = new byte[data.Length];
		data.CopyTo(array, 0);
		for (int i = 0; i < bytes.Length; i++)
		{
			array[i] ^= bytes[i];
		}
		MD5 mD = new MD5CryptoServiceProvider();
		return mD.ComputeHash(array);
	}

	public string CryptMD5String(string oriStr, string key)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(oriStr);
		byte[] bytes2 = Encoding.ASCII.GetBytes(key);
		byte[] array = new byte[bytes.Length];
		bytes.CopyTo(array, 0);
		for (int i = 0; i < bytes2.Length; i++)
		{
			array[i] ^= bytes2[i];
		}
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] array2 = mD.ComputeHash(array);
		return array2.ToString();
	}

	public bool VerifyMD5(byte[] original, byte[] md5)
	{
		try
		{
			if (original == null || original.Length == 0)
			{
				return false;
			}
			byte[] array = new byte[original.Length];
			original.CopyTo(array, 0);
			array = CryptMD5Buffer(array);
			if (array == null || md5 == null)
			{
				return false;
			}
			if (array.Length != md5.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != md5[i])
				{
					return false;
				}
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public byte[] GetBytesFromStream(Stream stream)
	{
		byte[] array = new byte[stream.Length];
		int num = (int)stream.Length;
		int num2 = 0;
		while (num > 0)
		{
			int num3 = stream.Read(array, num2, num);
			if (num3 == 0)
			{
				break;
			}
			num2 += num3;
			num -= num3;
		}
		return array;
	}

	public byte[] CryptBuffer(byte[] data)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(key);
		byte[] array = new byte[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			array[i] = (byte)(data[i] ^ bytes[i % bytes.Length]);
		}
		return array;
	}

	public byte[] DecryptBuffer(byte[] buffer)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(key);
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] ^= bytes[i % bytes.Length];
		}
		return buffer;
	}

	public void UnlockEquipsByLevel()
	{
		byte unlockedStageId = GetUnlockedStageId(EDifficultMode.NORMAL);
		byte unlockedSubStageId = GetUnlockedSubStageId(EDifficultMode.NORMAL);
		byte unlockedStageId2 = GetUnlockedStageId(EDifficultMode.HEROIC);
		byte unlockedSubStageId2 = GetUnlockedSubStageId(EDifficultMode.HEROIC);
		EDifficultMode unlockedDifficultMode = GetUnlockedDifficultMode();
		UnlockEquipsByLevel(unlockedDifficultMode, unlockedStageId, unlockedSubStageId, unlockedStageId2, unlockedSubStageId2);
	}

	public void UnlockEquipsByLevel(EDifficultMode diff, byte stage, byte subStage, byte heroStage, byte heroSubstage)
	{
		foreach (Weapon value in weaponList.Values)
		{
			LevelID levelID = default(LevelID);
			if (value.UnlockLevel.Difficult == EDifficultMode.NORMAL)
			{
				levelID.Difficult = EDifficultMode.NORMAL;
				levelID.StageID = stage;
				levelID.SubStageID = subStage;
			}
			else if (value.UnlockLevel.Difficult == EDifficultMode.HEROIC)
			{
				if (diff == EDifficultMode.NORMAL)
				{
					continue;
				}
				levelID.Difficult = EDifficultMode.HEROIC;
				levelID.StageID = heroStage;
				levelID.SubStageID = heroSubstage;
			}
			if (value.UnlockLevel.IsBefore(levelID) && value.Level == 0)
			{
				value.Level = 15;
			}
		}
		for (int i = 0; i < armorList.Count; i++)
		{
			for (int j = 0; j < armorList[i].Count; j++)
			{
				LevelID levelID2 = default(LevelID);
				if (armorList[i][j].UnlockLevel.Difficult == EDifficultMode.NORMAL)
				{
					levelID2.Difficult = EDifficultMode.NORMAL;
					levelID2.StageID = stage;
					levelID2.SubStageID = subStage;
				}
				else if (armorList[i][j].UnlockLevel.Difficult == EDifficultMode.HEROIC)
				{
					if (diff == EDifficultMode.NORMAL)
					{
						continue;
					}
					levelID2.Difficult = EDifficultMode.HEROIC;
					levelID2.StageID = heroStage;
					levelID2.SubStageID = heroSubstage;
				}
				if (armorList[i][j].UnlockLevel.IsBefore(levelID2) && armorList[i][j].Level == 0)
				{
					armorList[i][j].Level = 15;
				}
			}
		}
	}

	public void SetOnlineCustomizeUserStateInfo(OnlineCustomizeData data)
	{
		if (data != null)
		{
			if (data.swVoidData != null && data.swVoidData.id != 0)
			{
				Debug.Log("SetOnlineCustomizeUserStateInfo : swVoid id " + data.mSwVoidId + "swvoidData id : " + data.swVoidData.id);
				SwVoidSavedata value = new SwVoidSavedata(data.swVoidData);
				Debug.Log("Old : " + swVoidList.ContainsKey(data.swVoidData.id));
				swVoidList[data.swVoidData.id] = value;
				Debug.Log("New : " + swVoidList[data.swVoidData.id]);
			}
			else
			{
				Debug.Log("SetOnlineCustomizeUserStateInfo : No online SwVoid");
			}
		}
	}

	public string GetPlayerTitle()
	{
		return GameApp.GetInstance().GetUserState().TitleDictionary[mCurrentTitleId];
	}

	public void SetGraphicQuality(GraphicQuality gq)
	{
		mGraphicQuality.mSwitchBloom = gq.mSwitchBloom;
		mGraphicQuality.mSwitchColorCorrection = gq.mSwitchColorCorrection;
		mGraphicQuality.SetCapbility(gq.mCapbility);
	}

	public bool IsLowQuality()
	{
		return mGraphicQuality.mCapbility <= DeviceCaps.Capbility.General;
	}

	public bool UsingBloom()
	{
		return mGraphicQuality.mSwitchBloom;
	}

	public bool UsingColorCorrection()
	{
		return mGraphicQuality.mSwitchColorCorrection;
	}

	public bool UsingImageEffects()
	{
		return UsingBloom() || UsingColorCorrection();
	}

	public void SetNetDifficultMode(byte mode)
	{
		mNetDifficultMode = (EDifficultMode)mode;
	}

	public byte GetSelectedStageIndexGeneral()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			return mSelectNetStageIdx;
		}
		return mSelectedStageIndex;
	}

	public byte GetSelectedSubStageIndexGeneral()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				return (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				return (byte)Global.BOSS_SUB_STAGE_ID;
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				return (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
			Debug.LogError("GetSelectedSubStageIndexGeneral wrong.");
			return 0;
		}
		return mSelectedSubStageIndex;
	}

	public byte GetBoss1ByteType()
	{
		if (Boss1Type == EnemyType.NA)
		{
			return 100;
		}
		return (byte)(Boss1Type - 9);
	}

	public byte GetBoss2ByteType()
	{
		if (Boss2Type == EnemyType.NA)
		{
			return 100;
		}
		return (byte)(Boss2Type - 9);
	}

	public bool GetMultiColorUnlocked()
	{
		return mMultiColorUnlocked;
	}

	public void SetMultiColorUnlocked(bool value)
	{
		mMultiColorUnlocked = value;
	}

	public bool GetUnlockPackBought()
	{
		return mUnlockPackBought;
	}

	public void SetUnlockPackBought(bool value)
	{
		mUnlockPackBought = value;
	}

	public bool GetRookiePackBought()
	{
		return mRookiePackBought;
	}

	public void SetRookiePackBought(bool value)
	{
		mRookiePackBought = value;
	}

	public bool GetExpertPackBought()
	{
		return mExpertPackBought;
	}

	public void SetExpertPackBought(bool value)
	{
		mExpertPackBought = value;
	}

	public void SetMissionNum(int num)
	{
		mMissionNum = num;
	}

	public int GetMissionNum()
	{
		return mMissionNum;
	}

	public void SetWeaponNum(int num)
	{
		mWeaponNum = num;
	}

	public int GetWeaponNum()
	{
		return mWeaponNum;
	}

	public void SetEquipNum(int num)
	{
		mEquipNum = num;
	}

	public int GetEquipNum()
	{
		return mEquipNum;
	}

	public void SetSwVoidNum(int num)
	{
		mSwVoidNum = num;
	}

	public int GetSwVoidNum()
	{
		return mSwVoidNum;
	}

	public bool GetPVEAutoAim()
	{
		return mPVEAutoAimOn;
	}

	public void SetPVEAutoAim(bool value)
	{
		mPVEAutoAimOn = value;
	}

	public bool GetPVPAutoAim()
	{
		return mPVPAutoAimOn;
	}

	public void SetPVPAutoAim(bool value)
	{
		mPVPAutoAimOn = value;
	}

	public bool GetSteadyModeOn()
	{
		return mSteadyModeOn;
	}

	public void SetSteadyModeOn(bool value)
	{
		mSteadyModeOn = value;
	}

	public DateTime GetPackagePushResetTime()
	{
		return mPackagePushResetTime;
	}

	public void SetPackagePushResetTime(DateTime time)
	{
		mPackagePushResetTime = time;
	}

	public int GetLoseRookiePushTimes()
	{
		return mLoseRookiePushTimes;
	}

	public void SetLoseRookiePushTimes(int times)
	{
		mLoseRookiePushTimes = times;
	}

	public bool GetIsWinRookiePush()
	{
		return mIsWinRookiePush;
	}

	public void SetIsWinRookiePush(bool isPush)
	{
		mIsWinRookiePush = isPush;
	}

	public int GetLoseExpertPushTimes()
	{
		return mLoseExpertPushTimes;
	}

	public void SetLoseExpertPushTimes(int times)
	{
		mLoseExpertPushTimes = times;
	}

	public bool GetIsWinExpertPush()
	{
		return mIsWinExpertPush;
	}

	public void SetIsWinExpertPush(bool isPush)
	{
		mIsWinExpertPush = isPush;
	}

	public void v100To104MithrilHandler()
	{
		bool flag = false;
		int num = 0;
		int[] array = new int[7] { 0, 20, 140, 400, 2100, 6000, 14000 };
		for (int i = 0; i < AnalysisManager.GetGoldBuyNum().Length; i++)
		{
			flag = flag || AnalysisManager.GetGoldBuyNum()[i] > 0 || AnalysisManager.GetMithrilBuyNum()[i] > 0;
			if (AnalysisManager.GetMithrilBuyNum()[i] > 0)
			{
				num += AnalysisManager.GetMithrilBuyNum()[i] * array[i];
			}
		}
		for (int j = 0; j < AnalysisManager.GetPackageBuyNum().Count; j++)
		{
			flag = flag || AnalysisManager.GetPackageBuy(j) > 0;
			if (j == 3 && AnalysisManager.GetPackageBuy(j) > 0)
			{
				mUnlockPackBought = true;
			}
		}
		int num2 = 0;
		int num3 = 0;
		for (int k = 0; k < Global.TOTAL_STAGE; k++)
		{
			for (int l = 0; l < Global.TOTAL_SUB_STAGE; l++)
			{
				PVEStageAnalysisInfo pVEInfo = AnalysisManager.GetPVEInfo(k, l, EDifficultMode.NORMAL);
				if (pVEInfo.passTime > 0 && k < 4 && l > 0 && l < 6)
				{
					MissionStateGroup missionStateGroup = mNormalMissionState.GetMissionStatusArray()[k][l];
					num3 += pVEInfo.mithrilCollect;
					int num4 = 0;
					for (int m = 0; m < 5; m++)
					{
						if (missionStateGroup[m])
						{
							num4++;
							if (m > 0)
							{
								int num5 = 4 + k;
								num2 += num5;
							}
						}
					}
					if (num4 == 5)
					{
						int num6 = 10 * (4 + k);
						num2 += num6;
					}
				}
				PVEStageAnalysisInfo pVEInfo2 = AnalysisManager.GetPVEInfo(k, l, EDifficultMode.HEROIC);
				if (pVEInfo2.passTime <= 0 || k >= 4 || l <= 0 || l >= 6)
				{
					continue;
				}
				MissionStateGroup missionStateGroup2 = mHeroicMissionState.GetMissionStatusArray()[k][l];
				num3 += pVEInfo2.mithrilCollect;
				int num7 = 0;
				for (int n = 0; n < 5; n++)
				{
					if (missionStateGroup2[n])
					{
						num7++;
						if (n > 0)
						{
							int num8 = 8 + k;
							num2 += num8;
						}
					}
				}
				if (num7 == 5)
				{
					int num9 = 10 * (8 + k);
					num2 += num9;
				}
			}
		}
		if (flag)
		{
			int num10 = num + 400;
			AddMithril(num10);
			AnalysisManager.mithrilInfo.OfficialMithril += num10;
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.SetMithrilHandleMessage(LocalizationManager.GetInstance().GetString("LOC_SPHINT10", 400, num));
			}
		}
		else if (num3 < 5000)
		{
			AddMithril(400);
			AnalysisManager.mithrilInfo.OfficialMithril += 400;
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.SetMithrilHandleMessage(LocalizationManager.GetInstance().GetString("LOC_SPHINT13", 400));
			}
		}
		else if (num3 < 6000)
		{
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.SetMithrilHandleMessage(LocalizationManager.GetInstance().GetString("LOC_SPHINT14", num3));
			}
		}
		else
		{
			int num11 = num3 - 6000;
			BuyWithMithril(num11);
			AnalysisManager.mithrilInfo.OfficialMithril -= num11;
			if (num3 > 9000 && weaponList[7].Level != 0 && weaponList[7].Level != 15)
			{
				weaponList[7].SetLevel(15);
				AnalysisManager.mithrilInfo.OfficialMithril -= 3000;
				for (int num12 = 0; num12 < mCoopCustomizeData.mWeaponIdArray.Length; num12++)
				{
					if (mCoopCustomizeData.mWeaponIdArray[num12] == 7)
					{
						mCoopCustomizeData.mWeaponIdArray[num12] = 0;
					}
				}
			}
			if (num3 > 12000 && weaponList[14].Level != 0 && weaponList[14].Level != 15)
			{
				weaponList[14].SetLevel(15);
				AnalysisManager.mithrilInfo.OfficialMithril -= 3600;
				for (int num13 = 0; num13 < mCoopCustomizeData.mWeaponIdArray.Length; num13++)
				{
					if (mCoopCustomizeData.mWeaponIdArray[num13] == 14)
					{
						mCoopCustomizeData.mWeaponIdArray[num13] = 0;
					}
				}
			}
			if (num3 > 15000 && weaponList[6].Level != 0 && weaponList[6].Level != 15)
			{
				weaponList[6].SetLevel(15);
				AnalysisManager.mithrilInfo.OfficialMithril -= 4000;
				for (int num14 = 0; num14 < mCoopCustomizeData.mWeaponIdArray.Length; num14++)
				{
					if (mCoopCustomizeData.mWeaponIdArray[num14] == 6)
					{
						mCoopCustomizeData.mWeaponIdArray[num14] = 0;
					}
				}
			}
			mCoopCustomizeData.mWeaponIdArray[0] = 1;
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.SetMithrilHandleMessage(LocalizationManager.GetInstance().GetString("LOC_SPHINT15", num3));
			}
		}
		isV100To104MithrilHandled = true;
	}

	public int GetBestAvatarIndex()
	{
		int num = 0;
		for (int i = 0; i <= 3; i++)
		{
			for (int j = 0; j < Global.TOTAL_ARMOR_NUM; j++)
			{
				if (j > num && armorList[i][j].Level != 0 && armorList[i][j].Level != 15)
				{
					num = j;
				}
			}
		}
		return num;
	}

	public bool IsThereNullItem()
	{
		short[] battleItemIDs = GetBattleItemIDs();
		foreach (short num in battleItemIDs)
		{
			if (num == 0)
			{
				return true;
			}
			if (GetItems()[num].GetCount() == 0)
			{
				return true;
			}
		}
		return false;
	}

	public int GetNullItemNum()
	{
		int num = 0;
		short[] battleItemIDs = GetBattleItemIDs();
		foreach (short num2 in battleItemIDs)
		{
			if (num2 == 0)
			{
				num++;
			}
			else if (GetItems()[num2].GetCount() == 0)
			{
				num++;
			}
		}
		return num;
	}

	public void ClearTrialWeapon()
	{
		if (weaponList.ContainsKey(mTrialWeaponID))
		{
			Weapon weapon = weaponList[mTrialWeaponID];
			weapon.ReturnFromTrialWeapon();
		}
		UnlockEquipsByLevel();
		mTrialWeaponID = -1;
	}

	public short GetTrialWeaponID()
	{
		Debug.Log("mTrialWeaponID = " + mTrialWeaponID);
		return mTrialWeaponID;
	}

	public void GenerateTrialWeaponID(EDifficultMode diff, int stageID, int subStageID)
	{
		if (subStageID == Global.SURVIVAL_SUB_STAGE_ID || subStageID == Global.BOSS_SUB_STAGE_ID || subStageID == Global.BOSS_FINAL_SUB_STAGE_ID)
		{
			return;
		}
		Debug.Log("mTodayTrialed = " + mTodayTrialed);
		if (!mTodayCanTrial || mTodayTrialed)
		{
			return;
		}
		Weapon weapon = GetBattleWeapons()[0];
		Weapon weapon2 = null;
		List<Weapon> list = new List<Weapon>();
		List<Weapon> list2 = new List<Weapon>();
		foreach (Weapon value in weaponList.Values)
		{
			if (value.Level != 0 && value.Level != 15 && value.DisplayOrder >= weapon.DisplayOrder)
			{
				weapon = value;
			}
		}
		List<Weapon> list3 = new List<Weapon>();
		foreach (Weapon value2 in weaponList.Values)
		{
			if (!mPushedWeaponOrderList.Contains(value2.DisplayOrder) && (value2.Level == 15 || value2.Level == 0))
			{
				int num = value2.DisplayOrder - weapon.DisplayOrder;
				if (4 <= num && num <= 6)
				{
					list3.Add(value2);
				}
			}
		}
		if (weapon.DisplayOrder + 4 < 14)
		{
			List<Weapon> weaponByDisplayOrder = GetWeaponByDisplayOrder(Global.ROOKIE_TRIAL_PUSH_DISPLAY_ID_LIST);
			for (int i = 0; i < weaponByDisplayOrder.Count; i++)
			{
				if (!mPushedWeaponOrderList.Contains(weaponByDisplayOrder[i].DisplayOrder) && (weaponByDisplayOrder[i].Level == 15 || weaponByDisplayOrder[i].Level == 0))
				{
					list3.Add(weaponByDisplayOrder[i]);
				}
			}
		}
		else if (weapon.DisplayOrder + 4 > 21)
		{
			List<Weapon> weaponByDisplayOrder2 = GetWeaponByDisplayOrder(Global.ETERNAL_TRIAL_PUSH_DISPLAY_ID_LIST);
			for (int j = 0; j < weaponByDisplayOrder2.Count; j++)
			{
				if (!mPushedWeaponOrderList.Contains(weaponByDisplayOrder2[j].DisplayOrder) && (weaponByDisplayOrder2[j].Level == 15 || weaponByDisplayOrder2[j].Level == 0))
				{
					list3.Add(weaponByDisplayOrder2[j]);
				}
			}
		}
		MissionWeaponKillEnemy missionWeaponKillEnemy = null;
		for (int k = 0; k < Global.MISSION_SIZE; k++)
		{
			Mission mission = MissionManager.GetInstance().GetMission((byte)stageID, (byte)subStageID, k, diff);
			if (mission.Type == MissionType.WEAPON_KILL_ENEMY)
			{
				missionWeaponKillEnemy = mission as MissionWeaponKillEnemy;
				break;
			}
		}
		foreach (Weapon item in list3)
		{
			if (missionWeaponKillEnemy != null)
			{
				if (missionWeaponKillEnemy.GetMissionWeaponType() != MissionWeaponType.SPECIAL_GUN)
				{
					if (missionWeaponKillEnemy.GetMissionWeaponType() == Weapon.GetMissionWeaponType(item.GetWeaponType()))
					{
						Debug.Log("SSSSSSSSSSSSSkip " + missionWeaponKillEnemy.GetMissionWeaponType());
						continue;
					}
				}
				else if (missionWeaponKillEnemy.GetSpecialGunID() == item.GunID)
				{
					Debug.Log("SSSSSSSSSSSSSkip GunID = " + missionWeaponKillEnemy.GetSpecialGunID());
					continue;
				}
			}
			list.Add(item);
			if (item.GetWeaponType() != weapon.GetWeaponType())
			{
				list2.Add(item);
			}
		}
		if (list2.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list2.Count);
			weapon2 = list2[index];
		}
		else if (list.Count > 0)
		{
			int index2 = UnityEngine.Random.Range(0, list.Count);
			weapon2 = list[index2];
		}
		else
		{
			weapon2 = null;
		}
		if (weapon2 != null)
		{
			mTrialWeaponID = (short)weapon2.GunID;
			Weapon weapon3 = weaponList[mTrialWeaponID];
			weapon3.ChangeToTrialWeapon();
			mTrialWeaponPushedTimes++;
			mTodayTrialed = true;
			mPushedWeaponOrderList.Add(weapon2.DisplayOrder);
			GameApp.GetInstance().Save();
		}
		else
		{
			mTrialWeaponID = -1;
		}
	}

	public short GetTrialWeaponBuyID()
	{
		return mTrialWeaponBuyID;
	}

	public void SetTrialWeaponBuyID(short id)
	{
		mTrialWeaponBuyID = id;
	}

	public void ConfirmTrialWeapon()
	{
		mTrialWeaponBuyID = mTrialWeaponID;
		mTrialWeaponTimeLimit = DateTime.Now.AddDays(1.0);
		SetTrialWeaponBuySeconds(mTrialWeaponTimeLimit);
		mTodayCanTrial = false;
	}

	public void GetServerTime()
	{
		SwGetServerTimeRequest request = new SwGetServerTimeRequest();
		GameApp.GetInstance().GetWWWManager().SendRequest(request);
	}

	public bool CheckTrialPushDate(DateTime serverTime)
	{
		LevelID levelID = new LevelID
		{
			Difficult = EDifficultMode.NORMAL,
			StageID = 0,
			SubStageID = 2
		};
		LevelID levelID2 = new LevelID
		{
			Difficult = EDifficultMode.NORMAL,
			StageID = mNormalMissionState.GetUnlockedStageId(),
			SubStageID = mNormalMissionState.GetUnlockedSubStageId()
		};
		if (levelID2.IsBefore(levelID))
		{
			return false;
		}
		int num = 1;
		switch (mTrialWeaponPushedTimes)
		{
		case 0:
		case 1:
		case 2:
			num = 1;
			break;
		case 3:
		case 4:
		case 5:
			num = 2;
			break;
		case 6:
		case 7:
		case 8:
			num = 3;
			break;
		default:
			num = 4;
			break;
		}
		int days = (serverTime - mTrialWeaponPushResetTime).Days;
		Debug.Log("mTrialWeaponPushResetTime = " + mTrialWeaponPushResetTime.ToString() + "----passedTimeCount = " + days);
		if (days >= num)
		{
			mTrialWeaponPushResetTime = serverTime.Date;
			mTodayCanTrial = true;
			mTodayTrialed = false;
			return true;
		}
		Debug.Log("mTodayCanTrial = " + mTodayCanTrial);
		return false;
	}

	public DateTime GetTrialWeaponTimeLimit()
	{
		return mTrialWeaponTimeLimit;
	}

	public void SetTrialWeaponTimeLimit(DateTime time)
	{
		mTrialWeaponTimeLimit = time;
	}

	public DateTime GetTrialWeaponPushResetTime()
	{
		return mTrialWeaponPushResetTime;
	}

	public void SetTrialWeaponPushResetTime(DateTime time)
	{
		mTrialWeaponPushResetTime = time;
	}

	public int GetTrialWeaponPushedTimes()
	{
		return mTrialWeaponPushedTimes;
	}

	public void SetTrialWeaponPushedTimes(int times)
	{
		mTrialWeaponPushedTimes = times;
	}

	public void TrialWeaponBuySecondsCountDown(float deltaTime)
	{
		if (mTrialWeaponBuySecondsLeft > 0f)
		{
			mSecondCount += deltaTime;
			if (mSecondCount >= 1f)
			{
				mSecondCount = 0f;
				SetTrialWeaponBuySeconds(mTrialWeaponTimeLimit);
			}
		}
	}

	public string GetTrialWeaponBuySecondsString()
	{
		string empty = string.Empty;
		int num = Mathf.CeilToInt(mTrialWeaponBuySecondsLeft);
		if (num < 0)
		{
			return "--:--:--";
		}
		int num2 = num / 3600;
		empty = ((num2 < 10) ? ("0" + num2) : num2.ToString()) + ":";
		int num3 = num / 60 - num2 * 60;
		empty = empty + ((num3 < 10) ? ("0" + num3) : num3.ToString()) + ":";
		int num4 = num % 60;
		return empty + ((num4 < 10) ? ("0" + num4) : num4.ToString());
	}

	public float GetTrialWeaponBuySeconds()
	{
		return mTrialWeaponBuySecondsLeft;
	}

	public void SetTrialWeaponBuySeconds(DateTime time)
	{
		mTrialWeaponBuySecondsLeft = Mathf.CeilToInt((float)(time - DateTime.Now).TotalSeconds);
	}

	public void CheckTrialWeaponBuySecondsOver()
	{
		if (mTrialWeaponBuyID > 0)
		{
			if (mTrialWeaponBuySecondsLeft <= 0f)
			{
				ClearTrialWeaponBuyInfo();
			}
			else if (mTrialWeaponBuySecondsLeft > Global.TRIAL_WEAPON_BUY_SECONDS_MAX)
			{
				ClearTrialWeaponBuyInfo();
			}
		}
		else if (mTrialWeaponBuySecondsLeft > 0f)
		{
			ClearTrialWeaponBuyInfo();
		}
	}

	public void ClearTrialWeaponBuyInfo()
	{
		if (weaponList.ContainsKey(mTrialWeaponBuyID))
		{
			Weapon weapon = weaponList[mTrialWeaponBuyID];
			weapon.ReturnFromTrialBuyWeapon();
		}
		UnlockEquipsByLevel();
		mTrialWeaponID = -1;
		mTrialWeaponBuyID = -1;
		mTrialWeaponBuySecondsLeft = 0f;
		GameApp.GetInstance().Save();
	}

	public bool GetTodayCanTrail()
	{
		return mTodayCanTrial;
	}

	public void SetTodayCanTrail(bool canTrail)
	{
		mTodayCanTrial = canTrail;
	}

	public bool GetTodayTrailed()
	{
		return mTodayTrialed;
	}

	public void SetTodayTrailed(bool trailed)
	{
		mTodayTrialed = trailed;
	}

	public List<short> GetPushedWeaponOrderList()
	{
		return mPushedWeaponOrderList;
	}

	public void SetPushedWeaponOrderList(List<short> list)
	{
		mPushedWeaponOrderList = new List<short>(list);
	}

	public short GetWeaponUpgradePushID()
	{
		return mWeaponUpgradePushID;
	}

	public DateTime GetWeaponUpgradePushResetTime()
	{
		return mWeaponUpgradePushResetTime;
	}

	public void SetWeaponUpgradePushResetTime(DateTime time)
	{
		mWeaponUpgradePushResetTime = time;
	}

	public bool CheckWeaponUpgradePushDate(DateTime serverTime)
	{
		LevelID levelID = new LevelID
		{
			Difficult = EDifficultMode.NORMAL,
			StageID = 1,
			SubStageID = 0
		};
		LevelID levelID2 = new LevelID
		{
			Difficult = EDifficultMode.NORMAL,
			StageID = mNormalMissionState.GetUnlockedStageId(),
			SubStageID = mNormalMissionState.GetUnlockedSubStageId()
		};
		if (levelID2.IsBefore(levelID))
		{
			return false;
		}
		int num = Mathf.CeilToInt((float)(serverTime - mWeaponUpgradePushResetTime).TotalHours);
		Debug.Log("mWeaponUpgradePushResetTime = " + mWeaponUpgradePushResetTime.ToString() + "----passedTimeCount = " + num + "----Interval = " + mUpgradePushIntervalHours);
		if (num >= mUpgradePushIntervalHours)
		{
			mUpgradePushIntervalHours = UnityEngine.Random.Range(Global.WEAPON_UPGRADE_PUSH_INTERVAL_HOUR_MIN, Global.WEAPON_UPGRADE_PUSH_INTERVAL_HOUR_MAX + 1);
			mWeaponUpgradePushResetTime = serverTime;
			mTodayCanPushUpgrade = true;
			mTodayUpgradePushed = false;
			return true;
		}
		Debug.Log("mTodayCanPushUpgrade = " + mTodayCanPushUpgrade);
		return false;
	}

	public void GeneratePushUpgradeWeaponID()
	{
		if (mLastestBuyWeaponID > 0)
		{
			byte level = weaponList[mLastestBuyWeaponID].Level;
			if (level != 0 && level != 15 && level < Global.MAX_LEVEL_WEAPONW - Global.WEAPON_UPGRADE_PUSH_UP_LEVELS[0])
			{
				mWeaponUpgradePushID = mLastestBuyWeaponID;
				mLastestBuyWeaponID = -1;
				return;
			}
		}
		if (!mTodayCanPushUpgrade || mTodayUpgradePushed)
		{
			return;
		}
		Weapon weapon = null;
		List<Weapon> list = new List<Weapon>();
		foreach (Weapon value in weaponList.Values)
		{
			if (value.Level != 0 && value.Level != 15 && value.Level < Global.MAX_LEVEL_WEAPONW - Global.WEAPON_UPGRADE_PUSH_UP_LEVELS[0])
			{
				list.Add(value);
			}
		}
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			weapon = list[index];
		}
		if (weapon != null)
		{
			mWeaponUpgradePushID = (short)weapon.GunID;
		}
		else
		{
			mWeaponUpgradePushID = -1;
		}
		mTodayCanPushUpgrade = false;
	}

	public void ClearWeaponUpgradeID()
	{
		mWeaponUpgradePushID = -1;
	}

	public bool GetTodayCanPushUpgrade()
	{
		return mTodayCanPushUpgrade;
	}

	public void SetTodayCanPushUpgrade(bool canPush)
	{
		mTodayCanPushUpgrade = canPush;
	}

	public bool GetTodayUpgradeHasPushed()
	{
		return mTodayUpgradePushed;
	}

	public void SetTodayUpgradeHasPushed(bool hasPushed)
	{
		mTodayUpgradePushed = hasPushed;
	}

	public void SetLastestBuyWeaponID(short id)
	{
		mLastestBuyWeaponID = id;
	}

	public int GetUpgradePushIntervalHours()
	{
		return mUpgradePushIntervalHours;
	}

	public void SetUpgradePushInervalHours(int intervalHours)
	{
		mUpgradePushIntervalHours = intervalHours;
	}

	private List<Weapon> GetWeaponByDisplayOrder(List<short> orders)
	{
		List<Weapon> list = new List<Weapon>();
		for (int i = 0; i < orders.Count; i++)
		{
			foreach (Weapon value in weaponList.Values)
			{
				if (value.DisplayOrder == orders[i])
				{
					list.Add(value);
					break;
				}
			}
		}
		return list;
	}

	public void SetPushItemInterval(int minute)
	{
		mPushItemIntervalMinutes = minute;
	}

	public bool CanPushItem()
	{
		return (DateTime.Now - mLastPushItemTime).Minutes > mPushItemIntervalMinutes;
	}

	public void SetPushItemTime(DateTime time)
	{
		mLastPushItemTime = time;
	}

	public bool GetSmoothMoveOn()
	{
		return mSmoothMove;
	}

	public void SmoothMoveHandler()
	{
	}

	public void SetSmoothMoveOn(bool value)
	{
		mSmoothMove = value;
	}
}
