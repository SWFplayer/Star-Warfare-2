using System.Collections.Generic;
using UnityEngine;

public class MissionProtectionScript : MonoBehaviour
{
	public bool Movable;

	public int MaxHP = 100;

	public EProtectionItemType Type;

	public string Prefab = string.Empty;

	public float Speed = 2f;

	public List<GameObject> PathPoints = new List<GameObject>();

	public List<float> SpeedValues = new List<float>();

	public List<float> WaitTime = new List<float>();
}
