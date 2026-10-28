using UnityEngine;

public class SelfDestructionObject : MonoBehaviour
{
	public SelfDestructionObjectType type;

	public GameObject effect;

	public void SelfDestroy()
	{
		if (effect != null)
		{
			Object.Instantiate(effect);
		}
		else
		{
			Debug.LogError(base.gameObject.name + "'s effect missed!");
		}
		Object.Destroy(base.gameObject);
	}
}
