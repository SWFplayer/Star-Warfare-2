using UnityEngine;

public class VoidExplodeScript : MonoBehaviour
{
	public Vector3 beginPos;

	public Vector3 endPos;

	public float speed;

	private bool startFloat;

	protected float startTime;

	protected float deltaTime;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		if (!startFloat)
		{
			return;
		}
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			base.transform.Translate(speed * (endPos - beginPos).normalized * deltaTime, Space.World);
			deltaTime = 0f;
			if (Vector3.Distance(base.transform.position, endPos) < 0.1f)
			{
				startFloat = false;
			}
		}
	}

	public void StartFloat()
	{
		startFloat = true;
	}
}
