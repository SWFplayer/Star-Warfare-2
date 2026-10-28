public class SwVoidShieldStateRequest : Request
{
	private bool isPlayerAttack;

	private int damage;

	private int channelID;

	private byte attackerType;

	private byte damageFactor;

	private byte weaponId;

	private bool mSendAttacker;

	private int AttackerId;

	public SwVoidShieldStateRequest(int damage, int channelID)
	{
		requestID = 172;
		isPlayerAttack = false;
		this.damage = damage;
		this.channelID = channelID;
		attackerType = 0;
		damageFactor = 0;
		weaponId = 0;
	}

	public SwVoidShieldStateRequest(DamageProperty dp, int channelID)
	{
		requestID = 172;
		isPlayerAttack = true;
		damage = dp.damage;
		this.channelID = channelID;
		attackerType = (byte)dp.mAttackerType;
		damageFactor = dp.damageFactor;
		weaponId = (byte)dp.weaponid;
		LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (channelID == localPlayer.GetChannelID())
		{
			mSendAttacker = true;
			Player player = dp.attackerGameUnit as Player;
			AttackerId = player.GetChannelID();
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = ((!isPlayerAttack) ? new BytesBuffer(12) : (mSendAttacker ? new BytesBuffer(17) : new BytesBuffer(13)));
		bytesBuffer.AddByte((byte)(isPlayerAttack ? 1u : 0u));
		bytesBuffer.AddInt(damage);
		bytesBuffer.AddInt(channelID);
		bytesBuffer.AddByte(attackerType);
		bytesBuffer.AddByte(damageFactor);
		bytesBuffer.AddByte(weaponId);
		if (isPlayerAttack)
		{
			bytesBuffer.AddByte((byte)(mSendAttacker ? 1u : 0u));
			if (mSendAttacker)
			{
				bytesBuffer.AddInt(AttackerId);
			}
		}
		return bytesBuffer;
	}
}
