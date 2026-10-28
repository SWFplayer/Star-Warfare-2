using UnityEngine;

internal class SlasherBladeWindResponse : Response
{
	private int mEnemyID;

	private byte mBladeWindNumber;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mEnemyID = bytesBuffer.ReadInt();
		mBladeWindNumber = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(mEnemyID);
			if (remotePlayerByUserID != null)
			{
				if (!remotePlayerByUserID.StateData.ContainsKey(Player.SLASHER_WEAPON_BLADE_STATE))
				{
					remotePlayerByUserID.StateData.Add(Player.SLASHER_WEAPON_BLADE_STATE, new PlayerSlasherWeaponBladeState.MyData());
				}
				PlayerSlasherWeaponBladeState.MyData myData = remotePlayerByUserID.StateData[Player.SLASHER_WEAPON_BLADE_STATE] as PlayerSlasherWeaponBladeState.MyData;
				myData.BladeWindNumber = mBladeWindNumber;
			}
			else
			{
				Debug.Log("error " + mEnemyID);
			}
			return;
		}
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + mEnemyID);
		if (enemyByID != null)
		{
			Slasher slasher = enemyByID as Slasher;
			if (slasher != null)
			{
				slasher.BladeWindNumber = mBladeWindNumber;
			}
		}
	}
}
