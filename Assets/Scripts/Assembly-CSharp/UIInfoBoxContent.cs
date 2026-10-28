using UnityEngine;

public class UIInfoBoxContent : MonoBehaviour
{
	public UIInfoBox mUIInfoBox;

	public UILabel mUILabel1;

	public UILabel mUILabel2;

	public UISprite mWeaponIcon;

	public UISprite mOtherIcon;

	private void Awake()
	{
		if (mUIInfoBox == null)
		{
			Object.Destroy(this);
		}
	}

	public void SetInfo(string str1, string str2)
	{
		SetInfo(str1, str2, string.Empty);
	}

	public void SetInfo(string str1, string str2, string iconString)
	{
		mUILabel1.text = str1;
		mUILabel2.text = str2;
		if (!(mWeaponIcon != null))
		{
			return;
		}
		if (mWeaponIcon.atlas.GetSprite(iconString) != null)
		{
			if (mOtherIcon != null)
			{
				mOtherIcon.gameObject.SetActive(false);
			}
			mWeaponIcon.gameObject.SetActive(true);
			mWeaponIcon.spriteName = iconString;
		}
		else if (mOtherIcon != null)
		{
			mWeaponIcon.gameObject.SetActive(false);
			mOtherIcon.gameObject.SetActive(true);
			mOtherIcon.spriteName = iconString;
		}
	}
}
