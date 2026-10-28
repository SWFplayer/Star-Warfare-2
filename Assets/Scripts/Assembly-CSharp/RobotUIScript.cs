using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class RobotUIScript : MonoBehaviour
{
	public static RobotUIScript Instance;

	public List<UIRobot> robots = new List<UIRobot>();

	protected List<RobotRoom> rooms = new List<RobotRoom>();

	protected Queue<Task> tasks = new Queue<Task>();

	protected Timer getRoomListTimer = new Timer();

	protected RobotStatistics robotStatistics = new RobotStatistics();

	protected Timer doStatisticsTimer = new Timer();

	protected string startUserID = "0";

	private int RobotNum;

	private int userID;

	public static float PlayingDurationMin;

	public static float PlayingDurationMax;

	private string IP;

	public string exceptionStr = string.Empty;

	public List<RobotRemotePlayer> RobotRemotePlayerList = new List<RobotRemotePlayer>();

	private bool running;

	protected RobotRoom GetRobotRoom(short id)
	{
		foreach (RobotRoom room in rooms)
		{
			if (room.roomID == id)
			{
				return room;
			}
		}
		return null;
	}

	private void Start()
	{
		Debug.Log("-----------------------------------------");
		Instance = this;
		UIRobot.sRobotTest = true;
		exceptionStr = Application.dataPath;
		XmlDocument xmlDocument = new XmlDocument();
		string text = Application.dataPath + "/";
		Stream stream = File.Open(text + "config.xml", FileMode.Open);
		xmlDocument.Load(stream);
		stream.Close();
		IP = xmlDocument.SelectSingleNode("Config/Server").Attributes["IP"].Value;
		RobotNum = int.Parse(xmlDocument.SelectSingleNode("Config/Robot").Attributes["Num"].Value);
		userID = int.Parse(xmlDocument.SelectSingleNode("Config/Robot").Attributes["UserID"].Value);
		PlayingDurationMin = float.Parse(xmlDocument.SelectSingleNode("Config/PlayingDuration").Attributes["MinTime"].Value);
		PlayingDurationMax = float.Parse(xmlDocument.SelectSingleNode("Config/PlayingDuration").Attributes["MaxTime"].Value);
		getRoomListTimer.SetTimer(3f, false);
		doStatisticsTimer.SetTimer(1f, false);
		StartCoroutine(LoginUsers(RobotNum));
		running = true;
	}

	public void StopThreadReceiver()
	{
	}

	public void StopThreadSender()
	{
	}

	public void Close()
	{
		if (running)
		{
			try
			{
			}
			catch (IOException)
			{
			}
			running = false;
		}
	}

	public bool IsRunning()
	{
		return running;
	}

	public void SetRunning(bool run)
	{
		running = run;
	}

	public RobotRoom FindIdleRoom()
	{
		foreach (RobotRoom room in rooms)
		{
			if (room.rrState == RobotRoomState.Idle && room.PlayerNum < 3)
			{
				return room;
			}
		}
		return null;
	}

	public void ClearRoom()
	{
		rooms.Clear();
	}

	public void AddRoom(RobotRoom room)
	{
		rooms.Add(room);
	}

	private void Update()
	{
		if (getRoomListTimer.Ready())
		{
			RefreshRoomList();
			getRoomListTimer.Do();
		}
		if (doStatisticsTimer.Ready())
		{
			DoStatistics();
			doStatisticsTimer.Do();
		}
		foreach (UIRobot robot in robots)
		{
			robot.State.NextState(robot);
			robot.network.SendData();
			robot.network.ProcessRobotReceivedPackets(robot);
		}
		if (tasks.Count > 0)
		{
			Task task = tasks.Dequeue();
			if (task != null && task.taskID != 100 && task.taskID != 101)
			{
			}
		}
		foreach (UIRobot robot2 in robots)
		{
			if (robot2.IsDead && robot2.network != null)
			{
				robots.Remove(robot2);
				robot2.network.CloseRobotConnection();
				Debug.Log("Robot " + robot2.userName + " Is Dead");
				robotStatistics.robotLoginNumber--;
				LoginUser();
				break;
			}
		}
	}

	public void MasterStartGame(UIRobot robot)
	{
		if (robot.lobby.IsMasterPlayer)
		{
			StartGameRequest request = new StartGameRequest();
			robot.network.SendRequest(request);
		}
	}

	public void LoginUser()
	{
		NetworkManager networkManager = new NetworkManager(true);
		UIRobot uIRobot = new UIRobot();
		uIRobot.SetNetworkManager(networkManager);
		networkManager.StartNetworkRobot(uIRobot, IP, 10360);
		string empty = string.Empty;
		userID++;
		empty = "TestUser" + userID;
		RoleLoginRequest request = new RoleLoginRequest(empty, userID, "2014-07-25 14:42:58", 99, false);
		networkManager.SendRequest(request);
		uIRobot.userName = empty;
		uIRobot.o.name = empty;
		uIRobot.id = userID;
		uIRobot.timeMgr.Init();
		uIRobot.uiScript = this;
		robots.Add(uIRobot);
		robotStatistics.robotLoginNumber++;
	}

	private IEnumerator LoginUsers(int number)
	{
		for (int i = 0; i < number; i++)
		{
			LoginUser();
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void RefreshRoomList()
	{
		foreach (UIRobot robot in robots)
		{
			GetRoomListRequest request = new GetRoomListRequest(0, 0);
			robot.network.SendRequest(request);
		}
	}

	public void DoStatistics()
	{
		robotStatistics.robotIdleNumber = 0;
		robotStatistics.robotInGameNumber = 0;
		robotStatistics.robotInRoomWaitingMasterNumber = 0;
		robotStatistics.robotInRoomWaitingJoinNumber = 0;
		foreach (UIRobot robot in robots)
		{
			if (robot.State == robot.PlayingState)
			{
				robotStatistics.robotInGameNumber++;
			}
			else if (robot.State == robot.MasterInRoomState)
			{
				robotStatistics.robotInRoomWaitingMasterNumber++;
			}
			else if (robot.State == robot.InRoomState)
			{
				robotStatistics.robotInRoomWaitingJoinNumber++;
			}
			else if (robot.State == robot.IdleState)
			{
				robotStatistics.robotIdleNumber++;
			}
		}
	}

	private void OnGUI()
	{
		startUserID = GUI.TextField(new Rect(10f, 170f, 200f, 30f), startUserID);
		GUI.Label(new Rect(10f, 210f, 200f, 30f), "\ufffd\u0735\ufffd¼\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd" + robotStatistics.robotLoginNumber);
		GUI.Label(new Rect(10f, 250f, 200f, 30f), "\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdϷ\ufffdĻ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd" + robotStatistics.robotInGameNumber);
		GUI.Label(new Rect(10f, 290f, 200f, 30f), "\ufffd\ufffd\ufffd\ufffd\ufffdڵȴ\ufffd\ufffdĻ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdmaster\ufffd\ufffd" + robotStatistics.robotInRoomWaitingMasterNumber);
		GUI.Label(new Rect(10f, 330f, 200f, 30f), "\ufffd\ufffd\ufffd\ufffd\ufffdڵȴ\ufffd\ufffdĻ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdjoin\ufffd\ufffd" + robotStatistics.robotInRoomWaitingJoinNumber);
		GUI.Label(new Rect(10f, 370f, 200f, 30f), "\ufffd\ufffd\ufffdл\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd" + robotStatistics.robotIdleNumber);
		GUI.Label(new Rect(10f, 410f, 400f, 200f), exceptionStr);
		if (GUI.Button(new Rect(10f, 10f, 200f, 30f), "Login One Player"))
		{
			LoginUsers(1);
		}
		if (GUI.Button(new Rect(10f, 50f, 200f, 30f), "Login 10 Players"))
		{
			LoginUsers(10);
		}
		if (GUI.Button(new Rect(10f, 90f, 200f, 30f), "Login 30 Players"))
		{
			tasks.Enqueue(new Task(100));
		}
		if (GUI.Button(new Rect(250f, 10f, 200f, 30f), "Each Player Create A Game 2"))
		{
			foreach (UIRobot robot in robots)
			{
				if (robot.State == robot.IdleState)
				{
					CreateRoomRequest request = new CreateRoomRequest(robot.userName + "'s room", -1, 3, false, 2, 1, 1, 500, 0, 99, 0);
					robot.network.SendRequest(request);
				}
			}
		}
		if (GUI.Button(new Rect(250f, 50f, 200f, 30f), "Each Player Create A Game 3"))
		{
			foreach (UIRobot robot2 in robots)
			{
				if (robot2.State == robot2.IdleState)
				{
					CreateRoomRequest request2 = new CreateRoomRequest(robot2.userName + "'s room", -1, 3, false, 3, 1, 1, 500, 0, 99, 0);
					robot2.network.SendRequest(request2);
				}
			}
		}
		if (GUI.Button(new Rect(250f, 90f, 200f, 30f), "Each Player Join A Game"))
		{
			tasks.Enqueue(new Task(101));
		}
		if (GUI.Button(new Rect(10f, 130f, 200f, 30f), "Refresh RoomList"))
		{
			RefreshRoomList();
		}
		if (GUI.Button(new Rect(250f, 130f, 200f, 30f), "Master Start Game"))
		{
			foreach (UIRobot robot3 in robots)
			{
				MasterStartGame(robot3);
			}
		}
		if (GUI.Button(new Rect(500f, 130f, 200f, 30f), "Show Log"))
		{
			UIRobot.sShowLog = !UIRobot.sShowLog;
		}
	}

	private void OnApplicationQuit()
	{
		foreach (UIRobot robot in robots)
		{
			if (robot.network != null)
			{
				robot.network.CloseRobotConnection();
			}
		}
		Close();
	}

	public bool IsRobot(int channelID)
	{
		foreach (UIRobot robot in robots)
		{
			if (channelID == robot.lobby.GetChannelID())
			{
				return true;
			}
		}
		return false;
	}

	public void AddRobotRemotePlayer(int channelID, int roomID)
	{
		foreach (RobotRemotePlayer robotRemotePlayer2 in RobotRemotePlayerList)
		{
			if (robotRemotePlayer2.ChannelID == channelID)
			{
				return;
			}
		}
		RobotRemotePlayer robotRemotePlayer = new RobotRemotePlayer();
		robotRemotePlayer.ChannelID = channelID;
		robotRemotePlayer.RoomID = roomID;
		RobotRemotePlayerList.Add(robotRemotePlayer);
	}

	public void SetRobotRemotePlayerPosition(int channelID, Vector3 position)
	{
		foreach (RobotRemotePlayer robotRemotePlayer in RobotRemotePlayerList)
		{
			if (robotRemotePlayer.ChannelID == channelID)
			{
				robotRemotePlayer.Position = position;
			}
		}
	}

	public bool GetHumanPlayerPosition(int roomID, out Vector3 position)
	{
		foreach (RobotRemotePlayer robotRemotePlayer in RobotRemotePlayerList)
		{
			if (robotRemotePlayer.RoomID == roomID)
			{
				position = robotRemotePlayer.Position;
				return true;
			}
		}
		position = Vector3.zero;
		return false;
	}

	public UIRobot GetRobot(string name)
	{
		foreach (UIRobot robot in robots)
		{
			if (robot.userName == name)
			{
				return robot;
			}
		}
		return null;
	}
}
