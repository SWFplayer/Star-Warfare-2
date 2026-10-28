using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
	private static GameDataManager mInstance;

	private static bool mIsInited;

	private Dictionary<string, GameDataTable> mTableDictionary;

	public static GameDataManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new GameDataManager();
			mInstance.Init();
		}
		return mInstance;
	}

	public void Init()
	{
		if (mIsInited)
		{
			return;
		}
		mIsInited = true;
		mTableDictionary = new Dictionary<string, GameDataTable>();
		GameObject gameObject = Resources.Load("Data/DataTable") as GameObject;
		DataTableScript component = gameObject.GetComponent<DataTableScript>();
		if (null != component)
		{
			for (int i = 0; i < component.TableList.Count; i++)
			{
				mTableDictionary.Add(component.TableList[i].TableName, component.TableList[i]);
			}
		}
		InitBossDamage();
	}

	private void InitBossDamage()
	{
		GameDataTable gameDataTable = GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.BOSS_DAMAGE);
		GameDataTable gameDataTable2 = GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.BOSS_DAMAGE_TYPE);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int count = gameDataTable[i].Count;
			int[] array = new int[count];
			PlayerDamageType[] array2 = new PlayerDamageType[count];
			for (int j = 0; j < count; j++)
			{
				array[j] = gameDataTable.GetInt(i, j);
				array2[j] = (PlayerDamageType)(int)Enum.Parse(typeof(PlayerDamageType), gameDataTable2.GetString(i, j));
			}
			switch (i)
			{
			case 0:
				SlasherParameter.GetInstance().DAMAGES = array;
				SlasherParameter.GetInstance().DAMAGE_TYPES = array2;
				break;
			case 1:
				DeadEyeParameter.GetInstance().DAMAGES = array;
				DeadEyeParameter.GetInstance().DAMAGE_TYPES = array2;
				break;
			case 2:
				FrostStoneParameter.GetInstance().DAMAGES = array;
				FrostStoneParameter.GetInstance().DAMAGE_TYPES = array2;
				break;
			case 3:
				CerberliskParameter.GetInstance().DAMAGES = array;
				CerberliskParameter.GetInstance().DAMAGE_TYPES = array2;
				break;
			case 4:
				CrimsonQueenParameter.GetInstance().DAMAGES = array;
				CrimsonQueenParameter.GetInstance().DAMAGE_TYPES = array2;
				break;
			default:
				Debug.LogError("Type wrong.");
				break;
			}
		}
	}

	public GameDataTable GetGameDataTable(string fileName, string sheetName)
	{
		string key = fileName + "_" + sheetName;
		if (!mTableDictionary.ContainsKey(key))
		{
			Debug.LogError("invalid data name! fileName = " + fileName + ", sheetName = " + sheetName);
			return null;
		}
		return mTableDictionary[key];
	}
}
