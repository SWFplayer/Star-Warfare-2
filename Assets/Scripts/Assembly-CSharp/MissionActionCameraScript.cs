using UnityEngine;

public class MissionActionCameraScript : MonoBehaviour
{
	public GameObject mCameraObject;

	public string AnimationName;

	public bool HasAnimation;

	public float TotalTime;

	private bool mIsActive;

	private float mStartTime;

	private Animation mAnimation;

	private void Start()
	{
		mIsActive = false;
		mCameraObject.SetActive(false);
	}

	private void Update()
	{
		if (!mIsActive)
		{
			return;
		}
		if (HasAnimation)
		{
			if (mAnimation[AnimationName].time >= mAnimation[AnimationName].clip.length)
			{
				mIsActive = false;
				mCameraObject.SetActive(false);
				Camera.main.enabled = true;
				HUDBase.instance.Show();
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.InCutscene = false;
			}
		}
		else if (Time.time - mStartTime > TotalTime)
		{
			mIsActive = false;
			mCameraObject.SetActive(false);
			Camera.main.enabled = true;
			HUDBase.instance.Show();
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.InCutscene = false;
		}
	}

	public void StartAction()
	{
		mIsActive = true;
		mCameraObject.SetActive(true);
		mAnimation = mCameraObject.GetComponent<Animation>();
		mAnimation[AnimationName].wrapMode = WrapMode.ClampForever;
		mAnimation.Play(AnimationName);
		mStartTime = Time.time;
		HUDBase.instance.Hide();
		GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.InCutscene = true;
	}
}
