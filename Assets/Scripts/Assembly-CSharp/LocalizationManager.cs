using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
	private static LocalizationManager instance;

	public Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	public Dictionary<string, string> mAtlasPath = new Dictionary<string, string>();

	private EUserLanguage mLanguage;

	public EUserLanguage currentLanguage
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (mLanguage != value || value == EUserLanguage.DEFAULT)
			{
				bool flag = LoadAtlas(value);
				if (LoadLanguage(value) && flag)
				{
					mLanguage = value;
					return;
				}
				string message = ((!flag) ? ("Can't load the Atlas " + value) : ("Can't load the language " + value));
				Debug.LogError(message);
			}
		}
	}

	public static LocalizationManager GetInstance()
	{
		if (instance == null)
		{
			instance = new LocalizationManager();
		}
		return instance;
	}

	public string GetString(string key)
	{
		return GetString(key, 0, 0, 0, string.Empty, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, int num)
	{
		return GetString(key, num, 0, 0, string.Empty, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, string str)
	{
		return GetString(key, 0, 0, 0, str, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, string str1, string str2)
	{
		return GetString(key, 0, 0, 0, string.Empty, str1, str2, string.Empty);
	}

	public string GetString(string key, string str, string str2, string str3)
	{
		return GetString(key, 0, 0, 0, str, str2, str3, string.Empty);
	}

	public string GetString(string key, string str, string str2, string str3, string str4)
	{
		return GetString(key, 0, 0, 0, str, str2, str3, str4);
	}

	public string GetString(string key, int num, int num1)
	{
		return GetString(key, num, num1, 0, string.Empty, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, int num, int num1, int num2)
	{
		return GetString(key, num, num1, num2, string.Empty, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, int num, string str1)
	{
		return GetString(key, num, 0, 0, str1, string.Empty, string.Empty, string.Empty);
	}

	public string GetString(string key, int num, int num1, int num2, string str, string str1, string str2, string str3)
	{
		string value;
		mDictionary.TryGetValue(key, out value);
		if (!string.IsNullOrEmpty(value))
		{
			value = value.Replace("@_NUM0", num.ToString());
			value = value.Replace("@_NUM1", num1.ToString());
			value = value.Replace("@_NUM2", num2.ToString());
			value = value.Replace("@_STR0", str);
			value = value.Replace("@_STR1", str1);
			value = value.Replace("@_STR2", str2);
			value = value.Replace("@_STR3", str3);
			while (value.Contains("@[n]"))
			{
				value = value.Replace("@[n]", "\n");
			}
			return value;
		}
		return key;
	}

	public string GetString(string key, int num, int num1, int num2, string str, string str1, string str2, string str3, Player playerOne, Player playerTwo)
	{
		string text = GetString(key, num, num1, num2, str, str1, str2, str3);
		if (!string.IsNullOrEmpty(text))
		{
			string newValue = string.Empty;
			if (playerOne != null)
			{
				newValue = ModifyTeamNameColor(playerOne.GetName(), playerOne.Team, playerOne.GetSeatID());
			}
			text = text.Replace("@_NAME01", newValue);
			string newValue2 = string.Empty;
			if (playerTwo != null)
			{
				newValue2 = ModifyTeamNameColor(playerTwo.GetName(), playerTwo.Team, playerTwo.GetSeatID());
			}
			return text.Replace("@_NAME02", newValue2);
		}
		return key;
	}

	public string GetErrorCodeString(int errorCodeId)
	{
		string key = "LOC_ERROR_CODE_" + errorCodeId;
		string value;
		mDictionary.TryGetValue(key, out value);
		if (!string.IsNullOrEmpty(value))
		{
			return value;
		}
		return GetString("LOC_ERROR_CODE_DEFAULT", errorCodeId);
	}

	public bool LoadAtlas(EUserLanguage language)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.LOCALIZATION_LIST, DataSheetName.LOCALIZATION_UI);
		bool flag = false;
		switch (language)
		{
		case EUserLanguage.CHINESE_SIMPLIFIED:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Chinese);
			return true;
		case EUserLanguage.ENGLISH:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.English);
			return true;
		case EUserLanguage.FRENCH:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.French);
			return true;
		case EUserLanguage.GERMAN:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Germany);
			return true;
		case EUserLanguage.SPANISH:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Spanish);
			return true;
		case EUserLanguage.CHINESE_TRADITIONAL:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Chinese_T);
			return true;
		case EUserLanguage.KOREAN:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Korean);
			return true;
		case EUserLanguage.JAPANESE:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Japanese);
			return true;
		case EUserLanguage.RUSSIAN:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.Russian);
			return true;
		default:
			mAtlasPath = gameDataTable.ReadDictionary(DataLanguageName.English);
			Debug.Log("The language is not found in the Atlas Table, use English instead.");
			return true;
		}
	}

	public bool LoadLanguage(EUserLanguage language)
	{
		bool flag = false;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.LOCALIZATION_LIST, DataSheetName.LOCALIZATION_ALL);
		switch (language)
		{
		case EUserLanguage.CHINESE_SIMPLIFIED:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Chinese);
			return true;
		case EUserLanguage.ENGLISH:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.English);
			return true;
		case EUserLanguage.FRENCH:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.French);
			return true;
		case EUserLanguage.GERMAN:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Germany);
			return true;
		case EUserLanguage.SPANISH:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Spanish);
			return true;
		case EUserLanguage.KOREAN:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Korean);
			return true;
		case EUserLanguage.CHINESE_TRADITIONAL:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Chinese_T);
			return true;
		case EUserLanguage.JAPANESE:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Japanese);
			return true;
		case EUserLanguage.RUSSIAN:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.Russian);
			return true;
		default:
			mDictionary = gameDataTable.ReadDictionary(DataLanguageName.English);
			Debug.Log("The language is not found in the Localization Table, use English instead.");
			return true;
		}
	}

	public string ModifyTeamNameColor(string name, TeamName teamName, int seatID)
	{
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			return UIConstant.FFA_SEAT_COLOR_PREFIX[seatID] + name + "[-]";
		}
		switch (teamName)
		{
		case TeamName.Red:
			return UIConstant.COLOR_RED_TEAM_STRING + name + "[-]";
		case TeamName.Blue:
			return UIConstant.COLOR_BLUE_TEAM_STRING + name + "[-]";
		default:
			return name;
		}
	}
}
