public class PlayerChargeWeaponInfoResponse : Response
{
	protected int playerId;

	protected byte charge;

	protected bool isPower;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		charge = bytesBuffer.ReadByte();
		isPower = bytesBuffer.ReadBool();
		playerId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerId);
		if (remotePlayerByUserID != null)
		{
			if (remotePlayerByUserID.GetWeapon() != null && remotePlayerByUserID.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
			{
				ChargeDoublePistol chargeDoublePistol = remotePlayerByUserID.GetWeapon() as ChargeDoublePistol;
				chargeDoublePistol.setInfo(charge, isPower);
			}
			return;
		}
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && localPlayer.GetChannelID() == playerId)
		{
			ChargeDoublePistol chargeDoublePistol2 = localPlayer.GetWeapon() as ChargeDoublePistol;
			chargeDoublePistol2.setInfo(charge, isPower);
		}
	}
}
