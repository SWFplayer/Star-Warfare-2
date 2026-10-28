using UnityEngine;

public class BowSwordChangeResponse : Response
{
	public int mPlayerID;

	public bool mIsBow;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mPlayerID = bytesBuffer.ReadInt();
		mIsBow = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(mPlayerID);
		if (remotePlayerByUserID == null)
		{
			return;
		}
		BowSword bowSword = remotePlayerByUserID.GetWeapon() as BowSword;
		if (bowSword != null)
		{
			if (mIsBow)
			{
				Debug.Log("1");
				bowSword.ChangeToBow();
			}
			else
			{
				Debug.Log("2");
				bowSword.ChangeToSword();
			}
		}
		else
		{
			Debug.LogError("居然不是弓剑。");
		}
	}
}
