public class MissionActionSpawnParam : MissionAction
{
	private SpawnParam mSpawnParam;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		mSpawnParam = new SpawnParam();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				mSpawnParam.MaxEnemyCount = dataTable.GetInt(i, 1);
				mSpawnParam.MaxMergeEnemyCount = dataTable.GetInt(i, 2);
				mSpawnParam.SpawnInterval = dataTable.GetFloat(i, 3);
				mSpawnParam.MoveSpawnDistance = dataTable.GetFloat(i, 4);
				mSpawnParam.StopSpawnDistance = dataTable.GetFloat(i, 5);
				mSpawnParam.ActionSpawnRadiusNear1 = dataTable.GetFloat(i, 6);
				mSpawnParam.ActionSpawnRadiusNear2 = dataTable.GetFloat(i, 7);
				mSpawnParam.EnemyAcceleration = dataTable.GetFloat(i, 8);
				mSpawnParam.MaxEnemySpeed = dataTable.GetFloat(i, 9);
				break;
			}
		}
	}

	public override void StartAction()
	{
		EnemySpawnScript.Instance.SpawnParameters = mSpawnParam;
	}
}
