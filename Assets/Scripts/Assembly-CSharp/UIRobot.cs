using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRobot
{
	public static bool sRobotTest;

	public static bool sShowLog;

	public bool IsDead;

	public NetworkManager network;

	public string userName = string.Empty;

	public int id;

	public TimeManager timeMgr = new TimeManager();

	public GameApp gameApp = new GameApp();

	public Lobby lobby = new Lobby();

	public RobotUIScript uiScript;

	public GameObject o = new GameObject();

	private InputInfo inputInfo = new InputInfo();

	public Hashtable enemyList = new Hashtable();

	public int roundNum;

	public Timer spawnEnemyTimer = new Timer();

	public int joinedPlayer;

	public int maxPlayer = 3;

	public RobotState State;

	public RobotState IdleState = new UIRobotIdleState();

	public RobotState MasterInRoomState = new RobotMasterInRoomState();

	public RobotState InRoomState = new RobotInRoomState();

	public RobotState PlayingState = new RobotPlayingState();

	public RobotState WaitingLeaveState = new UIRobotWaitingLeaveRoomState();

	public Timer sendingTimer = new Timer();

	public Timer HitSkillTimer = new Timer(8f);

	public Timer AfterAttackTimer = new Timer(5f);

	public Timer RebirthTimer = new Timer(12f);

	public Timer PlayerHitPlayerTimer = new Timer(4f);

	public bool sendingJoingRequest;

	public bool sendingCreatingRoomRequest;

	public bool sendingStartGameRequest;

	public PlayerSkill mPlayerSkill;

	public float PlayingDuration;

	public List<string> Friends = new List<string>();

	public UIRobot()
	{
		timeMgr.Init();
		timeMgr.InitTimeBeforeSyncArray();
		timeMgr.setMaxLoopTimes(-1);
		timeMgr.setPeriod(1f);
		SetState(IdleState);
		spawnEnemyTimer.SetTimer(1f, false);
		sendingTimer.SetTimer(0.2f, false);
		lobby.IsMasterPlayer = false;
		mPlayerSkill = new PlayerSkill();
		mPlayerSkill.Clear();
		mPlayerSkill.CreateSkills();
		List<AdvancedSkill> list = new List<AdvancedSkill>();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SKILL, DataSheetName.SKILL_ALL);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			AdvancedSkill advancedSkill = new AdvancedSkill();
			advancedSkill.source = string.Empty;
			advancedSkill.SetExternalId(gameDataTable.GetInt(i, 1));
			advancedSkill.SetIconName(gameDataTable.GetString(i, 3));
			advancedSkill.SetOnTriggerType(gameDataTable.GetInt(i, 6));
			advancedSkill.SetOffTriggerType(gameDataTable.GetInt(i, 9));
			advancedSkill.SetOnTriggerValues(gameDataTable.GetString(i, 7), gameDataTable.GetString(i, 8));
			advancedSkill.SetOffTriggerValues(gameDataTable.GetString(i, 10), gameDataTable.GetString(i, 11));
			advancedSkill.SetSpecialSkillType(gameDataTable.GetString(i, 12));
			advancedSkill.SetSpecialSkillValue(gameDataTable.GetString(i, 13));
			for (int j = 14; j < 79; j++)
			{
				if (gameDataTable.GetString(i, j) != string.Empty)
				{
					float num = gameDataTable.GetFloat(i, j);
					if (num != 0f)
					{
						SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), ((ESkillInfoColumn)j).ToString());
						advancedSkill.AddSkill(type, num);
					}
				}
			}
			list.Add(advancedSkill);
		}
		foreach (AdvancedSkill item in list)
		{
			item.InitToPlayerSkill(mPlayerSkill);
		}
		UIRobotDebugComponent uIRobotDebugComponent = o.AddComponent<UIRobotDebugComponent>();
		uIRobotDebugComponent.UIRobot = this;
	}

	public void SetNetworkManager(NetworkManager manager)
	{
		network = manager;
		mPlayerSkill.IsRobot = true;
		mPlayerSkill.NetworkManager = manager;
		mPlayerSkill.needSendServerPlayerSkillCheck = true;
	}

	public void SendInput()
	{
		o.transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0, 360), 0f));
		inputInfo.moveDirection = o.transform.forward;
		int num = UnityEngine.Random.Range(0, 100);
		if (num < 50)
		{
			inputInfo.fire = true;
		}
		else
		{
			inputInfo.fire = false;
		}
		SendPlayerInputRequest request = new SendPlayerInputRequest(inputInfo.fire, inputInfo.IsMoving());
		network.SendRequest(request);
	}

	public void Move()
	{
		o.transform.Translate(7f * Time.deltaTime * Vector3.forward, Space.Self);
		Vector3 position;
		if (uiScript.GetHumanPlayerPosition(lobby.GetCurrentRoomID(), out position))
		{
			if (Vector3.Distance(o.transform.position, position) > 10f)
			{
				o.transform.position = position;
			}
		}
		else if (Vector3.Distance(o.transform.position, Vector3.zero) > 20f)
		{
			o.transform.position = Vector3.zero;
		}
	}

	public void SetState(RobotState state)
	{
		State = state;
		State.Start(this);
		State.EnterStateTime = Time.time;
	}

	public void Update()
	{
		mPlayerSkill.Loop();
		if (HitSkillTimer.Ready2())
		{
			mPlayerSkill.PlayerBeHit();
		}
		if (AfterAttackTimer.Ready2())
		{
			int wType = UnityEngine.Random.Range(1, 15);
			Weapon weapon = WeaponFactory.GetInstance().CreateWeapon((WeaponType)wType, 0);
			weapon.mSkillType = (Weapon.ESkillType)UnityEngine.Random.Range(1, 7);
			mPlayerSkill.PlayerAttack(weapon);
		}
		if (RebirthTimer.Ready2())
		{
			mPlayerSkill.PlayerRebirth();
		}
		if (!PlayerHitPlayerTimer.Ready2())
		{
			return;
		}
		PlayerHitPlayerTimer.SetTimer(UnityEngine.Random.Range(2f, 4f), false);
		foreach (UIRobot robot in uiScript.robots)
		{
			if (robot.lobby.GetCurrentRoomID() == lobby.GetCurrentRoomID() && robot.id != id)
			{
				PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(robot.lobby.GetChannelID(), AttackerType.PLAYER, 0, 0, 0);
				network.SendRequest(request);
				break;
			}
		}
	}

	public void UpdateEnemy()
	{
		object[] array = new object[enemyList.Count];
		enemyList.Keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			RobotEnemy robotEnemy = enemyList[array[i]] as RobotEnemy;
			robotEnemy.Loop(this);
		}
	}

	public void RemoveEnemy(short enemyID)
	{
		RobotEnemy robotEnemy = enemyList[enemyID] as RobotEnemy;
		UnityEngine.Object.Destroy(robotEnemy.o);
		enemyList.Remove(enemyID);
	}

	public void ClearAllEnemies()
	{
		object[] array = new object[enemyList.Count];
		enemyList.Keys.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			RobotEnemy robotEnemy = enemyList[array[i]] as RobotEnemy;
			UnityEngine.Object.Destroy(robotEnemy.o);
		}
		enemyList.Clear();
	}

	public void SpawnEnemy()
	{
		if (spawnEnemyTimer.Ready() && enemyList.Count < 8 && lobby.IsMasterPlayer)
		{
			EnemyType enemyType = EnemyType.DRONE;
			EnemySpawnType spawnType = EnemySpawnType.NORMAL;
			roundNum++;
			EnemySpawnRequest request = new EnemySpawnRequest(enemyType, spawnType, Vector3.zero, 0, (short)roundNum, false, false);
			network.SendRequest(request);
			spawnEnemyTimer.Do();
		}
	}
}
