using UnityEngine;

public class RoomSetTemplate : MonoBehaviour
{
	public UISprite Highlight;

	public UILabel Value;

	private int mValue;

	private bool mIsHighlight;

	private string mHighlightValueColor = "[4dff2b]";

	public void Init(int value)
	{
		mValue = value;
		mIsHighlight = false;
		Value.text = mValue.ToString();
		Highlight.gameObject.SetActive(false);
	}

	public void SetHighlight(bool isHighlight)
	{
		mIsHighlight = isHighlight;
		Highlight.gameObject.SetActive(isHighlight);
		if (isHighlight)
		{
			Value.text = mHighlightValueColor + mValue + "[-]";
		}
		else
		{
			Value.text = mValue.ToString();
		}
	}

	public bool IsHighlight()
	{
		return mIsHighlight;
	}

	public int GetValue()
	{
		return mValue;
	}
}
