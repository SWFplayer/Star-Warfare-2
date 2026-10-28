using System.Collections.Generic;
using UnityEngine;

public class SwVoidMaterialInfo : MonoBehaviour
{
	public enum EClickType
	{
		SWVOID_MENU = 0,
		CHEST_INFO = 1,
		IAP_WISH = 2,
		IAP_RANDOM = 3,
		SWVOID_MENU_IN_BAG = 4
	}

	public UILabel info;

	public UISprite Icon;

	public UISprite Rarity;

	public short materialId;

	public bool isShowInfo;

	public GameObject selected;

	public GameObject mask;

	public UISprite newTag;

	public EClickType m_ClickType;

	private List<string> randomIcons;

	private Timer updateTimer;

	private byte index;

	public void OnClick()
	{
		switch (m_ClickType)
		{
		case EClickType.SWVOID_MENU:
			if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.gameObject.activeSelf)
			{
				if (!isShowInfo)
				{
					SwVoidMenuScript.mInstance.ShowSwVoidMaterialInfo(materialId, this);
					selected.SetActive(true);
					isShowInfo = true;
				}
				else
				{
					selected.SetActive(false);
					SwVoidMenuScript.mInstance.ClearSwVoidMaterialInfo();
				}
			}
			break;
		case EClickType.IAP_WISH:
		{
			bool activeSelf3 = mask.activeSelf;
			bool activeSelf4 = selected.activeSelf;
			if (IapScript.mInstance.lastSelected != null && IapScript.mInstance.lastSelected != this)
			{
				IapScript.mInstance.lastSelected.selected.SetActive(false);
			}
			IapScript.mInstance.lastSelected = this;
			if (!activeSelf4 && activeSelf3)
			{
				selected.SetActive(true);
				selected.GetComponent<UISprite>().alpha = 1f;
				TweenAlpha.Begin(selected, 0.5f, 0f);
			}
			else
			{
				selected.SetActive(false);
			}
			if (activeSelf3)
			{
				mask.SetActive(false);
				GameApp.GetInstance().GetUserState().SetMaterialWish(materialId, true);
				IapScript.mInstance.RecheckWishIcon(materialId);
			}
			else
			{
				mask.SetActive(true);
				GameApp.GetInstance().GetUserState().SetMaterialWish(materialId, false);
				IapScript.mInstance.RecheckWishIcon(materialId);
			}
			break;
		}
		case EClickType.IAP_RANDOM:
		{
			bool activeSelf = mask.activeSelf;
			bool activeSelf2 = selected.activeSelf;
			if (IapScript.mInstance.lastSelected != null && IapScript.mInstance.lastSelected != this)
			{
				IapScript.mInstance.lastSelected.selected.SetActive(false);
			}
			IapScript.mInstance.lastSelected = this;
			if (!activeSelf2)
			{
				selected.SetActive(true);
				selected.GetComponent<UISprite>().alpha = 1f;
				TweenAlpha.Begin(selected, 0.5f, 0f);
			}
			if (activeSelf)
			{
				mask.SetActive(false);
				short[] materialIdsInGroup = SwVoidDataManager.GetMaterialIdsInGroup(materialId);
				foreach (short num in materialIdsInGroup)
				{
					GameApp.GetInstance().GetUserState().SetMaterialWish(num, false);
				}
				IapScript.mInstance.RecheckWishIcon(materialId, true);
			}
			break;
		}
		case EClickType.SWVOID_MENU_IN_BAG:
			if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.gameObject.activeSelf)
			{
				if (!isShowInfo)
				{
					SwVoidMenuScript.mInstance.ShowSwVoidMaterialInfoInAllMaterial(materialId, this);
					selected.SetActive(true);
					isShowInfo = true;
				}
				else
				{
					selected.SetActive(false);
					SwVoidMenuScript.mInstance.ClearSwVoidMaterialInfoInAllMaterial();
				}
			}
			break;
		case EClickType.CHEST_INFO:
			break;
		}
	}

	public void CheckWish()
	{
		switch (m_ClickType)
		{
		case EClickType.IAP_WISH:
			if (GameApp.GetInstance().GetUserState().GetMaterialWish(materialId))
			{
				mask.SetActive(false);
			}
			else
			{
				mask.SetActive(true);
			}
			break;
		case EClickType.IAP_RANDOM:
			if (SwVoidDataManager.IsAllOrNoWishInGroup(materialId))
			{
				mask.SetActive(false);
			}
			else
			{
				mask.SetActive(true);
			}
			break;
		}
	}

	public void SetMask(bool isSelected)
	{
		switch (m_ClickType)
		{
		case EClickType.IAP_WISH:
			mask.SetActive(isSelected);
			if (isSelected)
			{
				selected.SetActive(false);
			}
			break;
		case EClickType.IAP_RANDOM:
			mask.SetActive(isSelected);
			break;
		}
	}

	public void SetRandomIapIcon()
	{
		if (m_ClickType == EClickType.IAP_RANDOM)
		{
			switch (materialId)
			{
			case 0:
				Icon.spriteName = "w1";
				break;
			case 1:
				Icon.spriteName = "w2";
				break;
			case 2:
				Icon.spriteName = "w1";
				break;
			case 3:
				Icon.spriteName = "w3";
				break;
			}
		}
	}

	public void SetRandomIapIconList()
	{
		if (m_ClickType == EClickType.IAP_RANDOM)
		{
			randomIcons = new List<string>();
			short[] materialIdsInGroup = SwVoidDataManager.GetMaterialIdsInGroup(materialId);
			foreach (short num in materialIdsInGroup)
			{
				randomIcons.Add(SwVoidDataManager.GetMaterialIconName(num));
			}
			updateTimer = new Timer(0.1f, true);
		}
	}

	private void Update()
	{
		if (m_ClickType != EClickType.IAP_RANDOM || updateTimer == null || !updateTimer.Ready())
		{
			return;
		}
		updateTimer.Do();
		if (!mask.activeSelf)
		{
			if (randomIcons != null && randomIcons.Count > 0)
			{
				index++;
				if (index >= randomIcons.Count)
				{
					index = 0;
				}
				Icon.spriteName = randomIcons[index];
			}
		}
		else
		{
			SetRandomIapIcon();
		}
	}
}
