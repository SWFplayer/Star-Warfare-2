using UnityEngine;

[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour
{
	public int mRendererQueue;

	private Renderer[] mRd;

	private void Start()
	{
		mRd = GetComponentsInChildren<Renderer>();
	}

	private void Update()
	{
		if (mRd == null || mRd.Length <= 0)
		{
			return;
		}
		Renderer[] array = mRd;
		foreach (Renderer renderer in array)
		{
			if (renderer != null && renderer.materials != null && renderer.materials.Length > 0)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					material.renderQueue = material.shader.renderQueue + mRendererQueue;
				}
			}
		}
	}
}
