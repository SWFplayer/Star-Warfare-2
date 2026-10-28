public class EnemyOnHitRequest : Request
{
	protected short enemyID;

	protected int damage;

	protected byte weaponType;

	protected bool criticalAttack;

	protected byte weaponId;

	public EnemyOnHitRequest(short enemyID, int damage, byte weaponType, bool criticalAttack, int weaponid)
	{
		requestID = 108;
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		this.enemyID = enemyID;
		this.damage = damage;
		this.weaponType = weaponType;
		this.criticalAttack = criticalAttack;
		weaponId = (byte)weaponid;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(9);
		bytesBuffer.AddShort(enemyID);
		bytesBuffer.AddInt(damage);
		bytesBuffer.AddByte(weaponType);
		bytesBuffer.AddBool(criticalAttack);
		bytesBuffer.AddByte(weaponId);
		return bytesBuffer;
	}
}
