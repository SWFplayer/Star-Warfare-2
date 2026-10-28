using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementState
{
	public class EWinType
	{
		public const int PVE = 0;

		public const int PVP = 1;
	}

	public static int ACHIEVEMENT_COUNT = 154;

	protected int score;

	protected AchievementInfo[] achievements = new AchievementInfo[ACHIEVEMENT_COUNT];

	protected int[] PVEWeaponTypeWins = new int[8];

	protected int[] PVPWeaponTypeWins = new int[8];

	protected int[] PVEWeaponUsed = new int[33];

	protected int[] PVPWeaponUsed = new int[33];

	protected int[] MissionPlayedNum = new int[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected int[] HeroMissionPlayedNum = new int[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected int[] MissionCompletedNum = new int[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected int[] HeroMissionCompletedNum = new int[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected bool[] MissionAllStarCompleted = new bool[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected bool[] HeroMissionAllStarCompleted = new bool[Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE];

	protected int[] PVPModePlayed = new int[6];

	protected int[] NormalEnemyKills = new int[9];

	protected int[] EliteEnemyKills = new int[9];

	protected int[] BossKills = new int[Global.TOTAL_STAGE];

	protected int[] VSModeWins = new int[6];

	protected UserState mUserstate;

	protected int VoidCancels;

	protected int PlayerKills;

	protected int DoubleKill;

	protected int TripleKill;

	protected int TotalGetCash;

	protected int TotalVoidMat;

	protected int MaxDamage;

	protected int MaxGS;

	protected int PVEDeathNum;

	protected int PVPDeathNum;

	protected int OccupationNum;

	protected int FlagNum;

	protected int FBNum;

	protected int ComboKillNum;

	protected int AttackCount;

	protected int HitEnemyCount;

	protected int VIPCount;

	protected List<int> titleList = new List<int>();

	protected int[] ItemUsed = new int[22];

	protected int[] FlaskUsed = new int[4];

	protected bool[] PickUpPicked = new bool[17];

	protected int pickUpNumCount;

	protected float[] pickUpTime = new float[4];

	protected int MaxComboWins;

	protected int CurrentComboWins;

	protected BigAchievement BigAchievements;

	public static List<int> HideAchivement = new List<int>();

	protected int rank = 1;

	protected int perkNum;

	protected List<int> killEnemyName = new List<int>();

	protected Dictionary<AchievementWeaponType, int> killEnemyWeaponDic = new Dictionary<AchievementWeaponType, int>();

	protected int FFAComboWins;

	protected float multiGameStartTime;

	private bool middleEnter;

	protected int currentPVPKillNum;

	protected int currentPVPAssistNum;

	protected int currentPVPDeadNum;

	protected int[] SpecialWinNum = new int[6];

	protected int KillWithFlagNum;

	protected int FlagerKills;

	protected int currentFlagNum;

	protected Dictionary<int, int> currentKillStat = new Dictionary<int, int>();

	protected bool alwaysFirst = true;

	protected int currentRelayNum;

	protected int currentOccupationNum;

	protected int currentDefenceNum;

	public bool C3PWinBack;

	public int[] ArmorSetWinnum = new int[Global.TOTAL_SET];

	public float missleLaunchTime;

	public int missle1count;

	public int missle2count;

	public int missle3count;

	private int CanonCount;

	private List<int> pickupIdList = new List<int>();

	private int SawKillNum;

	private float LastSwordKill;

	private int SwordKillNum;

	protected int[] SaturdayWinDate = new int[6];

	protected int[] InfiniteEnemyKills = new int[Global.TOTAL_STAGE];

	protected int[] HeroInifiniteEnemyKills = new int[Global.TOTAL_STAGE];

	protected int PVEMaxCombo;

	public AchievementState()
	{
		mUserstate = GameApp.GetInstance().GetUserState();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACHIEVEMENT, DataSheetName.TITLE);
		if (mUserstate.TitleDictionary.Count == 0)
		{
			mUserstate.TitleDictionary = new Dictionary<int, string>();
			for (int i = 0; i < gameDataTable.Count; i++)
			{
				mUserstate.TitleDictionary.Add(gameDataTable.GetInt(i, 0), gameDataTable.GetString(i, 1));
			}
		}
		titleList.Add(0);
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACHIEVEMENT, DataSheetName.ACHIEVEMENT_ALL);
		for (int j = 0; j < ACHIEVEMENT_COUNT; j++)
		{
			achievements[j] = new AchievementInfo();
			achievements[j].ID = gameDataTable2.GetInt(j, 0);
			achievements[j].Name = gameDataTable2.GetString(j, 1);
			achievements[j].Available = gameDataTable2.GetInt(j, 2) == 1;
			achievements[j].Visible = gameDataTable2.GetInt(j, 3) == 1;
			if (!achievements[j].Visible)
			{
				HideAchivement.Add(j);
			}
			achievements[j].Series = gameDataTable2.GetString(j, 4);
			achievements[j].Type = gameDataTable2.GetString(j, 5);
			achievements[j].Num = gameDataTable2.GetInt(j, 6);
			achievements[j].Description = gameDataTable2.GetString(j, 7);
			achievements[j].DisplayDescription = gameDataTable2.GetString(j, 7);
			achievements[j].TitleId = gameDataTable2.GetInt(j, 8);
			achievements[j].appleID = gameDataTable2.GetString(j, 9);
		}
		for (int k = 0; k < 8; k++)
		{
			PVPWeaponTypeWins[k] = 0;
			PVEWeaponTypeWins[k] = 0;
		}
		for (int l = 0; l < 33; l++)
		{
			PVPWeaponUsed[l] = 0;
			PVEWeaponUsed[l] = 0;
		}
		for (int m = 0; m < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; m++)
		{
			MissionPlayedNum[m] = 0;
			MissionCompletedNum[m] = 0;
			HeroMissionCompletedNum[m] = 0;
			HeroMissionPlayedNum[m] = 0;
		}
		for (int n = 0; n < 6; n++)
		{
			PVPModePlayed[n] = 0;
		}
		for (int num = 0; num < 9; num++)
		{
			NormalEnemyKills[num] = 0;
			EliteEnemyKills[num] = 0;
		}
		for (int num2 = 0; num2 < Global.TOTAL_STAGE; num2++)
		{
			BossKills[num2] = 0;
		}
		for (int num3 = 0; num3 < 6; num3++)
		{
			VSModeWins[num3] = 0;
			SpecialWinNum[num3] = 0;
			SaturdayWinDate[num3] = 0;
		}
		for (int num4 = 0; num4 < 17; num4++)
		{
			PickUpPicked[num4] = false;
		}
		for (int num5 = 0; num5 < 4; num5++)
		{
			pickUpTime[num5] = 0f;
		}
		for (int num6 = 0; num6 < 22; num6++)
		{
			ItemUsed[num6] = 0;
		}
		for (int num7 = 0; num7 < 4; num7++)
		{
			FlaskUsed[num7] = 0;
		}
		for (int num8 = 0; num8 < Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE; num8++)
		{
			MissionAllStarCompleted[num8] = false;
			HeroMissionAllStarCompleted[num8] = false;
		}
		for (int num9 = 0; num9 < Global.TOTAL_SET; num9++)
		{
			ArmorSetWinnum[num9] = 0;
		}
		for (int num10 = 0; num10 < Global.TOTAL_STAGE; num10++)
		{
			InfiniteEnemyKills[num10] = 0;
			HeroInifiniteEnemyKills[num10] = 0;
		}
	}

	public void LateInit()
	{
		BigAchievements = new BigAchievement(this);
		achievements[102].CurrentNum = 0;
		Dictionary<int, SwVoidSavedata> swVoidList = mUserstate.swVoidList;
		foreach (SwVoidSavedata value in swVoidList.Values)
		{
			if (value.GetLevel() >= 4)
			{
				achievements[102].CurrentNum++;
			}
		}
		achievements[93].CurrentNum = 0;
		Dictionary<int, Weapon> weapons = mUserstate.GetWeapons();
		foreach (Weapon value2 in weapons.Values)
		{
			if (value2.Level >= achievements[99].CurrentNum && value2.Level != 15)
			{
				achievements[99].CurrentNum = value2.Level;
			}
			if (value2.Level != 15 && value2.Level != 0)
			{
				achievements[93].CurrentNum++;
			}
		}
		achievements[94].CurrentNum = 0;
		List<List<Armor>> armor = mUserstate.GetArmor();
		for (int i = 0; i <= 3; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (armor[i][j].Level != 15 && armor[i][j].Level != 0)
				{
					achievements[94].CurrentNum++;
				}
			}
		}
		achievements[95].CurrentNum = 0;
		List<Armor> list = mUserstate.GetArmor()[4];
		for (int k = 0; k < 6; k++)
		{
			if (list[k].Level != 15 && list[k].Level != 0)
			{
				achievements[95].CurrentNum++;
			}
		}
		List<ESwVoidType> list2 = new List<ESwVoidType>();
		foreach (SwVoidSavedata value3 in mUserstate.swVoidList.Values)
		{
			if (!list2.Contains(value3.GetType()))
			{
				list2.Add(value3.GetType());
			}
		}
		achievements[96].CurrentNum = list2.Count;
	}

	public void SubmitAllToGameCenter()
	{
	}

	public void SubmitToGameCenter(int i)
	{
	}

	public void SubmitSuccess(string AppleId)
	{
		AchievementInfo[] array = achievements;
		foreach (AchievementInfo achievementInfo in array)
		{
			if (achievementInfo.appleID == AppleId)
			{
				achievementInfo.submitting = false;
			}
		}
	}

	public void AddScore(int scoreAdd)
	{
		score += scoreAdd;
	}

	public int GetScore()
	{
		return score;
	}

	public void SetScore(int num)
	{
		score = num;
	}

	public void AddTitleList(int title)
	{
		titleList.Add(title);
	}

	public List<int> GetTitleList()
	{
		return titleList;
	}

	public int GetVoidCancels()
	{
		return VoidCancels;
	}

	public void SetVoidCancels(int num)
	{
		VoidCancels = num;
		achievements[103].CurrentNum = num;
		achievements[104].CurrentNum = num;
		achievements[105].CurrentNum = num;
	}

	public int GetPlayerKills()
	{
		return PlayerKills;
	}

	public void SetPlayerKills(int num)
	{
		PlayerKills = num;
		achievements[0].CurrentNum = num;
		achievements[1].CurrentNum = num;
		achievements[2].CurrentNum = num;
		achievements[3].CurrentNum = num;
		achievements[4].CurrentNum = num;
		achievements[5].CurrentNum = num;
	}

	public int GetDoubleKill()
	{
		return DoubleKill;
	}

	public void SetDoubleKill(int num)
	{
		DoubleKill = num;
		achievements[6].CurrentNum = num;
	}

	public int GetTripleKill()
	{
		return TripleKill;
	}

	public void SetTripleKill(int num)
	{
		TripleKill = num;
		achievements[7].CurrentNum = num;
	}

	public int GetTotalCash()
	{
		return TotalGetCash;
	}

	public void AddTotalCash(int num)
	{
		TotalGetCash += num;
	}

	public void SetTotalCash(int num)
	{
		TotalGetCash = num;
	}

	public int GetVoidMat()
	{
		return TotalVoidMat;
	}

	public void SetVoidMat(int num)
	{
		TotalVoidMat = num;
	}

	public void AddVoidMat(int num)
	{
		TotalVoidMat += num;
	}

	public int GetPVEMaxCombo()
	{
		return PVEMaxCombo;
	}

	public void SetPVEMaxCombo(int num)
	{
		PVEMaxCombo = Mathf.Max(num, PVEMaxCombo);
	}

	public int GetMaxDamage()
	{
		return MaxDamage;
	}

	public void SetMaxDamage(int num)
	{
		MaxDamage = num;
	}

	public void CheckMaxDamage(int num)
	{
		MaxDamage = Mathf.Max(MaxDamage, num);
	}

	public int GetMaxGS()
	{
		return MaxGS;
	}

	public void SetMaxGS(int num)
	{
		MaxGS = num;
	}

	public int GetPVPDeath()
	{
		return PVPDeathNum;
	}

	public void SetPVPDeath(int num)
	{
		PVPDeathNum = num;
	}

	public int GetPVEDeath()
	{
		return PVEDeathNum;
	}

	public void SetPVEDeath(int num)
	{
		PVEDeathNum = num;
	}

	public int GetOccupationNum()
	{
		return OccupationNum;
	}

	public void SetOccupationNum(int num)
	{
		OccupationNum = num;
	}

	public int GetFlagNum()
	{
		return FlagNum;
	}

	public void SetFlagNum(int num)
	{
		FlagNum = num;
	}

	public int GetFBNum()
	{
		return FBNum;
	}

	public void SetFBNum(int num)
	{
		FBNum = num;
	}

	public int GetComboKillNum()
	{
		return ComboKillNum;
	}

	public void SetComboKillNum(int num)
	{
		ComboKillNum = num;
	}

	public int GetAttackCount()
	{
		return AttackCount;
	}

	public void SetAttackCount(int num)
	{
		AttackCount = num;
	}

	public void AddAttackCount()
	{
		AttackCount++;
	}

	public void AddHitEnemyCount()
	{
		HitEnemyCount++;
	}

	public int GetHitEnemyCount()
	{
		return HitEnemyCount;
	}

	public void SetHitEnemyCount(int num)
	{
		HitEnemyCount = num;
	}

	public int GetVipCount()
	{
		return VIPCount;
	}

	public void AddVipCount()
	{
		VIPCount++;
	}

	public void SetVipCount(int num)
	{
		VIPCount = num;
	}

	public int[] GetItemUsed()
	{
		return ItemUsed;
	}

	public int[] GetFlaskUsed()
	{
		return FlaskUsed;
	}

	public void AddFlaskUsed(int id)
	{
		FlaskUsed[id - 1]++;
	}

	public void AddItemUsed(int id)
	{
		ItemUsed[id - 1 - 4]++;
	}

	public int GetMaxComboWins()
	{
		return MaxComboWins;
	}

	public void SetMaxComboWins(int num)
	{
		MaxComboWins = num;
	}

	public void CheckComboWins(bool isWinning)
	{
		if (isWinning)
		{
			CurrentComboWins++;
		}
		else
		{
			CurrentComboWins = 0;
		}
		if (CurrentComboWins > MaxComboWins)
		{
			MaxComboWins = CurrentComboWins;
		}
	}

	public int GetCurrentComboWins()
	{
		return CurrentComboWins;
	}

	public void SetCurrentComboWins(int num)
	{
		CurrentComboWins = num;
	}

	public int[] GetMissionCompleted()
	{
		return MissionCompletedNum;
	}

	public int[] GetHeroMissionCompleted()
	{
		return HeroMissionCompletedNum;
	}

	public int[] GetHeroMissionPlayed()
	{
		return HeroMissionPlayedNum;
	}

	public AchievementInfo[] GetAchievements()
	{
		return achievements;
	}

	public int GetSawKill()
	{
		return SawKillNum;
	}

	public void SetSawKill(int num)
	{
		SawKillNum = num;
		achievements[84].CurrentNum = num;
	}

	public void AddSawKill()
	{
		SawKillNum++;
		achievements[84].CurrentNum++;
		if (SawKillNum >= 200)
		{
			CheckAchievement(84);
		}
	}

	public void AddSwordKill()
	{
		if (LastSwordKill == 0f || Time.time - LastSwordKill < 1f)
		{
			SwordKillNum++;
			LastSwordKill = Time.time;
		}
		else
		{
			LastSwordKill = Time.time;
			SwordKillNum = 1;
		}
		if (SwordKillNum >= 3)
		{
			CheckAchievment_SaintWarror();
		}
	}

	public int[] GetWeaponTypeWins(int WinType)
	{
		return (WinType != 0) ? PVPWeaponTypeWins : PVEWeaponTypeWins;
	}

	public void SetWeaponTypeWins()
	{
		for (int i = 0; i < 5; i++)
		{
			achievements[i * 4 + 15].CurrentNum = PVPWeaponTypeWins[i] + PVEWeaponTypeWins[i];
			achievements[i * 4 + 16].CurrentNum = PVPWeaponTypeWins[i] + PVEWeaponTypeWins[i];
			achievements[i * 4 + 17].CurrentNum = PVPWeaponTypeWins[i] + PVEWeaponTypeWins[i];
			achievements[i * 4 + 18].CurrentNum = PVPWeaponTypeWins[i] + PVEWeaponTypeWins[i];
		}
	}

	public void AddDeathNum(bool isPVPMode)
	{
		if (isPVPMode)
		{
			currentPVPDeadNum++;
			PVPDeathNum++;
			CanonCount = 0;
		}
		else
		{
			PVEDeathNum++;
		}
	}

	public int[] GetWeaponUsed(int WinType)
	{
		return (WinType != 0) ? PVPWeaponUsed : PVEWeaponUsed;
	}

	public int[] GetMissionPlayed()
	{
		return MissionPlayedNum;
	}

	public int[] GetVSModePlayed()
	{
		return PVPModePlayed;
	}

	public int[] GetNormalEnemyKills()
	{
		return NormalEnemyKills;
	}

	public int[] GetEliteEnemyKills()
	{
		return EliteEnemyKills;
	}

	public int[] GetBossKills()
	{
		return BossKills;
	}

	public void SetEnemyKills()
	{
		for (int i = 0; i < 9; i++)
		{
			achievements[i * 2 + 106].CurrentNum = NormalEnemyKills[i] + EliteEnemyKills[i];
			achievements[i * 2 + 107].CurrentNum = NormalEnemyKills[i] + EliteEnemyKills[i];
		}
	}

	public int[] GetVSModeWins()
	{
		return VSModeWins;
	}

	public int[] GetSpecialVSModeWins()
	{
		return SpecialWinNum;
	}

	public int[] GetSaturdayWinNum()
	{
		return SaturdayWinDate;
	}

	public void SetVSModeWins()
	{
		achievements[40].CurrentNum = VSModeWins[0];
		achievements[48].CurrentNum = VSModeWins[1];
		achievements[54].CurrentNum = VSModeWins[2];
		achievements[62].CurrentNum = VSModeWins[3];
		achievements[69].CurrentNum = VSModeWins[4];
	}

	public void SetSpecialVSModeWins()
	{
		achievements[51].CurrentNum = SpecialWinNum[1];
		achievements[42].CurrentNum = SpecialWinNum[0];
		achievements[67].CurrentNum = SpecialWinNum[3];
		achievements[70].CurrentNum = SpecialWinNum[4];
	}

	public bool[] GetPickUp()
	{
		return PickUpPicked;
	}

	public void SetPickUp()
	{
		int num = 0;
		bool[] pickUpPicked = PickUpPicked;
		for (int i = 0; i < pickUpPicked.Length; i++)
		{
			if (pickUpPicked[i])
			{
				num++;
			}
		}
		achievements[146].CurrentNum = num;
		achievements[147].CurrentNum = num;
	}

	public int GetRank()
	{
		return rank;
	}

	public void SetRank(int num)
	{
		rank = num;
		achievements[9].CurrentNum = rank;
		achievements[10].CurrentNum = rank;
		achievements[11].CurrentNum = rank;
		achievements[12].CurrentNum = rank;
		achievements[13].CurrentNum = rank;
	}

	public int GetPerkNum()
	{
		return perkNum;
	}

	public void SetPerkNum(int num)
	{
		perkNum = num;
		achievements[8].CurrentNum = perkNum;
	}

	public bool[] GetMissionAllStar()
	{
		return MissionAllStarCompleted;
	}

	public bool[] GetHeroMissionAllStar()
	{
		return HeroMissionAllStarCompleted;
	}

	public int GetKillWithFlagNum()
	{
		return KillWithFlagNum;
	}

	public void SetKillWithFlagNum(int num)
	{
		KillWithFlagNum = num;
		achievements[56].CurrentNum = num;
	}

	public void AddKillWithFlagNum()
	{
		KillWithFlagNum++;
		achievements[56].CurrentNum = KillWithFlagNum;
		if (KillWithFlagNum >= Global.ACHIEVEMENT_FLAGKILL)
		{
			CheckAchievement(56);
		}
	}

	public int GetFlagerKillsNum()
	{
		return FlagerKills;
	}

	public void SetFlagerKillsNum(int num)
	{
		FlagerKills = num;
		achievements[55].CurrentNum = num;
	}

	public void AddFlagerKills()
	{
		FlagerKills++;
		achievements[55].CurrentNum = FlagerKills;
		if (FlagerKills >= Global.ACHIEVEMENT_KILLFLAGER)
		{
			CheckAchievement(55);
		}
	}

	public int[] GetArmorSetWins()
	{
		return ArmorSetWinnum;
	}

	public void SetArmorSetWins()
	{
		int num = 0;
		for (int i = 0; i < Global.TOTAL_SET; i++)
		{
			if (ArmorSetWinnum[i] > 0)
			{
				num++;
			}
		}
		achievements[90].CurrentNum = num;
		achievements[91].CurrentNum = num;
		achievements[92].CurrentNum = num;
	}

	public int[] GetInfiniteKills()
	{
		return InfiniteEnemyKills;
	}

	public int[] GetHeroInfiniteKills()
	{
		return HeroInifiniteEnemyKills;
	}

	public void CheckAchievement(int i)
	{
		if (!achievements[i].complete)
		{
			achievements[i].complete = true;
			achievements[i].submitting = true;
			if (achievements[i].TitleId != 0)
			{
				titleList.Add(achievements[i].TitleId);
			}
			OnAchievementComplete(i);
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				SubmitToGameCenter(i);
			}
		}
	}

	public void CheckAchievement_Mission(int stage)
	{
		bool flag = true;
		for (int i = stage * Global.TOTAL_SUB_STAGE + 1; i < stage * Global.TOTAL_SUB_STAGE + 6; i++)
		{
			flag = MissionAllStarCompleted[i] && flag;
		}
		if (flag && MissionCompletedNum[stage * Global.TOTAL_SUB_STAGE + 6] > 0)
		{
			CheckAchievement(stage + 124);
		}
	}

	public void CheckAchievement_HeroMission(int stage)
	{
		bool flag = true;
		for (int i = stage * Global.TOTAL_SUB_STAGE + 1; i < stage * Global.TOTAL_SUB_STAGE + 6; i++)
		{
			flag = HeroMissionAllStarCompleted[i] && flag;
		}
		if (flag && HeroMissionCompletedNum[stage * Global.TOTAL_SUB_STAGE + 6] > 0)
		{
			CheckAchievement(stage + 129);
		}
	}

	public void AddAllStar()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		EDifficultMode selectedDifficultMode = userState.GetSelectedDifficultMode();
		int selectedStageIndex = userState.GetSelectedStageIndex();
		int selectedSubStageIndex = userState.GetSelectedSubStageIndex();
		switch (selectedDifficultMode)
		{
		case EDifficultMode.NORMAL:
			MissionAllStarCompleted[selectedStageIndex * Global.TOTAL_SUB_STAGE + selectedSubStageIndex] = true;
			break;
		case EDifficultMode.HEROIC:
			HeroMissionAllStarCompleted[selectedStageIndex * Global.TOTAL_SUB_STAGE + selectedSubStageIndex] = true;
			break;
		}
	}

	public void CheckAchievement_Register()
	{
		CheckAchievement(88);
	}

	public void CheckAchievement_OneVoid()
	{
		CheckAchievement(100);
	}

	public void CheckAchievement_VoidLevel5()
	{
		CheckAchievement(101);
		Dictionary<int, SwVoidSavedata> swVoidList = mUserstate.swVoidList;
		List<ESwVoidType> list = new List<ESwVoidType>();
		foreach (SwVoidSavedata value in swVoidList.Values)
		{
			if (value.GetLevel() >= 4 && !list.Contains(value.GetType()))
			{
				list.Add(value.GetType());
			}
		}
		achievements[102].CurrentNum = list.Count;
		if (list.Count >= 4)
		{
			CheckAchievement(102);
		}
	}

	public void AddVoidUpgradeCancels()
	{
		VoidCancels++;
		achievements[103].CurrentNum = VoidCancels;
		achievements[104].CurrentNum = VoidCancels;
		achievements[105].CurrentNum = VoidCancels;
		if (VoidCancels >= 1000)
		{
			CheckAchievement(105);
		}
		else if (VoidCancels >= 10)
		{
			CheckAchievement(104);
		}
		else if (VoidCancels >= 1)
		{
			CheckAchievement(103);
		}
	}

	public void OnAchievementComplete(int i)
	{
	}

	public AchievementWeaponType GetAchievementWeapon(WeaponType type)
	{
		switch (type)
		{
		case WeaponType.AssaultRifle:
		case WeaponType.MachineGun:
			return AchievementWeaponType.RIFLE;
		case WeaponType.ShotGun:
			return AchievementWeaponType.SHOTGUN;
		case WeaponType.RPG:
		case WeaponType.SplitRPG:
			return AchievementWeaponType.RPG;
		case WeaponType.LaserShotGun:
			return AchievementWeaponType.SHOTGUN;
		case WeaponType.LaserRifle:
			return AchievementWeaponType.RIFLE;
		case WeaponType.TwinRPG:
			return AchievementWeaponType.RPG;
		case WeaponType.TrackingRPG:
			return AchievementWeaponType.RPG;
		case WeaponType.Sniper:
		case WeaponType.UltraSniper:
			return AchievementWeaponType.SNIPER;
		case WeaponType.BowSword:
			return AchievementWeaponType.RPG;
		case WeaponType.RailGun:
			return AchievementWeaponType.RAIL_GUN;
		case WeaponType.DoublePistol:
		case WeaponType.ChargeDoublePistol:
			return AchievementWeaponType.PISTOL;
		case WeaponType.Fist:
		case WeaponType.QuadrupleFist:
		case WeaponType.Myth370Fist:
			return AchievementWeaponType.FIST;
		default:
			return AchievementWeaponType.OTHER;
		}
	}

	public AchievementEnemyType GetAchievementEnemyType(EnemyType type)
	{
		switch (type)
		{
		case EnemyType.DRONE:
			return AchievementEnemyType.DIGGER;
		case EnemyType.STALKER:
			return AchievementEnemyType.STALKER;
		case EnemyType.SCORPION:
			return AchievementEnemyType.TACORLION;
		case EnemyType.BANELING:
			return AchievementEnemyType.GREENLADY;
		case EnemyType.TANK:
			return AchievementEnemyType.MAMBULL;
		case EnemyType.MYTH370:
			return AchievementEnemyType.MYTH370;
		case EnemyType.BIGBEE:
			return AchievementEnemyType.BIGBEE;
		case EnemyType.DEMSTER:
			return AchievementEnemyType.DEMSTER;
		default:
			return AchievementEnemyType.OTHER;
		}
	}

	public void AddWeaponUsed()
	{
		int gunID = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetWeapon()
			.GunID;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			PVPWeaponUsed[gunID - 1]++;
		}
		else
		{
			PVEWeaponUsed[gunID - 1]++;
		}
	}

	public void AddMissionPlayed(EDifficultMode mode, int stage, int substage, bool isComplete)
	{
		switch (mode)
		{
		case EDifficultMode.NORMAL:
			MissionPlayedNum[stage * Global.TOTAL_SUB_STAGE + substage]++;
			if (isComplete)
			{
				MissionCompletedNum[stage * Global.TOTAL_SUB_STAGE + substage]++;
			}
			break;
		case EDifficultMode.HEROIC:
			HeroMissionPlayedNum[stage * Global.TOTAL_SUB_STAGE + substage]++;
			if (isComplete)
			{
				HeroMissionCompletedNum[stage * Global.TOTAL_SUB_STAGE + substage]++;
			}
			break;
		}
	}

	public void AddWeaponWinWith()
	{
		WeaponType weaponType = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetWeapon()
			.GetWeaponType();
		AchievementWeaponType achievementWeapon = GetAchievementWeapon(weaponType);
		if (achievementWeapon == AchievementWeaponType.OTHER)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			PVPWeaponTypeWins[(int)achievementWeapon]++;
		}
		else
		{
			PVEWeaponTypeWins[(int)achievementWeapon]++;
		}
		if (achievementWeapon == AchievementWeaponType.RAIL_GUN || achievementWeapon == AchievementWeaponType.FIST)
		{
			return;
		}
		int num = PVPWeaponTypeWins[(int)achievementWeapon];
		achievements[(int)achievementWeapon * 4 + 15].CurrentNum = num;
		achievements[(int)achievementWeapon * 4 + 16].CurrentNum = num;
		achievements[(int)achievementWeapon * 4 + 17].CurrentNum = num;
		achievements[(int)achievementWeapon * 4 + 18].CurrentNum = num;
		if (num >= 200)
		{
			CheckAchievement((int)achievementWeapon * 4 + 18);
			if (BigAchievements.IsComplete(39))
			{
				CheckAchievement(39);
				achievements[39].DisplayDescription = achievements[39].Description;
			}
			else
			{
				BigAchievements.GenerateDescription(39);
			}
		}
		else if (num >= 100)
		{
			CheckAchievement((int)achievementWeapon * 4 + 17);
		}
		else if (num >= 50)
		{
			CheckAchievement((int)achievementWeapon * 4 + 16);
		}
		else if (num >= 10)
		{
			CheckAchievement((int)achievementWeapon * 4 + 15);
		}
		int num2 = (GameApp.GetInstance().GetUserState().GetArmor(0)
			.SetElement - 1) * 18 + (GameApp.GetInstance().GetUserState().GetArmor(1)
			.SetElement - 1) * 9 + (GameApp.GetInstance().GetUserState().GetArmor(2)
			.SetElement - 1) * 3 + GameApp.GetInstance().GetUserState().GetArmor(3)
			.SetElement - 1;
		ArmorSetWinnum[num2]++;
		int num3 = 0;
		for (int i = 0; i < Global.TOTAL_SET; i++)
		{
			if (ArmorSetWinnum[i] > 0)
			{
				num3++;
			}
		}
		if (GameApp.GetInstance().GetUserState().GetArmor(0)
			.GetArmorID() != GameApp.GetInstance().GetUserState().GetArmor(1)
			.GetArmorID() || GameApp.GetInstance().GetUserState().GetArmor(1)
			.GetArmorID() != GameApp.GetInstance().GetUserState().GetArmor(2)
			.GetArmorID() || GameApp.GetInstance().GetUserState().GetArmor(2)
			.GetArmorID() != GameApp.GetInstance().GetUserState().GetArmor(3)
			.GetArmorID())
		{
			CheckAchievement(89);
		}
		achievements[90].CurrentNum = num3;
		achievements[91].CurrentNum = num3;
		achievements[92].CurrentNum = num3;
		if (num3 >= Global.ACHIVEMENT_SETWINCOUNT4)
		{
			CheckAchievement(92);
		}
		else if (num3 >= Global.ACHIVEMENT_SETWINCOUNT3)
		{
			CheckAchievement(91);
		}
		else if (num3 >= Global.ACHIVEMENT_SETWINCOUNT2)
		{
			CheckAchievement(90);
		}
	}

	public void AddEnemyKills(Enemy enemy)
	{
		EnemyType enemyType = enemy.EnemyType;
		AchievementEnemyType achievementEnemyType = GetAchievementEnemyType(enemyType);
		if (achievementEnemyType == AchievementEnemyType.OTHER)
		{
			if (mUserstate.GetSelectedDifficultMode() == EDifficultMode.HEROIC)
			{
				switch (enemyType)
				{
				case EnemyType.SLASHER:
					CheckAchievement(135);
					break;
				case EnemyType.DEAD_EYE:
					CheckAchievement(136);
					break;
				case EnemyType.FROST_STONE:
					CheckAchievement(137);
					break;
				case EnemyType.CERBERLISK:
					CheckAchievement(138);
					break;
				}
			}
			switch (enemyType)
			{
			case EnemyType.SLASHER:
				BossKills[0]++;
				Global.KillBossTime[0] = Time.time;
				break;
			case EnemyType.DEAD_EYE:
				BossKills[1]++;
				Global.KillBossTime[1] = Time.time;
				break;
			case EnemyType.FROST_STONE:
				BossKills[2]++;
				Global.KillBossTime[2] = Time.time;
				break;
			case EnemyType.CERBERLISK:
				BossKills[3]++;
				Global.KillBossTime[3] = Time.time;
				break;
			}
			bool flag = true;
			int[] bossKills = BossKills;
			foreach (int num in bossKills)
			{
				flag = flag && num != 0;
			}
			if (flag)
			{
				CheckAchievement(134);
			}
			if (Global.KillBossTime[0] != 0f && Global.KillBossTime[1] > Global.KillBossTime[0] && Global.KillBossTime[2] > Global.KillBossTime[1] && Global.KillBossTime[3] > Global.KillBossTime[2] && Global.KillBossTime[3] - Global.KillBossTime[0] <= 600f)
			{
				CheckAchievement(145);
			}
		}
		else
		{
			if (enemy.IsElite)
			{
				EliteEnemyKills[(int)achievementEnemyType]++;
			}
			else
			{
				NormalEnemyKills[(int)achievementEnemyType]++;
			}
			int num2 = NormalEnemyKills[(int)achievementEnemyType] + EliteEnemyKills[(int)achievementEnemyType];
			achievements[(int)achievementEnemyType * 2 + 106].CurrentNum = num2;
			achievements[(int)achievementEnemyType * 2 + 107].CurrentNum = num2;
			if (num2 >= 100000)
			{
				CheckAchievement((int)achievementEnemyType * 2 + 107);
			}
			else if (num2 >= 1000)
			{
				CheckAchievement((int)achievementEnemyType * 2 + 106);
			}
		}
	}

	public void CheckAchievement_WeaponMaxLevel()
	{
		CheckAchievement(99);
		Dictionary<int, Weapon> weapons = mUserstate.GetWeapons();
		foreach (Weapon value in weapons.Values)
		{
			if (value.Level >= achievements[99].CurrentNum && value.Level != 15)
			{
				achievements[99].CurrentNum = value.Level;
			}
		}
	}

	public void CheckAchievement_FullyArmored()
	{
		CheckAchievement(98);
	}

	public void CheckAchievement_NeedForSpeed()
	{
		CheckAchievement(151);
	}

	public void AddCombatKills()
	{
		PlayerKills++;
		currentPVPKillNum++;
		achievements[0].CurrentNum = PlayerKills;
		achievements[1].CurrentNum = PlayerKills;
		achievements[2].CurrentNum = PlayerKills;
		achievements[3].CurrentNum = PlayerKills;
		achievements[4].CurrentNum = PlayerKills;
		achievements[5].CurrentNum = PlayerKills;
		if (PlayerKills >= 1000000)
		{
			CheckAchievement(5);
		}
		else if (PlayerKills >= 100000)
		{
			CheckAchievement(4);
		}
		else if (PlayerKills >= 10000)
		{
			CheckAchievement(3);
		}
		else if (PlayerKills >= 1000)
		{
			CheckAchievement(2);
		}
		else if (PlayerKills >= 100)
		{
			CheckAchievement(1);
		}
		else if (PlayerKills >= 1)
		{
			CheckAchievement(0);
		}
	}

	public void AddAssistKills()
	{
		currentPVPAssistNum++;
	}

	public void AddDoubleKill()
	{
		DoubleKill++;
		achievements[6].CurrentNum = DoubleKill;
		if (DoubleKill >= 100)
		{
			CheckAchievement(6);
		}
	}

	public void AddTripleKill()
	{
		TripleKill++;
		achievements[7].CurrentNum = TripleKill;
		if (DoubleKill >= 100)
		{
			CheckAchievement(7);
		}
	}

	public void CheckMaxCombo(int maxCombo)
	{
		ComboKillNum = Mathf.Max(maxCombo, ComboKillNum);
	}

	public void CheckAchievement_GOODLUCK()
	{
		CheckAchievement(152);
	}

	public void CheckAchievement_DodgeSlaserWave()
	{
		CheckAchievement(140);
	}

	public AchievementVSMode GetAchievementMode(Mode mode)
	{
		switch (mode)
		{
		case Mode.VS_TDM:
			return AchievementVSMode.TDM;
		case Mode.VS_KOTH:
			return AchievementVSMode.KOTH;
		case Mode.VS_CTF_TDM:
			return AchievementVSMode.CTF;
		case Mode.VS_FFA:
			return AchievementVSMode.FFA;
		case Mode.Domination:
			return AchievementVSMode.DOM;
		case Mode.GIANT_BETRAYAL:
			return AchievementVSMode.GAINT;
		default:
			return AchievementVSMode.OTHER;
		}
	}

	public void AddVSWinNum(Mode mode, bool isWinning)
	{
		AchievementVSMode achievementMode = GetAchievementMode(mode);
		if (achievementMode == AchievementVSMode.OTHER)
		{
			return;
		}
		if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
		{
			SaturdayWinDate[(int)achievementMode] = DateTime.Today.Day;
			bool flag = true;
			int[] saturdayWinDate = SaturdayWinDate;
			foreach (int num in saturdayWinDate)
			{
				flag = flag && num == DateTime.Today.Day;
			}
			if (flag)
			{
				CheckAchievement(150);
			}
		}
		if (Time.time - missleLaunchTime < 15f && isWinning)
		{
			CheckAchievement(81);
		}
		if (missle1count > 0 && missle2count > 0 && missle3count > 0)
		{
			CheckAchievement(83);
		}
		if (achievementMode == AchievementVSMode.FFA && isWinning)
		{
			FFAComboWins++;
			if (FFAComboWins >= Global.ACHIEVEMENT_FFACOMBOWINS)
			{
				CheckAchievement(45);
			}
			if (Time.time - multiGameStartTime < 360f)
			{
				CheckAchievement(46);
			}
			if (middleEnter)
			{
				CheckAchievement(43);
			}
			CheckAchievement_AlwaysFirst();
		}
		else
		{
			FFAComboWins = 0;
		}
		if (achievementMode == AchievementVSMode.TDM && isWinning)
		{
			if (currentPVPKillNum >= Global.ACHIEVEMENT_TDMKILLWINS)
			{
				CheckAchievement(49);
			}
			if (Time.time - multiGameStartTime < 360f && GameApp.GetInstance().GetGameWorld().GetRemotePlayers()
				.Count >= 11)
			{
				CheckAchievement(46);
			}
			if (currentPVPAssistNum >= Global.ACHIEVEMENT_TDMGOODASSIST)
			{
				SpecialWinNum[(int)achievementMode]++;
				achievements[51].CurrentNum++;
				if (SpecialWinNum[(int)achievementMode] >= Global.ACHIEVEMENT_TDMGOODASSISTWINNUM)
				{
					CheckAchievement(51);
				}
			}
			if (currentPVPDeadNum == 0 && !middleEnter)
			{
				CheckAchievement(52);
			}
		}
		if (achievementMode == AchievementVSMode.CTF && isWinning)
		{
			if (currentFlagNum >= 5)
			{
				CheckAchievement(58);
			}
			if (currentFlagNum == 0)
			{
				CheckAchievement(59);
			}
		}
		if (achievementMode == AchievementVSMode.KOTH && isWinning)
		{
			if (Time.time - multiGameStartTime < 240f)
			{
				CheckAchievement(63);
			}
			if (GameApp.GetInstance().GetGameWorld().BattleInfo.GetEnemyTeamScore() < 100)
			{
				SpecialWinNum[3]++;
				achievements[67].CurrentNum++;
				if (SpecialWinNum[3] >= Global.ACHIEVEMENT_CTPSPECIALWINNUM)
				{
					CheckAchievement(67);
				}
			}
		}
		if (achievementMode == AchievementVSMode.DOM && isWinning)
		{
			if (currentOccupationNum == 0)
			{
				CheckAchievement(71);
			}
			if (Time.time - multiGameStartTime < 360f)
			{
				CheckAchievement(75);
			}
			if (currentOccupationNum >= 3)
			{
				SpecialWinNum[(int)achievementMode]++;
				achievements[70].CurrentNum++;
				if (SpecialWinNum[(int)achievementMode] >= Global.ACHIEVEMENT_C3PSPECIALWINNUM)
				{
					CheckAchievement(70);
				}
			}
			if (GameApp.GetInstance().GetGameWorld().BattleInfo.GetMyTeamScore() - GameApp.GetInstance().GetGameWorld().BattleInfo.GetEnemyTeamScore() <= 30)
			{
				CheckAchievement(72);
			}
			if (C3PWinBack)
			{
				CheckAchievement(74);
			}
		}
		PVPModePlayed[(int)achievementMode]++;
		if (!isWinning)
		{
			return;
		}
		VSModeWins[(int)achievementMode]++;
		if (achievementMode == AchievementVSMode.GAINT)
		{
			return;
		}
		achievements[40].CurrentNum = VSModeWins[0];
		achievements[48].CurrentNum = VSModeWins[1];
		achievements[54].CurrentNum = VSModeWins[2];
		achievements[62].CurrentNum = VSModeWins[3];
		achievements[69].CurrentNum = VSModeWins[4];
		if (VSModeWins[(int)achievementMode] >= 200)
		{
			switch (achievementMode)
			{
			case AchievementVSMode.FFA:
				CheckAchievement(40);
				break;
			case AchievementVSMode.TDM:
				CheckAchievement(48);
				break;
			case AchievementVSMode.CTF:
				CheckAchievement(54);
				break;
			case AchievementVSMode.KOTH:
				CheckAchievement(62);
				break;
			case AchievementVSMode.DOM:
				CheckAchievement(69);
				break;
			}
		}
	}

	public void CheckAchievement_RoofKill()
	{
		CheckAchievement(141);
	}

	public void CheckAchievement_GoodEquipment()
	{
		Dictionary<int, Weapon> weapons = mUserstate.GetWeapons();
		bool flag = true;
		int num = 0;
		for (int i = 1; i <= 24; i++)
		{
			if (weapons[i].Level == 15 || weapons[i].Level == 0)
			{
				flag = false;
			}
			else
			{
				num++;
			}
		}
		if (flag)
		{
			CheckAchievement(93);
		}
		achievements[93].CurrentNum = num;
		List<List<Armor>> armor = mUserstate.GetArmor();
		flag = true;
		num = 0;
		for (int j = 0; j <= 3; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				if (armor[j][k].Level == 15 || armor[j][k].Level == 0)
				{
					flag = false;
				}
				else
				{
					num++;
				}
			}
		}
		if (flag)
		{
			CheckAchievement(94);
		}
		achievements[94].CurrentNum = num;
		List<Armor> list = mUserstate.GetArmor()[4];
		flag = true;
		num = 0;
		for (int l = 0; l < 6; l++)
		{
			if (list[l].Level == 15 || list[l].Level == 0)
			{
				flag = false;
			}
			else
			{
				num++;
			}
		}
		if (flag)
		{
			CheckAchievement(95);
		}
		achievements[95].CurrentNum = num;
		List<ESwVoidType> list2 = new List<ESwVoidType>();
		foreach (SwVoidSavedata value in mUserstate.swVoidList.Values)
		{
			if (!list2.Contains(value.GetType()))
			{
				list2.Add(value.GetType());
			}
		}
		if (list2.Count >= 4)
		{
			CheckAchievement(96);
		}
		achievements[96].CurrentNum = list2.Count;
		if (BigAchievements.IsComplete(97))
		{
			CheckAchievement(97);
			achievements[97].DisplayDescription = achievements[97].Description;
		}
		else
		{
			BigAchievements.GenerateDescription(97);
		}
	}

	public void CheckAchievement_PickUp(int id)
	{
		pickUpTime[pickUpNumCount % 4] = Time.time;
		pickUpNumCount++;
		if (pickUpNumCount >= 4 && Time.time - pickUpTime[(pickUpNumCount + 1) % 4] <= 15f)
		{
			CheckAchievement(148);
		}
		PickUpPicked[id - 1] = true;
		int num = 0;
		for (int i = 0; i < 17; i++)
		{
			if (PickUpPicked[i])
			{
				num++;
			}
		}
		achievements[146].CurrentNum = num;
		achievements[147].CurrentNum = num;
		if (num >= 10)
		{
			CheckAchievement(147);
		}
		else if (num >= 1)
		{
			CheckAchievement(146);
		}
	}

	public void AddCaptureFlag()
	{
		FlagNum++;
		currentFlagNum++;
	}

	public void AddFBNum()
	{
		FBNum++;
	}

	public void AddOccupationNum()
	{
		OccupationNum++;
		currentOccupationNum++;
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination && currentOccupationNum >= 2 && currentDefenceNum >= 2)
		{
			CheckAchievement(73);
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
		{
			if (currentOccupationNum >= 1)
			{
				CheckAchievement(66);
			}
			if (currentOccupationNum >= 1 && currentDefenceNum >= 1)
			{
				CheckAchievement(64);
			}
		}
	}

	public void AddDefenceNum()
	{
		currentDefenceNum++;
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination && currentOccupationNum >= 2 && currentDefenceNum >= 2)
		{
			CheckAchievement(73);
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH && currentOccupationNum >= 1 && currentDefenceNum >= 1)
		{
			CheckAchievement(64);
		}
	}

	public void CheckAchievment_SaintWarror()
	{
		CheckAchievement(149);
	}

	public void CheckAchievement_5Perk(int num)
	{
		perkNum = num;
		achievements[8].CurrentNum = num;
		if (num >= Global.ACHIEVEMENT_PERK_NUM)
		{
			CheckAchievement(8);
		}
	}

	public void CheckAchivement_PVPLEVEL(int num)
	{
		rank = num;
		achievements[9].CurrentNum = num;
		achievements[10].CurrentNum = num;
		achievements[11].CurrentNum = num;
		achievements[12].CurrentNum = num;
		achievements[13].CurrentNum = num;
		if (num >= Global.ACHIVEMENT_PVPLEVELV)
		{
			CheckAchievement(13);
		}
		if (num >= Global.ACHIVEMENT_PVPLEVELIV)
		{
			CheckAchievement(12);
		}
		if (num >= Global.ACHIVEMENT_PVPLEVELIII)
		{
			CheckAchievement(11);
		}
		if (num >= Global.ACHIVEMENT_PVPLEVELII)
		{
			CheckAchievement(10);
		}
		if (num >= Global.ACHIVEMENT_PVPLEVELI)
		{
			CheckAchievement(9);
		}
	}

	public void CheckAchievement_KillAll(int num)
	{
		if (!killEnemyName.Contains(num))
		{
			killEnemyName.Add(num);
		}
		if (killEnemyName.Count >= 11)
		{
			CheckAchievement(41);
		}
	}

	public void AddKillStat(int killer)
	{
		if (!currentKillStat.ContainsKey(killer))
		{
			currentKillStat.Add(killer, 1);
		}
		else
		{
			Dictionary<int, int> dictionary2;
			Dictionary<int, int> dictionary = (dictionary2 = currentKillStat);
			int key2;
			int key = (key2 = killer);
			key2 = dictionary2[key2];
			dictionary[key] = key2 + 1;
		}
		int channelID = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetChannelID();
		if (!currentKillStat.ContainsKey(channelID) || currentKillStat[killer] > currentKillStat[channelID])
		{
			alwaysFirst = false;
		}
	}

	public void CheckAchievement_AlwaysFirst()
	{
		if (alwaysFirst)
		{
			SpecialWinNum[0]++;
			achievements[42].CurrentNum++;
		}
		if (SpecialWinNum[0] >= Global.ACHIEVEMENT_FFATOPONE)
		{
			CheckAchievement(42);
		}
	}

	public void CheckAchievement_AllWeaponKill(WeaponType type)
	{
		AchievementWeaponType achievementWeapon = GetAchievementWeapon(type);
		if (killEnemyWeaponDic.ContainsKey(achievementWeapon))
		{
			Dictionary<AchievementWeaponType, int> dictionary2;
			Dictionary<AchievementWeaponType, int> dictionary = (dictionary2 = killEnemyWeaponDic);
			AchievementWeaponType key2;
			AchievementWeaponType key = (key2 = achievementWeapon);
			int num = dictionary2[key2];
			dictionary[key] = num + 1;
		}
		else
		{
			killEnemyWeaponDic.Add(achievementWeapon, 1);
		}
		if (killEnemyWeaponDic.ContainsKey(AchievementWeaponType.RIFLE) && killEnemyWeaponDic.ContainsKey(AchievementWeaponType.SHOTGUN) && killEnemyWeaponDic.ContainsKey(AchievementWeaponType.SNIPER) && killEnemyWeaponDic.ContainsKey(AchievementWeaponType.RPG))
		{
			CheckAchievement(44);
		}
	}

	public void ResetPVPState()
	{
		killEnemyName = new List<int>();
		killEnemyWeaponDic = new Dictionary<AchievementWeaponType, int>();
		multiGameStartTime = Time.time;
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			middleEnter = GameApp.GetInstance().GetGameWorld().BattleInfo.TopScore.score > 0;
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_TDM)
		{
			middleEnter = GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[0] > 0 || GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[1] > 0;
		}
		else
		{
			middleEnter = false;
		}
		currentPVPKillNum = 0;
		currentPVPAssistNum = 0;
		currentPVPDeadNum = 0;
		currentFlagNum = 0;
		currentKillStat = new Dictionary<int, int>();
		alwaysFirst = true;
		currentDefenceNum = 0;
		currentOccupationNum = 0;
		C3PWinBack = false;
		missle1count = 0;
		missle2count = 0;
		missle3count = 0;
	}

	public void ResetRelayNum()
	{
		currentRelayNum = 0;
	}

	public void AddRelayNum()
	{
		currentRelayNum++;
		if (currentRelayNum >= Global.ACHIEVEMENT_CTFRELAY)
		{
			CheckAchievement(57);
		}
	}

	public void CheckAchievement_250WithFlag(float distance)
	{
		if (distance >= Global.ACHIEVEMENT_CTF250WITHFLAG)
		{
			CheckAchievement(60);
		}
	}

	public void CheckAchievement_6InPoint()
	{
		CheckAchievement(65);
	}

	public void CheckAchievement_NoKillStreak()
	{
		if (currentPVPKillNum == 0)
		{
			CheckAchievement(77);
		}
	}

	public void CheckAchievement_RadarExpert()
	{
		if (GameApp.GetInstance().GetGameMode().IsTeamMode() && GameApp.GetInstance().GetGameWorld().GetRemotePlayers()
			.Count == 11 && GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA && GameApp.GetInstance().GetGameWorld().GetRemotePlayers()
			.Count == 6)
		{
			CheckAchievement(78);
		}
	}

	public void CheckAchievemnet_Nuclear()
	{
		CheckAchievement(80);
	}

	public void SetMissleLaunchTime()
	{
		missleLaunchTime = Time.time;
	}

	public void AddCanonCount()
	{
		CanonCount++;
		if (CanonCount >= 2)
		{
			CheckAchievement(86);
		}
	}

	public void CheckAchievement_EnemyPointCanon()
	{
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.InEnemyPoint)
		{
			CheckAchievement(85);
		}
	}

	public void ResetPickupIDList()
	{
		pickupIdList = new List<int>();
	}

	public void AddPickupIDList(int ID)
	{
		if (!pickupIdList.Contains(ID))
		{
			pickupIdList.Add(ID);
		}
	}

	public void CheckAchievement_4Pickup()
	{
		if (pickupIdList.Count == 1)
		{
			CheckAchievement(82);
		}
	}

	public void CheckAchievement_50Spikes()
	{
		CheckAchievement(142);
	}

	public void CheckAchievement_NoFireDamage()
	{
		CheckAchievement(143);
	}

	public void SetInfiniteKillRecord()
	{
		if (GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex() == 0)
		{
			byte selectedStageIndex = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
			EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
			int kills = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Kills;
			switch (selectedDifficultMode)
			{
			case EDifficultMode.NORMAL:
				InfiniteEnemyKills[selectedStageIndex] = Mathf.Max(kills, InfiniteEnemyKills[selectedStageIndex]);
				break;
			case EDifficultMode.HEROIC:
				HeroInifiniteEnemyKills[selectedStageIndex] = Mathf.Max(kills, HeroInifiniteEnemyKills[selectedStageIndex]);
				break;
			}
		}
	}

	public void ReportToLeaderBoard()
	{
	}
}
