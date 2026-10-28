using UnityEngine;

public class ScaleSpriteByLabel : MonoBehaviour
{
	public UILabel Label;

	public UISprite TargetSprite;

	public int WidthOffset = 20;

	private void Awake()
	{
		if (Label == null)
		{
			Label = GetComponent<UILabel>();
		}
		if (TargetSprite == null)
		{
			TargetSprite = GetComponentInChildren<UISprite>();
		}
	}

	private void Start()
	{
		RefreshScale();
	}

	public void RefreshScale()
	{
		if (Label != null && TargetSprite != null)
		{
			TargetSprite.width = Label.width + WidthOffset;
			TargetSprite.height = Label.height + WidthOffset;
			if (Label.pivot == UIWidget.Pivot.Left)
			{
				TargetSprite.transform.localPosition += new Vector3(Label.width / 2, 0f, 0f);
			}
			else if (Label.pivot == UIWidget.Pivot.Right)
			{
				TargetSprite.transform.localPosition -= new Vector3(Label.width / 2, 0f, 0f);
			}
		}
	}
}
