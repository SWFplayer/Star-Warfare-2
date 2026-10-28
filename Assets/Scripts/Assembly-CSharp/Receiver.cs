using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class Receiver
{
	private BinaryReader br;

	private bool isRunning;

	private Thread t;

	private NetworkManager networkMgr;

	public bool isExit;

	private DateTime lastTime = DateTime.Now;

	private int packetNum;

	private int byteNum;

	public Receiver(NetworkManager networkMgr)
	{
		this.networkMgr = networkMgr;
	}

	public void Init(BinaryReader br)
	{
		isRunning = true;
		isExit = false;
		this.br = br;
		t = new Thread(Run);
		t.Start();
	}

	public void Run()
	{
		while (isRunning)
		{
			short num = -1;
			short num2 = 0;
			byte[] array = new byte[0];
			try
			{
				if (br != null)
				{
					num = BytesBuffer.ReverseShort(br.ReadInt16());
					num2 = BytesBuffer.ReverseShort(br.ReadInt16());
					if (num2 > 0)
					{
						array = new byte[num2];
						DateTime now = DateTime.Now;
						int i = br.Read(array, 0, num2);
						DateTime now2 = DateTime.Now;
						TimeSpan timeSpan = now2 - now;
						int num3;
						for (; i != num2; i += num3)
						{
							Debug.Log("error...");
							num3 = br.Read(array, i, num2 - i);
						}
					}
					Response response = Response.CreateResponse(num);
					response.responseID = num;
					response.ReadData(array);
					networkMgr.getReceivedPacketCache().AddPacket(response);
					updatePacket(num2);
				}
				else
				{
					Thread.Sleep(16);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Error Message : " + ex.Message + "\r\n" + ex.StackTrace);
				Debug.Log("comandID: " + num + ", length: " + num2);
				string text = BitConverter.ToString(array);
				Debug.Log("hex: " + text);
				string text2 = "Client Receiver Exception!!!\n\nCommandID: " + num + "\n\nPackets: " + text + "\n\n" + ex.Message + "\n" + ex.StackTrace;
				networkMgr.exceptionMessage += text2;
				networkMgr.IsDisplayErrorBox = true;
				if (num == -1)
				{
					networkMgr.IsTimeout = true;
				}
				else
				{
					networkMgr.IsTimeout = false;
				}
				break;
			}
		}
		networkMgr.IsDisconnected = true;
		isExit = true;
	}

	private void updatePacket(short length)
	{
		packetNum++;
		byteNum = byteNum + 2 + length;
		TimeSpan timeSpan = DateTime.Now - lastTime;
		if ((float)timeSpan.TotalSeconds > 1f)
		{
			GameApp.GetInstance().AverageReceivePacket = (float)packetNum / (float)timeSpan.TotalSeconds;
			GameApp.GetInstance().AverageReceiveByte = (float)byteNum / (float)timeSpan.TotalSeconds;
			packetNum = 0;
			byteNum = 0;
			lastTime = DateTime.Now;
		}
	}

	public void StopThread()
	{
		isRunning = false;
		if (t != null)
		{
			t.Interrupt();
		}
	}

	public void Close()
	{
		try
		{
			if (br != null)
			{
				br.Close();
				br = null;
			}
		}
		catch (IOException ex)
		{
			networkMgr.exceptionMessage = ex.Message;
		}
	}
}
