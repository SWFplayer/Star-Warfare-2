using UnityEngine;

public class CreatureTeleportResponse : Response
{
	private int mEnemyID;

	private Vector3 position;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mEnemyID = bytesBuffer.ReadShort();
		position.x = (float)bytesBuffer.ReadShort() / 10f;
		position.y = (float)bytesBuffer.ReadShort() / 10f;
		position.z = (float)bytesBuffer.ReadShort() / 10f;
	}

	public override void ProcessLogic()
	{
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + mEnemyID);
		if (enemyByID != null)
		{
			if (!enemyByID.StateData.ContainsKey(Myth370.TELEPORT_STATE))
			{
				enemyByID.StateData.Add(Myth370.TELEPORT_STATE, new TeleportState.MyData());
			}
			TeleportState.MyData myData = enemyByID.StateData[Myth370.TELEPORT_STATE] as TeleportState.MyData;
			myData.Postition = position;
		}
	}
}
