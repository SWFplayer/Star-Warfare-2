using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameDataTable
{
	public string TableName;

	public List<GameDataRow> RowList = new List<GameDataRow>();

	public GameDataRow this[int rowIndex]
	{
		get
		{
			return RowList[rowIndex];
		}
	}

	public int Count
	{
		get
		{
			return RowList.Count;
		}
	}

	public int GetInt(int row, int column)
	{
		int result = 0;
		if (row >= RowList.Count)
		{
			Debug.LogError("row index out of range: TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return result;
		}
		if (column >= RowList[row].Count)
		{
			Debug.LogError("column index out of range: TableName = " + TableName + ", columnSize = " + RowList[row].Count + ", column = " + column);
			return result;
		}
		if (!int.TryParse(RowList[row][column], out result))
		{
			Debug.LogError("invalid int data: TableName = " + TableName + ", row = " + row + ", column = " + column + ", data = " + RowList[row][column]);
		}
		return result;
	}

	public short GetShort(int row, int column)
	{
		short result = 0;
		if (row < 0)
		{
			Debug.LogError("row index lower than zero : TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return result;
		}
		if (row >= RowList.Count)
		{
			Debug.LogError("row index out of range: TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return result;
		}
		if (column >= RowList[row].Count)
		{
			Debug.LogError("column index out of range: TableName = " + TableName + ", columnSize = " + RowList[row].Count + ", column = " + column);
			return result;
		}
		if (!short.TryParse(RowList[row][column], out result))
		{
			Debug.LogError("invalid int data: TableName = " + TableName + ", row = " + row + ", column = " + column + ", data = " + RowList[row][column]);
		}
		return result;
	}

	public float GetFloat(int row, int column)
	{
		float result = 0f;
		if (row >= RowList.Count)
		{
			Debug.LogError("row index out of range: TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return result;
		}
		if (column >= RowList[row].Count)
		{
			Debug.LogError("column index out of range: TableName = " + TableName + ", columnSize = " + RowList[row].Count + ", column = " + column);
			return result;
		}
		if (!float.TryParse(RowList[row][column], out result))
		{
			Debug.LogError("invalid float data: TableName = " + TableName + ", row = " + row + ", column = " + column + ", data = " + RowList[row][column]);
		}
		return result;
	}

	public string GetString(int row, int column)
	{
		if (row >= RowList.Count)
		{
			Debug.LogError("row index out of range: TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return null;
		}
		if (column >= RowList[row].Count)
		{
			Debug.LogError("column index out of range: TableName = " + TableName + ", columnSize = " + RowList[row].Count + ", column = " + column);
			return null;
		}
		return RowList[row][column];
	}

	public bool GetBoolean(int row, int column)
	{
		bool result = false;
		if (row >= RowList.Count)
		{
			Debug.LogError("row index out of range: TableName = " + TableName + ", rowSize = " + RowList.Count + ", row = " + row);
			return result;
		}
		if (column >= RowList[row].Count)
		{
			Debug.LogError("column index out of range: TableName = " + TableName + ", columnSize = " + RowList[row].Count + ", column = " + column);
			return result;
		}
		if (!bool.TryParse(RowList[row][column], out result))
		{
			Debug.LogError("invalid boolean data: TableName = " + TableName + ", row = " + row + ", column = " + column + ", data = " + RowList[row][column]);
		}
		return result;
	}

	public Dictionary<string, string> ReadDictionary(string languageName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (RowList[0].DataList.Contains(languageName))
		{
			int num = RowList[0].DataList.FindIndex((string element) => element.Equals(languageName));
			for (int num2 = 0; num2 < RowList.Count; num2++)
			{
				if (string.IsNullOrEmpty(RowList[num2][0]))
				{
					Debug.LogError("LOC ID is invalid: row = " + num2);
				}
				else if (dictionary.ContainsKey(RowList[num2][0]))
				{
					Debug.LogError("Duplicated LOC ID: " + RowList[num2][0]);
				}
				else if (RowList[num2].Count <= num)
				{
					Debug.LogError(RowList[num2][0] + "ע\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdð\ufffdţ\ufffd");
				}
				else if (!string.IsNullOrEmpty(RowList[num2][num]))
				{
					dictionary.Add(RowList[num2][0], RowList[num2][num]);
				}
				else
				{
					dictionary.Add(RowList[num2][0], RowList[num2][1]);
				}
			}
			return dictionary;
		}
		Debug.LogError("The language is not found in the Localization Table: " + languageName);
		int columnIndex = RowList[0].DataList.FindIndex((string element) => element.Equals(DataLanguageName.English));
		for (int num3 = 0; num3 < RowList.Count; num3++)
		{
			if (string.IsNullOrEmpty(RowList[num3][0]))
			{
				Debug.LogError("LOC ID is invalid: row = " + num3);
			}
			else if (dictionary.ContainsKey(RowList[num3][0]))
			{
				Debug.LogError("Duplicated LOC ID: " + RowList[num3][0]);
			}
			else if (!string.IsNullOrEmpty(RowList[num3][columnIndex]))
			{
				dictionary.Add(RowList[num3][0], RowList[num3][columnIndex]);
			}
			else
			{
				dictionary.Add(RowList[num3][0], RowList[num3][1]);
			}
		}
		return dictionary;
	}
}
