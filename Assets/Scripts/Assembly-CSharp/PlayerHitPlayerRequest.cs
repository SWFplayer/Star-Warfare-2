public class PlayerHitPlayerRequest : Request
{
	protected int onHitPlayerID;

	protected byte attackerType;

	protected byte weaponId;

	protected byte damageFactorId;

	protected byte playerHitPlayerDistanceId;

	private bool mSendAttacker;

	private int AttackerId;

	public PlayerHitPlayerRequest(DamageProperty dp, int playerID, byte playerHitPlayerDistanceId = 0)
	{
		requestID = 115;
		onHitPlayerID = playerID;
		attackerType = (byte)dp.mAttackerType;
		weaponId = (byte)dp.weaponid;
		damageFactorId = dp.damageFactor;
		this.playerHitPlayerDistanceId = playerHitPlayerDistanceId;
		LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (playerID == localPlayer.GetChannelID())
		{
			mSendAttacker = true;
			Player player = dp.attackerGameUnit as Player;
			AttackerId = player.GetChannelID();
		}
	}

	public PlayerHitPlayerRequest(int playerID, AttackerType attackerType, byte weaponId, byte damageFact, byte playerHitPlayerDistanceId)
	{
		requestID = 115;
		onHitPlayerID = playerID;
		this.attackerType = (byte)attackerType;
		this.weaponId = weaponId;
		damageFactorId = damageFact;
		this.playerHitPlayerDistanceId = playerHitPlayerDistanceId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = ((!mSendAttacker) ? new BytesBuffer(9) : new BytesBuffer(13));
		bytesBuffer.AddInt(onHitPlayerID);
		bytesBuffer.AddByte(attackerType);
		bytesBuffer.AddByte(weaponId);
		bytesBuffer.AddByte(damageFactorId);
		bytesBuffer.AddByte(playerHitPlayerDistanceId);
		bytesBuffer.AddBool(mSendAttacker);
		if (mSendAttacker)
		{
			bytesBuffer.AddInt(AttackerId);
		}
		return bytesBuffer;
	}
}
