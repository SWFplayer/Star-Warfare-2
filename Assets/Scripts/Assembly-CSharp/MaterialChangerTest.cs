using UnityEngine;

public class MaterialChangerTest : MonoBehaviour
{
	public Material newMaterial;

	public Material oldMaterial;

	private bool isNew;

	private float timeToChange;

	private float timeInterval = 3f;

	private void Start()
	{
		timeToChange = Time.time + timeInterval;
	}

	private void Update()
	{
		if (Time.time > timeToChange)
		{
			timeToChange = Time.time + timeInterval;
			OnClick();
		}
	}

	private void OnClick()
	{
		if (isNew)
		{
			MaterialChanger.Change(base.gameObject, oldMaterial);
			isNew = false;
		}
		else
		{
			MaterialChanger.Change(base.gameObject, newMaterial);
			isNew = true;
		}
	}
}
