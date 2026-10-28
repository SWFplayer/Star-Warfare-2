using System.Collections.Generic;

public class BigAchievement
{
	public Dictionary<int, int[]> BigAchievementDic;

	private AchievementState Achievements;

	public BigAchievement(AchievementState achievement)
	{
		Achievements = achievement;
		BigAchievementDic = new Dictionary<int, int[]>();
		BigAchievementDic.Add(14, new int[7] { 39, 47, 53, 61, 68, 76, 87 });
		BigAchievementDic.Add(39, new int[6] { 18, 22, 26, 30, 34, 38 });
		BigAchievementDic.Add(47, new int[7] { 40, 41, 42, 43, 44, 45, 46 });
		BigAchievementDic.Add(53, new int[5] { 48, 49, 50, 51, 52 });
		BigAchievementDic.Add(61, new int[7] { 54, 55, 56, 57, 58, 59, 60 });
		BigAchievementDic.Add(68, new int[6] { 62, 63, 64, 65, 66, 67 });
		BigAchievementDic.Add(76, new int[7] { 69, 70, 71, 72, 73, 74, 75 });
		BigAchievementDic.Add(87, new int[10] { 77, 78, 79, 80, 81, 82, 83, 84, 85, 86 });
		BigAchievementDic.Add(97, new int[4] { 93, 94, 95, 96 });
		InitDescription();
	}

	public void InitDescription()
	{
		foreach (int key in BigAchievementDic.Keys)
		{
			if (!IsComplete(key))
			{
				GenerateDescription(key);
			}
		}
	}

	public bool IsComplete(int AchievementID)
	{
		bool result = true;
		int[] array = BigAchievementDic[AchievementID];
		foreach (int num in array)
		{
			if (Achievements.GetAchievements()[num].Available && !Achievements.GetAchievements()[num].complete)
			{
				result = false;
			}
		}
		return result;
	}

	public void GenerateDescription(int AchievementID)
	{
		int num = 0;
		string text = "LOC_ACHIEVEMENT_TIPS_2";
		int[] array = BigAchievementDic[AchievementID];
		foreach (int num2 in array)
		{
			if (Achievements.GetAchievements()[num2].Available && !Achievements.GetAchievements()[num2].complete)
			{
				string text2 = text;
				text = text2 + '@' + Achievements.GetAchievements()[num2].Name + "@#,";
			}
			else if (Achievements.GetAchievements()[num2].Available)
			{
				num++;
			}
		}
		text = text.Substring(0, text.Length - 1);
		text += '.';
		Achievements.GetAchievements()[AchievementID].DisplayDescription = text;
		Achievements.GetAchievements()[AchievementID].CurrentNum = num;
	}
}
