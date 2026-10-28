using UnityEngine;

public class PlayerBossStateInnerStateResponse : Response
{
	protected int playerID;

	protected byte stateID;

	protected byte InnerState;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		stateID = bytesBuffer.ReadByte();
		InnerState = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID != null)
		{
			PlayerBossState playerBossState = PlayerBossState.States[stateID] as PlayerBossState;
			playerBossState.ChangeInnerState(remotePlayerByUserID, InnerState);
		}
		else
		{
			Debug.Log("player is null");
		}
	}
}
