using System.Collections.Generic;
using UnityEngine;

public class MutantInfoDetailResponse : Response
{
	protected byte playerCount;

	protected List<int> channelIds;

	protected List<int> totleDamage;

	protected List<int> totleScoe;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerCount = bytesBuffer.ReadByte();
		channelIds = new List<int>();
		totleDamage = new List<int>();
		totleScoe = new List<int>();
		Debug.Log("playerCount is " + playerCount);
		for (int i = 0; i < playerCount; i++)
		{
			channelIds.Add(bytesBuffer.ReadInt());
			totleDamage.Add(bytesBuffer.ReadInt());
			totleScoe.Add(bytesBuffer.ReadInt());
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			Debug.Log("gameworld is null");
			return;
		}
		for (int i = 0; i < playerCount; i++)
		{
			Player remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(channelIds[i]);
			if (remotePlayerByUserID != null)
			{
				remotePlayerByUserID.VSStatistics.SetScore(totleScoe[i]);
				remotePlayerByUserID.VSStatistics.SetTotleDamageToPlayerBoss(totleDamage[i]);
			}
			else
			{
				Debug.Log("remote player is null");
			}
		}
	}
}
