using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class Sender
{
	public const int POLL_TIME = 20;

	public const int WRITE_BUF_SIZE = 512;

	private BinaryWriter bw;

	private bool isRunning;

	private Thread t;

	private NetworkManager networkMgr;

	public bool isExit;

	protected DateTime lastTime;

	private byte[] writebuffer = new byte[512];

	public Sender(NetworkManager networkMgr)
	{
		this.networkMgr = networkMgr;
	}

	public void Init(BinaryWriter bw)
	{
		isRunning = true;
		isExit = false;
		this.bw = bw;
		t = new Thread(Run);
		t.Start();
		lastTime = DateTime.Now;
	}

	public void Run()
	{
		short num = -1;
		while (isRunning)
		{
			try
			{
				if (bw != null)
				{
					int num2 = 0;
					while (!networkMgr.getSendPacketCache().isEmpty())
					{
						num = -1;
						Request request = (Request)networkMgr.getSendPacketCache().SendPacket();
						if (request != null)
						{
							num = request.GetRequestId();
							byte[] bytes = request.GetBytes();
							while (bytes.Length + num2 >= writebuffer.Length)
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
				}
				else
				{
					Debug.Log("bw is null..");
				}
				Thread.Sleep(20);
			}
			catch (Exception ex)
			{
				Debug.Log("sender:" + ex.Message + "\r\n" + ex.StackTrace);
				string text = "Client Sender Exception!!!\nID: " + num + "\n" + ex.Message + "\n" + ex.StackTrace;
				networkMgr.exceptionMessage += text;
				networkMgr.IsDisplayErrorBox = true;
				break;
			}
		}
		networkMgr.IsDisconnected = true;
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

	public void Close()
	{
		try
		{
			if (bw != null)
			{
				bw.Close();
				bw = null;
			}
		}
		catch (IOException ex)
		{
			networkMgr.exceptionMessage = ex.Message;
		}
	}
}
