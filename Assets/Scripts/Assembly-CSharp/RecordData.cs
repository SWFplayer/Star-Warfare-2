using System;
using System.Collections.Generic;

public class RecordData
{
	public int mSaveNum;

	public string mSaveIdentity = string.Empty;

	public int mMithril;

	public int mCash;

	public bool mMultiColorBought;

	public bool mRookiePackBought;

	public bool mExpertPackBought;

	public FriendType mFriendType;

	public string mTwitterName = string.Empty;

	public long mFacebookId;

	public long mGameCenterId;

	public int mTotalPlayTime;

	public bool mNeedResetFriendTime;

	public int mTitleId;

	public int mTitleFrameId;

	public EDifficultMode mUnlockedDifficultMode;

	public MissionState mNormalMissionState = new MissionState(EDifficultMode.NORMAL);

	public MissionState mHeroicMissionState = new MissionState(EDifficultMode.HEROIC);

	public OnlineCustomizeData mCustomizeData = new OnlineCustomizeData();

	public byte[] mOwnedWeapons = new byte[33];

	public byte[] mOwnedArmorsHeads = new byte[Global.TOTAL_ARMOR_HEAD_NUM];

	public byte[] mOwnedArmorsBodies = new byte[Global.TOTAL_ARMOR_BODY_NUM];

	public byte[] mOwnedArmorsArms = new byte[Global.TOTAL_ARMOR_ARM_NUM];

	public byte[] mOwnedArmorsFeet = new byte[Global.TOTAL_ARMOR_FOOT_NUM];

	public byte[] mOwnedArmorsBags = new byte[Global.TOTAL_ARMOR_BAG_NUM];

	public byte[] mOwnedItems = new byte[26];

	public OperatingInfo mOperatingInfo = new OperatingInfo();

	public AchievementState mAchievement = new AchievementState();

	public GameAnalysisManager mAnalysis = new GameAnalysisManager();

	public CustomizeColorData mCustomizeColorData = new CustomizeColorData();

	public bool[] mTutorials = new bool[18];

	public string[][] mWeaponBuyVer = new string[33][];

	public string[] mHeadBuyVer = new string[Global.TOTAL_ARMOR_NUM];

	public string[] mBodyBuyVer = new string[Global.TOTAL_ARMOR_NUM];

	public string[] mHandBuyVer = new string[Global.TOTAL_ARMOR_NUM];

	public string[] mFootBuyVer = new string[Global.TOTAL_ARMOR_NUM];

	public string[] mBagBuyVer = new string[Global.TOTAL_ARMOR_BAG_NUM];

	public bool mFollowTwitter;

	public bool mFollowFacebook;

	public bool mFirstIapSuccess;

	public DateTime mSyncRecordTime = DateTime.Now;

	public DateTime mRemindSyncRecordTime = DateTime.Now;

	public byte mSelectedStageIndex;

	public byte mSelectedSubStageIndex = 1;

	public EDifficultMode mSelectedDifficultMode;

	public bool mHasGetFirstBossVoidMaterials;

	public int mPlayTimesAfterIgnoreRating;

	public int mLastAnnouncementId;

	public bool isV100To104MithrilHandled;

	public bool isUnlockPackBought;

	public DateTime mPackagePushResetTime = DateTime.Now;

	public int mLoseRookiePushTimes;

	public bool mIsWinRookiePush;

	public int mLoseExpertPushTimes;

	public bool mIsWinExpertPush;

	public int mTrialWeaponBuyID;

	public DateTime mTrialWeaponTimeLimit = DateTime.Now;

	public DateTime mTrialWeaponPushResetTime = DateTime.Now;

	public int mTrialWeaponPushedTimes;

	public bool mTodayCanTrial;

	public bool mTodayTrialed;

	public List<short> mPushedWeaponOrderList = new List<short>();

	public DateTime mWeaponUpgradePushResetTime = DateTime.Now;

	public bool mTodayCanPushUpgrade;

	public bool mTodayUpgradeHasPushed;

	public int mUpgradePusheIntervalHours;

	public bool mPlaySound = true;

	public bool mPlayMusic = true;

	public float mSoundVolume = 1f;

	public float mMusicVolume = 1f;

	public byte mSensitivity;

	public byte mLanguage;

	public bool mGravity = true;

	public bool mEnableHide = true;

	public bool mSkipDialog = true;

	public bool mAutoHide = true;

	public bool mChangeSide = true;

	public bool mSkipTutorial = true;

	public bool mPVEAutoAim = true;

	public bool mPVPAutoAim = true;

	public bool mSteadyModeOn;

	public bool mSmoothMoveOn;

	public bool mLeftRevert;

	public bool mRightRevert;

	public GraphicQuality mGraphicQuality = new GraphicQuality();

	public Dictionary<int, int> mFriendRemainDictionary = new Dictionary<int, int>();
}
