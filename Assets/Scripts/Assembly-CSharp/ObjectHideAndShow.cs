using System.Collections.Generic;
using UnityEngine;

public class ObjectHideAndShow : MonoBehaviour
{
	public List<GameObject> thingsToHide;

	public List<GameObject> thingsToShow;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != 8)
		{
			return;
		}
		foreach (GameObject item in thingsToHide)
		{
			if (item != null)
			{
				item.SetActive(false);
			}
		}
		foreach (GameObject item2 in thingsToShow)
		{
			if (item2 != null)
			{
				item2.SetActive(true);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer != 8)
		{
			return;
		}
		foreach (GameObject item in thingsToHide)
		{
			if (item != null)
			{
				item.SetActive(true);
			}
		}
		foreach (GameObject item2 in thingsToShow)
		{
			if (item2 != null)
			{
				item2.SetActive(false);
			}
		}
	}
}
