using UnityEngine;

public class PlayerVSStatistics
{
	public enum StatisticType
	{
		Kill = 0,
		Assist = 1,
		StatisticCount = 2
	}

	public enum EPvpExpColumn
	{
		ID = 0,
		PERSONAL_MESSAGE = 1,
		PUBLIC_MESSAGE = 2,
		EXP = 3
	}

	public enum EExpType
	{
		KILLS = 0,
		ASSIST = 1,
		GOOD_ASSIST = 2,
		FIRST_BLOOD = 3,
		DOUBLE_KILL = 4,
		MULTI_KILL = 5,
		ONE_SHOT_KILL = 6,
		PAY_BACK = 7,
		LONG_SHOT = 8,
		LEGENDARY = 9,
		OCCUPATION = 10,
		OCCUPATION_ASSIST = 11,
		STOP_HIM = 12,
		UNSTOPABLE = 13,
		PLAYER_BOSS_KILL = 14,
		PLAYER_BOSS_DOUBLE_KILL = 15,
		PLAYER_BOSS_MULTI_KILL = 16,
		PLAYER_BOSS_ONE_SHOT_KILL = 17,
		PLAYER_BOSS_LONG_SHOT = 18,
		PLAYER_BOSS_RAMPAGE = 19,
		PLAYER_BOSS_WIN_SCORE = 20,
		TO_PLAYER_BOSS_ENOUGH_DAMAGE = 21,
		TO_PLAYER_BOSS_EXECUTED = 22,
		TO_PLAYER_BOSS_KILL_CONTRIBUTION = 23,
		DOMINATE = 24,
		REVENGE = 25,
		ASSASSINATE = 26,
		RAMPAGE = 27,
		CAPTURE = 28,
		ACHIEVEMENT = 29,
		WIN = 30,
		LOSE = 31,
		COUNT = 32
	}

	public class specialInfo
	{
		public specialInfoType type;

		public byte value;
	}

	public enum specialInfoType
	{
		NONE = 0,
		FIRST_BLOOD = 1,
		PAY_BACK = 2,
		DOUBLE_KILL = 3,
		MULTI_KILL = 4,
		LEGENDARY = 5,
		LONG_SHOT = 6,
		STOP_HIM = 7,
		UNSTOPABLE = 8,
		LEGENDARY_NO_EXP = 9,
		UNSTOPABLE_NO_EXP = 10,
		PLAYER_BOSS_RAMPAGE = 11,
		PLAYER_BOSS_RAMPAGE_NO_EXP = 12,
		DOMINATE = 13,
		REVENGE = 14,
		ASSASSINATE = 15,
		RAMPAGE = 16,
		CAPTURE = 17,
		RAMPAGE_NO_EXP = 18
	}

	public const int SCORE_PER_OCCUPY = 100;

	public const int SCORE_PER_KILL = 60;

	public const int SCORE_PER_ASSIST = 20;

	public const int SCORE_PER_GOOD_ASSIST = 20;

	public const int SCORE_PER_KILL_IN_FFA = 60;

	public const int SCORE_PER_KILL_IN_TDM = 50;

	public const int SCORE_PER_KILL_IN_CTF_TDM = 50;

	public const int SCORE_PER_FLAGS_IN_CTF_TDM = 100;

	public const int SCORE_PER_ASSIST_FLAGS_IN_CTF_TDM = 16;

	public const int SCORE_PER_KILL_IN_CTF_FFA = 50;

	public const int SCORE_PER_FLAGS_IN_CTF_FFA = 100;

	public const int SCORE_PER_ASSIST_FLAGS_IN_CTF_FFA = 20;

	public static float EXP_RATE = 1f;

	protected static int[] ExpDataList;

	public static int[] ExpForPlayerBossDamageRankList;

	public static string[] PrivateInfoLocId;

	public static string[] PublicInfoLocId;

	public short[] statisticsCountsList = new short[32];

	public int[] expList = new int[32];

	private int mScore;

	protected float lastKillTime;

	protected float COMBO_INTERVAL = 2f;

