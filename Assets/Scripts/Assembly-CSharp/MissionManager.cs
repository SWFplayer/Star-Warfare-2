using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager
{
	private static MissionManager mInstance;

	private MissionGroup[][] mNormalMissionArray;

	private MissionGroup[][] mHeroicMissionArray;

	private bool mIsLevelStarted;

	private int mTimeLimitation;

	private bool mIsSurvival;

	private bool mMainMissionFinished;

	private Timer mCheckTimer = new Timer();

	private bool mIsProtection;

	private bool mIsInited;

	private float LastRefreshTime;

	private List<MissionAction> mActionList = new List<MissionAction>();

	private List<int> mFirstCompleteSubMissionIdList = new List<int>();

	private bool mSpecialBossLeave;

	public int TimeLimitation
	{
		get
		{
			return mTimeLimitation;
		}
	}

	public bool MainMissionFinished
	{
		get
		{
			return mMainMissionFinished;
		}
	}

	public bool IsProtection
	{
		get
		{
			return mIsProtection;
		}
	}

	public int SpecialEnemyKills { get; set; }

	public EnemyType SpecialEnemyType { get; set; }

	public bool IsSpecialEnemyElite { get; set; }

	public MissionWeaponType WeaponKillEnemyType { get; set; }

	public int WeaponKillEnemyID { get; set; }

	public int WeaponKillEnemyAmount { get; set; }

	public EVehicleType VehicleKillEnemyType { get; set; }

	public int VehicleKillEnemyAmount { get; set; }

	public EVehicleWeaponType VehicleWeaponKillEnemyType { get; set; }

	public int VehicleWeaponKillEnemyAmount { get; set; }

	public int ExplosionKillEnemyAmount { get; set; }

	public int DamageToSpecialEnemy { get; set; }

	public float ProtectedItemHpPercentage { get; set; }

	private MissionManager()
	{
		mCheckTimer.SetTimer(1f, false);
	}

	public static MissionManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new MissionManager();
		}
		return mInstance;
	}

	public void Init()
	{
		if (mIsInited)
		{
			return;
		}
		mNormalMissionArray = new MissionGroup[Global.TOTAL_STAGE][];
		mHeroicMissionArray = new MissionGroup[Global.TOTAL_STAGE][];
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			mNormalMissionArray[i] = new MissionGroup[Global.TOTAL_SUB_STAGE];
			mHeroicMissionArray[i] = new MissionGroup[Global.TOTAL_SUB_STAGE];
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				mNormalMissionArray[i][j] = new MissionGroup();
				mNormalMissionArray[i][j].MissionArray = new Mission[Global.MISSION_SIZE];
				mHeroicMissionArray[i][j] = new MissionGroup();
				mHeroicMissionArray[i][j].MissionArray = new Mission[Global.MISSION_SIZE];
			}
		}
		mIsInited = true;
		Dictionary<int, Dictionary<int, Dictionary<int, MissionAction>>> dictionary = new Dictionary<int, Dictionary<int, Dictionary<int, MissionAction>>>();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACTION_LIST, DataSheetName.ACTION_OVERALL);
		for (int k = 0; k < gameDataTable.Count; k++)
		{
			int num = gameDataTable.GetInt(k, 0);
			if (num == 0)
			{
				continue;
			}
			int num2 = gameDataTable.GetInt(k, 1);
			int num3 = gameDataTable.GetInt(k, 2);
			string text = gameDataTable.GetString(k, 5);
			MissionActionType missionActionType = MissionActionType.NONE;
			if (Enum.IsDefined(typeof(MissionActionType), text))
			{
				missionActionType = (MissionActionType)(int)Enum.Parse(typeof(MissionActionType), text, true);
				MissionAction missionAction = null;
				switch (missionActionType)
				{
				case MissionActionType.ACT_CAMERA:
					missionAction = new MissionActionCamera();
					break;
				case MissionActionType.ACT_VFX:
					missionAction = new MissionActionVFX();
					break;
				case MissionActionType.ACT_OBJECT:
					missionAction = new MissionActionObject();
					break;
				case MissionActionType.ACT_PICKUP:
					missionAction = new MissionActionPickup();
					break;
				case MissionActionType.ACT_SPAWN:
					missionAction = new MissionActionEnemySpawn();
					break;
				case MissionActionType.ACT_SPAWN_PARAM:
					missionAction = new MissionActionSpawnParam();
					break;
				case MissionActionType.ACT_DIALOG:
					missionAction = new MissionActionDialog();
					break;
				case MissionActionType.ACT_BOSS:
					missionAction = new MissionActionBoss();
					break;
				case MissionActionType.ACT_DESTROY:
					missionAction = new MissionActionDestroy();
					break;
				case MissionActionType.ACT_RADAR:
					missionAction = new MissionActionRadar();
					break;
				case MissionActionType.ACT_VEHICLE:
					missionAction = new MissionActionVehicle();
					break;
				case MissionActionType.ACT_ROUTEENEMY:
					missionAction = new MissionActionRouteEnemy();
					break;
				}
				if (missionAction != null)
				{
					missionAction.Type = missionActionType;
					missionAction.ID = gameDataTable.GetInt(k, 3);
					missionAction.Delay = gameDataTable.GetFloat(k, 4);
					missionAction.difficultMode = EDifficultMode.NORMAL;
					missionAction.LoadConfig();
					if (!dictionary.ContainsKey(num))
					{
						Dictionary<int, Dictionary<int, MissionAction>> value = new Dictionary<int, Dictionary<int, MissionAction>>();
						dictionary.Add(num, value);
					}
					if (!dictionary[num].ContainsKey(num2))
					{
						Dictionary<int, MissionAction> value2 = new Dictionary<int, MissionAction>();
						dictionary[num].Add(num2, value2);
					}
					if (!dictionary[num][num2].ContainsKey(num3))
					{
						dictionary[num][num2].Add(num3, missionAction);
						continue;
					}
					Debug.LogError("DUPLICATED NORMAL ACTION: missioinId = " + num + ", triggerId = " + num2 + ", actionId = " + num3);
				}
				else
				{
					Debug.LogError("[NORMAL ACTION IS NULL]: row = " + k + ", typeString = " + text + ", actionType = " + missionActionType);
				}
			}
			else
			{
				Debug.LogError("[INVALID NORMAL ACTION TYPE]: row = " + k + ", typeString = " + text);
			}
		}
		Dictionary<int, Dictionary<int, Dictionary<int, MissionAction>>> dictionary2 = new Dictionary<int, Dictionary<int, Dictionary<int, MissionAction>>>();
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ACTION_LIST_HEROIC, DataSheetName.ACTION_OVERALL);
		for (int l = 0; l < gameDataTable2.Count; l++)
		{
			int num4 = gameDataTable2.GetInt(l, 0);
			if (num4 == 0)
			{
				continue;
			}
			int num5 = gameDataTable2.GetInt(l, 1);
			int num6 = gameDataTable2.GetInt(l, 2);
			string text2 = gameDataTable2.GetString(l, 5);
			MissionActionType missionActionType2 = MissionActionType.NONE;
			if (Enum.IsDefined(typeof(MissionActionType), text2))
			{
				missionActionType2 = (MissionActionType)(int)Enum.Parse(typeof(MissionActionType), text2, true);
				MissionAction missionAction2 = null;
				switch (missionActionType2)
				{
				case MissionActionType.ACT_CAMERA:
					missionAction2 = new MissionActionCamera();
					break;
				case MissionActionType.ACT_VFX:
					missionAction2 = new MissionActionVFX();
					break;
				case MissionActionType.ACT_OBJECT:
					missionAction2 = new MissionActionObject();
					break;
				case MissionActionType.ACT_PICKUP:
					missionAction2 = new MissionActionPickup();
					break;
				case MissionActionType.ACT_SPAWN:
					missionAction2 = new MissionActionEnemySpawn();
					break;
				case MissionActionType.ACT_SPAWN_PARAM:
					missionAction2 = new MissionActionSpawnParam();
					break;
				case MissionActionType.ACT_DIALOG:
					missionAction2 = new MissionActionDialog();
					break;
				case MissionActionType.ACT_BOSS:
					missionAction2 = new MissionActionBoss();
					break;
				case MissionActionType.ACT_DESTROY:
					missionAction2 = new MissionActionDestroy();
					break;
				case MissionActionType.ACT_RADAR:
					missionAction2 = new MissionActionRadar();
					break;
				case MissionActionType.ACT_VEHICLE:
					missionAction2 = new MissionActionVehicle();
					break;
				case MissionActionType.ACT_ROUTEENEMY:
					missionAction2 = new MissionActionRouteEnemy();
					break;
				}
				if (missionAction2 != null)
				{
					missionAction2.Type = missionActionType2;
					missionAction2.ID = gameDataTable2.GetInt(l, 3);
					missionAction2.Delay = gameDataTable2.GetFloat(l, 4);
					missionAction2.difficultMode = EDifficultMode.HEROIC;
					missionAction2.LoadConfig();
					if (!dictionary2.ContainsKey(num4))
					{
						Dictionary<int, Dictionary<int, MissionAction>> value3 = new Dictionary<int, Dictionary<int, MissionAction>>();
						dictionary2.Add(num4, value3);
					}
					if (!dictionary2[num4].ContainsKey(num5))
					{
						Dictionary<int, MissionAction> value4 = new Dictionary<int, MissionAction>();
						dictionary2[num4].Add(num5, value4);
					}
					if (!dictionary2[num4][num5].ContainsKey(num6))
					{
						dictionary2[num4][num5].Add(num6, missionAction2);
						continue;
					}
					Debug.LogError("DUPLICATED HEROIC ACTION: missioinId = " + num4 + ", triggerId = " + num5 + ", actionId = " + num6);
				}
				else
				{
					Debug.LogError("[HEROIC ACTION IS NULL]: row = " + l + ", typeString = " + text2 + ", actionType = " + missionActionType2);
				}
			}
			else
			{
				Debug.LogError("[INVALID HEROIC ACTION TYPE]: row = " + l + ", typeString = " + text2);
			}
		}
		GameDataTable gameDataTable3 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.MISSION_LIST, DataSheetName.MISSION_OVERALL);
		GameDataTable gameDataTable4 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.MISSION_LIST_HEROIC, DataSheetName.MISSION_OVERALL);
		UserState userState = GameApp.GetInstance().GetUserState();
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int count = gameDataTable3.Count;
		for (int m = 0; m < count; m++)
		{
			for (int n = 0; n < 2; n++)
			{
				GameDataTable gameDataTable5 = gameDataTable3;
				Dictionary<int, Dictionary<int, Dictionary<int, MissionAction>>> dictionary3 = dictionary;
				if (n == 1)
				{
					gameDataTable5 = gameDataTable4;
					dictionary3 = dictionary2;
				}
				int num10 = gameDataTable5.GetInt(m, 0);
				num7 = num10 / 100 - 1;
				num8 = num10 % 100 / 10;
				num9 = num10 % 10 - 1;
				string text3 = gameDataTable5.GetString(m, 1);
				MissionType missionType = MissionType.NONE;
				if (Enum.IsDefined(typeof(MissionType), text3))
				{
					missionType = (MissionType)(int)Enum.Parse(typeof(MissionType), text3, true);
					Mission mission = null;
					switch (missionType)
					{
					case MissionType.EMPTY:
						mission = new MissionEmpty();
						break;
					case MissionType.INFINITE:
						mission = new MissionInfinite();
						break;
					case MissionType.DESTINATION:
						mission = new MissionDestination();
						break;
					case MissionType.PROTECTION:
						mission = new MissionProtection();
						break;
					case MissionType.PROTECTION_MOVE:
						mission = new MissionProtectionMove();
						break;
					case MissionType.PROTECTION_NPC:
						mission = new MissionProtectionNPC();
						break;
					case MissionType.KILL_ENEMIES_AREA:
						mission = new MissionKillEnemiesArea();
						break;
					case MissionType.SURVIVAL:
						mission = new MissionSurvival();
						break;
					case MissionType.BASE_DESTRUCTION:
						mission = new MissionBaseDestruction();
						break;
					case MissionType.KILL_SPECIAL_ENEMY:
						mission = new MissionKillSpecialEnemy();
						break;
					case MissionType.ITEM_DESTRUCTION:
						mission = new MissionItemDestruction();
						break;
					case MissionType.RANDOM_DESTRUCTION:
						mission = new MissionRandomDestruction();
						break;
					case MissionType.WEAPON_KILL_ENEMY:
						mission = new MissionWeaponKillEnemy();
						break;
					case MissionType.VEHICLE_KILL_ENEMY:
						mission = new MissionVehicleKillEnemy();
						break;
					case MissionType.VEHICLE_WEAPON_KILL_ENEMY:
						mission = new MissionVehicleWeaponKillEnemy();
						break;
					case MissionType.EXPLOSION_KILL_ENEMY:
						mission = new MissionExplosionKillEnemy();
						break;
					case MissionType.PLAYER_STATUS:
						mission = new MissionPlayerStatus();
						break;
					case MissionType.PICK_UP:
						mission = new MissionPickup();
						break;
					case MissionType.PRE_BOSS:
						mission = new MissionPreBoss();
						break;
					case MissionType.BOSS:
						mission = new MissionBoss();
						break;
					case MissionType.CHARGE_DESTROY:
						mission = new MissionChargeDestroy();
						break;
					case MissionType.KILL_ROUTE_ENEMY:
						mission = new MissionKillRouteEnemy();
						break;
					}
					if (mission != null)
					{
						mission.Type = missionType;
						mission.DifficultMode = (EDifficultMode)n;
						mission.LoadConfig(m);
						if (userState.GetMissionStatus(num7, num8, num9, (EDifficultMode)n))
						{
							mission.SetComplete();
						}
						else
						{
							mission.SetUnComplete();
						}
						int key = (num7 + 1) * 100 + num8 * 10 + num9 + 1;
						if (dictionary3.ContainsKey(key))
						{
							mission.AddMissionActions(dictionary3[key]);
						}
						AddMission(num7, num8, num9, mission, (EDifficultMode)n);
					}
					else
					{
						Debug.LogError("[MISSION IS NULL]: row = " + m + ", typeString = " + text3 + ", missionType = " + missionType);
					}
				}
				else
				{
					Debug.LogError("[INVALID MISSION TYPE]: row = " + m + ", typeString = " + text3 + ", difficult = " + (EDifficultMode)n);
				}
			}
		}
	}

	public void UpdateMissionState()
	{
		if (!mIsInited)
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		MissionState normalMissionState = userState.GetNormalMissionState();
		MissionState heroicMissionState = userState.GetHeroicMissionState();
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				for (int k = 0; k < Global.MISSION_SIZE; k++)
				{
					if (normalMissionState.GetMissionStatus(i, j, k))
					{
						mNormalMissionArray[i][j].MissionArray[k].SetComplete();
					}
					else
					{
						mNormalMissionArray[i][j].MissionArray[k].SetUnComplete();
					}
					if (heroicMissionState.GetMissionStatus(i, j, k))
					{
						mHeroicMissionArray[i][j].MissionArray[k].SetComplete();
					}
					else
					{
						mHeroicMissionArray[i][j].MissionArray[k].SetUnComplete();
					}
				}
			}
		}
	}

	public void OnLanguageChanged()
	{
		mIsInited = false;
	}

	private void AddMission(int stageId, int subStageId, int missionId, Mission mission, EDifficultMode diffMode)
	{
		if (stageId < Global.TOTAL_STAGE && subStageId < Global.TOTAL_SUB_STAGE && missionId < Global.MISSION_SIZE)
		{
			switch (diffMode)
			{
			case EDifficultMode.NORMAL:
				mNormalMissionArray[stageId][subStageId][missionId] = mission;
				break;
			case EDifficultMode.HEROIC:
				mHeroicMissionArray[stageId][subStageId][missionId] = mission;
				break;
			}
		}
	}

	public Mission GetMission(byte stageId, byte subStageId, int missionId, EDifficultMode diffMode)
	{
		if (stageId < Global.TOTAL_STAGE && subStageId < Global.TOTAL_SUB_STAGE && missionId < Global.MISSION_SIZE)
		{
			switch (diffMode)
			{
			case EDifficultMode.NORMAL:
				return mNormalMissionArray[stageId][subStageId][missionId];
			case EDifficultMode.HEROIC:
				return mHeroicMissionArray[stageId][subStageId][missionId];
			}
		}
		return null;
	}

	public Mission GetCurrentStageMission(int missionId)
	{
		byte stageId = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte subStageId = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			stageId = selectedNetIndex;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				subStageId = (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				subStageId = (byte)Global.BOSS_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				subStageId = (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
		}
		return GetMission(stageId, subStageId, missionId, selectedDifficultMode);
	}

	public List<int> GetFirstCompleteSubMissionIdList()
	{
		return mFirstCompleteSubMissionIdList;
	}

	public void AddAction(MissionAction action)
	{
		mActionList.Add(action);
	}

	public void LevelInit()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		mIsLevelStarted = false;
		mFirstCompleteSubMissionIdList = new List<int>();
		GameApp.GetInstance().GetGameWorld().RemoveAllRadarPoints();
		byte b = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte b2 = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			b = selectedNetIndex;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				b2 = (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				b2 = (byte)Global.BOSS_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				b2 = (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
		}
		mTimeLimitation = 0;
		mMainMissionFinished = false;
		mIsSurvival = false;
		mIsProtection = false;
		WeaponKillEnemyAmount = 0;
		VehicleKillEnemyAmount = 0;
		VehicleWeaponKillEnemyAmount = 0;
		ExplosionKillEnemyAmount = 0;
		DamageToSpecialEnemy = 0;
		ProtectedItemHpPercentage = 100f;
		MissionGroup[][] array = mNormalMissionArray;
		if (selectedDifficultMode == EDifficultMode.HEROIC)
		{
			array = mHeroicMissionArray;
		}
		if (b < array.Length && b2 < array[b].Length)
		{
			Mission[] missionArray = array[b][b2].MissionArray;
			Mission[] array2 = missionArray;
			foreach (Mission mission in array2)
			{
				if (mission != null && mission.IsMainMission)
				{
					if (mission.TimeLimitation > 0)
					{
						mTimeLimitation = mission.TimeLimitation;
					}
					if (mission.Type == MissionType.SURVIVAL)
					{
						mIsSurvival = true;
					}
					if (mission.Type == MissionType.PROTECTION || mission.Type == MissionType.PROTECTION_MOVE || mission.Type == MissionType.PROTECTION_NPC)
					{
						mIsProtection = true;
					}
				}
			}
		}
		mSpecialBossLeave = false;
		mActionList.Clear();
	}

	private void LevelStart()
	{
		mIsLevelStarted = true;
		for (int i = 0; i < Global.MISSION_SIZE; i++)
		{
			Mission currentStageMission = GetCurrentStageMission(i);
			if (currentStageMission != null)
			{
				currentStageMission.LevelStart();
			}
		}
		if (TimeLimitation > 0)
		{
			GameApp.GetInstance().GetGameWorld().StartMissionClock();
		}
	}

	public void OnSpecialBossLeave()
	{
		mSpecialBossLeave = true;
	}

	public void OnMainMissionComplete(bool mainMissionSuccess)
	{
		mMainMissionFinished = true;
		byte b = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte b2 = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			b = selectedNetIndex;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				b2 = (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				b2 = (byte)Global.BOSS_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				b2 = (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
		}
		MissionGroup[][] array = mNormalMissionArray;
		if (selectedDifficultMode == EDifficultMode.HEROIC)
		{
			array = mHeroicMissionArray;
		}
		if (b >= array.Length || b2 >= array[b].Length)
		{
			return;
		}
		Mission[] missionArray = array[b][b2].MissionArray;
		for (int i = 0; i < missionArray.Length; i++)
		{
			if (missionArray[i] == null)
			{
				continue;
			}
			if (missionArray[i].IsMainMission)
			{
				if (mainMissionSuccess)
				{
					missionArray[i].TriggerSuccessAction();
				}
				else
				{
					missionArray[i].TriggerFailAction();
				}
			}
			else if (!missionArray[i].IsAtLeastHalfComplete())
			{
				if (mainMissionSuccess && missionArray[i].CheckIfComplete() == EMissionProcess.SUCCESS)
				{
					missionArray[i].SetComplete();
					mFirstCompleteSubMissionIdList.Add(i);
				}
			}
			else if (missionArray[i].IsHalfComplete())
			{
				if (mainMissionSuccess)
				{
					missionArray[i].SetComplete();
					mFirstCompleteSubMissionIdList.Add(i);
				}
				else
				{
					missionArray[i].SetUnComplete();
				}
			}
		}
	}

	public void Update()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		if (!mIsLevelStarted && InGameUIScript.bInited)
		{
			LevelStart();
		}
		if (mIsLevelStarted && !mMainMissionFinished && GameApp.GetInstance().GetGameWorld().State == GameState.Playing)
		{
			if (Time.time - LastRefreshTime > GetCurrentStageMission(0).mMissionRefreshInterval)
			{
				GetCurrentStageMission(0).ReFreshMissionTarget();
			}
			CheckMissionComplete();
			UpdateAction();
		}
	}

	private void UpdateAction()
	{
		List<MissionAction> list = new List<MissionAction>();
		foreach (MissionAction mAction in mActionList)
		{
			if (mAction.CanStart())
			{
				mAction.StartAction();
				list.Add(mAction);
			}
		}
		foreach (MissionAction item in list)
		{
			if (mActionList.Contains(item))
			{
				mActionList.Remove(item);
			}
		}
	}

	private void CheckMissionComplete()
	{
		if (!mCheckTimer.Ready())
		{
			return;
		}
		mCheckTimer.Do();
		byte b = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte b2 = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		byte selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			b = selectedNetIndex;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				b2 = (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				b2 = (byte)Global.BOSS_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				b2 = (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		MissionGroup[][] array = mNormalMissionArray;
		if (selectedDifficultMode == EDifficultMode.HEROIC)
		{
			array = mHeroicMissionArray;
		}
		if (b < array.Length && b2 < array[b].Length)
		{
			EMissionProcess eMissionProcess = EMissionProcess.NORMAL;
			Mission[] missionArray = array[b][b2].MissionArray;
			List<int> list = new List<int>();
			for (int i = 0; i < missionArray.Length; i++)
			{
				if (missionArray[i] == null)
				{
					continue;
				}
				if (missionArray[i].IsMainMission)
				{
					eMissionProcess = (mSpecialBossLeave ? EMissionProcess.SUCCESS : missionArray[i].CheckIfComplete());
					if (!missionArray[i].IsComplete() && eMissionProcess == EMissionProcess.SUCCESS)
					{
						mFirstCompleteSubMissionIdList.Add(i);
						missionArray[i].SetComplete();
						if (missionArray[i].Type == MissionType.BOSS)
						{
							array[b][b2 - 1][i].SetComplete();
						}
					}
				}
				else if (!missionArray[i].IsAtLeastHalfComplete() && missionArray[i].CheckIfComplete() == EMissionProcess.SUCCESS)
				{
					missionArray[i].SetHalfComplete();
					list.Add(i);
				}
			}
			if (list.Count > 0 && MissionDetailsHUD.m_Instance != null)
			{
				MissionDetailsHUD.m_Instance.OnSubMissionComplete(list);
			}
			switch (eMissionProcess)
			{
			case EMissionProcess.SUCCESS:
			{
				OnMainMissionComplete(true);
				if (localPlayer != null)
				{
					if (b == 0 && b2 == 1 && localPlayer.Kills == 0)
					{
						GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_NeedForSpeed();
					}
					GameApp.GetInstance().GetGameWorld().OnMainMissionFinished();
					localPlayer.OnWin();
					if (MissionDetailsHUD.m_Instance != null)
					{
						MissionDetailsHUD.m_Instance.OnWin();
					}
				}
				UserState userState = GameApp.GetInstance().GetUserState();
				for (int j = 0; j < Global.TOTAL_STAGE; j++)
				{
					for (int k = 0; k < Global.TOTAL_SUB_STAGE; k++)
					{
						for (int l = 0; l < Global.MISSION_SIZE; l++)
						{
							userState.SetMissionStatus(j, k, l, array[j][k][l].IsComplete());
						}
					}
				}
				break;
			}
			case EMissionProcess.FAIL:
				if (localPlayer != null && localPlayer.State != Player.WIN_STATE && localPlayer.State != Player.LOSE_STATE)
				{
					localPlayer.OnMissionFail();
				}
				break;
			}
		}
		if (!mMainMissionFinished && !mIsSurvival && !mIsProtection && localPlayer != null && localPlayer.State != Player.WIN_STATE && localPlayer.State != Player.LOSE_STATE)
		{
			VSClock missionClock = GameApp.GetInstance().GetGameWorld().MissionClock;
			if (missionClock != null && missionClock.GetTimeLeft() <= 0f)
			{
				localPlayer.OnMissionFail();
			}
		}
	}

	public static void SetMissionGameObjectState(GameObject obj, bool isActive)
	{
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetActive(isActive);
		}
		MonoBehaviour component = obj.GetComponent<MonoBehaviour>();
		if (null != component)
		{
			component.enabled = isActive;
		}
		obj.SetActive(isActive);
	}

	public void CheckWeaponKillEnemy(DamageProperty damageProperty)
	{
		if (damageProperty == null)
		{
			return;
		}
		if (damageProperty.isVehicle)
		{
			if (VehicleKillEnemyType == EVehicleType.NA || VehicleKillEnemyType == damageProperty.vehicleType)
			{
				VehicleKillEnemyAmount++;
			}
			if (VehicleWeaponKillEnemyType == damageProperty.vehicleWeaponType)
			{
				VehicleWeaponKillEnemyAmount++;
			}
		}
		else if (WeaponKillEnemyType != MissionWeaponType.SPECIAL_GUN)
		{
			if (WeaponKillEnemyType == Weapon.GetMissionWeaponType(damageProperty.wType))
			{
				WeaponKillEnemyAmount++;
			}
		}
		else if (WeaponKillEnemyID == damageProperty.weaponid)
		{
			WeaponKillEnemyAmount++;
		}
		if (damageProperty.damageType == PlayerDamageType.EXPLOSION && damageProperty.wType == WeaponType.NoGun)
		{
			ExplosionKillEnemyAmount++;
		}
	}

	public void SetAllMainMissionStatus(int stageId, int subStageId, int missionId, EMissionStatus status)
	{
		if (stageId == -1)
		{
			for (int i = 0; i < Global.TOTAL_STAGE; i++)
			{
				SetMissionStatusForStage(i, subStageId, missionId, status);
			}
		}
		else
		{
			SetMissionStatusForStage(stageId, subStageId, missionId, status);
		}
	}

	private void SetMissionStatusForStage(int stageId, int subStageId, int missionId, EMissionStatus status)
	{
		if (subStageId == -1)
		{
			for (int i = 0; i < Global.TOTAL_SUB_STAGE; i++)
			{
				SetMissionStatusForSubStage(stageId, i, missionId, status);
			}
		}
		else
		{
			SetMissionStatusForSubStage(stageId, subStageId, missionId, status);
		}
	}

	private void SetMissionStatusForSubStage(int stageId, int subStageId, int missionId, EMissionStatus status)
	{
		if (missionId == -1)
		{
			for (int i = 0; i < Global.MISSION_SIZE; i++)
			{
				if (status == EMissionStatus.COMPLETED)
				{
					mNormalMissionArray[stageId][subStageId][i].SetComplete();
				}
				else
				{
					mNormalMissionArray[stageId][subStageId][i].SetUnComplete();
				}
			}
		}
		else
		{
			mNormalMissionArray[stageId][subStageId][missionId].SetUnComplete();
		}
	}

	public void CheckSpecialEnemy(EnemyType type, bool isElite)
	{
		if (type.Equals(SpecialEnemyType) && isElite == IsSpecialEnemyElite)
		{
			SpecialEnemyKills++;
		}
	}

	public void SetLastRefreshTime()
	{
		LastRefreshTime = Time.time;
	}

	public bool IsInited()
	{
		return mIsInited;
	}

	public bool IsCurrentMissionKillingEnemy()
	{
		Mission currentStageMission = GetCurrentStageMission(0);
		if (currentStageMission.Type == MissionType.KILL_ENEMIES_AREA || currentStageMission.Type == MissionType.KILL_ROUTE_ENEMY || currentStageMission.Type == MissionType.KILL_SPECIAL_ENEMY)
		{
			return true;
		}
		return false;
	}
}
