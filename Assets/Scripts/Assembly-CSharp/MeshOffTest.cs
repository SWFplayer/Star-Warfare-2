using UnityEngine;

public class MeshOffTest : MonoBehaviour
{
	private GameObject meshToDisappear;

	private GameObject anotherMeshToDisappear;

	public string name1;

	public string name2;

	private int index;

	private void Start()
	{
		meshToDisappear = GameObject.Find(name1);
		anotherMeshToDisappear = GameObject.Find(name2);
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		if (meshToDisappear != null && anotherMeshToDisappear != null)
		{
			switch (index)
			{
			case 0:
				meshToDisappear.SetActive(false);
				anotherMeshToDisappear.SetActive(false);
				break;
			case 1:
				meshToDisappear.SetActive(true);
				anotherMeshToDisappear.SetActive(false);
				break;
			case 2:
				meshToDisappear.SetActive(false);
				anotherMeshToDisappear.SetActive(true);
				break;
			case 3:
				meshToDisappear.SetActive(true);
				anotherMeshToDisappear.SetActive(true);
				index = -1;
				break;
			}
			index++;
		}
	}
}
