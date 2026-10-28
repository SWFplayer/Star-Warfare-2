using System.Collections.Generic;
using UnityEngine;

public class ChangeToBossSceneResponse : Response
{
	public int bossGold;

	public int bossMithril;

	public int playerCountForBoss;

	public bool firstChallenge;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		bossGold = bytesBuffer.ReadInt();
		bossMithril = bytesBuffer.ReadInt();
		playerCountForBoss = bytesBuffer.ReadByte();
		firstChallenge = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)Global.BOSS_FINAL_SUB_STAGE_ID);
		if (gameWorld == null)
		{
			return;
		}
		gameWorld.GetLocalPlayer().SetSeatID(0);
		gameWorld.GetLocalPlayer().DropAtSpawnPosition();
		gameWorld.OnlineBossGold = bossGold;
		gameWorld.OnlineBossMithril = bossMithril;
		gameWorld.PlayerCountForBoss = playerCountForBoss;
		gameWorld.FirstChallenge = firstChallenge;
		Debug.Log("gameWorld.OnlineBossGold:" + gameWorld.OnlineBossGold);
		Debug.Log("gameWorld.OnlineBossMithril: " + gameWorld.OnlineBossMithril);
		Debug.Log("gameWorld.PlayerCountForBoss: " + gameWorld.PlayerCountForBoss);
		Debug.Log("gameWorld.FirstChallenge: " + gameWorld.FirstChallenge);
		List<RemotePlayer> remotePlayerList = GlobalData.GetInstance().RemotePlayerList;
		if (remotePlayerList == null)
		{
			return;
		}
		gameWorld.SetRemotePlayers(remotePlayerList);
		foreach (RemotePlayer item in remotePlayerList)
		{
			if (!item.HasLeavedGame())
			{
				item.InitInBossStage();
				item.DropAtSpawnPosition();
			}
		}
	}
}
