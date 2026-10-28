using System.Collections.Generic;

public class BattleItemManager
{
	private Dictionary<int, BattleItem> mBattleItemDictionary;

	private Player mPlayer;

	public void Init(Player player)
	{
		mBattleItemDictionary = new Dictionary<int, BattleItem>();
		mPlayer = player;
	}

	public void Loop()
	{
		foreach (BattleItem value in mBattleItemDictionary.Values)
		{
			value.CheckTimer();
		}
	}

	public void PlayerDeadClear()
	{
		foreach (BattleItem value in mBattleItemDictionary.Values)
		{
			value.TakeOff();
		}
	}

	public void OnItemUsed(int itemId)
	{
		if (!mBattleItemDictionary.ContainsKey(itemId))
		{
			BattleItem battleItem = new BattleItem();
			battleItem.Init(itemId, mPlayer);
			mBattleItemDictionary.Add(itemId, battleItem);
		}
		mBattleItemDictionary[itemId].TakeOn();
	}
}
