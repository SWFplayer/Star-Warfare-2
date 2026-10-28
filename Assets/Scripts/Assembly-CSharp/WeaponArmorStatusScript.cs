using UnityEngine;

public class WeaponArmorStatusScript : MonoBehaviour
{
	public UILabel[] Status;

	public void SetStatus(CustomizePlayerStatus.PlayerStatus status, float currentValue, float newValue, bool isInShop)
	{
		if (status >= CustomizePlayerStatus.PlayerStatus.StatusCount)
		{
			return;
		}
		if (status == CustomizePlayerStatus.PlayerStatus.FireRate)
		{
			currentValue = 1f / currentValue;
			newValue = 1f / newValue;
		}
		if (Status[(int)status] != null)
		{
			string compareString = GetCompareString(currentValue, newValue, status == CustomizePlayerStatus.PlayerStatus.FireRate);
			if (currentValue != 0f || newValue != 0f)
			{
				Status[(int)status].text = compareString;
				return;
			}
			if (isInShop)
			{
				Status[(int)status].text = "-";
				return;
			}
			Status[(int)status].transform.parent.gameObject.SetActive(false);
			base.gameObject.GetComponent<UIGrid>().Reposition();
		}
	}

	public void SetPercentStatus(CustomizePlayerStatus.PlayerStatus status, float currentValue, float newValue, bool isInShop)
	{
		if (status >= CustomizePlayerStatus.PlayerStatus.StatusCount)
		{
			return;
		}
		if (status == CustomizePlayerStatus.PlayerStatus.FireRate)
		{
			currentValue = 1f / currentValue;
			newValue = 1f / newValue;
		}
		if (Status[(int)status] != null)
		{
			Status[(int)status].transform.parent.gameObject.SetActive(true);
			base.gameObject.GetComponent<UIGrid>().Reposition();
			string text = GetCompareString(currentValue, newValue, status == CustomizePlayerStatus.PlayerStatus.FireRate);
			if (newValue != 0f)
			{
				text = ((!text.Contains(UIConstant.COLOR_POSTFIX)) ? (text + '%') : text.Insert(text.Length - 3, "%"));
			}
			if (currentValue != 0f || newValue != 0f)
			{
				Status[(int)status].text = text;
				return;
			}
			if (isInShop)
			{
				Status[(int)status].text = "-";
				return;
			}
			Status[(int)status].transform.parent.gameObject.SetActive(false);
			base.gameObject.GetComponent<UIGrid>().Reposition();
		}
	}

	public void ClearStatus()
	{
		UILabel[] status = Status;
		foreach (UILabel uILabel in status)
		{
			if (uILabel != null)
			{
				uILabel.text = "-";
			}
		}
	}

	public string GetCompareString(float currentValue, float newValue, bool needKeepDemical)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = "-";
		if (newValue > currentValue)
		{
			text = UIConstant.EQUIP_COMPARE_GOOD_PREFIX;
			text2 = UIConstant.COLOR_POSTFIX;
			text3 = (newValue - currentValue).ToString();
		}
		else if (newValue < currentValue)
		{
			text = UIConstant.EQUIP_COMPARE_BAD_PREFIX;
			text2 = UIConstant.COLOR_POSTFIX;
			text3 = (newValue - currentValue).ToString();
		}
		if (needKeepDemical)
		{
			return text + newValue.ToString("#0.00") + text2;
		}
		return text + newValue + text2;
	}
}
