using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class RobotSender
{
	public const int POLL_TIME = 20;

	public const int WRITE_BUF_SIZE = 512;

	private bool isRunning;

	private Thread t;

	public bool isExit;

	protected DateTime lastTime;

	private byte[] writebuffer = new byte[512];

	private List<UIRobot> robotList;

	public RobotSender(List<UIRobot> list)
	{
		robotList = list;
	}

	public void Init()
	{
		isRunning = true;
		isExit = false;
		t = new Thread(Run);
		t.Start();
		lastTime = DateTime.Now;
	}

	public void Run()
	{
		while (isRunning)
		{
			try
			{
				int count = robotList.Count;
				int num = 0;
				while (num < count)
				{
					UIRobot uIRobot = robotList[num];
					num++;
					if (uIRobot == null)
					{
						continue;
					}
					NetworkManager network = uIRobot.network;
					if (network == null)
					{
						continue;
					}
					RobotConnection robotConn = network.robotConn;
					if (robotConn == null)
					{
						continue;
					}
					BinaryWriter bw = robotConn.bw;
					if (bw == null)
					{
						continue;
					}
					int num2 = 0;
					while (!uIRobot.network.getSendPacketCache().isEmpty())
					{
						Request request = (Request)uIRobot.network.getSendPacketCache().SendPacket();
						if (request != null)
						{
							byte[] bytes = request.GetBytes();
							if (bytes.Length >= writebuffer.Length)
							{
								writebuffer = extendWriteBuf(writebuffer);
							}
							bytes.CopyTo(writebuffer, num2);
							num2 += bytes.Length;
						}
					}
					if (num2 > 0)
					{
						bw.Write(writebuffer, 0, num2);
						bw.Flush();
					}
					Thread.Sleep(20);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("sender:" + ex.StackTrace);
				break;
			}
		}
		isExit = true;
	}

	private byte[] extendWriteBuf(byte[] _writeBuf)
	{
		byte[] array = new byte[_writeBuf.Length * 2];
		_writeBuf.CopyTo(array, 0);
		return array;
	}

	public void StopThread()
	{
		isRunning = false;
	}
}
