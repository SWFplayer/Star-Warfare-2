using UnityEngine;

public class EnemyStateDataResponse : Response
{
	public BytesBuffer Data;

	public override void ReadData(byte[] data)
	{
		Data = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			Debug.LogError("not finish.");
			return;
		}
		int num = Data.ReadInt();
		byte b = Data.ReadByte();
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + num);
		if (enemyByID != null)
		{
			GameUnitState mState = enemyByID.mAllConditionState[b - 1].mState;
			mState.DoResponse(enemyByID, Data);
		}
	}
}
