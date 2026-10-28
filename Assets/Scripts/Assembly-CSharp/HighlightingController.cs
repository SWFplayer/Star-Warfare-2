using UnityEngine;

public class HighlightingController : MonoBehaviour
{
	protected HighlightableObject ho;

	private void Awake()
	{
		ho = base.gameObject.AddComponent<HighlightableObject>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			ho.ConstantSwitch();
		}
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			ho.ConstantSwitchImmediate();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			ho.Off();
		}
		AfterUpdate();
	}

	protected virtual void AfterUpdate()
	{
	}
}
