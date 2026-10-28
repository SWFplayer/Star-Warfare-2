using System.Collections.Generic;
using UnityEngine;

public class MissionEnemyRouteScript : MonoBehaviour
{
	public int currentIndex;

	public List<GameObject> PathPoints = new List<GameObject>();

	public List<float> SpeedValues = new List<float>();

	public List<float> WaitTime = new List<float>();

	public Vector3 GetNextPointPosition()
	{
		return PathPoints[currentIndex].transform.position;
	}

	public float GetWaitTime()
	{
		return WaitTime[currentIndex];
	}

	public float GetSpeed()
	{
		return SpeedValues[currentIndex];
	}

	public bool isTheLastPoint()
	{
		return currentIndex >= PathPoints.Count;
	}
}
