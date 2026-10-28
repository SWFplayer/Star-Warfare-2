using UnityEngine;

public class PlayerChangeToBossResponse : Response
{
	protected int playerID;

	protected byte type;

	protected bool newSpawn;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		type = bytesBuffer.ReadByte();
		newSpawn = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		int channelID = Lobby.GetInstance().GetChannelID();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		Player playerBoss = null;
		if (gameWorld != null)
		{
			if (playerID != channelID)
			{
				RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
				if (remotePlayerByUserID != null)
				{
					remotePlayerByUserID.RefreshAvatar((EnemyType)type);
					playerBoss = remotePlayerByUserID;
					remotePlayerByUserID.DropAtSpawnPositionVS();
				}
				else
				{
					Debug.Log("player is null");
				}
			}
			else
			{
				LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
				if (localPlayer != null)
				{
					localPlayer.RefreshAvatar((EnemyType)type);
					playerBoss = localPlayer;
					localPlayer.DropAtSpawnPositionVS();
					localPlayer.SetState(Player.BOSS_IDLE_STATE);
				}
			}
		}
		GiantBetrayalModeLogic giantBetrayalModeLogic = gameWorld.VsBattleLogic as GiantBetrayalModeLogic;
		if (giantBetrayalModeLogic != null)
		{
			if (newSpawn)
			{
				giantBetrayalModeLogic.SetPlayerBossWinTime(120f);
			}
			giantBetrayalModeLogic.SetPlayerBoss(playerBoss);
			giantBetrayalModeLogic.State = GiantBetrayalModeLogic.EGiantBetrayalState.FIGHTING_BOSS;
			if (MutantHUD.mInstance != null)
			{
				MutantHUD.mInstance.Reset();
			}
			if (HUDBase.instance != null)
			{
				((HUDOnline)HUDBase.instance).OpenScorePage(false);
			}
		}
	}
}
