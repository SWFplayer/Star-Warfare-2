using UnityEngine;

public class SecondFontForSpecialLanguage : MonoBehaviour
{
	public EUserLanguage Language;

	public Font SecondFont;

	private UILabel mSelfLabel;

	private void Awake()
	{
		mSelfLabel = GetComponent<UILabel>();
		if (mSelfLabel != null && LocalizationManager.GetInstance().currentLanguage == Language)
		{
			mSelfLabel.trueTypeFont = SecondFont;
		}
	}
}
