using UnityEngine;

public class PlaySoundIn : MonoBehaviour
{
	public float InTime;

	public string Sound;

	private bool Active;

	private float mStartTime;

	private void Start()
	{
		mStartTime = Time.time;
		Active = true;
	}

	private void Update()
	{
		if (Active && Time.time - mStartTime > InTime)
		{
			AudioManager.GetInstance().PlaySoundAt(Sound, base.transform.position);
			Active = false;
		}
	}
}
