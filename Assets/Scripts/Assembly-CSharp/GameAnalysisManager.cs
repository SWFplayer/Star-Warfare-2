using System.Collections.Generic;
using UnityEngine;

public class GameAnalysisManager
{
	public const int TWIN_BOSS_NUM = 2;

	public const int PVP_MODE_NUM = 6;

	public const int RECORD_NUM = 20;

	public const int SHOP_ITEM_NUM = 34;

	public const int IAP_GOLD_NUM = 7;

	public const int IAP_MITHRIL_NUM = 7;

	public const int SWVOID_MAX_LEVEL = 6;

	public static string currentAnalysisVersion = "104";

	private PVEStageAnalysisInfo[,] NormalSingleStageInfo = new PVEStageAnalysisInfo[Global.TOTAL_STAGE, Global.TOTAL_SUB_STAGE];

	private PVEStageAnalysisInfo[,] HeroSingleStageInfo = new PVEStageAnalysisInfo[Global.TOTAL_STAGE, Global.TOTAL_SUB_STAGE];

	private PVEStageAnalysisInfo[,] OnlineSurvivalInfo = new PVEStageAnalysisInfo[2, Global.TOTAL_STAGE];

	private PVEStageAnalysisInfo[,] OnlineBossInfo = new PVEStageAnalysisInfo[2, Global.TOTAL_STAGE];

	private PVEStageAnalysisInfo[,] OnlineTwinBossInfo = new PVEStageAnalysisInfo[2, 2];

	private PVPAnalysisInfo[] PVPInfo = new PVPAnalysisInfo[6];

	private int[] PVERecord = new int[20];

	private int[] PVPRecord = new int[20];

	private int[] ItemBuyNum = new int[34];

	private SwVoidAnalysisInfo[] VoidInfo = new SwVoidAnalysisInfo[6];

	private int[] GoldBuyNum = new int[7];

	private int[] MithrilBuyNum = new int[7];

	private string[] v104GoldBuyNum = new string[7];

	private string[] v104MithrilBuyNum = new string[7];

	private List<int> PackageBuyNum = new List<int>();

	public MithrilAnalysisInfo mithrilInfo = new MithrilAnalysisInfo();

	public GoldAnalysisInfo goldInfo = new GoldAnalysisInfo();

