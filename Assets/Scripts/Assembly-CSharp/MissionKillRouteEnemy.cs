using UnityEngine;

public class MissionKillRouteEnemy : Mission
{
	private int mTotalNumber;

	private int mCurrentNumber;

	private GameObject Flag;

	private GameObject Portal;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				mTotalNumber = dataTable.GetInt(j, 1);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mCurrentNumber = 0;
		mProcess = EMissionProcess.NORMAL;
		Flag = Object.Instantiate(Resources.Load(EffectName.FLAG_ON_ENEMY)) as GameObject;
		Flag.SetActive(false);
		Portal = Object.Instantiate(Resources.Load(EffectName.PORTAL_OBJECT)) as GameObject;
		Portal.SetActive(false);
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && mCurrentNumber >= mTotalNumber)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)mCurrentNumber / (float)mTotalNumber;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + mCurrentNumber + " / " + mTotalNumber + ")[-]";
	}

	public void AddKillNum()
	{
		mCurrentNumber++;
		Flag.SetActive(false);
		TriggerAction();
	}

	public void FailTheMission()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			mProcess = EMissionProcess.FAIL;
		}
	}

	public void SetFlag(Enemy enemy)
	{
		Flag.transform.parent = enemy.GetTransform();
		Flag.transform.localPosition = Vector3.up * 4f;
		Flag.SetActive(true);
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_ACTION_BOSS, "routeEnemy", enemy.GetTransform());
	}

	public void SetDestination(GameObject point)
	{
		Portal.transform.position = point.transform.position;
		Portal.transform.rotation = point.transform.rotation;
		Portal.SetActive(true);
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_DESTINATION, "EnemyDestination", point.transform);
	}
}
