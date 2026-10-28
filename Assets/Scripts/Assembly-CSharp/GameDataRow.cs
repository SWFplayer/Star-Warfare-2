using System;
using System.Collections.Generic;

[Serializable]
public class GameDataRow
{
	public List<string> DataList = new List<string>();

	public string this[int columnIndex]
	{
		get
		{
			return DataList[columnIndex];
		}
	}

	public int Count
	{
		get
		{
			return DataList.Count;
		}
	}
}
