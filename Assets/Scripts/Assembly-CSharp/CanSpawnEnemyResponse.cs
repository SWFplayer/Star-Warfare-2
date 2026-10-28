using UnityEngine;

public class CanSpawnEnemyResponse : Response
{
	public override void ReadData(byte[] data)
	{
	}

	public override void ProcessLogic()
	{
		GameWorld.sCanSpawnEnemy = true;
		Debug.Log(",......CanSpawnEnemyResponse");
	}
}
