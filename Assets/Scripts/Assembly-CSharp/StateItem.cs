using System.Collections.Generic;
using UnityEngine;

public class StateItem : MonoBehaviour
{
	public GameObject ItemTemplete;

	public UIGrid ItemsGrid;

	public void Init()
	{
		while (ItemsGrid.transform.childCount > 0)
		{
			Object.DestroyImmediate(ItemsGrid.transform.GetChild(0).gameObject);
		}
		short[] array = null;
		array = ((GameApp.GetInstance().GetGameMode().ModePlay != Mode.BOSS_FINAL) ? GameApp.GetInstance().GetUserState().GetBattleItemIDs() : GlobalData.GetInstance().ItemArray);
		Dictionary<int, Item> items = GameApp.GetInstance().GetUserState().GetItems();
		if (array != null)
		{
			short[] array2 = array;
			foreach (short key in array2)
			{
				if (items.ContainsKey(key))
				{
					Item item = items[key];
					if (item.GetCount() > 0)
					{
						GameObject gameObject = Object.Instantiate(ItemTemplete) as GameObject;
						gameObject.transform.parent = ItemsGrid.transform;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localRotation = Quaternion.identity;
						gameObject.transform.localScale = Vector3.one;
						ItemButton component = gameObject.GetComponent<ItemButton>();
						component.Init(item.GetItemID(), this);
					}
				}
			}
		}
		Reposition();
	}

	public void Reposition()
	{
		float num = (float)(ItemsGrid.transform.childCount - 1) * ItemsGrid.cellWidth;
		Vector3 localPosition = ItemsGrid.transform.localPosition;
		ItemsGrid.transform.localPosition = new Vector3((0f - num) / 2f, localPosition.y, localPosition.z);
		ItemsGrid.Reposition();
	}

	private void Start()
	{
		Init();
	}

	private void OnClick()
	{
	}

	private void Update()
	{
	}

	public short[] GetRemainedItemArray()
	{
		short[] array = new short[ItemsGrid.transform.childCount];
		for (int i = 0; i < ItemsGrid.transform.childCount; i++)
		{
			ItemButton component = ItemsGrid.transform.GetChild(i).gameObject.GetComponent<ItemButton>();
			if (null != component)
			{
				array[i] = (short)component.ItemID;
			}
		}
		return array;
	}
}
