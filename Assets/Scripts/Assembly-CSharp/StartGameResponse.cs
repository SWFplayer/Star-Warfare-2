using System.Collections;
using UnityEngine;

internal class StartGameResponse : Response
{
	public short roomID;

	public byte gameMode;

	public byte mapID;

	public byte hasBossCoop;

	public byte boss1;

	public byte boss2;

	public byte difficultMode;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		roomID = bytesBuffer.ReadShort();
		gameMode = bytesBuffer.ReadByte();
		mapID = bytesBuffer.ReadByte();
		hasBossCoop = bytesBuffer.ReadByte();
		boss1 = bytesBuffer.ReadByte();
		boss2 = bytesBuffer.ReadByte();
		difficultMode = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameApp.GetInstance().GetGameMode().ModePlay = (Mode)gameMode;
		GameApp.GetInstance().GetUserState().SetSelectedNetIndex(mapID);
		GameApp.GetInstance().GetUserState().SetNetDifficultMode(difficultMode);
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
		{
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)Global.SURVIVAL_SUB_STAGE_ID);
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
		{
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)Global.BOSS_SUB_STAGE_ID);
		}
		if (hasBossCoop == 0)
		{
			GameApp.GetInstance().GetUserState().Boss1Type = EnemyType.NA;
			GameApp.GetInstance().GetUserState().Boss2Type = EnemyType.NA;
			GameApp.GetInstance().GetUserState().CoopBossType = EnemyType.NA;
		}
		else
		{
			GameApp.GetInstance().GetUserState().Boss1Type = (EnemyType)(9 + boss1);
			GameApp.GetInstance().GetUserState().Boss2Type = (EnemyType)(9 + boss2);
			GameApp.GetInstance().GetUserState().CoopBossType = EnemyType.NA;
		}
		GameObject gameObject = GameObject.Find("MultiMenu");
		if (gameObject != null)
		{
			MultiMenuScript component = gameObject.GetComponent<MultiMenuScript>();
			if (component != null && Lobby.GetInstance().GetCurrentRoomID() != -1)
			{
				component.StartGame(mapID);
			}
		}
		else
		{
			Debug.Log("No MultiMenu");
			MultiMenuScript.StartGameInGame(mapID);
		}
		GameWorld.sCanSpawnEnemy = false;
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		robot.SetState(robot.PlayingState);
		robot.PlayingDuration = Random.Range(RobotUIScript.PlayingDurationMin * 60f, RobotUIScript.PlayingDurationMax * 60f);
	}

	private IEnumerator StartGameLater()
	{
		yield return new WaitForSeconds(10f);
		GameObject obj = GameObject.Find("MultiMenu");
		MultiMenuScript rus = obj.GetComponent<MultiMenuScript>();
		if (rus != null && Lobby.GetInstance().GetCurrentRoomID() != -1)
		{
			rus.StartGame(mapID);
		}
	}
}
