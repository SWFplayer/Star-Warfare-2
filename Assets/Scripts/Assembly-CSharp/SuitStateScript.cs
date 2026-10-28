using UnityEngine;

public class SuitStateScript : MonoBehaviour
{
	public UISprite[] Suits;

	public UISprite Highlight;

	public UISprite suitBonus;

	public UISprite bk;

	public GameObject m_Detial_Object;

	public UILabel m_SuitName;

	public UILabel m_SuitDescription;

	public UILabel m_SuitAttributes;

	public UISprite realBk;

	private byte[] mSuitElements = new byte[4];

	private byte[] mSuitIds = new byte[4];

	private int mHighlightIndex = -1;

	private ArmorSet mArmorSet;

	private bool isAutoShow = true;

	private void Awake()
	{
		SetHighlight(-1);
		mArmorSet = GameApp.GetInstance().GetUserState().mCurrentArmorSet;
		for (int i = 0; i < mSuitElements.Length; i++)
		{
			mSuitElements[i] = mArmorSet.GetElementSets()[i];
			Suits[i].spriteName = "suits_" + mSuitElements[i];
		}
		for (int j = 0; j < mSuitIds.Length; j++)
		{
			mSuitIds[j] = mArmorSet.GetSuitIds()[j];
		}
		RefreshSuitBk();
	}

	public void RefreshFromEquiped()
	{
		mArmorSet = GameApp.GetInstance().GetUserState().mCurrentArmorSet;
		if (mArmorSet != null)
		{
			for (int i = 0; i < mSuitElements.Length; i++)
			{
				mSuitElements[i] = mArmorSet.GetElementSets()[i];
				Suits[i].spriteName = "suits_" + mSuitElements[i];
			}
			for (int j = 0; j < mSuitIds.Length; j++)
			{
				mSuitIds[j] = mArmorSet.GetSuitIds()[j];
			}
		}
		for (int k = 0; k < mSuitElements.Length; k++)
		{
			Suits[k].spriteName = "suits_" + mSuitElements[k];
		}
		if (m_Detial_Object.activeSelf)
		{
			RefreshInfo();
		}
		RefreshSuitBk();
	}

	public void Refresh(int bodyType, byte setElement, byte suitId)
	{
		mSuitElements[bodyType] = setElement;
		mSuitIds[bodyType] = suitId;
		Suits[bodyType].spriteName = "suits_" + mSuitElements[bodyType];
		if (m_Detial_Object.activeSelf)
		{
			RefreshTempInfo();
		}
		else
		{
			RefreshSuitBk();
		}
	}

	public void SetHighlight(int index)
	{
		if (!(Highlight == null))
		{
			UISprite[] suits = Suits;
			foreach (UISprite uISprite in suits)
			{
				TweenColor component = uISprite.GetComponent<TweenColor>();
				component.Reset();
				component.enabled = false;
			}
			mHighlightIndex = index;
			if (mHighlightIndex == -1 || mHighlightIndex >= mSuitElements.Length)
			{
				Highlight.gameObject.SetActive(false);
			}
			else
			{
				Highlight.gameObject.SetActive(true);
				Highlight.transform.localPosition = Suits[mHighlightIndex].transform.localPosition;
				TweenColor component2 = Suits[mHighlightIndex].GetComponent<TweenColor>();
				component2.Reset();
				component2.enabled = true;
			}
			if (hasSuitBonus())
			{
				Highlight.gameObject.SetActive(false);
			}
		}
	}

	private void OnClick()
	{
		if (m_Detial_Object.activeSelf)
		{
			m_Detial_Object.SetActive(false);
			if (realBk != null)
			{
				realBk.gameObject.SetActive(false);
			}
			RefreshSuitBk();
		}
		else
		{
			showDetailWithTween();
			if (realBk != null)
			{
				realBk.gameObject.SetActive(true);
			}
			RefreshTempInfo();
		}
		isAutoShow = false;
	}

	public void AutoShowDetail(CustomizeItemType oldType, CustomizeItemType newType)
	{
		if (!isAutoShow)
		{
			return;
		}
		if (isArmor(newType))
		{
			if (m_Detial_Object != null)
			{
				showDetailWithTween();
				if (realBk != null)
				{
					realBk.gameObject.SetActive(true);
				}
				RefreshTempInfo();
			}
		}
		else if (m_Detial_Object != null)
		{
			m_Detial_Object.SetActive(false);
			if (realBk != null)
			{
				realBk.gameObject.SetActive(false);
			}
		}
	}

	public void AutoHideDetail()
	{
		if (isAutoShow && m_Detial_Object != null)
		{
			m_Detial_Object.SetActive(false);
			if (realBk != null)
			{
				realBk.gameObject.SetActive(false);
			}
		}
	}

	public bool isArmor(CustomizeItemType type)
	{
		return type == CustomizeItemType.Arm || type == CustomizeItemType.Body || type == CustomizeItemType.Foot || type == CustomizeItemType.Head;
	}

	private void RefreshInfo()
	{
		if (m_SuitName != null)
		{
			m_SuitName.text = LocalizationManager.GetInstance().GetString(mArmorSet.GetName());
		}
		if (m_SuitDescription != null)
		{
			m_SuitDescription.text = LocalizationManager.GetInstance().GetString(mArmorSet.GetDescription());
		}
		RefreshSuitBk();
	}

	private void RefreshTempInfo()
	{
		if (m_SuitName != null)
		{
			m_SuitName.text = LocalizationManager.GetInstance().GetString(mArmorSet.GetName(mSuitElements, mSuitIds));
		}
		if (m_SuitDescription != null)
		{
			m_SuitDescription.text = LocalizationManager.GetInstance().GetString(mArmorSet.GetDescription(mSuitElements, mSuitIds));
		}
		RefreshSuitBk();
	}

	public void RefreshSuitBk()
	{
		bool flag = hasSuitBonus();
		bk.gameObject.SetActive(!flag);
		for (int i = 0; i < Suits.Length; i++)
		{
			Suits[i].gameObject.SetActive(!flag);
		}
		suitBonus.gameObject.SetActive(flag);
		if (Highlight != null)
		{
			Highlight.gameObject.SetActive(!flag);
			if (mHighlightIndex == -1 || mHighlightIndex >= mSuitElements.Length)
			{
				Highlight.gameObject.SetActive(false);
			}
		}
	}

	public bool hasSuitBonus()
	{
		byte b = mSuitIds[0];
		if (b == 0)
		{
			return false;
		}
		for (int i = 0; i < mSuitIds.Length; i++)
		{
			if (b != mSuitIds[i])
			{
				return false;
			}
		}
		return true;
	}

	public void showDetailWithTween()
	{
		bool activeSelf = m_Detial_Object.activeSelf;
		m_Detial_Object.SetActive(true);
		if (!activeSelf)
		{
			TweenScale component = m_Detial_Object.gameObject.GetComponent<TweenScale>();
			component.Reset();
			component.PlayForward();
			TweenPosition component2 = m_Detial_Object.gameObject.GetComponent<TweenPosition>();
			component2.Reset();
			component2.PlayForward();
		}
	}
}
