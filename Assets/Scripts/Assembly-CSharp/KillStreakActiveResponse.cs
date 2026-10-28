using UnityEngine;

public class KillStreakActiveResponse : Response
{
	protected int mKillStreakId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mKillStreakId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && gameWorld.State == GameState.Playing && gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				Debug.Log("player.EnableKillStreak: " + mKillStreakId);
				localPlayer.EnableKillStreak(mKillStreakId);
				localPlayer.GetUserState().Achievement.CheckAchievement_NoKillStreak();
			}
		}
	}
}
