using UnityEngine;

internal class EnemySpawnResponse : Response
{
	protected byte m_enemyType;

	protected byte m_SpawnType;

	protected short m_enemyID;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	protected short m_difficultyUp;

	protected bool m_elite;

	protected bool m_fromGrave;

	protected byte m_playerCount;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_enemyType = bytesBuffer.ReadByte();
		m_SpawnType = bytesBuffer.ReadByte();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
		m_difficultyUp = bytesBuffer.ReadShort();
		m_enemyID = bytesBuffer.ReadShort();
		m_elite = bytesBuffer.ReadBool();
		m_fromGrave = bytesBuffer.ReadBool();
		m_playerCount = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		if (!gameWorld.StartSpawnEnemy)
		{
			if (m_enemyID != 0)
			{
				return;
			}
			gameWorld.StartSpawnEnemy = true;
		}
		Vector3 spawnPosition = new Vector3((float)m_x / 10f, (float)m_y / 10f, (float)m_z / 10f);
		GameApp.GetInstance().GetGameWorld().SpawnEnemy((EnemyType)m_enemyType, m_enemyID, spawnPosition, m_elite, m_fromGrave, m_playerCount, m_difficultyUp);
		if (!Lobby.GetInstance().IsMasterPlayer)
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				GameApp.GetInstance().GetGameWorld().DifficultyLevel = m_difficultyUp;
			}
			switch ((EnemySpawnType)m_SpawnType)
			{
			case EnemySpawnType.MERGE:
				GameApp.GetInstance().GetGameWorld().MergeEnemyID = m_enemyID - Global.ENEMY_SPAWN_ID_OFFSET[m_SpawnType];
				break;
			case EnemySpawnType.EVENT_NORMAL:
				GameApp.GetInstance().GetGameWorld().EventEnemyID = m_enemyID - Global.ENEMY_SPAWN_ID_OFFSET[m_SpawnType];
				break;
			case EnemySpawnType.EVENT_MERGE:
				GameApp.GetInstance().GetGameWorld().EventMergeEnemyID = m_enemyID - Global.ENEMY_SPAWN_ID_OFFSET[m_SpawnType];
				break;
			default:
				GameApp.GetInstance().GetGameWorld().EnemyID = m_enemyID - Global.ENEMY_SPAWN_ID_OFFSET[m_SpawnType];
				break;
			}
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (robot.lobby.IsMasterPlayer)
		{
			RobotEnemy robotEnemy = new RobotEnemy();
			robotEnemy.o = new GameObject();
			robotEnemy.o.transform.position = robotEnemy.targetPoses[0];
			robotEnemy.o.name = robot.userName + "_E" + m_enemyID;
			robotEnemy.EnemyID = m_enemyID;
			robot.enemyList.Add(m_enemyID, robotEnemy);
		}
	}
}
