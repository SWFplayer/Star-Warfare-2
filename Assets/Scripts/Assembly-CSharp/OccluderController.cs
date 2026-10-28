using UnityEngine;

public class OccluderController : HighlightingController
{
	private int ox = -220;

	private int oy = 20;

	private void OnGUI()
	{
		float left = Screen.width + ox;
		GUI.Label(new Rect(left, oy, 500f, 100f), "Occluder (moving wall) controls:");
		if (GUI.Button(new Rect(left, oy + 30, 200f, 30f), "Toggle Occluder"))
		{
			ho.OccluderSwitch();
		}
	}
}
