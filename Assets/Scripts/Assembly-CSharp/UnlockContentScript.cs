using UnityEngine;

public class UnlockContentScript : MonoBehaviour
{
	public UISprite Icon;

	public UILabel Type;

	public UILabel Name;

	public void Init(string iconString, string typeString, string nameString)
	{
		Icon.spriteName = iconString;
		Type.text = typeString;
		Name.text = nameString;
	}
}
