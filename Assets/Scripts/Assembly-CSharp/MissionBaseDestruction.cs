using System.Collections.Generic;
using UnityEngine;

public class MissionBaseDestruction : Mission
{
	private List<byte> mBombIdList;

	private List<byte> mBaseIdList;

	private List<MissionEnemyBaseScript> mBaseScriptList = new List<MissionEnemyBaseScript>();

	private List<MissionBombScript> mBombScriptList = new List<MissionBombScript>();

	private int mCurrentBaseId;

	private int mCurrentBombId;

	private int mTotalNumber;

	public Vector3 mEffectPosition;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mBombIdList = new List<byte>();
		mBaseIdList = new List<byte>();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				int num2 = dataTable.GetInt(j, 1);
				for (int k = 0; k < num2; k++)
				{
					byte item = (byte)dataTable.GetInt(j, 2 + k);
					mBaseIdList.Add(item);
				}
				int num3 = dataTable.GetInt(j, 7);
				for (int l = 0; l < num3; l++)
				{
					byte item2 = (byte)dataTable.GetInt(j, 8 + l);
					mBombIdList.Add(item2);
				}
				mTotalNumber = num3;
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		mCurrentBaseId = 0;
		mCurrentBombId = 0;
		mBaseScriptList = new List<MissionEnemyBaseScript>();
		mBombScriptList = new List<MissionBombScript>();
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ENEMY_BASE);
		foreach (byte mBaseId in mBaseIdList)
		{
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				int result = 0;
				string text = gameObject.name.Replace(MissionNamePrefix.ENEMY_BASE, string.Empty);
				if (int.TryParse(text, out result))
				{
					if (mBaseId == result)
					{
						GameObject gameObject2 = gameObject;
						mBaseScriptList.Add(gameObject2.GetComponent<MissionEnemyBaseScript>());
					}
					MissionManager.SetMissionGameObjectState(gameObject, false);
				}
				else
				{
					Debug.LogError("Failed to parse id. p = " + text);
				}
			}
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag(TagName.MISSION_BOMB);
		foreach (byte mBombId in mBombIdList)
		{
			GameObject[] array4 = array3;
			foreach (GameObject gameObject3 in array4)
			{
				int result2 = 0;
				string text2 = gameObject3.name.Replace(MissionNamePrefix.BOMB, string.Empty);
				if (int.TryParse(text2, out result2))
				{
					if (mBombId == result2)
					{
						MissionBombScript component = gameObject3.GetComponent<MissionBombScript>();
						mBombScriptList.Add(component);
						component.BaseScript = mBaseScriptList;
						component.SetMissionBaseDestruction(this);
					}
					MissionManager.SetMissionGameObjectState(gameObject3, false);
				}
				else
				{
					Debug.LogError("Failed to parse id. p = " + text2);
				}
			}
		}
		foreach (MissionEnemyBaseScript mBaseScript in mBaseScriptList)
		{
			mBaseScript.SetMissionBaseDestruction(this);
			mBaseScript.BombList = new List<GameObject>();
			for (int k = 0; k < mBombScriptList.Count; k++)
			{
				mBaseScript.BombList.Add(mBombScriptList[k].gameObject);
			}
		}
		if (mBaseScriptList[0].BombList.Count > 0)
		{
			MissionManager.SetMissionGameObjectState(mBaseScriptList[0].BombList[0], true);
			AddMissionBombPoint(mBaseScriptList[0].BombList[0]);
			mEffectPosition = mBaseScriptList[0].BombList[0].transform.position;
		}
		mMissionRefreshInterval = (float)base.TimeLimitation / (float)(mBaseIdList.Count + mBombIdList.Count);
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && mCurrentBaseId == mTotalNumber)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + mCurrentBaseId + " / " + mTotalNumber + ")[-]";
	}

	public void IncreaseBombNumber()
	{
		mCurrentBombId++;
	}

	public void IncreaseBaseNumber()
	{
		mCurrentBaseId++;
	}

	public int GetBombNumber()
	{
		return mCurrentBombId;
	}

	public int GetBaseNumber()
	{
		return mCurrentBaseId;
	}

	public void AddMissionEnemyBasePoint(GameObject go)
	{
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_ENEMY_BASE, "enemyBase", go.transform);
	}

	public void AddMissionBombPoint(GameObject go)
	{
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_BOMB, "bomb", go.transform);
	}

	public override float GetProgressPercentage()
	{
		return (float)mCurrentBaseId / (float)mTotalNumber;
	}

	public override void ReFreshMissionTarget()
	{
		base.ReFreshMissionTarget();
		if (UIRadar.mInstance != null && MissionDetailsHUD.m_Instance != null)
		{
			UIRadar.mInstance.RefreshTargetEffect("RefreshMissionBaseDestruction", mEffectPosition);
			MissionDetailsHUD.m_Instance.RefreshMission();
		}
	}
}
