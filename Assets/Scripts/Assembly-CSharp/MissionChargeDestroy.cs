using System.Collections.Generic;
using UnityEngine;

public class MissionChargeDestroy : Mission
{
	private byte mPowerId;

	private List<byte> mItemIdList;

	private int mTotalNumber;

	private int mDestroyedCount;

	public GameObject mPowerCharger;

	private List<GameObject> mDestructibleItems;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mItemIdList = new List<byte>();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num != base.ID)
			{
				continue;
			}
			mPowerId = (byte)dataTable.GetInt(j, 1);
			mTotalNumber = dataTable.GetInt(j, 2);
			for (int k = 0; k < mTotalNumber; k++)
			{
				byte item = (byte)dataTable.GetInt(j, 3 + k);
				if (!mItemIdList.Contains(item))
				{
					mItemIdList.Add(item);
				}
			}
			break;
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mDestroyedCount = 0;
		mDestructibleItems = new List<GameObject>();
		mProcess = EMissionProcess.NORMAL;
		GameApp.GetInstance().GetGameWorld().ClearDestructibleDictionary();
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		localPlayer.isPowerful = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_POWER_CHARGER);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			int result = 0;
			string text = gameObject.name.Replace(MissionNamePrefix.POWER_CHARGER, string.Empty);
			if (int.TryParse(text, out result))
			{
				if ((byte)result == mPowerId)
				{
					MissionManager.SetMissionGameObjectState(gameObject, true);
					gameObject.GetComponent<MissionPowerScript>().enabled = true;
					gameObject.GetComponent<MissionPowerScript>().SetMission(this);
					mPowerCharger = gameObject;
					AddMissionChargerPoint();
				}
				else
				{
					gameObject.GetComponent<MissionPowerScript>().enabled = false;
					MissionManager.SetMissionGameObjectState(gameObject, false);
				}
			}
			else
			{
				Debug.Log("Failed to parse id. p = " + text);
			}
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag(TagName.DESTRUCTIBLE);
		GameObject[] array4 = array3;
		foreach (GameObject gameObject2 in array4)
		{
			int result2 = 0;
			string text2 = gameObject2.name.Replace(MissionNamePrefix.DESTRUCTIBLE, string.Empty);
			if (int.TryParse(text2, out result2))
			{
				if (!mItemIdList.Contains((byte)result2))
				{
					MissionManager.SetMissionGameObjectState(gameObject2, false);
					continue;
				}
				MissionManager.SetMissionGameObjectState(gameObject2, true);
				mDestructibleItems.Add(gameObject2);
				DestructibleObjectScript component = gameObject2.GetComponent<DestructibleObjectScript>();
				component.SetOwnerMission(this);
				GameApp.GetInstance().GetGameWorld().AddDestructible(gameObject2);
			}
			else
			{
				Debug.Log("Failed to parse id. p = " + text2);
			}
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && mDestroyedCount == mTotalNumber)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + mDestroyedCount + " / " + mTotalNumber + ")[-]";
	}

	public void IncreaseDestroyedNumber()
	{
		mDestroyedCount++;
	}

	public void AddMissionDestructiblePoint()
	{
		string empty = string.Empty;
		for (int i = 0; i < mDestructibleItems.Count; i++)
		{
			DestructibleObjectScript component = mDestructibleItems[i].GetComponent<DestructibleObjectScript>();
			if (!component.mDestroyed)
			{
				empty = RadarPointKeyName.CHARGE_DESTRUCTIBLE_ITEM + i;
				GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_ENEMY_BASE, empty, mDestructibleItems[i].transform);
			}
		}
	}

	public void RemoveMissionDestructiblePoint()
	{
		foreach (GameObject mDestructibleItem in mDestructibleItems)
		{
			GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(mDestructibleItem.transform);
		}
	}

	public void AddMissionChargerPoint()
	{
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_BOMB, RadarPointKeyName.POWER_CHARGER, mPowerCharger.transform);
	}

	public override float GetProgressPercentage()
	{
		return (float)mDestroyedCount / (float)mTotalNumber;
	}
}
