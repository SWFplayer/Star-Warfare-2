using UnityEngine;

public class EnemySyncResponse : Response
{
	protected short mSurvivalDifficultLevel;

	protected short mSurvivalEnemyID;

	protected short mSurvivalTotalKilled;

	protected EnemySyncData[] mEnemyDataArray;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mSurvivalEnemyID = bytesBuffer.ReadShort();
		mSurvivalDifficultLevel = bytesBuffer.ReadShort();
		mSurvivalTotalKilled = bytesBuffer.ReadShort();
		byte b = bytesBuffer.ReadByte();
		mEnemyDataArray = new EnemySyncData[b];
		for (int i = 0; i < b; i++)
		{
			mEnemyDataArray[i] = new EnemySyncData();
			mEnemyDataArray[i].m_enemyID = bytesBuffer.ReadShort();
			mEnemyDataArray[i].m_enemyType = bytesBuffer.ReadByte();
			mEnemyDataArray[i].m_x = bytesBuffer.ReadShort();
			mEnemyDataArray[i].m_y = bytesBuffer.ReadShort();
			mEnemyDataArray[i].m_z = bytesBuffer.ReadShort();
			mEnemyDataArray[i].m_difficultyUp = bytesBuffer.ReadShort();
			mEnemyDataArray[i].m_playerCount = bytesBuffer.ReadByte();
			mEnemyDataArray[i].m_targetID = bytesBuffer.ReadInt();
			mEnemyDataArray[i].m_elite = bytesBuffer.ReadBool();
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		GameApp.GetInstance().GetGameWorld().EnemyID = mSurvivalEnemyID - Global.ENEMY_SPAWN_ID_OFFSET[0];
		GameApp.GetInstance().GetGameWorld().DifficultyLevel = mSurvivalDifficultLevel;
		GameApp.GetInstance().GetGameWorld().TotalEnemyKilled = mSurvivalTotalKilled;
		for (int i = 0; i < mEnemyDataArray.Length; i++)
		{
			Vector3 spawnPosition = new Vector3((float)mEnemyDataArray[i].m_x / 10f, (float)mEnemyDataArray[i].m_y / 10f, (float)mEnemyDataArray[i].m_z / 10f);
			Enemy enemy = GameApp.GetInstance().GetGameWorld().SpawnEnemy((EnemyType)mEnemyDataArray[i].m_enemyType, mEnemyDataArray[i].m_enemyID, spawnPosition, mEnemyDataArray[i].m_elite, false, mEnemyDataArray[i].m_playerCount, mEnemyDataArray[i].m_difficultyUp);
			if (enemy != null)
			{
				enemy.ChangeTargetPlayer(mEnemyDataArray[i].m_targetID);
			}
		}
		gameWorld.StartSpawnEnemy = true;
	}
}
