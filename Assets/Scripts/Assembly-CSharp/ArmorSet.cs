using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSet
{
	public enum ESetListAndSuitColumn
	{
		ID = 0,
		SET_ID = 1,
		NAME = 2,
		SKILL_INFO_ID = 3,
		BULLET_DAMAGE = 4,
		ENERGY_DAMAGE = 5,
		EXPLOSION_DAMAGE = 6,
		MELEE_DAMAGE = 7,
		ALL_DAMAGE = 8,
		COOL_DOWN_SPEEDUP = 9,
		BULLET_RES = 10,
		ENERGY_RES = 11,
		EXPLOSION_RES = 12,
		MELEE_RES = 13,
		ALL_RES = 14,
		HP_UP_RATE = 15,
		HP_AUTO_RECOVERY = 16,
		SPEED_BOOTH = 17,
		MONEY_BOOTH = 18,
		EXP_BOOTH = 19,
		ICON_NAME = 20,
		DES = 21
	}

	private struct ArmorSetSkillInfo
	{
		public string name;

		public string des;

		public SkillGroup mSkillGroup;
	}

	private byte[] mSetElements = new byte[4];

	private byte[] mSuitIds = new byte[4];

	private short mSetId;

	private Dictionary<short, ArmorSetSkillInfo> mSkillGroupList = new Dictionary<short, ArmorSetSkillInfo>();

	public void InitSkillGroupToList(short setId, bool isSuit)
	{
		string sheetName = ((!isSuit) ? DataSheetName.ARMOR_SET_LIST : DataSheetName.SUIT);
		ArmorSetSkillInfo value = default(ArmorSetSkillInfo);
		SkillGroup skillGroup = new SkillGroup();
		skillGroup.source = "Armor Set " + setId;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, sheetName);
		if (gameDataTable != null)
		{
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				if (gameDataTable.GetInt(i, 1) != setId)
				{
					continue;
				}
				value.name = gameDataTable.GetString(i, 2);
				value.des = gameDataTable.GetString(i, 21);
				int num = gameDataTable.GetInt(i, 3);
				if (num != 0)
				{
					skillGroup.InitSkillInfo(num);
				}
				for (int j = 4; j <= 19; j++)
				{
					if (gameDataTable.GetString(i, j) != string.Empty)
					{
						float num2 = gameDataTable.GetFloat(i, j);
						if (num2 != 0f)
						{
							SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), ((ESetListAndSuitColumn)j).ToString());
							skillGroup.AddStaticSkill(type, num2);
						}
					}
				}
			}
		}
		value.mSkillGroup = skillGroup;
		mSkillGroupList.Add(setId, value);
	}

	public void SetArmorElement(BodyType bodyType, byte newElement, byte newSuitId, PlayerSkill playerSkill)
	{
		if (bodyType == BodyType.Bag)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (newSuitId != mSuitIds[(int)bodyType])
		{
			if (hasSuitBonus() && !hasSuitBonus(bodyType, newSuitId))
			{
				flag2 = true;
			}
			else if (!hasSuitBonus() && hasSuitBonus(bodyType, newSuitId))
			{
				flag = true;
				if (!NotInitFinish() && mSkillGroupList.ContainsKey(mSetId))
				{
					mSkillGroupList[mSetId].mSkillGroup.UninitFromPlayerSkill();
				}
				mSetElements[(int)bodyType] = newElement;
				mSuitIds[(int)bodyType] = newSuitId;
				mSetId = mSuitIds[0];
				if (!NotInitFinish())
				{
					if (!mSkillGroupList.ContainsKey(mSetId))
					{
						InitSkillGroupToList(mSetId, true);
					}
					mSkillGroupList[mSetId].mSkillGroup.InitToPlayerSkill(playerSkill);
				}
			}
		}
		if ((newElement != mSetElements[(int)bodyType] && !flag) || flag2)
		{
			if (!NotInitFinish() && mSkillGroupList.ContainsKey(mSetId))
			{
				mSkillGroupList[mSetId].mSkillGroup.UninitFromPlayerSkill();
			}
			mSetElements[(int)bodyType] = newElement;
			mSuitIds[(int)bodyType] = newSuitId;
			mSetId = CalculateSetId(mSetElements);
			if (!NotInitFinish())
			{
				if (!mSkillGroupList.ContainsKey(mSetId))
				{
					InitSkillGroupToList(mSetId, false);
				}
				mSkillGroupList[mSetId].mSkillGroup.InitToPlayerSkill(playerSkill);
			}
		}
		mSetElements[(int)bodyType] = newElement;
		mSuitIds[(int)bodyType] = newSuitId;
	}

	public void InitArmorSet(PlayerSkill playerSkill)
	{
		if (!NotInitFinish())
		{
			if (!mSkillGroupList.ContainsKey(mSetId))
			{
				InitSkillGroupToList(mSetId, hasSuitBonus());
			}
			mSkillGroupList[mSetId].mSkillGroup.InitToPlayerSkill(playerSkill);
		}
	}

	public short CalculateSetId(byte[] setElement)
	{
		short num = 0;
		num += (short)(setElement[0] * 1000);
		num += (short)(setElement[1] * 100);
		num += (short)(setElement[2] * 10);
		return (short)(num + setElement[3]);
	}

	private bool NotInitFinish()
	{
		for (int i = 0; i < 4; i++)
		{
			if (mSetElements[i] == 0)
			{
				return true;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (mSuitIds[j] == 0)
			{
				return true;
			}
		}
		return false;
	}

	public byte[] GetElementSets()
	{
		return mSetElements;
	}

	public byte[] GetSuitIds()
	{
		return mSuitIds;
	}

	public void SetElementSets(byte[] setElements, byte[] suitIds)
	{
		for (int i = 0; i < setElements.Length; i++)
		{
			mSetElements[i] = setElements[i];
		}
		for (int j = 0; j < suitIds.Length; j++)
		{
			mSuitIds[j] = suitIds[j];
		}
		if (hasSuitBonus())
		{
			mSetId = mSuitIds[0];
		}
		else
		{
			mSetId = CalculateSetId(mSetElements);
		}
	}

	public string GetName()
	{
		return mSkillGroupList[mSetId].name;
	}

	public string GetDescription()
	{
		return mSkillGroupList[mSetId].des;
	}

	public string GetAttributeForUI()
	{
		string text = string.Empty;
		if (mSkillGroupList.ContainsKey(mSetId))
		{
			SkillGroup mSkillGroup = mSkillGroupList[mSetId].mSkillGroup;
			foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
			{
				if (advancedSkill.GetOnTriggerType() != AdvancedSkill.EOnTriggerType.ALWAYS)
				{
					continue;
				}
				foreach (Skill skill in advancedSkill.GetSkills())
				{
					text += LocalizationManager.GetInstance().GetString(Skill.GetSkillsTypeString(skill.skillType));
					text = ((!(skill.data > 0f)) ? (text + " : ") : (text + " : +"));
					text += skill.data;
					text += "\n";
				}
			}
		}
		return text;
	}

	public string GetName(byte[] elements, byte[] suitIds)
	{
		bool flag = hasSuitBonus(suitIds);
		short num = ((!flag) ? CalculateSetId(elements) : suitIds[0]);
		if (!mSkillGroupList.ContainsKey(num))
		{
			InitSkillGroupToList(num, flag);
		}
		return mSkillGroupList[num].name;
	}

	public string GetDescription(byte[] elements, byte[] suitIds)
	{
		bool flag = hasSuitBonus(suitIds);
		short num = ((!flag) ? CalculateSetId(elements) : suitIds[0]);
		if (!mSkillGroupList.ContainsKey(num))
		{
			InitSkillGroupToList(num, flag);
		}
		return mSkillGroupList[num].des;
	}

	public string GetAttributeForUI(byte[] elements, byte[] suitIds)
	{
		bool flag = hasSuitBonus(suitIds);
		short num = ((!flag) ? CalculateSetId(elements) : suitIds[0]);
		if (!mSkillGroupList.ContainsKey(num))
		{
			InitSkillGroupToList(num, flag);
		}
		string text = string.Empty;
		if (mSkillGroupList.ContainsKey(num))
		{
			SkillGroup mSkillGroup = mSkillGroupList[num].mSkillGroup;
			foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
			{
				if (advancedSkill.GetOnTriggerType() != AdvancedSkill.EOnTriggerType.ALWAYS)
				{
					continue;
				}
				foreach (Skill skill in advancedSkill.GetSkills())
				{
					text += LocalizationManager.GetInstance().GetString(Skill.GetSkillsTypeString(skill.skillType));
					text = ((!(skill.data > 0f)) ? (text + " : ") : (text + " : +"));
					text += skill.data;
					text += "\n";
				}
			}
		}
		return text;
	}

	public bool hasSuitBonus()
	{
		byte b = mSuitIds[0];
		if (b == 0)
		{
			return false;
		}
		for (int i = 0; i < mSuitIds.Length; i++)
		{
			if (b != mSuitIds[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool hasSuitBonus(BodyType bodyType, byte newSuitId)
	{
		byte b = mSuitIds[0];
		if (bodyType == BodyType.Head)
		{
			b = newSuitId;
		}
		if (b == 0)
		{
			return false;
		}
		Debug.Log("here1");
		for (int i = 0; i < mSuitIds.Length; i++)
		{
			if (i == (int)bodyType)
			{
				if (b != newSuitId)
				{
					Debug.Log("here2");
					return false;
				}
			}
			else if (b != mSuitIds[i])
			{
				Debug.Log("here3");
				return false;
			}
		}
		Debug.Log("here4");
		return true;
	}

	public bool hasSuitBonus(byte[] suitIds)
	{
		byte b = suitIds[0];
		if (b == 0)
		{
			return false;
		}
		for (int i = 0; i < suitIds.Length; i++)
		{
			if (b != suitIds[i])
			{
				return false;
			}
		}
		return true;
	}

	public SkillGroup GetSkillGropById(short id, bool isSuitSet)
	{
		if (mSkillGroupList.ContainsKey(id))
		{
			return mSkillGroupList[id].mSkillGroup;
		}
		InitSkillGroupToList(id, isSuitSet);
		if (mSkillGroupList.ContainsKey(id))
		{
			return mSkillGroupList[id].mSkillGroup;
		}
		return null;
	}
}
