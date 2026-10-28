using UnityEngine;

public class IndicationEffect : MonoBehaviour
{
	public Camera TargetCamera;

	protected Vector3 mTargetPositionInScene;

	public string mKey = string.Empty;

	private float startTime;

	public void Init(string key, Vector3 position)
	{
		mKey = key;
		mTargetPositionInScene = position + new Vector3(0f, 2f, 0f);
		startTime = Time.time;
		Update();
	}

	private void Update()
	{
		float num = Vector3.Distance(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition(), mTargetPositionInScene);
		Vector3 position = Camera.main.WorldToViewportPoint(mTargetPositionInScene);
		position.x = Mathf.Clamp(position.x, 0f, 1f);
		position.y = Mathf.Clamp(position.y, 0f, 1f);
		position.z = Mathf.Clamp(position.z, 0f, 1f);
		base.transform.position = TargetCamera.ViewportToWorldPoint(position);
		base.transform.Translate(0f, Mathf.Sin((Time.time - startTime) * 3f) / 50f, 0f, base.transform);
		if (num > 1f)
		{
			base.transform.localScale = Vector3.one / num * 8f;
		}
		else
		{
			base.transform.localScale = Vector3.one;
		}
	}

	public void SelfDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
