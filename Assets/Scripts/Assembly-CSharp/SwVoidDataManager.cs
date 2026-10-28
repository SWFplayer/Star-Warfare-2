using System.Collections.Generic;
using UnityEngine;

public class SwVoidDataManager
{
	public enum ESwVoidMaterialType
	{
		GEM_N1 = 1,
		GEM_N2 = 2,
		GEM_N3 = 3,
		GEM_N4 = 4,
		GEM_N5 = 5,
		GEM_N6 = 6,
		GEM_N7 = 7,
		GEM_H1 = 101,
		GEM_H2 = 102,
		GEM_H3 = 103,
		GEM_H4 = 104,
		GEM_H5 = 105,
		GEM_H6 = 106,
		GEM_H7 = 107,
		MINERAL_N1 = 201,
		MINERAL_N2 = 202,
		MINERAL_N3 = 203,
		MINERAL_N4 = 204,
		MINERAL_N5 = 205,
		MINERAL_N6 = 206,
		MINERAL_N7 = 207,
		MINERAL_H1 = 301,
		MINERAL_H2 = 302,
		MINERAL_H3 = 303,
		MINERAL_H4 = 304,
		MINERAL_H5 = 305,
		MINERAL_H6 = 306,
		MINERAL_H7 = 307
	}

	public enum ESwVoidTypeIndex
	{
		TYPE1_LV1 = 1,
		TYPE1_LV2 = 2,
		TYPE1_LV3 = 3,
		TYPE1_LV4 = 4,
		TYPE1_LV5 = 5,
		TYPE1_LV6 = 6,
		TYPE2_LV1 = 101,
		TYPE2_LV2 = 102,
		TYPE2_LV3 = 103,
		TYPE2_LV4 = 104,
		TYPE2_LV5 = 105,
		TYPE2_LV6 = 106,
		TYPE3_LV1 = 201,
		TYPE3_LV2 = 202,
		TYPE3_LV3 = 203,
		TYPE3_LV4 = 204,
		TYPE3_LV5 = 205,
		TYPE3_LV6 = 206,
		TYPE4_LV1 = 301,
		TYPE4_LV2 = 302,
		TYPE4_LV3 = 303,
		TYPE4_LV4 = 304,
		TYPE4_LV5 = 305,
		TYPE4_LV6 = 306,
		TYPE5_LV1 = 401,
		TYPE5_LV2 = 402,
		TYPE5_LV3 = 403,
		TYPE5_LV4 = 404,
		TYPE5_LV5 = 405,
		TYPE5_LV6 = 406,
		TYPE6_LV1 = 501,
		TYPE6_LV2 = 502,
		TYPE6_LV3 = 503,
		TYPE6_LV4 = 504,
		TYPE6_LV5 = 505,
		TYPE6_LV6 = 506
	}

	public enum ESwVoidMaterialDropColumn
	{
		ID = 0,
		MODE = 1,
		LEVEL = 2,
		ITEM_ORDER = 3,
		MATERIAL_1 = 4,
		MATERIAL_1_RATE = 5,
		MATERIAL_2 = 6,
		MATERIAL_2_RATE = 7,
		MATERIAL_3 = 8,
		MATERIAL_3_RATE = 9,
		MATERIAL_4 = 10,
		MATERIAL_4_RATE = 11
	}

	public enum ESwVoidComposeTableColumn
	{
		ID = 0,
		NAME = 1,
		AVAILABLE = 2,
		TYPE = 3,
		ITEM_LEVEL = 4,
		UNLOCK_MODE = 5,
		UNLOCK_LEVEL = 6,
		REQUIREMENT = 7,
		MATERIAL_1 = 8,
		MATERIAL_1_NUM = 9,
		MATERIAL_2 = 10,
		MATERIAL_2_NUM = 11,
		MATERIAL_3 = 12,
		MATERIAL_3_NUM = 13,
		MATERIAL_4 = 14,
		MATERIAL_4_NUM = 15,
		ACTIVE_SKILL = 16,
		ICON_NAME = 17,
		DESCRIPTION = 18
	}

	public enum ESwVoidUpgradeAttributeColumn
	{
		ID = 0,
		ATTRIBUTE = 1,
		ATTRIBUTE_LEVEL = 2,
		AVAILABLE = 3,
		UPGRADE_MIN = 4,
		UPGRADE_MAX = 5
	}

	public enum ESwVoidAttribute
	{
		NONE = 0,
		BULLET_DAMAGE = 1,
		ENERGY_DAMAGE = 2,
		EXPLOSIVE_DAMAGE = 3,
		MELEE_DAMAGE = 4,
		ALL_DAMAGE = 5,
		COOL_DOWN = 6,
		BULLET_RES = 7,
		ENERGY_RES = 8,
		EXPLOSIVE_RES = 9,
		MELEE_RES = 10,
		ALL_RES = 11,
		LIFE_UP = 12,
		COUNT_INCLUDE_NONE = 13
	}

