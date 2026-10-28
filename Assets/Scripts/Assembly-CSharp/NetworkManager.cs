using System;
using UnityEngine;

public class NetworkManager
{
	public const int WRITE_BUF_SIZE = 512;

	public string exceptionMessage;

	public bool IsTimeout;

	public string strIP;

	public int port;

	public Connection conn;

	public RobotConnection robotConn;

	private object lockThis = new object();

	private DateTime lastTime = DateTime.Now;

	private int packetNum;

	private int byteNum;

	private ReceivedPacketCache receivedPacketCache;

	private SendPacketCache sendPacketCache;

	private byte[] writebuffer = new byte[512];

	public bool sending;

	public bool IsDisconnected { get; set; }

	public bool IsDisplayErrorBox { get; set; }

	public NetworkManager()
	{
		conn = new Connection(this);
		strIP = "124.232.150.72";
		port = 10395;
	}

	public NetworkManager(bool robot)
	{
		robotConn = new RobotConnection(this);
	}

	public ReceivedPacketCache getReceivedPacketCache()
	{
		return receivedPacketCache;
	}

	public SendPacketCache getSendPacketCache()
	{
		return sendPacketCache;
	}

	public void StartNetwork(string ip, int port)
	{
		receivedPacketCache = new ReceivedPacketCache();
		sendPacketCache = new SendPacketCache();
		conn.StartConn(ip, port);
	}

	public void StartNetworkRobot(UIRobot robot, string ip, int port)
	{
		receivedPacketCache = new ReceivedPacketCache();
		sendPacketCache = new SendPacketCache();
		robotConn.StartConn(ip, port);
	}

	public static void RobotThreadProc(object obj)
	{
	}

	public void SendRequest(object request)
	{
		sendPacketCache.AddPacket(request);
		updatePacket(request);
	}

	public void SendData()
	{
		if (robotConn.IsConnected && !getSendPacketCache().isEmpty() && !sending)
		{
			Request request = (Request)getSendPacketCache().SendPacket();
			robotConn.SendData(request);
		}
	}

	private bool needSend(object request)
	{
		return true;
	}

	private void updatePacket(object request)
	{
		packetNum++;
		TimeSpan timeSpan = DateTime.Now - lastTime;
		if ((float)timeSpan.TotalSeconds > 1f)
		{
			GameApp.GetInstance().AverageSendPacket = (float)packetNum / (float)timeSpan.TotalSeconds;
			if (GameApp.GetInstance().AverageSendPacket > 50f)
			{
				Debug.LogWarning("Too many packets in one seconds:" + GameApp.GetInstance().AverageSendPacket);
			}
			packetNum = 0;
			lastTime = DateTime.Now;
		}
	}

	public void ProcessReceivedPackets()
	{
		while (!receivedPacketCache.isEmpty())
		{
			Response response = (Response)receivedPacketCache.RetrivePacket();
			if (response != null)
			{
				if (Application.loadedLevelName == "MultiMenu" && response.responseID > 100)
				{
					break;
				}
				try
				{
					response.ProcessLogic();
				}
				catch (Exception ex)
				{
					Debug.Log("Error Message : " + ex.Message + "\r\n" + ex.StackTrace);
					string text = "Client ProcessLogic Exception!!!\n\nResponse: " + response.responseID + "\n\n" + ex.Message + "\n" + ex.StackTrace;
					exceptionMessage += text;
					IsDisplayErrorBox = true;
					IsDisconnected = true;
					IsTimeout = false;
				}
			}
		}
	}

	public void ProcessRobotReceivedPackets(UIRobot robot)
	{
		while (!receivedPacketCache.isEmpty())
		{
			Response response = (Response)receivedPacketCache.RetrivePacket();
			if (response != null)
			{
				if (Application.loadedLevelName == "Lobby" && response.responseID >= 100)
				{
					break;
				}
				response.ProcessRobotLogic(robot);
			}
		}
	}

	public void CloseRobotConnection()
	{
		lock (lockThis)
		{
			if (robotConn != null)
			{
				robotConn.CloseSocket();
			}
			robotConn = null;
			receivedPacketCache.Clear();
			sendPacketCache.Clear();
		}
	}

	public bool IsConnected()
	{
		if (conn != null)
		{
			return conn.IsConnected();
		}
		return false;
	}

	public void CloseConnection()
	{
		lock (lockThis)
		{
			if (conn != null)
			{
				conn.Close();
			}
			conn = null;
			if (receivedPacketCache != null)
			{
				receivedPacketCache.Clear();
			}
			if (sendPacketCache != null)
			{
				sendPacketCache.Clear();
			}
		}
	}
}
