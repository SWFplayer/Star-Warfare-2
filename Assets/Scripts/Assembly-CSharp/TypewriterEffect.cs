using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 40;

	private UILabel mLabel;

	private string mText;

	private int mOffset;

	private float mNextChar;

	private void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = UIFont.SymbolStyle.None;
			if (mLabel.bitmapFont != null)
			{
				mLabel.bitmapFont.WrapText(mLabel.text, mLabel.fontSize, out mText, mLabel.width, mLabel.height, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
			}
			else if (mLabel.trueTypeFont != null)
			{
				NGUIText.WrapText(mLabel.text, mLabel.trueTypeFont, mLabel.fontSize, FontStyle.Normal, mLabel.width, mLabel.height, mLabel.maxLineCount, false, out mText);
			}
		}
		if (mOffset < mText.Length)
		{
			if (mNextChar <= Time.time)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);
				float num = 1f / (float)charsPerSecond;
				char c = mText[mOffset];
				if (c == '.' || c == '\n' || c == '!' || c == '?')
				{
					num *= 4f;
				}
				mNextChar = Time.time + num;
				mLabel.text = mText.Substring(0, ++mOffset);
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
