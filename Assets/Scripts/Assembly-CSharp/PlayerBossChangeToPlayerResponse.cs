using UnityEngine;

public class PlayerBossChangeToPlayerResponse : Response
{
	protected int playerID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		int channelID = Lobby.GetInstance().GetChannelID();
		if (playerID == channelID)
		{
			localPlayer.GetUserState().LoadCurrentCustomize(true, 0);
			localPlayer.InitSkills();
			localPlayer.IsBoss = false;
			if (localPlayer.inputController != null)
			{
				localPlayer.inputController.ResetSensitivity(localPlayer.GetUserState().TouchInputSensitivity, false);
			}
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			if (null != component)
			{
				component.ResetAngleV();
			}
			localPlayer.ReSpawnAtPointVS(localPlayer.GetSeatID());
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				localPlayer.CreatePlayerSign();
			}
			return;
		}
		RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID != null)
		{
			remotePlayerByUserID.IsBoss = false;
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				remotePlayerByUserID.ReSpawnAtPointVS(remotePlayerByUserID.GetSeatID());
			}
			else
			{
				remotePlayerByUserID.ReSpawnPVE();
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				remotePlayerByUserID.CreatePlayerSign();
			}
		}
	}
}
