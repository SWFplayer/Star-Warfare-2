using UnityEngine;

public class PlayerBossStateResponse : Response
{
	protected int playerID;

	protected byte stateID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		stateID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID != null)
		{
			if (remotePlayerByUserID.IsBoss)
			{
				GameUnitState state = PlayerBossState.States[stateID];
				remotePlayerByUserID.SetState(state);
			}
		}
		else
		{
			Debug.Log("player is null");
		}
	}
}