	public GameAnalysisManager()
	{
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				NormalSingleStageInfo[i, j] = new PVEStageAnalysisInfo();
				HeroSingleStageInfo[i, j] = new PVEStageAnalysisInfo();
			}
		}
		for (int k = 0; k < 2; k++)
		{
			for (int l = 0; l < Global.TOTAL_STAGE; l++)
			{
				OnlineSurvivalInfo[k, l] = new PVEStageAnalysisInfo();
				OnlineBossInfo[k, l] = new PVEStageAnalysisInfo();
			}
		}
		for (int m = 0; m < 2; m++)
		{
			for (int n = 0; n < 2; n++)
			{
				OnlineTwinBossInfo[m, n] = new PVEStageAnalysisInfo();
			}
		}
		for (int num = 0; num < 6; num++)
		{
			PVPInfo[num] = new PVPAnalysisInfo();
		}
		for (int num2 = 0; num2 < 20; num2++)
		{
			PVERecord[num2] = 0;
			PVPRecord[num2] = 0;
		}
		for (int num3 = 0; num3 < 34; num3++)
		{
			ItemBuyNum[num3] = 0;
		}
		for (int num4 = 0; num4 < 6; num4++)
		{
			VoidInfo[num4] = new SwVoidAnalysisInfo();
		}
		for (int num5 = 0; num5 < 7; num5++)
		{
			GoldBuyNum[num5] = 0;
		}
		for (int num6 = 0; num6 < 7; num6++)
		{
			MithrilBuyNum[num6] = 0;
		}
		for (int num7 = 0; num7 < 7; num7++)
		{
			v104GoldBuyNum[num7] = string.Empty;
		}
		for (int num8 = 0; num8 < 7; num8++)
		{
			v104MithrilBuyNum[num8] = string.Empty;
		}
	}

	public PVEStageAnalysisInfo[,] GetNormalSingleStageInfo()
	{
		return NormalSingleStageInfo;
	}

	public PVEStageAnalysisInfo[,] GetHeroSingleStageInfo()
	{
		return HeroSingleStageInfo;
	}

	public PVEStageAnalysisInfo[,] GetOnlineSurvivalInfo()
	{
		return OnlineSurvivalInfo;
	}

	public PVEStageAnalysisInfo[,] GetOnlineBossInfo()
	{
		return OnlineBossInfo;
	}

	public PVEStageAnalysisInfo[,] GetTwinBossInfo()
	{
		return OnlineTwinBossInfo;
	}

	public PVPAnalysisInfo[] GetPvpInfo()
	{
		return PVPInfo;
	}

	public int[] GetPveRecord()
	{
		return PVERecord;
	}

	public int[] GetPvpRecord()
	{
		return PVPRecord;
	}

	public SwVoidAnalysisInfo[] GetVoidInfo()
	{
		return VoidInfo;
	}

	public int[] GetGoldBuyNum()
	{
		return GoldBuyNum;
	}

	public string[] Getv104GoldBuyNum()
	{
		return v104GoldBuyNum;
	}

	public void AddGoldBuyNum(int index, string ver)
	{
		string[] array = v104GoldBuyNum[index].Split('|');
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(text.Split('@')[1], int.Parse(text.Split('@')[0]));
			}
		}
		if (dictionary.ContainsKey(ver))
		{
			Dictionary<string, int> dictionary3;
			Dictionary<string, int> dictionary2 = (dictionary3 = dictionary);
			string key2;
			string key = (key2 = ver);
			int num = dictionary3[key2];
			dictionary2[key] = num + 1;
		}
		else
		{
			dictionary.Add(ver, 1);
		}
		string text2 = string.Empty;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			text2 = text2 + item.Value + "@" + item.Key + "|";
		}
		text2 = text2.Substring(0, text2.Length - 1);
		v104GoldBuyNum[index] = text2;
	}

	public void SetGoldBuyNum(int index, string ver, int num)
	{
		string[] array = v104GoldBuyNum[index].Split('|');
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(text.Split('@')[1], int.Parse(text.Split('@')[0]));
			}
		}
		if (dictionary.ContainsKey(ver))
		{
			if (num > dictionary[ver])
			{
				dictionary[ver] = num;
			}
		}
		else
		{
			dictionary.Add(ver, num);
		}
		string text2 = string.Empty;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			text2 = text2 + item.Value + "@" + item.Key + "|";
		}
		text2 = text2.Substring(0, text2.Length - 1);
		v104GoldBuyNum[index] = text2;
	}

	public int[] GetMithrilBuyNum()
	{
		return MithrilBuyNum;
	}

	public string[] Getv104MithrilBuyNum()
	{
		return v104MithrilBuyNum;
	}

	public void AddMithrilBuyNum(int index, string ver)
	{
		string[] array = v104MithrilBuyNum[index].Split('|');
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(text.Split('@')[1], int.Parse(text.Split('@')[0]));
			}
		}
		if (dictionary.ContainsKey(ver))
		{
			Dictionary<string, int> dictionary3;
			Dictionary<string, int> dictionary2 = (dictionary3 = dictionary);
			string key2;
			string key = (key2 = ver);
			int num = dictionary3[key2];
			dictionary2[key] = num + 1;
		}
		else
		{
			dictionary.Add(ver, 1);
		}
		string text2 = string.Empty;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			text2 = text2 + item.Value + "@" + item.Key + "|";
		}
		text2 = text2.Substring(0, text2.Length - 1);
		v104MithrilBuyNum[index] = text2;
	}

	public void SetMithrilBuyNum(int index, string ver, int num)
	{
		string[] array = v104MithrilBuyNum[index].Split('|');
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(text.Split('@')[1], int.Parse(text.Split('@')[0]));
			}
		}
		if (dictionary.ContainsKey(ver))
		{
			if (num > dictionary[ver])
			{
				dictionary[ver] = num;
			}
		}
		else
		{
			dictionary.Add(ver, num);
		}
		string text2 = string.Empty;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			text2 = text2 + item.Value + "@" + item.Key + "|";
		}
		text2 = text2.Substring(0, text2.Length - 1);
		v104MithrilBuyNum[index] = text2;
	}

	public List<int> GetPackageBuyNum()
	{
		return PackageBuyNum;
	}

	public void VoidUpgrade(int i, int toLevel)
	{
		VoidInfo[i].UpgradeNum[toLevel - 1]++;
		Debug.Log("Upgrade Void" + i + " to level" + toLevel);
	}

	public void ProduceVoid(int i)
	{
		VoidInfo[i].UpgradeNum[0]++;
		Debug.Log("Create Void" + i);
	}

	public void ResolveVoid(int i, int level)
	{
		VoidInfo[i].ResolveNum[level - 1]++;
		Debug.Log("Resolve Void" + i + " level" + level);
	}

	public PVEStageAnalysisInfo GetPVEInfo(int stage, int substage, EDifficultMode mode)
	{
		if (mode == EDifficultMode.NORMAL)
		{
			return NormalSingleStageInfo[stage, substage];
		}
		return HeroSingleStageInfo[stage, substage];
	}

	public PVEStageAnalysisInfo GetCurrentPVEStageInfo()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			int num = userState.GetSelectedSubStageIndex();
			int selectedStageIndex = userState.GetSelectedStageIndex();
			EDifficultMode selectedDifficultMode = userState.GetSelectedDifficultMode();
			if (num == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				num = Global.BOSS_SUB_STAGE_ID;
			}
			if (selectedDifficultMode == EDifficultMode.NORMAL)
			{
				return NormalSingleStageInfo[selectedStageIndex, num];
			}
			return HeroSingleStageInfo[selectedStageIndex, num];
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
		{
			byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
			int selectedDifficultMode2 = (int)userState.GetSelectedDifficultMode();
			return OnlineSurvivalInfo[selectedDifficultMode2, selectedNetIndex];
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss || GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
		{
			if (userState.Boss1Type == EnemyType.NA)
			{
				byte selectedNetIndex2 = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
				int selectedDifficultMode3 = (int)userState.GetSelectedDifficultMode();
				return OnlineBossInfo[selectedDifficultMode3, selectedNetIndex2];
			}
			if (userState.Boss1Type == EnemyType.SLASHER)
			{
				int selectedDifficultMode4 = (int)userState.GetSelectedDifficultMode();
				return OnlineTwinBossInfo[selectedDifficultMode4, 0];
			}
			int selectedDifficultMode5 = (int)userState.GetSelectedDifficultMode();
			return OnlineTwinBossInfo[selectedDifficultMode5, 1];
		}
		Debug.Log("Cannot analyze this stage!");
		return NormalSingleStageInfo[0, 0];
	}

	public PVPAnalysisInfo GetCurrentPVPInfo()
	{
		AchievementVSMode achievementMode = GameApp.GetInstance().GetUserState().Achievement.GetAchievementMode(GameApp.GetInstance().GetGameMode().ModePlay);
		if (achievementMode != AchievementVSMode.OTHER)
		{
			return PVPInfo[(int)achievementMode];
		}
		Debug.Log("Cannot analyze this PVP mode!");
		return PVPInfo[0];
	}

	public void SetCurrentPlayRecord()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			for (int num = 19; num > 0; num--)
			{
				PVPRecord[num] = PVPRecord[num - 1];
			}
			PVPRecord[0] = (int)GameApp.GetInstance().GetUserState().Achievement.GetAchievementMode(GameApp.GetInstance().GetGameMode().ModePlay);
			return;
		}
		for (int num2 = 19; num2 > 0; num2--)
		{
			PVERecord[num2] = PVERecord[num2 - 1];
		}
		int num3 = 0;
		UserState userState = GameApp.GetInstance().GetUserState();
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			int num4 = userState.GetSelectedSubStageIndex();
			int selectedStageIndex = userState.GetSelectedStageIndex();
			EDifficultMode selectedDifficultMode = userState.GetSelectedDifficultMode();
			if (num4 == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				num4 = Global.BOSS_SUB_STAGE_ID;
			}
			num3 = (int)selectedDifficultMode * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE + selectedStageIndex * Global.TOTAL_SUB_STAGE + num4;
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
		{
			byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
			int selectedDifficultMode2 = (int)userState.GetSelectedDifficultMode();
			num3 = 2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE + selectedDifficultMode2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE + selectedNetIndex * Global.TOTAL_SUB_STAGE;
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss || GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
		{
			if (userState.Boss1Type == EnemyType.NA)
			{
				byte selectedNetIndex2 = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
				int selectedDifficultMode3 = (int)userState.GetSelectedDifficultMode();
				num3 = 2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE + selectedDifficultMode3 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE + selectedNetIndex2 * Global.TOTAL_SUB_STAGE + Global.TOTAL_SUB_STAGE - 1;
			}
			else if (userState.Boss1Type == EnemyType.SLASHER)
			{
				int selectedDifficultMode4 = (int)userState.GetSelectedDifficultMode();
				num3 = 2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE * 2;
			}
			else
			{
				int selectedDifficultMode5 = (int)userState.GetSelectedDifficultMode();
				num3 = 2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE * 2 + 1;
			}
		}
		PVERecord[0] = num3;
		int[] array = DecodePVEID(PVERecord[0]);
	}

	public int[] DecodePVEID(int ID)
	{
		int[] array = new int[4]
		{
			ID / (2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE),
			0,
			0,
			0
		};
		ID -= array[0] * 2 * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE;
		array[1] = ID / (Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE);
		ID -= array[1] * Global.TOTAL_STAGE * Global.TOTAL_SUB_STAGE;
		array[2] = ID / Global.TOTAL_SUB_STAGE;
		array[3] = ID - array[2] * Global.TOTAL_SUB_STAGE;
		return array;
	}

	public int[] GetItemBuyNum()
	{
		return ItemBuyNum;
	}

	public void IncreasePackageBuy(int index)
	{
		if (index >= PackageBuyNum.Count)
		{
			while (PackageBuyNum.Count <= index)
			{
				PackageBuyNum.Add(0);
			}
		}
		List<int> packageBuyNum;
		List<int> list = (packageBuyNum = PackageBuyNum);
		int index3;
		int index2 = (index3 = index);
		index3 = packageBuyNum[index3];
		list[index2] = index3 + 1;
	}

	public int GetPackageBuy(int index)
	{
		if (index >= PackageBuyNum.Count)
		{
			while (PackageBuyNum.Count <= index)
			{
				PackageBuyNum.Add(0);
			}
		}
		return PackageBuyNum[index];
	}

	public void AddADCashAnalysis(int num)
	{
		goldInfo.ADGold += num;
		if (GameApp.GetInstance().IsConnectedToServer())
		{
			SyncCashRequest request = new SyncCashRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void AddADMithrilAnalysis(int num)
	{
		mithrilInfo.ADMithril += num;
		if (GameApp.GetInstance().IsConnectedToServer())
		{
			SyncMithrilRequest request = new SyncMithrilRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public bool IsMissionMithrilLimited()
	{
		int num = 0;
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				PVEStageAnalysisInfo pVEInfo = GetPVEInfo(i, j, EDifficultMode.NORMAL);
				num += pVEInfo.mithrilCollect;
				PVEStageAnalysisInfo pVEInfo2 = GetPVEInfo(i, j, EDifficultMode.HEROIC);
				num += pVEInfo2.mithrilCollect;
			}
		}
		return num > 5000;
	}
}
