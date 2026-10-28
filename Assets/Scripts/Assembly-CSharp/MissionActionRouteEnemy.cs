using System;
using UnityEngine;

public class MissionActionRouteEnemy : MissionAction
{
	public static short RouteEnemyIndex = 10000;

	private EnemyType type = EnemyType.NA;

	private int startPoint;

	private int HP;

	private MissionEnemyRouteScript script;

	private bool isElite;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				string text = dataTable.GetString(i, 1);
				if (Enum.IsDefined(typeof(EnemyType), text))
				{
					type = (EnemyType)(int)Enum.Parse(typeof(EnemyType), text, true);
				}
				else
				{
					Debug.LogError("[INVALID ENEMY TYPE]: ID = " + base.ID + ", row = " + i + ", col = 1, value = " + text);
				}
				startPoint = dataTable.GetInt(i, 2);
				HP = dataTable.GetInt(i, 3);
				isElite = dataTable.GetBoolean(i, 4);
			}
		}
	}

	public override void LevelStart()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.ACTION_ENEMY_ROUTE);
		string text = MissionNamePrefix.ACTION_ROUTEENEMY + startPoint;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name == text)
			{
				script = gameObject.GetComponent<MissionEnemyRouteScript>();
				script.currentIndex = 0;
			}
		}
		RouteEnemyIndex = 10000;
	}

	public override void StartAction()
	{
		if (GameApp.GetInstance().GetGameWorld() == null)
		{
			return;
		}
		Enemy enemy = GameApp.GetInstance().GetGameWorld().SpawnEnemy(type, RouteEnemyIndex, script.transform.position, isElite, false, 1, GameApp.GetInstance().GetGameWorld().DifficultyLevel);
		RouteEnemyIndex++;
		enemy.MaxHp = HP;
		enemy.Hp = HP;
		enemy.currentRouteScript = script;
		enemy.SetState(Enemy.ROUTE_STATE);
		GameObject destination = script.PathPoints[script.PathPoints.Count - 1];
		MissionKillRouteEnemy missionKillRouteEnemy = null;
		for (int i = 0; i < 5; i++)
		{
			Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(i);
			if (currentStageMission.Type == MissionType.KILL_ROUTE_ENEMY)
			{
				missionKillRouteEnemy = currentStageMission as MissionKillRouteEnemy;
				break;
			}
		}
		if (missionKillRouteEnemy != null)
		{
			missionKillRouteEnemy.SetFlag(enemy);
			missionKillRouteEnemy.SetDestination(destination);
		}
	}
}