	public enum ESwVoidPassiveSkillColumn
	{
		ID = 0,
		TYPE = 1,
		PASSIVE_SKILL_NAME = 2,
		SKILL_INFO_ID = 3,
		ICON_NAME = 4,
		DESCRIPTION = 5
	}

	public enum ESwVoidActiveSkillColumn
	{
		ID = 0,
		NAME = 1,
		SKILL_TYPE = 2,
		VALUE1 = 3,
		VALUE2 = 4,
		VALUE3 = 5,
		VALUE4 = 6,
		VALUE5 = 7,
		VALUE6 = 8,
		CD = 9,
		ICON_NAME = 10,
		DESC = 11
	}

	public enum ESwVoidStaticRow
	{
		MaterialDropRate = 0,
		MaterialDropRatePlus = 1,
		FirstBossStaticMaterialId1 = 2,
		FirstBossStaticMaterialNum1 = 3,
		FirstBossStaticMaterialId2 = 4,
		FirstBossStaticMaterialNum2 = 5,
		RareDropRate = 6,
		MaxSlot = 7
	}

	public enum ESwVoidStaticColumn
	{
		ID = 0,
		DESC = 1,
		VALUE = 2
	}

	public enum ESwVoidMaterialColumn
	{
		ID = 0,
		NAME = 1,
		RARITY = 2,
		ICON = 3,
		DES = 4
	}

	public enum ESwVoidSlotPriceColumn
	{
		COUNT = 0,
		PRICE = 1
	}

	public enum ESwVoidGetRewardState
	{
		NO_REWARD = 0,
		FIRST_BOSS = 1,
		WAITING = 2,
		NO_CONNECT = 3,
		GOT = 4
	}

	public enum ESwVoidMaterialDropTrashColumn
	{
		ID = 0,
		MODE = 1,
		LEVEL = 2,
		STAGE = 3,
		MATERIAL1 = 4,
		MATERIAL2 = 5,
		MATERIAL3 = 6,
		MATERIAL4 = 7,
		RARE_MATERIAL = 8
	}

	private const int MaterialGroupLen = 7;

	public const int SWVOID_MAX_LEVEL = 6;

	public const int SWVOID_CURRENT_MAX_LEVEL = 4;

	public const int SWVOID_ATTRIBUTES = 2;

	public const int SWVOID_PASSIVE_SKILLS = 2;

	public const int SWVOID_COUNT_LIMIT = 10;

	public const int SWVOID_MATERIAL_COLOR_SIZE = 5;

	public const int SWVOID_MATERIAL_GROUP_SIZE = 4;

	public static short[][] sortedPassiveSkill;

	private int enemyKillCount;

	private short[] dropRoundArray;

	private short[] dropTypeArray;

	private short[] dropBossArray;

	private byte arrayIndex;

	private bool noMaterialDrop = true;

	private bool firstTimeToFightBoss;

	public static SwVoidDataManager mInstance;

