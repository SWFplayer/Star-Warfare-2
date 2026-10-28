using UnityEngine;

public class UISpriteBundle : MonoBehaviour
{
	public string key = string.Empty;

	public UISprite sprite;

	private void Awake()
	{
		if (sprite == null)
		{
			sprite = GetComponent<UISprite>();
			if (sprite == null)
			{
				Object.Destroy(this);
			}
		}
	}
}
