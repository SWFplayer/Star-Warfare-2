using System.Collections.Generic;
using UnityEngine;

public class MissionProtectionMove : Mission
{
	private List<byte> mIdList;

	private List<ProtectedItem> mProtectedItemList;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mIdList = new List<byte>();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num != base.ID)
			{
				continue;
			}
			int num2 = dataTable.GetInt(j, 1);
			for (int k = 0; k < num2; k++)
			{
				byte item = (byte)dataTable.GetInt(j, 2 + k);
				if (!mIdList.Contains(item))
				{
					mIdList.Add(item);
				}
			}
			break;
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		mProtectedItemList = new List<ProtectedItem>();
		Dictionary<byte, GameObject> dictionary = new Dictionary<byte, GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_PROTECTED_ITEM);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			int result = 0;
			string text = gameObject.name.Replace(MissionNamePrefix.PROTECTED_TIME, string.Empty);
			if (int.TryParse(text, out result))
			{
				if (!dictionary.ContainsKey((byte)result))
				{
					dictionary.Add((byte)result, gameObject);
				}
				else
				{
					Debug.LogError("Duplicated Destination ID = " + result);
				}
			}
			else
			{
				Debug.LogError("Failed to parse id. p = " + text);
			}
		}
		for (int j = 0; j < mIdList.Count; j++)
		{
			byte key = mIdList[j];
			if (dictionary.ContainsKey(key))
			{
				MissionProtectionScript component = dictionary[key].GetComponent<MissionProtectionScript>();
				if (null != component)
				{
					ProtectedItem protectedItem = new ProtectedItem();
					protectedItem.OwnerMission = this;
					protectedItem.Name = dictionary[key].name;
					protectedItem.Speed = component.Speed;
					protectedItem.SpeedValues = component.SpeedValues;
					protectedItem.WaitTimes = component.WaitTime;
					protectedItem.CanMove = true;
					protectedItem.MaxHp = component.MaxHP;
					protectedItem.PathPoints = component.PathPoints;
					protectedItem.PrefabName = "Mission/Protected/" + component.Prefab;
					protectedItem.SpawnPosition = dictionary[key].transform.position;
					protectedItem.spawnRotation = dictionary[key].transform.rotation;
					protectedItem.ItemType = component.Type;
					protectedItem.Init();
					GameApp.GetInstance().GetGameWorld().AddProtectedItem(protectedItem);
					mProtectedItemList.Add(protectedItem);
					GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_PROTECTED_ITEM, "PortectedItem" + j, protectedItem.GetTransform());
				}
				else
				{
					Debug.LogError("Failed to find MissionProtectionScript in " + dictionary[key].name);
				}
			}
			else
			{
				Debug.LogError("Failed to find Destination GameObject ID = " + mIdList[j]);
			}
		}
		if (mProtectedItemList.Count > 0)
		{
			mProtectedItemList[0].IsLeader = true;
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			bool flag = true;
			foreach (ProtectedItem mProtectedItem in mProtectedItemList)
			{
				if (!mProtectedItem.Arrived)
				{
					flag = false;
				}
			}
			if (flag)
			{
				mProcess = EMissionProcess.SUCCESS;
			}
		}
		if (mProcess == EMissionProcess.NORMAL)
		{
			foreach (ProtectedItem mProtectedItem2 in mProtectedItemList)
			{
				if (mProtectedItem2.Destroyed)
				{
					Debug.Log("FAIL");
					mProcess = EMissionProcess.FAIL;
				}
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return mProtectedItemList[0].GetCurrentIndexPercent();
	}
}
