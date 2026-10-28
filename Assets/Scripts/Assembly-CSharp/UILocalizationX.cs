using UnityEngine;

public class UILocalizationX : MonoBehaviour
{
	private EUserLanguage mLanguage;

	private string key = string.Empty;

	private string value = string.Empty;

	private void OnEnable()
	{
		if (mLanguage == LocalizationManager.GetInstance().currentLanguage && mLanguage != EUserLanguage.DEFAULT)
		{
			return;
		}
		UILabel component = GetComponent<UILabel>();
		if (!(component != null))
		{
			return;
		}
		if (component.text != key && component.text != value)
		{
			key = component.text;
			string[] array = component.text.Split('@');
			component.text = string.Empty;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text[0] != '#')
				{
					component.text += LocalizationManager.GetInstance().GetString(text);
				}
				else
				{
					component.text += text.Remove(0, 1);
				}
			}
			mLanguage = LocalizationManager.GetInstance().currentLanguage;
			value = component.text;
		}
		else
		{
			component.text = value;
		}
	}

	public void Refresh()
	{
		UILabel component = GetComponent<UILabel>();
		if (!(component != null))
		{
			return;
		}
		if (component.text != key && component.text != value)
		{
			key = component.text;
			string[] array = component.text.Split('@');
			component.text = string.Empty;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text[0] != '#')
				{
					component.text += LocalizationManager.GetInstance().GetString(text);
				}
				else
				{
					component.text += text.Remove(0, 1);
				}
			}
			value = component.text;
		}
		else
		{
			component.text = value;
		}
	}

	public void SetText(string newText)
	{
		UILabel component = GetComponent<UILabel>();
		component.text = newText;
		if (!(component != null))
		{
			return;
		}
		if (component.text != key && component.text != value)
		{
			key = component.text;
			string[] array = component.text.Split('@');
			component.text = string.Empty;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text[0] != '#')
				{
					component.text += LocalizationManager.GetInstance().GetString(text);
				}
				else
				{
					component.text += text.Remove(0, 1);
				}
			}
			value = component.text;
		}
		else
		{
			component.text = value;
		}
	}
}
