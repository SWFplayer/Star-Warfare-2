using UnityEngine;

public class CreateObjectTestScript : MonoBehaviour
{
	public GameObject Prefab;

	private void Start()
	{
		if (Prefab != null)
		{
			Object.Instantiate(Prefab, base.gameObject.transform.position, base.gameObject.transform.rotation);
		}
	}
}
