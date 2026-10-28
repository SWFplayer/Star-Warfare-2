using UnityEngine;

public class NGUIButtonScript : MonoBehaviour
{
	public ShopUIScript ShopUI;

	public NGUIButtonType ButtonType;

	public int Value = -1;

	private void OnClick()
	{
		switch (ButtonType)
		{
		case NGUIButtonType.PurchaseEquip:
			break;
		case NGUIButtonType.Equip:
			break;
		case NGUIButtonType.Exit:
			Application.LoadLevel("StartMenu");
			break;
		case NGUIButtonType.ChangePage:
			ShopUI.ChangePage(Value);
			break;
		}
	}
}
