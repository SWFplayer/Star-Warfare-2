using UnityEngine;

public class AvatarParamScript : MonoBehaviour
{
	public UILabel ParamName;

	public UILabel ParamValue;

	private bool mNeedShow;

	private void Start()
	{
	}

	public void Set(string paraName, string valueString)
	{
		base.gameObject.SetActive(true);
		ParamName.text = LocalizationManager.GetInstance().GetString(paraName);
		ParamValue.text = valueString;
		mNeedShow = true;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public bool NeedShow()
	{
		return mNeedShow;
	}

	public void Recycle()
	{
		mNeedShow = false;
	}
}
