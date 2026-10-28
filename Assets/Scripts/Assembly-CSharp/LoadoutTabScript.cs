using UnityEngine;

public class LoadoutTabScript : MonoBehaviour
{
	public UISprite Background;

	public UISprite Frame;

	public UILabel LabelMode;

	public UILabel LabelNumber;

	private void Start()
	{
	}

	public void OnSelected()
	{
		Background.alpha = 1f;
		LabelMode.alpha = 1f;
		LabelNumber.alpha = 1f;
		Frame.gameObject.SetActive(true);
	}

	public void OnDeselected()
	{
		Background.alpha = 0.5f;
		LabelMode.alpha = 0.5f;
		LabelNumber.alpha = 0.5f;
		Frame.gameObject.SetActive(false);
	}
}
