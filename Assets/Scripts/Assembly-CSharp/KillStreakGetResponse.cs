using UnityEngine;

public class KillStreakGetResponse : Response
{
	private int[] mIdArray;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		byte b = bytesBuffer.ReadByte();
		mIdArray = new int[b];
		for (int i = 0; i < b; i++)
		{
			mIdArray[i] = bytesBuffer.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing || gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null)
		{
			int[] array = mIdArray;
			foreach (int num in array)
			{
				Debug.Log("player.EnableKillStreak: " + num);
				localPlayer.EnableKillStreak(num);
			}
		}
	}
}
