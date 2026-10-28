using UnityEngine;

public class EnemySpawnRequest : Request
{
	protected byte eType;

	protected byte spType;

	protected short x;

	protected short y;

	protected short z;

	protected short difficultyUp;

	protected short index;

	protected bool elite;

	protected bool fromGrave;

	public EnemySpawnRequest(EnemyType enemyType, EnemySpawnType spawnType, Vector3 pos, int difficultyUp, short index, bool elite, bool fromGrave)
	{
		requestID = 104;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		eType = (byte)enemyType;
		spType = (byte)spawnType;
		this.difficultyUp = (short)difficultyUp;
		this.index = index;
		this.elite = elite;
		this.fromGrave = fromGrave;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(14);
		bytesBuffer.AddByte(eType);
		bytesBuffer.AddByte(spType);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		bytesBuffer.AddShort(difficultyUp);
		bytesBuffer.AddShort(index);
		bytesBuffer.AddBool(elite);
		bytesBuffer.AddBool(fromGrave);
		return bytesBuffer;
	}
}
