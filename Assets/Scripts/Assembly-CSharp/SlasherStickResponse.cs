internal class SlasherStickResponse : Response
{
	private short mEnemyID;

	private byte mStickID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mEnemyID = bytesBuffer.ReadShort();
		mStickID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + mEnemyID);
		if (enemyByID != null)
		{
			Slasher slasher = enemyByID as Slasher;
			if (slasher != null)
			{
				slasher.FindWeaponByID(mStickID);
			}
		}
	}
}
