using UnityEngine;

public class SimpleAnimationTestScript : MonoBehaviour
{
	public string[] AnimationNames;

	public float AnimationTime = 2f;

	private int mIndex;

	private float mLastChangeTime;

	private void Start()
	{
		mIndex = 0;
		mLastChangeTime = Time.time;
	}

	private void Update()
	{
		if (AnimationNames.Length == 0)
		{
			return;
		}
		if (Time.time - mLastChangeTime > AnimationTime)
		{
			mIndex = (mIndex + 1) % AnimationNames.Length;
			mLastChangeTime = Time.time;
			if (base.gameObject.GetComponent<Animation>()[AnimationNames[mIndex]] == null)
			{
				Debug.LogWarning("Invalid animation name: " + AnimationNames[mIndex]);
			}
		}
		if (!(base.gameObject.GetComponent<Animation>()[AnimationNames[mIndex]] == null))
		{
			base.gameObject.GetComponent<Animation>()[AnimationNames[mIndex]].wrapMode = WrapMode.Loop;
			base.gameObject.GetComponent<Animation>().CrossFade(AnimationNames[mIndex], 0.2f);
		}
	}
}
