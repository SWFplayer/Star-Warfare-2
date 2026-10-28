public class PlayerNightmarePowerAttackResponse : Response
{
	protected int playerID;

	protected byte type;

	protected bool isLeft;

	protected int beginID;

	protected int targetID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		type = bytesBuffer.ReadByte();
		isLeft = bytesBuffer.ReadBool();
		beginID = bytesBuffer.ReadInt();
		targetID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
			if (remotePlayerByUserID != null && remotePlayerByUserID.GetWeapon() != null && remotePlayerByUserID.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
			{
				ChargeDoublePistol chargeDoublePistol = remotePlayerByUserID.GetWeapon() as ChargeDoublePistol;
				chargeDoublePistol.PlayVfxFromServer(type, isLeft, beginID, targetID);
			}
		}
	}
}
