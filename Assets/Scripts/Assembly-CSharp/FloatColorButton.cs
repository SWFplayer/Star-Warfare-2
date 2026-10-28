using UnityEngine;

public class FloatColorButton : MonoBehaviour
{
	public UISprite ColorIcon;

	public UISprite Highlight;

	public void SetHighlight(bool isHighlight)
	{
		Highlight.gameObject.SetActive(isHighlight);
	}

	public void SetColor(Color color)
	{
		ColorIcon.color = color;
	}
}