	public static SwVoidDataManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new SwVoidDataManager();
		}
		return mInstance;
	}

	public static int GetMaterialRowIndexById(short id)
	{
		int num = id;
		int num2 = 0;
		while (num >= 100)
		{
			num -= 100;
			num2++;
		}
		return num2 * 7 + num - 1;
	}

	public static int GetMaterialRowIndexByType(ESwVoidMaterialType type)
	{
		return GetMaterialGroup(type) * 7 + GetMaterialIdInGroup(type);
	}

	public static int GetMaterialGroup(ESwVoidMaterialType type)
	{
		if (type < ESwVoidMaterialType.GEM_H1)
		{
			return 0;
		}
		if (type < ESwVoidMaterialType.MINERAL_N1)
		{
			return 1;
		}
		if (type < ESwVoidMaterialType.MINERAL_H1)
		{
			return 2;
		}
		return 3;
	}

	public static int GetMaterialIdInGroup(ESwVoidMaterialType type)
	{
		int num;
		for (num = (int)(type - 1); num > 7; num -= 100)
		{
		}
		return num;
	}

	public static void AddMaterial(short materialId, short number)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.swVoidMaterialList.ContainsKey(materialId))
		{
			Dictionary<short, short> swVoidMaterialList;
			Dictionary<short, short> dictionary = (swVoidMaterialList = userState.swVoidMaterialList);
			short key2;
			short key = (key2 = materialId);
			key2 = swVoidMaterialList[key2];
			dictionary[key] = (short)(key2 + number);
		}
		else
		{
			userState.swVoidMaterialList.Add(materialId, number);
		}
	}

	public static short[] GetMaterialIdsInGroup(int rareId)
	{
		short[] array = new short[5];
		for (int i = 0; i < 5; i++)
		{
			array[i] = GetMaterialIdByGroupAndColor(rareId, i);
		}
		return array;
	}

	public static short GetMaterialIdByGroupAndColor(int rareId, int colorId)
	{
		return (short)(rareId * 100 + colorId + 1);
	}

	public static void UseMaterial(short materialId, short number)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		Dictionary<short, short> swVoidMaterialList;
		Dictionary<short, short> dictionary = (swVoidMaterialList = userState.swVoidMaterialList);
		short key2;
		short key = (key2 = materialId);
		key2 = swVoidMaterialList[key2];
		dictionary[key] = (short)(key2 - number);
	}

	public static string GetMaterialIconName(short materialId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int num = gameDataTable.GetInt(i, 0);
			if (num == materialId)
			{
				return gameDataTable.GetString(i, 3);
			}
		}
		return string.Empty;
	}

	public static string GetMaterialRarityName(short materialId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int num = gameDataTable.GetInt(i, 0);
			if (num == materialId)
			{
				return "item_" + gameDataTable.GetString(i, 2);
			}
		}
		return string.Empty;
	}

	public static string GetMaterialName(short materialId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int num = gameDataTable.GetInt(i, 0);
			if (num == materialId)
			{
				return LocalizationManager.GetInstance().GetString(gameDataTable.GetString(i, 1));
			}
		}
		return string.Empty;
	}

	public static string GetMaterialDes(short materialId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			int num = gameDataTable.GetInt(i, 0);
			if (num == materialId)
			{
				return LocalizationManager.GetInstance().GetString(gameDataTable.GetString(i, 4));
			}
		}
		return string.Empty;
	}

	public static void GetMaterialAfterMission(int mode, int level, bool hasExtraSkill)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL_DROP);
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL);
		int num = mode * 28 + (level - 1) * 4;
		int num2 = ((!hasExtraSkill) ? 3 : 4);
		for (int i = 0; i < num2; i++)
		{
			int[] rates = new int[4]
			{
				gameDataTable.GetInt(num, 5),
				gameDataTable.GetInt(num, 7),
				gameDataTable.GetInt(num, 9),
				gameDataTable.GetInt(num, 11)
			};
			int column = 2 * GetMaterialRandomIndex(rates) + 4;
			short materialId = (short)gameDataTable.GetInt(num, column);
			AddMaterial(materialId, 1);
			num++;
		}
	}

	private static int GetMaterialRandomIndex(int[] rates)
	{
		int num = Random.Range(0, 100);
		int num2 = 0;
		for (int i = 0; i < rates.Length; i++)
		{
			num2 += rates[i];
			if (num < num2)
			{
				return i;
			}
		}
		return rates.Length - 1;
	}

	public static bool CheckMaterial(byte type, byte nextlevel, UserState userState)
	{
		bool flag = true;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE);
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel(type, nextlevel);
		short[] array = new short[4]
		{
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 9),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 11),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 13),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 15)
		};
		short[] array2 = new short[4]
		{
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 8),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 10),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 12),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 14)
		};
		short[] array3 = new short[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			if (array[i] == 0 || array2[i] == 0)
			{
				array3[i] = 0;
			}
			else
			{
				array3[i] = userState.GetMaterialNumber(array2[i]);
			}
		}
		for (int j = 0; j < array2.Length; j++)
		{
			if (array[j] > array3[j])
			{
				flag = false;
			}
		}
		if (flag)
		{
			for (int k = 0; k < array2.Length; k++)
			{
				if (array2[k] != 0)
				{
					UseMaterial(array2[k], array[k]);
				}
			}
		}
		return flag;
	}

	public static short[] GetMaterialNeedNum(byte type, byte level)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE);
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel(type, level);
		return new short[4]
		{
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 9),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 11),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 13),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 15)
		};
	}

	public static short[] GetMaterialNeedId(byte type, byte level)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE);
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel(type, level);
		return new short[4]
		{
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 8),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 10),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 12),
			gameDataTable.GetShort(swVoidRowByTypeAndLevel, 14)
		};
	}

	public static int GetSwVoidRowByTypeAndLevel(byte type, byte level)
	{
		int num = 0;
		return type * 6 + level - 1;
	}

	public static void CreateAttribute(SwVoidSavedata newSwVoid)
	{
		byte[] array = new byte[2];
		short[] array2 = new short[2];
		for (int i = 0; i < 2; i++)
		{
			bool flag = true;
			do
			{
				flag = false;
				array[i] = (byte)Random.Range(1, 13);
				for (int j = 0; j < i; j++)
				{
					if (array[i] == array[j])
					{
						flag = true;
					}
				}
				if (!flag)
				{
					array2[i] = GetRandomInAttributeUpgrade(array[i], 1);
					newSwVoid.SetAttribute(array[i], array2[i]);
				}
			}
			while (flag);
		}
	}

	public static void TryLevelUp(SwVoidSavedata swVoid)
	{
		if (!swVoid.isMaxLevel() && CheckMaterial(swVoid.GetTypeInByte(), (byte)(swVoid.GetLevel() + 1), GameApp.GetInstance().GetUserState()))
		{
			swVoid.LevelUp();
			for (int i = 0; i < 2; i++)
			{
				short attributeValue = swVoid.GetAttributeValue(i);
				attributeValue += GetRandomInAttributeUpgrade(swVoid.GetAttributeId(i), swVoid.GetLevel());
				swVoid.ChangeAttributeValue(i, attributeValue);
			}
			swVoid.RefreshAttributeDes();
		}
	}

	public static short[] TryPreLevelUp(SwVoidSavedata swVoid)
	{
		if (!swVoid.isMaxLevel() && CheckMaterial(swVoid.GetTypeInByte(), (byte)(swVoid.GetLevel() + 1), GameApp.GetInstance().GetUserState()))
		{
			short[] array = new short[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = GetRandomInAttributeUpgrade(swVoid.GetAttributeId(i), (byte)(swVoid.GetLevel() + 1));
			}
			return array;
		}
		return null;
	}

	public static void RealLevelUp(SwVoidSavedata swVoid, short[] upgradeValue)
	{
		swVoid.LevelUp();
		for (int i = 0; i < 2; i++)
		{
			short attributeValue = swVoid.GetAttributeValue(i);
			attributeValue += upgradeValue[i];
			swVoid.ChangeAttributeValue(i, attributeValue);
		}
		swVoid.RefreshAttributeDes();
		swVoid.InitSkillGroup();
	}

	public static string GetOriginNameFromIndex(ESwVoidType type, byte level)
	{
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		if (swVoidRowByTypeAndLevel < 0)
		{
			return string.Empty;
		}
		return GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE).GetString(swVoidRowByTypeAndLevel, 1);
	}

	public static string GetFinalNameFromIndex(ESwVoidType type, byte level)
	{
		return LocalizationManager.GetInstance().GetString(GetOriginNameFromIndex(type, level)) + " LV" + level;
	}

	public static bool GetAvailable(ESwVoidType type, byte level)
	{
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		int num = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE).GetInt(swVoidRowByTypeAndLevel, 2);
		if (num == 1)
		{
			return true;
		}
		return false;
	}

	public static short GetRandomInAttributeUpgrade(byte attributeId, byte level)
	{
		int row = (attributeId - 1) * 6 + level - 1;
		int min = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_UPGRADE_ATTRIBUTE).GetShort(row, 4);
		int num = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_UPGRADE_ATTRIBUTE).GetShort(row, 5);
		return (short)Random.Range(min, num + 1);
	}

	public static bool IsAttributeMax(byte attributeId, byte level, short value)
	{
		int row = (attributeId - 1) * 6 + level - 1;
		int num = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_UPGRADE_ATTRIBUTE).GetShort(row, 5);
		return value == num;
	}

	public static string GetDescriptionOfAttribueUpgrade(byte attributeId, byte level)
	{
		int row = (attributeId - 1) * 6 + level - 1;
		short num = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_UPGRADE_ATTRIBUTE).GetShort(row, 4);
		short num2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_UPGRADE_ATTRIBUTE).GetShort(row, 5);
		string empty = string.Empty;
		empty += GetAttributeNameFromId(attributeId);
		empty += " + ( ";
		empty += num;
		empty += " - ";
		empty += num2;
		return empty + " )%";
	}

	public static string GetAttributeNameFromId(byte id)
	{
		return LocalizationManager.GetInstance().GetString(GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_ATTRIBUTE).GetString(id, 1));
	}

	public static void SortPassiveSkill()
	{
		if (sortedPassiveSkill != null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_PASSIVE_SKILL);
		for (int i = 0; i < gameDataTable.RowList.Count; i++)
		{
			if (gameDataTable.GetString(i, 1) != string.Empty)
			{
				if (gameDataTable.GetInt(i, 1) == 1)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
		}
		sortedPassiveSkill = new short[2][];
		sortedPassiveSkill[0] = new short[num];
		sortedPassiveSkill[1] = new short[num2];
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < gameDataTable.RowList.Count; j++)
		{
			if (gameDataTable.GetString(j, 1) != string.Empty)
			{
				if (gameDataTable.GetInt(j, 1) == 1)
				{
					sortedPassiveSkill[0][num3] = gameDataTable.GetShort(j, 0);
					num3++;
				}
				else
				{
					sortedPassiveSkill[1][num4] = gameDataTable.GetShort(j, 0);
					num4++;
				}
			}
		}
	}

	public static void CreatePassiveSkill(SwVoidSavedata newSwVoid)
	{
		SortPassiveSkill();
		newSwVoid.SetPassiveSkill(new short[2]
		{
			(byte)sortedPassiveSkill[0][Random.Range(0, sortedPassiveSkill[0].Length)],
			(byte)sortedPassiveSkill[1][Random.Range(0, sortedPassiveSkill[1].Length)]
		});
	}

	public static SkillsType GetSkillTypeFromAttributeId(byte attrId)
	{
		switch (attrId)
		{
		case 1:
			return SkillsType.BULLET_DAMAGE;
		case 2:
			return SkillsType.ENERGY_DAMAGE;
		case 3:
			return SkillsType.EXPLOSION_DAMAGE;
		case 4:
			return SkillsType.MELEE_DAMAGE;
		case 5:
			return SkillsType.ALL_DAMAGE;
		case 6:
			return SkillsType.COOL_DOWN_SPEEDUP;
		case 7:
			return SkillsType.BULLET_RES;
		case 8:
			return SkillsType.ENERGY_RES;
		case 9:
			return SkillsType.EXPLOSION_RES;
		case 10:
			return SkillsType.MELEE_RES;
		case 11:
			return SkillsType.ALL_RES;
		case 12:
			return SkillsType.HP_UP_RATE;
		default:
			Debug.LogError("The attribute is not defined in skill type");
			return SkillsType.SKILL_NONE;
		}
	}

	public static SkillsType GetSkillTypeFromPassiveSkillId(short passiveSkillId)
	{
		switch (passiveSkillId)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
			return (SkillsType)(64 + passiveSkillId);
		default:
			return SkillsType.SKILL_NONE;
		}
	}

	public static int GetPassiveSkillIdInfoId(short skillId)
	{
		return GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_PASSIVE_SKILL).GetShort(skillId - 1, 3);
	}

	public static string GetDescriptionOfSwVoid(ESwVoidType type, byte level, UserState mUserState, bool noNextLevel = false)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE);
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		string key = gameDataTable.GetString(swVoidRowByTypeAndLevel, 18);
		string result = string.Empty;
		string text = string.Empty;
		switch (type)
		{
		case ESwVoidType.DEMOMAN:
		{
			int swVoidParameterInt2 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE2);
			swVoidParameterInt2 += (int)(GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
			int swVoidParameterInt3 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.CD);
			if (level < 4 && !noNextLevel)
			{
				int swVoidParameterInt4 = GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE2);
				swVoidParameterInt4 += (int)(GetSwVoidParameterFloat(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
				text = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt4);
			}
			result = LocalizationManager.GetInstance().GetString(key, swVoidParameterInt3, swVoidParameterInt2, 0, text, string.Empty, string.Empty, string.Empty);
			break;
		}
		case ESwVoidType.MEDIC:
		{
			string str3 = string.Empty;
			int num5 = (int)(0.01f * GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE3) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f));
			int swVoidParameterInt8 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE1);
			swVoidParameterInt8 += (int)(GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE2) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f));
			int swVoidParameterInt9 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.CD);
			if (level < 4 && !noNextLevel)
			{
				int num6 = (int)(0.01f * GetSwVoidParameterFloat(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE3) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f));
				text = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", num6);
				int swVoidParameterInt10 = GetSwVoidParameterInt(type, (byte)(1 + level), ESwVoidActiveSkillColumn.VALUE1);
				swVoidParameterInt10 += (int)(GetSwVoidParameterFloat(type, (byte)(1 + level), ESwVoidActiveSkillColumn.VALUE2) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f));
				str3 = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt10);
			}
			result = LocalizationManager.GetInstance().GetString(key, swVoidParameterInt9, num5, swVoidParameterInt8, text, str3, string.Empty, string.Empty);
			break;
		}
		case ESwVoidType.SOLDIER:
		{
			int swVoidParameterInt5 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE2);
			swVoidParameterInt5 += (int)(GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
			int swVoidParameterInt6 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.CD);
			if (level < 4 && !noNextLevel)
			{
				int swVoidParameterInt7 = GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE2);
				swVoidParameterInt7 += (int)(GetSwVoidParameterFloat(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
				text = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt7);
			}
			result = LocalizationManager.GetInstance().GetString(key, swVoidParameterInt6, swVoidParameterInt5, 0, text, string.Empty, string.Empty, string.Empty);
			break;
		}
		case ESwVoidType.SCOUT:
		{
			int swVoidParameterInt11 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE4);
			int swVoidParameterInt12 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.CD);
			int swVoidParameterInt13 = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE2);
			swVoidParameterInt13 += (int)(GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
			string str4 = string.Empty;
			string str5 = string.Empty;
			if (level < 4 && !noNextLevel)
			{
				int swVoidParameterInt14 = GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE4);
				str5 = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt14);
				int swVoidParameterInt15 = GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.CD);
				str4 = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt15);
				int swVoidParameterInt16 = GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE2);
				swVoidParameterInt16 += (int)(GetSwVoidParameterFloat(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE3) * mUserState.GetBattleWeapons()[0].Dps);
				text = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", swVoidParameterInt16);
			}
			result = LocalizationManager.GetInstance().GetString(key, swVoidParameterInt12, swVoidParameterInt11, swVoidParameterInt13, str5, str4, text, string.Empty);
			break;
		}
		case ESwVoidType.HEAVY:
		{
			int swVoidParameterInt = GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.CD);
			int num = (int)((float)GetSwVoidParameterInt(type, level, ESwVoidActiveSkillColumn.VALUE2) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f) / 100f);
			int num2 = (int)(GetSwVoidParameterFloat(type, level, ESwVoidActiveSkillColumn.VALUE6) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f) / 100f);
			string str = string.Empty;
			string str2 = string.Empty;
			if (level < 4 && !noNextLevel)
			{
				int num3 = (int)(GetSwVoidParameterFloat(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE6) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f) / 100f);
				str2 = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", num3);
				int num4 = (int)((float)GetSwVoidParameterInt(type, (byte)(level + 1), ESwVoidActiveSkillColumn.VALUE2) * mUserState.playerSkill.GetSkill(SkillsType.HP_BOOTH) * (mUserState.playerSkill.GetSkill(SkillsType.HP_UP_RATE) + 1f) / 100f);
				str = LocalizationManager.GetInstance().GetString("LOC_VOID_NEXT_LEVEL", num4);
			}
			result = LocalizationManager.GetInstance().GetString(key, swVoidParameterInt, num, num2, str, str2, string.Empty, string.Empty);
			break;
		}
		}
		return result;
	}

	public static string GetIconNameOfSwVoid(ESwVoidType type, byte level)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_COMPOSE_TABLE);
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		return gameDataTable.GetString(swVoidRowByTypeAndLevel, 17);
	}

	public static string GetNameOfPassiveSkill(short passiveSkillId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_PASSIVE_SKILL);
		return gameDataTable.GetString(passiveSkillId - 1, 2);
	}

	public static string GetIconNameOfPassiveSkill(short passiveSkillId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_PASSIVE_SKILL);
		return gameDataTable.GetString(passiveSkillId - 1, 4);
	}

	public static string GetDescriptionOfPassiveSkill(short passiveSkillId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_PASSIVE_SKILL);
		return gameDataTable.GetString(passiveSkillId - 1, 5);
	}

	public static List<short> GetMaterialDrops(EDifficultMode difficultMode, int stageId, int subStageId)
	{
		List<short> list = new List<short>();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_MATERIAL_DROP_TRASH);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			int num = (int)difficultMode * 1000 + stageId * 10 + subStageId;
			if (gameDataTable.GetInt(i, 0) != num)
			{
				continue;
			}
			for (ESwVoidMaterialDropTrashColumn eSwVoidMaterialDropTrashColumn = ESwVoidMaterialDropTrashColumn.MATERIAL1; eSwVoidMaterialDropTrashColumn <= ESwVoidMaterialDropTrashColumn.MATERIAL4; eSwVoidMaterialDropTrashColumn++)
			{
				short num2 = gameDataTable.GetShort(i, (int)eSwVoidMaterialDropTrashColumn);
				if (num2 != 0)
				{
					list.Add(num2);
				}
			}
			break;
		}
		return list;
	}

	public void CheckDropMaterial(Vector3 dropPosition)
	{
		if (noMaterialDrop)
		{
			return;
		}
		enemyKillCount++;
		if (dropRoundArray[arrayIndex] == enemyKillCount)
		{
			arrayIndex++;
			if (arrayIndex >= dropRoundArray.Length)
			{
				arrayIndex = 0;
				enemyKillCount -= 500;
			}
			DropMaterialEffect((ESwVoidMaterialType)dropTypeArray[arrayIndex], dropPosition);
		}
	}

	public void DropBossMaterial(Vector3 dropPosition)
	{
		if (dropBossArray != null)
		{
			for (int i = 0; i < dropBossArray.Length; i++)
			{
				DropMaterialEffect((ESwVoidMaterialType)dropBossArray[i], dropPosition);
			}
		}
	}

	public static void DropMaterialEffect(ESwVoidMaterialType materialType, Vector3 dropPosition)
	{
		if (GameApp.GetInstance().IsConnectedToInternet())
		{
			int num = (int)materialType % 10;
			GameObject gameObject = null;
			switch (num)
			{
			case 1:
				gameObject = Resources.Load("SW2_Effect/sw2_debris02") as GameObject;
				break;
			case 2:
				gameObject = Resources.Load("SW2_Effect/sw2_debris03") as GameObject;
				break;
			case 3:
				gameObject = Resources.Load("SW2_Effect/sw2_debris01") as GameObject;
				break;
			case 4:
				gameObject = Resources.Load("SW2_Effect/sw2_debris04") as GameObject;
				break;
			case 5:
				gameObject = Resources.Load("SW2_Effect/sw2_debris05") as GameObject;
				break;
			}
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
				gameObject2.transform.position = dropPosition;
				gameObject2.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
			}
		}
	}

	public void SetDropMaterialInfo(short[] dropRound, short[] dropType, short[] dropBossArray)
	{
		noMaterialDrop = false;
		if (dropRound == null || dropType == null)
		{
			noMaterialDrop = true;
			return;
		}
		dropRoundArray = dropRound;
		for (int i = 0; i < dropRoundArray.Length; i++)
		{
		}
		dropTypeArray = dropType;
		for (int j = 0; j < dropTypeArray.Length; j++)
		{
		}
		this.dropBossArray = dropBossArray;
		for (int k = 0; k < dropBossArray.Length; k++)
		{
		}
		arrayIndex = 0;
		enemyKillCount = 0;
		if (dropRoundArray == null || dropRoundArray.Length == 0 || dropTypeArray == null || dropTypeArray.Length == 0)
		{
			noMaterialDrop = true;
		}
	}

	public void NoMaterialDrop()
	{
		noMaterialDrop = true;
	}

	public static float GetSwVoidParameterFloat(ESwVoidType type, byte level, ESwVoidActiveSkillColumn columnId)
	{
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_ACTIVE_SKILL);
		return gameDataTable.GetFloat(swVoidRowByTypeAndLevel, (int)columnId);
	}

	public static int GetSwVoidParameterInt(ESwVoidType type, byte level, ESwVoidActiveSkillColumn columnId)
	{
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_ACTIVE_SKILL);
		return gameDataTable.GetInt(swVoidRowByTypeAndLevel, (int)columnId);
	}

	public static string GetSwVoidParameterString(ESwVoidType type, byte level, ESwVoidActiveSkillColumn columnId)
	{
		int swVoidRowByTypeAndLevel = GetSwVoidRowByTypeAndLevel((byte)type, level);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_ACTIVE_SKILL);
		return gameDataTable.GetString(swVoidRowByTypeAndLevel, (int)columnId);
	}

	public static short[] GetFirstBossStaticMaterial()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_STATIC);
		short num = gameDataTable.GetShort(2, 2);
		short num2 = gameDataTable.GetShort(3, 2);
		short num3 = gameDataTable.GetShort(4, 2);
		short num4 = gameDataTable.GetShort(5, 2);
		short[] array = new short[num2 + num4];
		int num5 = 0;
		for (num5 = 0; num5 < num2; num5++)
		{
			array[num5] = num;
		}
		for (; num5 < num2 + num4; num5++)
		{
			array[num5] = num3;
		}
		return array;
	}

	public static void TryGetRandomDropFromOnline(EDifficultMode difficultMode, byte stage, byte subStage, bool isCoopMode)
	{
		GetInstance().firstTimeToFightBoss = false;
		if (difficultMode == EDifficultMode.NORMAL && stage == 0 && subStage == 6 && !isCoopMode)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			if (userState != null && userState.GetNormalMissionState().GetUnlockedStageId() < 1)
			{
				GetInstance().firstTimeToFightBoss = true;
			}
		}
		if (GetInstance().firstTimeToFightBoss)
		{
			SwVoidGetRandomDropResponseData swVoidGetRandomDropResponseData = new SwVoidGetRandomDropResponseData();
			swVoidGetRandomDropResponseData.result = SwErrorCode.SUCCESS;
			swVoidGetRandomDropResponseData.dropRoundArray = new short[0];
			swVoidGetRandomDropResponseData.dropTypeArray = new short[0];
			swVoidGetRandomDropResponseData.dropBossArray = GetFirstBossStaticMaterial();
			SwVoidOnlineProcess.SwVoidGetRandomDropResponse(swVoidGetRandomDropResponseData);
		}
		else
		{
			if (!GameApp.GetInstance().IsConnectedToInternet())
			{
				return;
			}
			if (GameApp.GetInstance().GetUserState().HasLoginOnline())
			{
				if (GameApp.GetInstance().IsConnectedToServer())
				{
					SwVoidGetRandomDropRequest request = new SwVoidGetRandomDropRequest(GameApp.GetInstance().GetUserState().SwVoidIdInUsing, difficultMode, stage, subStage);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				else
				{
					SwVoidGetRandomDropWWWRequest request2 = new SwVoidGetRandomDropWWWRequest(GameApp.GetInstance().GetUserState().SwVoidIdInUsing, (byte)difficultMode, stage, subStage);
					GameApp.GetInstance().GetWWWManager().SendRequest(request2);
				}
			}
			else
			{
				SwVoidOnlineProcess.NotRegistedInfo();
			}
		}
	}

	public static ESwVoidGetRewardState TryGetRewardFromOnline()
	{
		ESwVoidGetRewardState result;
		if (GetInstance().firstTimeToFightBoss)
		{
			result = ESwVoidGetRewardState.FIRST_BOSS;
			SwVoidGetRewardResponseData swVoidGetRewardResponseData = new SwVoidGetRewardResponseData();
			swVoidGetRewardResponseData.result = SwErrorCode.SUCCESS;
			swVoidGetRewardResponseData.materialTypeArray = GetInstance().dropBossArray;
			GetInstance().dropBossArray = null;
			SwVoidOnlineProcess.SwVoidGetRewardResponse(swVoidGetRewardResponseData);
		}
		else
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			if (userState.HasLoginOnline())
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					result = ESwVoidGetRewardState.WAITING;
					int killsByAllPlayers = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
						.GetKillsByAllPlayers();
					bool isWin = GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin;
					if (GameApp.GetInstance().IsConnectedToServer())
					{
						SwVoidGetRewardAfterGameRequest request = new SwVoidGetRewardAfterGameRequest(userState.SwVoidIdInUsing, userState.GetSelectedDifficultMode(), killsByAllPlayers, userState.GetSelectedStageIndexGeneral(), userState.GetSelectedSubStageIndex(), isWin);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					else
					{
						SwVoidGetRewardWWWRequest request2 = new SwVoidGetRewardWWWRequest(userState.SwVoidIdInUsing, killsByAllPlayers, (byte)userState.GetSelectedDifficultMode(), userState.GetSelectedStageIndexGeneral(), userState.GetSelectedSubStageIndex(), isWin);
						GameApp.GetInstance().GetWWWManager().SendRequest(request2);
					}
				}
				else
				{
					result = ESwVoidGetRewardState.NO_CONNECT;
				}
			}
			else
			{
				SwVoidOnlineProcess.NotRegistedInfo();
				result = ESwVoidGetRewardState.NO_REWARD;
			}
		}
		return result;
	}

	public static void SendAddSlotRequest()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_STATIC);
		int swvoidCountLimit = userState.swvoidCountLimit;
		if (swvoidCountLimit >= gameDataTable.GetInt(7, 2))
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_SWVOID_REACH_MAX_SLOT"));
			return;
		}
		byte b = (byte)(userState.swvoidCountLimit + 1);
		if (GameApp.GetInstance().IsConnectedToInternet() && userState.HasLoginOnline())
		{
			CustomizeUIScript.mInstance.WaitForSwVoid();
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidAddSlotRequest request = new SwVoidAddSlotRequest(b);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				SwVoidAddSlotWWWRequest request2 = new SwVoidAddSlotWWWRequest(b);
				GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			}
		}
	}

	public static int CostOfBuyNewSlot(int nextSlotNumber)
	{
		int result = 0;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SWVOID_LIST, DataSheetName.SWVOID_SLOT_PRICE);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			if (nextSlotNumber == gameDataTable.GetInt(i, 0))
			{
				result = gameDataTable.GetInt(i, 1);
			}
		}
		return result;
	}

	public static void SendIapChestRequest(IapScript.IapPackageType chestType)
	{
		if (GameApp.GetInstance().IsConnectedToInternet() && GameApp.GetInstance().GetUserState().HasLoginOnline())
		{
			List<short> list = RecheckMaterialWishList();
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidIapChestRequest request = new SwVoidIapChestRequest(chestType, list);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				SwVoidIapChestWWWRequest request2 = new SwVoidIapChestWWWRequest(chestType, list);
				GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			}
		}
	}

	public static void SendCheckServerRequest()
	{
		if (GameApp.GetInstance().IsConnectedToInternet() && GameApp.GetInstance().GetUserState().HasLoginOnline())
		{
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidCheckServerRequest request = new SwVoidCheckServerRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				SwVoidCheckServerWWWRequest request2 = new SwVoidCheckServerWWWRequest();
				GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			}
		}
	}

	private static List<short> RecheckMaterialWishList()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		List<short> list = new List<short>();
		for (int i = 0; i < 4; i++)
		{
			bool flag = false;
			short[] materialIdsInGroup = GetMaterialIdsInGroup(i);
			foreach (short num in materialIdsInGroup)
			{
				if (userState.GetMaterialWish(num))
				{
					flag = true;
					list.Add(num);
				}
			}
			if (!flag)
			{
				short[] materialIdsInGroup2 = GetMaterialIdsInGroup(i);
				foreach (short item in materialIdsInGroup2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static int CalcAllMaterialCount()
	{
		int num = 0;
		UserState userState = GameApp.GetInstance().GetUserState();
		Dictionary<short, short> swVoidMaterialList = userState.swVoidMaterialList;
		foreach (KeyValuePair<short, short> item in swVoidMaterialList)
		{
			num += item.Value;
		}
		return num;
	}

	public static bool HasNewMaterial()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		Dictionary<short, bool> swVoidMaterialIsNewList = userState.swVoidMaterialIsNewList;
		if (swVoidMaterialIsNewList != null)
		{
			return swVoidMaterialIsNewList.ContainsValue(true);
		}
		return false;
	}

	public static bool IsAllOrNoWishInGroup(int rareId)
	{
		bool flag = false;
		bool flag2 = true;
		short[] materialIdsInGroup = GetMaterialIdsInGroup(rareId);
		foreach (short materialId in materialIdsInGroup)
		{
			if (GameApp.GetInstance().GetUserState().GetMaterialWish(materialId))
			{
				flag = true;
			}
			else
			{
				flag2 = false;
			}
		}
		if (flag2)
		{
			short[] materialIdsInGroup2 = GetMaterialIdsInGroup(rareId);
			foreach (short materialId2 in materialIdsInGroup2)
			{
				GameApp.GetInstance().GetUserState().SetMaterialWish(materialId2, false);
			}
		}
		return flag2 || !flag;
	}

	public static string GetIapAmount(IapScript.IapPackageType type, int rareId)
	{
		string result = string.Empty;
		switch (type)
		{
		case IapScript.IapPackageType.CopperChest:
			if (rareId == 0 || rareId == 2)
			{
				result = "120 - 180";
			}
			break;
		case IapScript.IapPackageType.SilverChest:
			switch (rareId)
			{
			case 0:
			case 2:
				result = "16 - 24";
				break;
			case 1:
				result = "128 - 192";
				break;
			}
			break;
		case IapScript.IapPackageType.GoldChest:
			switch (rareId)
			{
			case 0:
			case 2:
				result = "96 - 180";
				break;
			case 1:
				result = "180 - 312";
				break;
			case 3:
				result = "8 - 30";
				break;
			}
			break;
		}
		return result;
	}
}
