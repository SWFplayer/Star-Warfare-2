using System.Collections.Generic;
using UnityEngine;

public class PvpSkillManager
{
	private Dictionary<int, Perk> mPerkDictionary = new Dictionary<int, Perk>();

	private Dictionary<int, KillStreak> mKillStreakDictionary = new Dictionary<int, KillStreak>();

	private Dictionary<int, MutantSkill> mMutantSkillDictionary = new Dictionary<int, MutantSkill>();

	private Dictionary<int, PrestigeSkill> mPrestigeSkillDictionary = new Dictionary<int, PrestigeSkill>();

	public PvpSkillManager()
	{
		LoadConfig();
	}

	public void LoadConfig()
	{
		mPerkDictionary = new Dictionary<int, Perk>();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PERK);
		if (gameDataTable != null)
		{
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				int num = gameDataTable.GetInt(i, 0);
				if (num != 0)
				{
					Perk perk = new Perk();
					perk.LoadConfig(i);
					mPerkDictionary.Add(num, perk);
				}
				else
				{
					Debug.LogError("PerkId is 0, row = " + i);
				}
			}
		}
		mKillStreakDictionary = new Dictionary<int, KillStreak>();
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_KILL_STREAK);
		if (gameDataTable2 != null)
		{
			for (int j = 0; j < gameDataTable2.Count - 1; j++)
			{
				int num2 = gameDataTable2.GetInt(j, 0);
				if (num2 != 0)
				{
					KillStreak killStreak = new KillStreak();
					killStreak.LoadConfig(j);
					mKillStreakDictionary.Add(num2, killStreak);
				}
				else
				{
					Debug.LogError("KillStreakId is 0, row = " + j);
				}
			}
		}
		mMutantSkillDictionary = new Dictionary<int, MutantSkill>();
		GameDataTable gameDataTable3 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		if (gameDataTable3 != null)
		{
			for (int k = 0; k < gameDataTable3.Count - 1; k++)
			{
				int num3 = gameDataTable3.GetInt(k, 0);
				if (num3 != 0)
				{
					MutantSkill mutantSkill = new MutantSkill();
					mutantSkill.LoadConfig(k);
					mMutantSkillDictionary.Add(num3, mutantSkill);
				}
				else
				{
					Debug.LogError("MutantSkillId is 0, row = " + k);
				}
			}
		}
		mPrestigeSkillDictionary = new Dictionary<int, PrestigeSkill>();
		GameDataTable gameDataTable4 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PRESTIGE_SKILL);
		if (gameDataTable4 != null)
		{
			for (int l = 0; l < gameDataTable4.Count - 1; l++)
			{
				PrestigeSkill prestigeSkill = new PrestigeSkill();
				prestigeSkill.LoadConfig(l);
				mPrestigeSkillDictionary.Add(l, prestigeSkill);
			}
		}
	}

	public List<PvpSkill> GetAllPerk()
	{
		List<PvpSkill> list = new List<PvpSkill>();
		foreach (Perk value in mPerkDictionary.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public List<PvpSkill> GetPerkByType(Perk.EPerkType type)
	{
		List<PvpSkill> list = new List<PvpSkill>();
		foreach (Perk value in mPerkDictionary.Values)
		{
			if (value.GetPerkType() == type)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public PvpSkill GetPerkById(int id)
	{
		if (mPerkDictionary.ContainsKey(id))
		{
			return mPerkDictionary[id];
		}
		return null;
	}

	public List<PvpSkill> GetAllKillStreak()
	{
		List<PvpSkill> list = new List<PvpSkill>();
		foreach (KillStreak value in mKillStreakDictionary.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public PvpSkill GetKillStreakById(int id)
	{
		if (mKillStreakDictionary.ContainsKey(id))
		{
			return mKillStreakDictionary[id];
		}
		return null;
	}

	public PvpSkill GetKillStreakByExternalId(int id)
	{
		foreach (KillStreak value in mKillStreakDictionary.Values)
		{
			if (value.GetExternalID() == id)
			{
				return value;
			}
		}
		return null;
	}

	public List<MutantSkill> GetAllMutantSkill()
	{
		List<MutantSkill> list = new List<MutantSkill>();
		foreach (MutantSkill value in mMutantSkillDictionary.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public MutantSkill GetMutantSkillById(int id)
	{
		if (mMutantSkillDictionary.ContainsKey(id))
		{
			return mMutantSkillDictionary[id];
		}
		return null;
	}

	public List<PrestigeSkill> GetAllPrestigeSkill()
	{
		List<PrestigeSkill> list = new List<PrestigeSkill>();
		foreach (PrestigeSkill value in mPrestigeSkillDictionary.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public PrestigeSkill GetPrestigeSkillById(int id)
	{
		if (mPrestigeSkillDictionary.ContainsKey(id))
		{
			return mPrestigeSkillDictionary[id];
		}
		return null;
	}
}
