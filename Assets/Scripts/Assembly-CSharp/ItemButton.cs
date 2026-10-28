using UnityEngine;

public class ItemButton : MonoBehaviour
{
	private int m_ItemID;

	public UISprite Icon;

	private StateItem m_StateItem;

	public int ItemID
	{
		get
		{
			return m_ItemID;
		}
	}

	public void Init(int id, StateItem stateItem)
	{
		m_ItemID = id;
		m_StateItem = stateItem;
		Icon.spriteName = GameApp.GetInstance().GetUserState().GetItems()[m_ItemID].GetIconName();
		Icon.MakePixelPerfect();
	}

	public void OnClick()
	{
		LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer != null && localPlayer.InPlayingState())
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerUseItemRequest request = new PlayerUseItemRequest(m_ItemID, false);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				localPlayer.UseItem(m_ItemID, false);
			}
			Object.DestroyImmediate(base.gameObject);
			HUDBase.instance.ItemBarReposition();
		}
	}
}