	protected Player mPlayer;

	protected int playerBossModeScore;

	protected int totleDamageToPlayerBoss;

	private AchievementState achievements;

	private int[] mStatisticValues = new int[2];

	private static int[] mStatisticParameters = new int[2] { 100, 10 };

	public int Kills
	{
		get
		{
			return statisticsCountsList[0];
		}
		set
		{
			statisticsCountsList[0] = (short)value;
		}
	}

	public int PlayerBossKills
	{
		get
		{
			return statisticsCountsList[14];
		}
		set
		{
			statisticsCountsList[14] = (short)value;
		}
	}

	public int Death { get; set; }

	public int Assist
	{
		get
		{
			return statisticsCountsList[1];
		}
		set
		{
			statisticsCountsList[1] = (short)value;
		}
	}

	public int GoodAssist
	{
		get
		{
			return statisticsCountsList[2];
		}
		set
		{
			statisticsCountsList[2] = (short)value;
		}
	}

	public int Capture
	{
		get
		{
			return statisticsCountsList[28];
		}
		set
		{
			statisticsCountsList[28] = (short)value;
		}
	}

	public int Assassinate
	{
		get
		{
			return statisticsCountsList[26];
		}
		set
		{
			statisticsCountsList[26] = (short)value;
		}
	}

	public int ControlPoint
	{
		get
		{
			return statisticsCountsList[10];
		}
		set
		{
			statisticsCountsList[10] = (short)value;
		}
	}

	public int ControlPointAssist
	{
		get
		{
			return statisticsCountsList[11];
		}
		set
		{
			statisticsCountsList[11] = (short)value;
		}
	}

	public int StopHim
	{
		get
		{
			return statisticsCountsList[12];
		}
		set
		{
			statisticsCountsList[12] = (short)value;
		}
	}

	public int Occupy { get; set; }

