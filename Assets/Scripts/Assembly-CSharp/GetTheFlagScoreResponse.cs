using UnityEngine;

internal class GetTheFlagScoreResponse : Response
{
	protected int playerID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.FlagClock == null)
		{
			return;
		}
		gameWorld.HideTheFlag();
		gameWorld.LastFlagInPlayerID = -1;
		gameWorld.FlagInPlayerID = -1;
		gameWorld.FlagClock.Restart();
		gameWorld.FlagClock.StopTime();
		Player playerByUserID = gameWorld.GetPlayerByUserID(playerID);
		if (playerByUserID == null)
		{
			return;
		}
		playerByUserID.isCapturingFlag = false;
		playerByUserID.CreatePlayerFlag();
		if (gameWorld.FlagDirObj != null)
		{
			Color white = Color.white;
			gameWorld.FlagDirObj.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", white);
		}
		if (playerByUserID.IsLocal())
		{
			playerByUserID.VSStatistics.SecureFlags++;
			playerByUserID.VSStatistics.AddSecureFlagScore();
			Debug.Log("Get the flag score:" + playerByUserID.VSStatistics.SecureFlags);
		}
		AudioManager.GetInstance().PlaySound("_Old/_Audio/pickup/win_flag");
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
