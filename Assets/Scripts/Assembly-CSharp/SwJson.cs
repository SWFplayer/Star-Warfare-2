using System.Collections.Generic;
using UnityEngine;

public class SwJson
{
	public static List<string> ToJsonArray(string str)
	{
		List<string> list = new List<string>();
		int length = str.Length;
		length -= 2;
		length = Mathf.Clamp(length, 0, length);
		str = str.Substring(1, length);
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			if (str.Substring(i, 1).Equals(","))
			{
				if (num == i)
				{
					list.Add("0");
					num = i + 1;
				}
				else
				{
					string item = str.Substring(num, i - num);
					list.Add(item);
					num = i + 1;
				}
			}
		}
		string item2 = str.Substring(num, length - num);
		list.Add(item2);
		return list;
	}

	public static string ToJson<T>(IList<T> jsonObjectList)
	{
		try
		{
			string text = "[";
			for (int i = 0; i < jsonObjectList.Count; i++)
			{
				object obj = jsonObjectList[i];
				if (obj != null)
				{
					text = text + obj.ToString() + ",";
				}
			}
			return text.ToString().TrimEnd(',') + "]";
		}
		catch
		{
		}
		return "{}";
	}
}