	public int Score
	{
		get
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
			{
				return playerBossModeScore;
			}
			return mScore;
		}
		set
		{
			mScore = value;
		}
	}

	public int Bonus { get; set; }

	public int Combo { get; set; }

	public int ComboInOneLife { get; set; }

	public int PlayerHit { get; set; }

	public int CashReward { get; set; }

	public int SecureFlags { get; set; }

	public int AssistFlags { get; set; }

	public int FfaScore { get; set; }

	public PlayerVSStatistics(Player player)
	{
		mPlayer = player;
		if (player.IsLocal())
		{
			achievements = player.GetUserState().Achievement;
		}
	}

	public PlayerVSStatistics()
	{
		ClearAll();
	}

	public void MakeCombo()
	{
		Combo++;
		ComboInOneLife++;
		lastKillTime = Time.time;
	}

	public void AddSecureFlagScore()
	{
		if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_CTF_TDM && GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_CTF_FFA)
		{
		}
	}

	public void UpdateKingOfHillSocre()
	{
	}

	public void UpdataScore()
	{
		if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.GIANT_BETRAYAL)
		{
			Occupy = statisticsCountsList[10] + statisticsCountsList[11] + statisticsCountsList[12];
			Occupy += statisticsCountsList[28] + statisticsCountsList[26];
			Score = Occupy * 100 + Kills * 60 + Assist * 20 + GoodAssist * 20;
		}
	}

	public void CalculateBonus(Mode mode, bool win)
	{
	}

	public void ClearAll()
	{
		Kills = 0;
		Death = 0;
		Assist = 0;
		GoodAssist = 0;
		Score = 0;
		Bonus = 0;
		Combo = 0;
		ComboInOneLife = 0;
		PlayerHit = 0;
		CashReward = 0;
		SecureFlags = 0;
		AssistFlags = 0;
		playerBossModeScore = 0;
		FfaScore = 0;
		totleDamageToPlayerBoss = 0;
		for (int i = 0; i < expList.Length; i++)
		{
			expList[i] = 0;
		}
		for (int j = 0; j < expList.Length; j++)
		{
			statisticsCountsList[j] = 0;
		}
	}

	public void CheckComboInLocal()
	{
		if (Time.time - lastKillTime > COMBO_INTERVAL)
		{
			Combo = 0;
		}
		MakeCombo();
	}

	public void SetStatisticValues(StatisticType type, int value)
	{
		mStatisticValues[(int)type] = value;
	}

	public int GetStatisticValues(StatisticType type)
	{
		return mStatisticValues[(int)type] * mStatisticParameters[(int)type];
	}

	private static void InitExpDataList()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_EXP, DataSheetName.PVP_EXP);
		ExpDataList = new int[32];
		for (int i = 0; i < 32; i++)
		{
			ExpDataList[i] = gameDataTable.GetInt(i, 3);
		}
	}

	public static int GetExpData(EExpType type)
	{
		if (ExpDataList == null)
		{
			InitExpDataList();
		}
		return ExpDataList[(int)type];
	}

	public int GetExpDataAfterExpBooth(EExpType type)
	{
		float num = 0f;
		if (mPlayer != null)
		{
			num = mPlayer.GetSkills().GetSkill(SkillsType.EXP_BOOTH);
		}
		int expData = GetExpData(type);
		return (int)((float)expData * EXP_RATE * (1f + num));
	}

	public void AddExp(EExpType type)
	{
		expList[(int)type] += GetExpDataAfterExpBooth(type);
		statisticsCountsList[(int)type]++;
	}

	public void AddExp(EExpType type, int exp)
	{
		expList[(int)type] += exp;
		statisticsCountsList[(int)type]++;
	}

	public void MakeKillPlayerBossAssist(int exp)
	{
		AddExp(EExpType.TO_PLAYER_BOSS_KILL_CONTRIBUTION, exp);
	}

	public void MakePlayerBossWin(int exp)
	{
		AddExp(EExpType.PLAYER_BOSS_WIN_SCORE, exp);
	}

	public void MakeKill(Player killed)
	{
		AddExp(EExpType.KILLS);
		mPlayer.Kills++;
		mPlayer.GetSkills().PlayerKillAmountOfEnemy(killed);
		if (achievements != null)
		{
			achievements.AddCombatKills();
		}
	}

	public void MakeAssist()
	{
		AddExp(EExpType.ASSIST);
		if (achievements != null)
		{
			achievements.AddAssistKills();
		}
	}

	public void MakeGoodAssist()
	{
		AddExp(EExpType.GOOD_ASSIST);
	}

	public void MakeFirstBlood()
	{
		AddExp(EExpType.FIRST_BLOOD);
		if (achievements != null)
		{
			achievements.AddFBNum();
		}
	}

	public void MakeDoubleKill()
	{
		AddExp(EExpType.DOUBLE_KILL);
		if (achievements != null)
		{
			achievements.AddDoubleKill();
		}
	}

	public void MakeMultiKill(int killCount)
	{
		AddExp(EExpType.MULTI_KILL);
		if (achievements != null)
		{
			achievements.AddTripleKill();
			achievements.CheckMaxCombo(killCount);
		}
	}

	public void MakePayBack()
	{
		AddExp(EExpType.PAY_BACK);
	}

	public void MakeRevenge()
	{
		AddExp(EExpType.REVENGE);
	}

	public void MakeCapture()
	{
		AddExp(EExpType.CAPTURE);
		if (achievements != null)
		{
			achievements.AddCaptureFlag();
		}
	}

	public void MakeAssassinate()
	{
		AddExp(EExpType.ASSASSINATE);
	}

	public void MakeRampage()
	{
		AddExp(EExpType.RAMPAGE);
	}

	public void MakeDominate()
	{
		AddExp(EExpType.DOMINATE);
	}

	public void MakeLongShot()
	{
		AddExp(EExpType.LONG_SHOT);
	}

	public void MakeLegendary()
	{
		AddExp(EExpType.LEGENDARY);
	}

	public void MakeOccupation()
	{
		AddExp(EExpType.OCCUPATION);
		if (achievements != null)
		{
			achievements.AddOccupationNum();
		}
	}

	public void MakeOccupationAssist()
	{
		AddExp(EExpType.OCCUPATION_ASSIST);
		if (achievements != null)
		{
			achievements.AddOccupationNum();
		}
	}

	public void MakeStopHim()
	{
		AddExp(EExpType.STOP_HIM);
	}

	public void MakeUnStopable()
	{
		AddExp(EExpType.UNSTOPABLE);
	}

	public void MakePlayerBossKill()
	{
		AddExp(EExpType.PLAYER_BOSS_KILL);
	}

	public void MakePlayerBossDoubleKill()
	{
		AddExp(EExpType.PLAYER_BOSS_DOUBLE_KILL);
	}

	public void MakePlayerBossMultiKill()
	{
		AddExp(EExpType.PLAYER_BOSS_MULTI_KILL);
	}

	public void MakePlayerBossLongShotKill()
	{
		AddExp(EExpType.PLAYER_BOSS_LONG_SHOT);
	}

	public void MakePlayerBossRampage()
	{
		AddExp(EExpType.PLAYER_BOSS_RAMPAGE);
	}

	public void MakeEnoughDamageToPlayerBoss()
	{
		AddExp(EExpType.TO_PLAYER_BOSS_ENOUGH_DAMAGE);
	}

	public void MakeExecuted()
	{
		AddExp(EExpType.TO_PLAYER_BOSS_EXECUTED);
	}

	public void MakeWin(int exp)
	{
		AddExp(EExpType.WIN, exp);
	}

	public void MakeLose()
	{
		AddExp(EExpType.LOSE);
	}

	public int GetScore()
	{
		return playerBossModeScore;
	}

	public void SetScore(int score)
	{
		playerBossModeScore = score;
	}

	public int GetTotleDamageToPlayerBoss()
	{
		return totleDamageToPlayerBoss;
	}

	public void SetTotleDamageToPlayerBoss(int damage)
	{
		totleDamageToPlayerBoss = damage;
	}

	public static string GetLocIdOfExpType(EExpType expType, bool isPrivate)
	{
		if (isPrivate)
		{
			if (PrivateInfoLocId == null)
			{
				InitLocId();
			}
			return PrivateInfoLocId[(int)expType];
		}
		if (PublicInfoLocId == null)
		{
			InitLocId();
		}
		return PublicInfoLocId[(int)expType];
	}

	public static void InitLocId()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_EXP, DataSheetName.PVP_EXP);
		PrivateInfoLocId = new string[32];
		for (int i = 0; i < 32; i++)
		{
			PrivateInfoLocId[i] = gameDataTable.GetString(i, 1);
		}
		PublicInfoLocId = new string[32];
		for (int j = 0; j < 32; j++)
		{
			PublicInfoLocId[j] = gameDataTable.GetString(j, 2);
		}
	}

	public void ShowPrivateInfo(UserStateHUD.InfoBoxHUD infoBox, EExpType expType)
	{
		infoBox.SetPrivateAchieve(LocalizationManager.GetInstance().GetString(GetLocIdOfExpType(expType, true)), GetExpDataAfterExpBooth(expType));
	}

	public void ShowPrivateInfo(UserStateHUD.InfoBoxHUD infoBox, EExpType expType, int staticNum)
	{
		infoBox.SetPrivateAchieve(LocalizationManager.GetInstance().GetString(GetLocIdOfExpType(expType, true)), staticNum);
	}

	public void ShowPublicInfo(UserStateHUD.InfoBoxHUD infoBox, EExpType expType, Player playerOne, Player playerTwo, int num)
	{
		infoBox.SetBroadcasting(LocalizationManager.GetInstance().GetString(GetLocIdOfExpType(expType, false), num, 0, 0, string.Empty, string.Empty, string.Empty, string.Empty, playerOne, playerTwo));
	}

	public void ShowPickUpInfo(UserStateHUD.InfoBoxHUD infoBox, int pickUpId)
	{
		infoBox.SetPrivateAchieveWithoutNum(LocalizationManager.GetInstance().GetString("LOC_PICKUP_DESC_" + pickUpId));
	}
}
