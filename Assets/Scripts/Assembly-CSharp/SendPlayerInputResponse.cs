using UnityEngine;

internal class SendPlayerInputResponse : Response
{
	public int playerID;

	public bool fire;

	public bool isMoving;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		fire = bytesBuffer.ReadBool();
		isMoving = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID != null)
		{
			InputInfo previousInputInfo = remotePlayerByUserID.inputController.previousInputInfo;
			if (fire != previousInputInfo.fire)
			{
				int changeType = (fire ? 1 : 0);
				remotePlayerByUserID.GetSkills().PlayerInputControllerChange(changeType);
				previousInputInfo.fire = fire;
			}
			remotePlayerByUserID.inputController.inputInfo.fire = fire;
			if (isMoving)
			{
				remotePlayerByUserID.inputController.inputInfo.moveDirection = new Vector3(1f, 0f, 1f);
			}
			else
			{
				remotePlayerByUserID.inputController.inputInfo.moveDirection = Vector3.zero;
			}
		}
	}
}
