using UnityEngine;

public class SwAnnouncement
{
	private int id;

	private string announcement;

	public void ReadData(BytesBuffer bb)
	{
		id = bb.ReadInt();
		announcement = bb.ReadStringShortLength();
		announcement = announcement.Replace("\\n", "\n");
		Debug.Log(announcement);
	}

	public int GetId()
	{
		return id;
	}

	public string GetAnnouncement()
	{
		return announcement;
	}
}
