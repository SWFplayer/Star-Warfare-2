using UnityEngine;

public class FaceToPlayerScript : MonoBehaviour
{
	protected Transform cameraTransform;

	protected float lastUpdateTime;

	protected Vector3 Position = default(Vector3);

	public float Radius;

	private void Start()
	{
		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
		Position = base.transform.position;
	}

	private void Update()
	{
		if (!(Time.time - lastUpdateTime < 0.02f))
		{
			lastUpdateTime = Time.time;
			base.transform.LookAt(cameraTransform);
			base.transform.localPosition = (cameraTransform.position - Position).normalized * Radius;
		}
	}
}
