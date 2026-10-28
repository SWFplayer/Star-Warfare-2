using System.Collections.Generic;

public abstract class Mission
{
	protected const int ACTION_NUMBER = 8;

	private const int SUCCESS_ACTION_ID = 98;

	private const int FAIL_ACTION_ID = 99;

	protected EMissionStatus mStatus;

	protected EMissionProcess mProcess;

	protected int mActionCounter;

	private Dictionary<int, List<MissionAction>> mActionDictionary = new Dictionary<int, List<MissionAction>>();

	private string _mGoldReward;

	private string _mMithrilReward;

	private string _mGoldReward_Heroic;

	private string _mMithrilReward_Heroic;

	public float mMissionRefreshInterval = Global.REFRESH_MISSION_INTERVAL;

	public bool mIsBossAppeared;

	public int ID { get; set; }

	public EDifficultMode DifficultMode { get; set; }

	public MissionType Type { get; set; }

	public bool IsMainMission { get; set; }

	public int TimeLimitation { get; set; }

	public string SummaryText { get; set; }

	public Dictionary<byte, SpawnConfig> EnemySpawnDictionary { get; set; }

	private int mGoldReward
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mGoldReward, "rengwujiangli");
		}
		set
		{
			_mGoldReward = AntiCracking.CryptBufferStr(value, "rengwujiangli");
		}
	}

	private int mMithrilReward
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mMithrilReward, "rengwujiangli");
		}
		set
		{
			_mMithrilReward = AntiCracking.CryptBufferStr(value, "rengwujiangli");
		}
	}

	private int mGoldReward_Heroic
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mGoldReward_Heroic, "rengwujiangli");
		}
		set
		{
			_mGoldReward_Heroic = AntiCracking.CryptBufferStr(value, "rengwujiangli");
		}
	}

	private int mMithrilReward_Heroic
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mMithrilReward_Heroic, "rengwujiangli");
		}
		set
		{
			_mMithrilReward_Heroic = AntiCracking.CryptBufferStr(value, "rengwujiangli");
		}
	}

	public EMissionProcess Process
	{
		get
		{
			return mProcess;
		}
	}

	public int ActionCounter
	{
		get
		{
			return mActionCounter;
		}
	}

	public void SetComplete()
	{
		mStatus = EMissionStatus.COMPLETED;
	}

	public void SetHalfComplete()
	{
		mStatus = EMissionStatus.HALF_COMPLETED;
	}

	public void SetUnComplete()
	{
		mStatus = EMissionStatus.UMCOMPLETED;
	}

	public bool IsComplete()
	{
		return mStatus == EMissionStatus.COMPLETED;
	}

	public bool IsHalfComplete()
	{
		return mStatus == EMissionStatus.HALF_COMPLETED;
	}

	public bool IsAtLeastHalfComplete()
	{
		return mStatus == EMissionStatus.HALF_COMPLETED || mStatus == EMissionStatus.COMPLETED;
	}

	public void AddMissionActions(Dictionary<int, Dictionary<int, MissionAction>> dic)
	{
		foreach (int key in dic.Keys)
		{
			List<MissionAction> list = new List<MissionAction>();
			foreach (MissionAction value in dic[key].Values)
			{
				list.Add(value);
			}
			mActionDictionary.Add(key, list);
		}
	}

	public virtual string GetProgressText()
	{
		return SummaryText;
	}

	protected GameDataTable GetDataTable()
	{
		string fileName = DataFileName.MISSION_LIST;
		if (DifficultMode == EDifficultMode.HEROIC)
		{
			fileName = DataFileName.MISSION_LIST_HEROIC;
		}
		return GameDataManager.GetInstance().GetGameDataTable(fileName, Type.ToString());
	}

	protected GameDataTable GetRobotTable()
	{
		string fileName = DataFileName.MISSION_LIST;
		if (DifficultMode == EDifficultMode.HEROIC)
		{
			fileName = DataFileName.MISSION_LIST_HEROIC;
		}
		return GameDataManager.GetInstance().GetGameDataTable(fileName, DataSheetName.ROBOT_PROPERTY);
	}

	public virtual void LoadConfig(int i)
	{
		string fileName = DataFileName.MISSION_LIST;
		if (DifficultMode == EDifficultMode.HEROIC)
		{
			fileName = DataFileName.MISSION_LIST_HEROIC;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(fileName, DataSheetName.MISSION_OVERALL);
		ID = gameDataTable.GetInt(i, 0);
		IsMainMission = gameDataTable.GetBoolean(i, 2);
		TimeLimitation = gameDataTable.GetInt(i, 3);
		SummaryText = LocalizationManager.GetInstance().GetString(gameDataTable.GetString(i, 4));
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.MISSION_REWARDS, DataSheetName.MISSION_REWARDS);
		mGoldReward = gameDataTable2.GetInt(i, 1);
		mMithrilReward = gameDataTable2.GetInt(i, 2);
		gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.MISSION_REWARDS_HEROIC, DataSheetName.MISSION_REWARDS);
		mGoldReward_Heroic = gameDataTable2.GetInt(i, 1);
		mMithrilReward_Heroic = gameDataTable2.GetInt(i, 2);
	}

	public void TriggerAction()
	{
		if (EnemySpawnScript.Instance != null)
		{
			EnemySpawnScript.Instance.ClearEventEnemy();
		}
		if (mActionDictionary.ContainsKey(mActionCounter))
		{
			foreach (MissionAction item in mActionDictionary[mActionCounter])
			{
				if (item.Type == MissionActionType.ACT_DIALOG)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_DIALOGUE);
				}
				item.OnTrigger();
			}
		}
		mActionCounter++;
	}

	public void TriggerSuccessAction()
	{
		if (!mActionDictionary.ContainsKey(98))
		{
			return;
		}
		foreach (MissionAction item in mActionDictionary[98])
		{
			item.StartAction();
		}
	}

	public void TriggerFailAction()
	{
		if (!mActionDictionary.ContainsKey(99))
		{
			return;
		}
		foreach (MissionAction item in mActionDictionary[99])
		{
			item.StartAction();
		}
	}

	public virtual void LevelStart()
	{
		mActionCounter = 0;
		mIsBossAppeared = false;
		MissionManager.GetInstance().SetLastRefreshTime();
		GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.inputController.ResetJoystickControlTime();
		foreach (List<MissionAction> value in mActionDictionary.Values)
		{
			foreach (MissionAction item in value)
			{
				item.LevelStart();
			}
		}
		TriggerAction();
	}

	public abstract EMissionProcess CheckIfComplete();

	public virtual float GetProgressPercentage()
	{
		return 0f;
	}

	public virtual int GetRewardCash()
	{
		return GetRewardCash(GameApp.GetInstance().GetUserState().GetSelectedDifficultMode());
	}

	public virtual int GetRewardCash(EDifficultMode diff)
	{
		if (diff == EDifficultMode.NORMAL)
		{
			return mGoldReward;
		}
		return mGoldReward_Heroic;
	}

	public virtual int GetRewardMithril()
	{
		return GetRewardMithril(GameApp.GetInstance().GetUserState().GetSelectedDifficultMode());
	}

	public virtual int GetRewardMithril(EDifficultMode diff)
	{
		if (diff == EDifficultMode.NORMAL)
		{
			return mMithrilReward;
		}
		return mMithrilReward_Heroic;
	}

	public virtual void ReFreshMissionTarget()
	{
		MissionManager.GetInstance().SetLastRefreshTime();
	}

	public List<EnemyType> GetBossTypeList()
	{
		List<EnemyType> list = new List<EnemyType>();
		foreach (List<MissionAction> value in mActionDictionary.Values)
		{
			foreach (MissionAction item in value)
			{
				if (item.Type == MissionActionType.ACT_BOSS)
				{
					MissionActionBoss missionActionBoss = item as MissionActionBoss;
					if (missionActionBoss != null && !list.Contains(missionActionBoss.BossType))
					{
						list.Add(missionActionBoss.BossType);
					}
				}
			}
		}
		return list;
	}
}
