using System.Collections.Generic;

internal class VSGameEndResponse : Response
{
	protected byte id;

	protected int winExp;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		id = bytesBuffer.ReadByte();
		winExp = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		int channelID = Lobby.GetInstance().GetChannelID();
		if (localPlayer == null)
		{
			return;
		}
		Lobby.GetInstance().GetVSClock().StopAtEnd();
		List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
		UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			localPlayer.OnWin();
			{
				foreach (RemotePlayer item in remotePlayers)
				{
					if (item != null)
					{
						item.OnWin();
					}
				}
				return;
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsTeamMode() && GameApp.GetInstance().GetGameMode().ModePlay != Mode.GIANT_BETRAYAL)
		{
			if (localPlayer.Team == (TeamName)id)
			{
				localPlayer.VSStatistics.MakeWin(winExp);
				localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.WIN, winExp);
				localPlayer.OnWin();
			}
			else
			{
				localPlayer.VSStatistics.MakeLose();
				localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.LOSE);
				localPlayer.OnLose();
			}
			{
				foreach (RemotePlayer item2 in remotePlayers)
				{
					if (item2 != null)
					{
						if (item2.Team == (TeamName)id)
						{
							item2.OnWin();
						}
						else
						{
							item2.OnLose();
						}
					}
				}
				return;
			}
		}
		if (localPlayer.GetSeatID() == id)
		{
			localPlayer.VSStatistics.MakeWin(winExp);
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.WIN, winExp);
			localPlayer.OnWin();
		}
		else
		{
			localPlayer.VSStatistics.MakeLose();
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.LOSE);
			localPlayer.OnLose();
		}
		foreach (RemotePlayer item3 in remotePlayers)
		{
			if (item3 != null)
			{
				if (item3.GetSeatID() == id)
				{
					item3.OnWin();
				}
				else
				{
					item3.OnLose();
				}
			}
		}
	}
}
