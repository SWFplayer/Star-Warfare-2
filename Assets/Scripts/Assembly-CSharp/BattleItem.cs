using UnityEngine;

public class BattleItem
{
	private int mItemId;

	private Player mPlayer;

	private GameObject effectObj;

	private Timer effectTimer;

	public void Init(int itemId, Player player)
	{
		mItemId = itemId;
		mPlayer = player;
	}

	public void TakeOff()
	{
		if (effectObj != null)
		{
			effectObj.SetActive(false);
		}
	}

	public void TakeOn()
	{
		if (effectObj == null)
		{
			InitEffect();
		}
		if (effectObj != null)
		{
			effectObj.SetActive(true);
			effectTimer.Do();
		}
	}

	public void CheckTimer()
	{
		if (effectObj != null && effectTimer != null && effectTimer.Ready())
		{
			effectObj.SetActive(false);
		}
	}

	private void InitEffect()
	{
		if (mPlayer != null && !mPlayer.IsBoss)
		{
			GameObject gameObject = null;
			switch (mItemId)
			{
			case 10:
			case 11:
			case 12:
			case 13:
			case 21:
				gameObject = Resources.Load("SW2_Effect/HYPER CLIP") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			case 14:
				gameObject = Resources.Load("SW2_Effect/BULLET SHIELD") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 15:
				gameObject = Resources.Load("SW2_Effect/ENERGY SHIELD") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 16:
				gameObject = Resources.Load("SW2_Effect/Explosive Shield") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 17:
				gameObject = Resources.Load("SW2_Effect/Melee Shield") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 18:
				gameObject = Resources.Load("SW2_Effect/Wizard") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 20:
				gameObject = Resources.Load("SW2_Effect/Rush") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 22:
				gameObject = Resources.Load("SW2_Effect/Skill_5") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 24:
				gameObject = Resources.Load("SW2_Effect/Rage") as GameObject;
				effectObj = Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			}
			if (effectObj != null && effectObj.activeSelf)
			{
				effectObj.SetActive(false);
				SetEffectTimer();
			}
		}
	}

	private void SetEffectTimer()
	{
		if (effectObj != null)
		{
			effectTimer = new Timer();
			effectTimer.SetTimer(GameApp.GetInstance().GetUserState().GetItems()[mItemId].GetEffectTime(), false);
		}
	}
}
