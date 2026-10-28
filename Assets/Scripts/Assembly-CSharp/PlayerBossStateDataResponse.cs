using UnityEngine;

public class PlayerBossStateDataResponse : Response
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
			int playerID = Data.ReadInt();
			byte key = Data.ReadByte();
			Player playerByUserID = GameApp.GetInstance().GetGameWorld().GetPlayerByUserID(playerID);
			if (playerByUserID != null)
			{
				PlayerBossState playerBossState = PlayerBossState.States[key] as PlayerBossState;
				playerBossState.DoResponse(playerByUserID, Data);
			}
		}
		else
		{
			Debug.LogError("不应该在BOSS VS PLAYER模式之外使用。");
		}
	}
}
