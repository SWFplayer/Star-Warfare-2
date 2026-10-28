using System.Collections.Generic;
using UnityEngine;

public class TwinBlockScript : MonoBehaviour
{
	public enum TwinBlockType
	{
		LEFT = 0,
		MIDDLE = 1,
		RIGHT = 2
	}

	public TwinBlockType type;

	public int ID;

	public void Init()
	{
		Dictionary<int, TwinBlockInfo> twinBlockDic = GameApp.GetInstance().GetGameWorld().twinBlockDic;
		if (!twinBlockDic.ContainsKey(ID))
		{
			twinBlockDic.Add(ID, new TwinBlockInfo());
			if (type == TwinBlockType.LEFT)
			{
				twinBlockDic[ID].left = this;
			}
			else if (type == TwinBlockType.MIDDLE)
			{
				twinBlockDic[ID].middle = this;
			}
			else if (type == TwinBlockType.RIGHT)
			{
				twinBlockDic[ID].right = this;
			}
		}
		else if (type == TwinBlockType.LEFT)
		{
			twinBlockDic[ID].left = this;
		}
		else if (type == TwinBlockType.MIDDLE)
		{
			twinBlockDic[ID].middle = this;
		}
		else if (type == TwinBlockType.RIGHT)
		{
			twinBlockDic[ID].right = this;
		}
	}

	private void OnDestroy()
	{
		Dictionary<int, TwinBlockInfo> twinBlockDic = GameApp.GetInstance().GetGameWorld().twinBlockDic;
		if (((type == TwinBlockType.LEFT && twinBlockDic[ID].right == null) || (type == TwinBlockType.RIGHT && twinBlockDic[ID].left == null)) && twinBlockDic[ID].middle != null)
		{
			Object.Destroy(twinBlockDic[ID].middle.gameObject);
		}
	}
}
