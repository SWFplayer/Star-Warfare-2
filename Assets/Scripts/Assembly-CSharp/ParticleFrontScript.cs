using UnityEngine;

[ExecuteInEditMode]
public class ParticleFrontScript : MonoBehaviour
{
	public int renderQueue = 4000;

	private void Start()
	{
		SetRenderQueue(base.transform);
	}

	private void SetRenderQueue(Transform currentTransform)
	{
		if (currentTransform.GetComponent<Renderer>() != null && currentTransform.GetComponent<Renderer>().sharedMaterial != null)
		{
			currentTransform.GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueue;
		}
		if (currentTransform.childCount == 0)
		{
			return;
		}
		foreach (Transform item in currentTransform)
		{
			SetRenderQueue(item);
		}
	}
}
