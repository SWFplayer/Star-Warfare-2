using UnityEngine;

public class RotationController : MonoBehaviour
{
	public float speedX = 20f;

	public float speedY = 30f;

	public float speedZ = 40f;

	private Transform tr;

	private void Awake()
	{
		tr = GetComponent<Transform>();
	}

	private void Update()
	{
		tr.Rotate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime);
	}
}
