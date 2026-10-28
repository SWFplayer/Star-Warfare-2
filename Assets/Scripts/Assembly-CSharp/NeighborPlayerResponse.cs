using UnityEngine;

public class NeighborPlayerResponse : Response
{
	private int OwnerID;

	private int ID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		OwnerID = bytesBuffer.ReadInt();
		ID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		Debug.Log("Heal Player: " + ID);
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && gameWorld.State == GameState.Playing)
		{
			Player playerByUserID = gameWorld.GetPlayerByUserID(ID);
			Player playerByUserID2 = gameWorld.GetPlayerByUserID(OwnerID);
			if (playerByUserID != null && playerByUserID2 != null && playerByUserID2.mSwVoid != null)
			{
				((SwVoidMedic)playerByUserID2.mSwVoid).HealOthers(playerByUserID);
			}
		}
	}
}
