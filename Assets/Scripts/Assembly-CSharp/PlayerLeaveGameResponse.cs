using UnityEngine;

internal class PlayerLeaveGameResponse : Response
{
	protected int playerID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		Debug.Log("player leave game " + playerID);
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			Debug.Log("gameWorld == null");
			return;
		}
		gameWorld.RemoveRemotePlayer(playerID);
		gameWorld.VSTimeStopResume();
	}
}
