using UnityEngine;

public class SwVoidEffectScript : MonoBehaviour
{
	public float StartAdsorbDelay = 1.5f;

	public float Accelerate = 0.5f;

	private float mStartTime;

	private Player mPlayer;

	private void Start()
	{
		mStartTime = Time.time;
		mPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
	}

	private void Update()
	{
		if (mPlayer == null)
		{
			mPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		else if (Time.time - mStartTime >= StartAdsorbDelay)
		{
			Vector3 vector = mPlayer.GetPosition() - Vector3.up * 2f;
			Vector3 vector2 = vector - base.transform.position;
			base.transform.position = base.transform.position + vector2 * 0.5f * Accelerate * Mathf.Pow(Time.time - mStartTime - StartAdsorbDelay, 2f);
			if (Vector3.Distance(base.transform.position, vector) < 0.1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
