using UnityEngine;

public class TestScript : MonoBehaviour
{
	private float mStartTime;

	private void Start()
	{
		mStartTime = Time.time;
		string text = "111[n]2222[n]3333";
		text = text.Replace("[n]", "|");
		string[] array = text.Split('|');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			Debug.Log("sss : " + text2);
		}
	}

	private void LateUpdate()
	{
		float y = (Time.time - mStartTime) * 60f;
		base.transform.rotation = Quaternion.Euler(0f, y, 0f);
	}
}
