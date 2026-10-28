using UnityEngine;

public class VehicleEntity : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerExit(Collider c)
	{
		Debug.Log(c);
		if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log(c);
		}
	}

	private void OnTriggerStay(Collider c)
	{
		Debug.Log(string.Concat(c, "aasdasd"));
		if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log(c);
		}
	}
}
