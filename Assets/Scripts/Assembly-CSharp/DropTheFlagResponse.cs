using UnityEngine;

internal class DropTheFlagResponse : Response
{
	protected int playerID;

	protected Vector3 pos;

	protected bool bInit;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		short num = bytesBuffer.ReadShort();
		short num2 = bytesBuffer.ReadShort();
		short num3 = bytesBuffer.ReadShort();
		bInit = bytesBuffer.ReadBool();
		pos = new Vector3((float)num * 1f / 10f, (float)num2 * 1f / 10f, (float)num3 * 1f / 10f);
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.FlagClock == null)
		{
			return;
		}
		gameWorld.DropTheFlag(pos + Vector3.up * 0.8f);
		gameWorld.FlagClock.StopTime();
		gameWorld.FlagInPlayerID = -1;
		if (bInit)
		{
			gameWorld.LastFlagInPlayerID = -1;
		}
		else
		{
			gameWorld.LastFlagInPlayerID = playerID;
		}
		if (gameWorld.LastFlagInPlayerID == -1)
		{
			gameWorld.FlagClock.Reset();
		}
		Player playerByUserID = gameWorld.GetPlayerByUserID(playerID);
		if (playerByUserID == null)
		{
			return;
		}
		playerByUserID.isCapturingFlag = false;
		AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/drop_flag");
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_CTF_FFA && gameWorld.FlagDirObj != null)
		{
			Color white = Color.white;
			gameWorld.FlagDirObj.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", white);
		}
		playerByUserID.CreatePlayerFlag();
		GameObject gameObject = GameObject.Find("GameUI");
		if (gameObject != null)
		{
			InGameUIScript component = gameObject.GetComponent<InGameUIScript>();
			if (!(component != null))
			{
			}
		}
	}
}
