public class DailyQuest
{
	public enum EDailyQuestType
	{
		NONE = 0,
		BOSS = 1,
		SURVIVAL = 2,
		PVP = 3,
		SPECIAL = 4
	}

	public enum EDailyQuestScoreWithMode
	{
		ID = 0,
		TYPE = 1,
		MODE = 2,
		SCORE = 3,
		LOC = 4
	}

	public byte index;

	public byte type;

	public byte questId;

	public byte valueId;

	public byte weaponId;

	public int value;

	public int valueGot;

	public byte hasDone;

	public byte hasRewardReceived;

	public byte isHeroic;

	public short rewardPoint;

	public DailyQuest(DailyQuestData questData)
	{
		index = questData.index;
		type = questData.type;
		questId = questData.questId;
		valueId = questData.valueId;
		weaponId = questData.weaponId;
		value = questData.value;
		valueGot = questData.valueGot;
		hasDone = questData.hasDone;
		hasRewardReceived = questData.hasRewardReceived;
		isHeroic = questData.isHeroic;
		rewardPoint = questData.rewardPoint;
	}

	public void SetData(DailyQuestData questData)
	{
		index = questData.index;
		type = questData.type;
		questId = questData.questId;
		valueId = questData.valueId;
		weaponId = questData.weaponId;
		value = questData.value;
		valueGot = questData.valueGot;
		hasDone = questData.hasDone;
		hasRewardReceived = questData.hasRewardReceived;
		isHeroic = questData.isHeroic;
		rewardPoint = questData.rewardPoint;
	}

	public string GetDes()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.DAILY_BONUS, DataSheetName.DAILY_QUEST_SCORE_WITH_MODE);
		int i;
		for (i = 0; i < gameDataTable.Count && (type != gameDataTable.GetInt(i, 1) || questId != gameDataTable.GetInt(i, 2)); i++)
		{
		}
		if (i == gameDataTable.Count)
		{
			return string.Empty;
		}
		string key = gameDataTable.GetString(i, 4);
		string text = string.Empty;
		switch (type)
		{
		case 1:
			text = LocalizationManager.GetInstance().GetString(key);
			if (isHeroic == 1)
			{
				text += LocalizationManager.GetInstance().GetString("LOC_DAILYBONUS_HEROIC");
			}
			break;
		case 2:
		{
			string empty2 = string.Empty;
			empty2 = ((weaponId != 0) ? GameApp.GetInstance().GetUserState().GetWeapons()[weaponId].Info : LocalizationManager.GetInstance().GetString("LOC_DAILYBONUS_PVE_RWEAPON"));
			text = LocalizationManager.GetInstance().GetString(key, value, empty2);
			if (isHeroic == 1)
			{
				text += LocalizationManager.GetInstance().GetString("LOC_DAILYBONUS_HEROIC");
			}
			break;
		}
		case 3:
		{
			string empty = string.Empty;
			empty = ((weaponId != 0) ? GameApp.GetInstance().GetUserState().GetWeapons()[weaponId].Info : LocalizationManager.GetInstance().GetString("LOC_DAILYBONUS_PVE_RWEAPON"));
			text = LocalizationManager.GetInstance().GetString(key, value, empty);
			break;
		}
		case 4:
			text = LocalizationManager.GetInstance().GetString(key, value);
			break;
		}
		return text;
	}
}
