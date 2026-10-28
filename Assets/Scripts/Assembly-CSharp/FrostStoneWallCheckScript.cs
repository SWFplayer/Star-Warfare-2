using System;
using UnityEngine;

public class FrostStoneWallCheckScript : MonoBehaviour
{
	[NonSerialized]
	public bool IsHitWall;

	private void OnTriggerEnter(Collider collider)
	{
		IsHitWall = true;
	}

	private void OnTriggerStay(Collider collider)
	{
		IsHitWall = true;
	}

	private void Update()
	{
		IsHitWall = false;
	}
}
