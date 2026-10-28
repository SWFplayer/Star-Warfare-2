using System;
using UnityEngine;

public class Item
{
	public enum EItemColumn
	{
		ItemID = 0,
		ItemType = 1,
		ItemSkillId = 2,
		FINAL_DAMAGE = 3,
		FINAL_RES = 4,
		MONEY_BOOTH = 5,
		EXP_BOOTH = 6,
		ItemIcon = 7
	}

	public enum ItemType
	{
		Flask = 1,
		Potion = 2
	}

	protected string mName;

	protected string mDescription;

	protected string mBuffDesc;

	protected int mItemID;

	protected ItemType mItemType;

	protected SkillsType mFlaskSkillsType;

	protected int mCount;

	protected int mEquipCount;

	protected int mExternalId;

	protected string mIconName;

	protected SkillGroup mSkillGroup = new SkillGroup();

	public void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.ITEMS);
		if (gameDataTable != null)
		{
			mItemID = gameDataTable.GetInt(row, 0);
			mSkillGroup.source = "Item " + mItemID;
			mItemType = (ItemType)gameDataTable.GetInt(row, 1);
			mIconName = gameDataTable.GetString(row, 7);
			mEquipCount = 1;
			if (gameDataTable.GetString(row, 2) != string.Empty)
			{
				mExternalId = gameDataTable.GetInt(row, 2);
				if (mExternalId != 0)
				{
					mSkillGroup.InitSkillInfo(mExternalId);
				}
			}
			for (EItemColumn eItemColumn = EItemColumn.FINAL_DAMAGE; eItemColumn <= EItemColumn.EXP_BOOTH; eItemColumn++)
			{
				if (!(gameDataTable.GetString(row, (int)eItemColumn) != string.Empty))
				{
					continue;
				}
				float num = gameDataTable.GetFloat(row, (int)eItemColumn);
				if (num != 0f)
				{
					SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), eItemColumn.ToString());
					mSkillGroup.AddStaticSkill(type, num);
					if (mItemType == ItemType.Flask)
					{
						mFlaskSkillsType = type;
					}
				}
			}
		}
		gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.SHOP_ITEMS);
		if (gameDataTable == null)
		{
			return;
		}
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			int num2 = gameDataTable.GetInt(i, 2);
			if (num2 == mItemID)
			{
				mName = gameDataTable.GetString(i, 1);
				mDescription = gameDataTable.GetString(i, 7);
				if (mItemType == ItemType.Flask)
				{
					mBuffDesc = gameDataTable.GetString(i, 8);
				}
			}
		}
	}

	public void TakeOn(PlayerSkill playerSkill)
	{
		mSkillGroup.InitToPlayerSkill(playerSkill);
	}

	public void TakeOff()
	{
		mSkillGroup.UninitFromPlayerSkill();
	}

	public int GetItemID()
	{
		return mItemID;
	}

	public ItemType GetItemType()
	{
		return mItemType;
	}

	public SkillsType GetFlaskSkillsType()
	{
		return mFlaskSkillsType;
	}

	public int GetCount()
	{
		return mCount;
	}

	public void SetCount(int count)
	{
		mCount = count;
	}

	public void AddCount(int value)
	{
		mCount += value;
	}

	public void Use(int value)
	{
		mCount = Mathf.Max(0, mCount - value);
	}

	public int GetExternalId()
	{
		return mExternalId;
	}

	public string GetIconName()
	{
		return mIconName;
	}

	public int GetEquipCount()
	{
		return mEquipCount;
	}

	public void OnUsed(bool isCopy)
	{
		if (!isCopy)
		{
			if (mCount <= 0)
			{
				return;
			}
			mCount--;
			{
				foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
				{
					advancedSkill.Active();
				}
				return;
			}
		}
		foreach (AdvancedSkill advancedSkill2 in mSkillGroup.GetAdvancedSkills())
		{
			advancedSkill2.Active();
		}
	}

	public bool IsHpRestore()
	{
		foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
		{
			if (advancedSkill.IsHpRestore())
			{
				return true;
			}
		}
		return false;
	}

	public float GetEffectTime()
	{
		foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
		{
			if (advancedSkill.GetOffTriggerType() == AdvancedSkill.EOffTriggerType.WAIT_TIME)
			{
				return advancedSkill.GetOffTriggerTime();
			}
		}
		return 0f;
	}

	public string GetItemName()
	{
		if (mItemType == ItemType.Potion)
		{
			return LocalizationManager.GetInstance().GetString(mName);
		}
		return string.Empty;
	}

	public string GetItemDescription()
	{
		if (mItemType == ItemType.Potion)
		{
			return LocalizationManager.GetInstance().GetString(mDescription);
		}
		return string.Empty;
	}

	public string GetBuffDesc()
	{
		if (mItemType == ItemType.Flask)
		{
			return LocalizationManager.GetInstance().GetString(mBuffDesc);
		}
		return string.Empty;
	}

	public string GetFlaskItemName()
	{
		return LocalizationManager.GetInstance().GetString(mName);
	}
}
