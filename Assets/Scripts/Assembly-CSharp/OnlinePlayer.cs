using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayer
{
	protected int mExp;

	protected byte mPrestige;

	protected OnlineCustomizeData[] mCustomizeDataArray;

	public int GetExp()
	{
		return mExp;
	}

	public void SetExp(int exp)
	{
		mExp = exp;
	}

	public void AddExp(int value)
	{
		mExp += value;
		UserState userState = GameApp.GetInstance().GetUserState();
		List<Rank> rankList = userState.GetRankList();
		mExp = Mathf.Min(mExp, rankList[rankList.Count - 1].exp);
	}

	public byte GetPrestige()
	{
		return mPrestige;
	}

	public void SetPrestige(byte prestige)
	{
		mPrestige = prestige;
	}

	public int GetOnlineCustomizeSize()
	{
		return mCustomizeDataArray.Length;
	}

	public OnlineCustomizeData GetOnlineCustomizeById(int id)
	{
		if (id < mCustomizeDataArray.Length)
		{
			return mCustomizeDataArray[id];
		}
		return null;
	}

	public void SetOnlineCustomizeArray(OnlineCustomizeData[] array)
	{
		mCustomizeDataArray = array;
	}

	public void SetOnlineCustomizeArray(int id, OnlineCustomizeData data)
	{
		mCustomizeDataArray[id] = new OnlineCustomizeData(data);
	}
}
