using UnityEngine;

public class RouteState : EnemyState
{
	private float waitStartTime;

	private bool isGoingToNextPoint;

	private MissionKillRouteEnemy currentMission;

	private ProtectedItemHpScript hpScript;

	public override void Start(Enemy enemy)
	{
		if (enemy.currentRouteScript != null)
		{
			waitStartTime = Time.time;
			isGoingToNextPoint = false;
			enemy.PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		}
		else
		{
			Debug.LogError("找不到当前路径点！");
		}
		for (int i = 0; i < 5; i++)
		{
			Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(i);
			if (currentStageMission.Type == MissionType.KILL_ROUTE_ENEMY)
			{
				currentMission = currentStageMission as MissionKillRouteEnemy;
				break;
			}
		}
		if (hpScript == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load(EffectName.ENEMY_HP_BAR), enemy.GetPosition(), Quaternion.identity) as GameObject;
			gameObject.transform.parent = enemy.GetTransform();
			gameObject.transform.localPosition = new Vector3(0f, 3.5f, 0f);
			hpScript = gameObject.GetComponent<ProtectedItemHpScript>();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		hpScript.SetHpFillAmount((float)enemy.Hp / (float)enemy.MaxHp);
		if (!(Time.time - waitStartTime >= enemy.currentRouteScript.GetWaitTime()))
		{
			return;
		}
		if (!enemy.currentRouteScript.isTheLastPoint())
		{
			enemy.MoveToward(enemy.currentRouteScript.GetNextPointPosition(), enemy.currentRouteScript.GetSpeed());
			enemy.PlayRunAnimation();
			Vector3 nextPointPosition = enemy.currentRouteScript.GetNextPointPosition();
			nextPointPosition.y = enemy.GetPosition().y;
			if (Vector3.Distance(nextPointPosition, enemy.GetPosition()) < 3f)
			{
				enemy.currentRouteScript.currentIndex++;
				isGoingToNextPoint = true;
				enemy.SetState(Enemy.ROUTE_STATE);
			}
		}
		else if (currentMission != null)
		{
			currentMission.FailTheMission();
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		if (!isGoingToNextPoint)
		{
			GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(enemy.GetTransform());
			Object.Destroy(hpScript.gameObject);
			hpScript = null;
			if (currentMission != null)
			{
				currentMission.AddKillNum();
			}
		}
	}
}
