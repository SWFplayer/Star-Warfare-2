using System.Collections.Generic;

public class PlayerPVEStatistics
{
	public Dictionary<EnemyType, KillEnemyInfo> KillEnemyInfos = new Dictionary<EnemyType, KillEnemyInfo>();

	public void Add(EnemyType type, bool isElite)
	{
		if (!KillEnemyInfos.ContainsKey(type))
		{
			KillEnemyInfo value = new KillEnemyInfo(type);
			KillEnemyInfos.Add(type, value);
		}
		KillEnemyInfos[type].AddKill(isElite);
	}

	public void Reset()
	{
		foreach (KillEnemyInfo value in KillEnemyInfos.Values)
		{
			value.Reset();
		}
	}

	public List<KillEnemyInfo> GetSortList()
	{
		List<KillEnemyInfo> list = new List<KillEnemyInfo>();
		foreach (KillEnemyInfo value in KillEnemyInfos.Values)
		{
			Sort(value, list);
		}
		return list;
	}

	private bool Sort(KillEnemyInfo info, List<KillEnemyInfo> list)
	{
		if (list.Count == 0)
		{
			list.Add(info);
		}
		else
		{
			int num = 0;
			int num2 = list.Count - 1;
			int num3 = (num + num2) / 2;
			if (info.mKillCount <= list[num].mKillCount)
			{
				list.Insert(num, info);
			}
			else if (info.mKillCount >= list[num2].mKillCount)
			{
				list.Insert(num2 + 1, info);
			}
			else
			{
				while (num2 - num > 1)
				{
					float num4 = list[num3].mKillCount;
					if ((float)info.mKillCount == num4)
					{
						num = num3;
						break;
					}
					if ((float)info.mKillCount > num4)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
					num3 = (num + num2) / 2;
				}
				list.Insert(num + 1, info);
			}
		}
		return true;
	}
}
