using UnityEngine;

public class ScaleFullScreenScript : MonoBehaviour
{
	private void Start()
	{
		base.transform.localScale = new Vector3(Screen.width, Screen.height, base.transform.localScale.z);
	}
}
