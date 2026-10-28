using UnityEngine;

public class SwVoidStateResponse : Response
{
	protected int swVoidOwnerPlayerId;

	protected byte state;

	protected short x;

	protected short y;

	protected short z;

	protected int targetID;

	protected short enemyID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		swVoidOwnerPlayerId = bytesBuffer.ReadInt();
		state = bytesBuffer.ReadByte();
		x = bytesBuffer.ReadShort();
		y = bytesBuffer.ReadShort();
		z = bytesBuffer.ReadShort();
		targetID = bytesBuffer.ReadInt();
		enemyID = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		Player playerByUserID = gameWorld.GetPlayerByUserID(swVoidOwnerPlayerId);
		if (playerByUserID != gameWorld.GetLocalPlayer() && playerByUserID != null)
		{
			ESwVoidStateID eSwVoidStateID = (ESwVoidStateID)state;
			if (playerByUserID.mSwVoid != null)
			{
				playerByUserID.mSwVoid.ChangeStateFromResponse(eSwVoidStateID, new Vector3((float)x / 10f, (float)y / 10f, (float)z / 10f), targetID, enemyID);
			}
		}
	}
}
