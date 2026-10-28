public class PlayerOnHitRequest : Request
{
	protected int damage;

	protected bool hasPlayerID;

	protected int playerID;

	protected bool hasDamageReturnSkill;

	protected short enemyID;

	public PlayerOnHitRequest(int damage, bool hasPlayerID, int playerID)
	{
		requestID = 110;
		this.damage = damage;
		this.hasPlayerID = hasPlayerID;
		this.playerID = playerID;
	}

	public void SetEnemyId(Enemy enemy)
	{
		if (enemy != null && enemy.InPlayingState())
		{
			hasDamageReturnSkill = true;
			enemyID = enemy.EnemyID;
		}
	}

	public override BytesBuffer GetBody()
	{
		byte b = 6;
		if (hasPlayerID)
		{
			b += 4;
		}
		if (hasDamageReturnSkill)
		{
			b += 2;
		}
		BytesBuffer bytesBuffer = new BytesBuffer(b);
		bytesBuffer.AddBool(hasPlayerID);
		bytesBuffer.AddInt(damage);
		if (hasPlayerID)
		{
			bytesBuffer.AddInt(playerID);
		}
		bytesBuffer.AddBool(hasDamageReturnSkill);
		if (hasDamageReturnSkill)
		{
			bytesBuffer.AddShort(enemyID);
		}
		return bytesBuffer;
	}
}
