using UnityEngine;

public class FrostStoneRemoveIceArmorResponse : Response
{
	private int mEnemyID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mEnemyID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + mEnemyID);
		if (enemyByID != null)
		{
			FrostStone frostStone = enemyByID as FrostStone;
			if (frostStone != null)
			{
				frostStone.HasIceArmor = false;
				frostStone.mRemoveArmorTime = Time.time;
				frostStone.SetArmor(false);
				Debug.Log("移除remote冰甲");
			}
		}
	}
}
