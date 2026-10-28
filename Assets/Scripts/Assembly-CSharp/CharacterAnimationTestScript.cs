using System;
using UnityEngine;

public class CharacterAnimationTestScript : MonoBehaviour
{
	[Serializable]
	public class AnimationStruct
	{
		public string AnimationName;

		public WrapMode AnimationMode = WrapMode.ClampForever;

		public float AnimationTime = 2f;
	}

	public AnimationStruct[] SimpleAnimations;

	private int mIndex;

	private float mLastChangeTime;

	private void Start()
	{
		mIndex = 0;
		mLastChangeTime = Time.time;
	}

	private void Update()
	{
		if (SimpleAnimations.Length == 0)
		{
			return;
		}
		if (SimpleAnimations[mIndex].AnimationMode == WrapMode.Loop)
		{
			base.gameObject.GetComponent<Animation>()[SimpleAnimations[mIndex].AnimationName].wrapMode = WrapMode.Loop;
			base.gameObject.GetComponent<Animation>().CrossFade(SimpleAnimations[mIndex].AnimationName, 0.2f);
			if (Time.time - mLastChangeTime > SimpleAnimations[mIndex].AnimationTime)
			{
				mIndex = (mIndex + 1) % SimpleAnimations.Length;
				mLastChangeTime = Time.time;
			}
		}
		else
		{
			base.gameObject.GetComponent<Animation>()[SimpleAnimations[mIndex].AnimationName].wrapMode = WrapMode.ClampForever;
			base.gameObject.GetComponent<Animation>().CrossFade(SimpleAnimations[mIndex].AnimationName, 0.2f);
			if (base.gameObject.GetComponent<Animation>()[SimpleAnimations[mIndex].AnimationName].time >= base.gameObject.GetComponent<Animation>()[SimpleAnimations[mIndex].AnimationName].clip.length)
			{
				mIndex = (mIndex + 1) % SimpleAnimations.Length;
				mLastChangeTime = Time.time;
			}
		}
	}
}
