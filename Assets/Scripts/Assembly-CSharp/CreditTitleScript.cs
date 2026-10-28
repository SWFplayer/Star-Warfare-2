using UnityEngine;

public class CreditTitleScript : MonoBehaviour
{
	public UISprite EffectSprite;

	public Transform AnchorBottom;

	public Transform AnchorTop;

	private bool mCanPlayEffect;

	private void Update()
	{
		if (base.transform.position.y < AnchorBottom.position.y)
		{
			if (EffectSprite.transform.localPosition.x < 0f)
			{
				EffectSprite.transform.localPosition = new Vector3(1024f, 4.7f, 0f);
			}
			mCanPlayEffect = false;
		}
		else if (AnchorBottom.position.y <= base.transform.position.y && base.transform.position.y <= AnchorTop.position.y)
		{
			if (!mCanPlayEffect)
			{
				TweenPosition.Begin(EffectSprite.gameObject, 0.15f, new Vector3(0f, 4.7f, 0f));
			}
			mCanPlayEffect = true;
		}
		else if (base.transform.position.y > AnchorTop.position.y)
		{
			if (mCanPlayEffect)
			{
				TweenPosition.Begin(EffectSprite.gameObject, 0.15f, new Vector3(-1024f, 4.7f, 0f));
			}
			mCanPlayEffect = false;
		}
	}
}
