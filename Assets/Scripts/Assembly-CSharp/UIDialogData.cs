using System.Collections.Generic;

public class UIDialogData
{
	public const string AVATAR_POS_LEFT = "Left";

	public const string AVATAR_POS_RIGHT = "Right";

	private string mAvatarName;

	private string mAvatarPos;

	private string mSpeakerName;

	private List<string> mDialogTextList = new List<string>();

	public string AvatarName
	{
		get
		{
			return mAvatarName;
		}
	}

	public string AvatarPos
	{
		get
		{
			return mAvatarPos;
		}
	}

	public string SpeakerName
	{
		get
		{
			return mSpeakerName;
		}
	}

	public List<string> DialogTextList
	{
		get
		{
			return mDialogTextList;
		}
	}

	public UIDialogData(string avatarName, string avatarPos, string speakerName, List<string> dialogTextList)
	{
		mAvatarName = avatarName;
		mAvatarPos = avatarPos;
		mSpeakerName = speakerName;
		mDialogTextList = dialogTextList;
	}

	public static List<UIDialogData> CreateDialog(string str)
	{
		List<string> list = GetString(str, "[Dialog]", "[/Dialog]");
		List<UIDialogData> list2 = new List<UIDialogData>();
		foreach (string item3 in list)
		{
			string firstString = GetFirstString(item3, "[Character]", "[/Character]");
			string firstString2 = GetFirstString(firstString, "[Avatar]", "[/Avatar]");
			string firstString3 = GetFirstString(firstString, "[AvatarPos]", "[/AvatarPos]");
			string firstString4 = GetFirstString(firstString, "[Name]", "[/Name]");
			firstString3 = ((!firstString3.Equals("Left") && !firstString3.Equals("Right")) ? "Left" : firstString3);
			string firstString5 = GetFirstString(item3, "[Text]", "[/Text]");
			firstString5 = firstString5.Replace("[n]", "|");
			string[] array = firstString5.Split('|');
			List<string> list3 = new List<string>();
			string[] array2 = array;
			foreach (string item in array2)
			{
				list3.Add(item);
			}
			UIDialogData item2 = new UIDialogData(firstString2, firstString3, firstString4, list3);
			list2.Add(item2);
		}
		return list2;
	}

	private static string GetFirstString(string str, string start, string end)
	{
		List<string> list = GetString(str, start, end);
		if (list.Count > 0)
		{
			return list[0];
		}
		return string.Empty;
	}

	private static List<string> GetString(string str, string start, string end)
	{
		List<string> list = new List<string>();
		while (str.IndexOf(start) != -1 && str.IndexOf(end) != -1)
		{
			int num = str.IndexOf(start) + start.Length;
			int length = str.IndexOf(end) - num;
			int startIndex = str.IndexOf(end) + end.Length;
			list.Add(str.Substring(num, length));
			str = str.Substring(startIndex);
		}
		return list;
	}
}
