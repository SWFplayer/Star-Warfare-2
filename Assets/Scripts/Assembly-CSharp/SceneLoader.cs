using UnityEngine;

public class SceneLoader : MonoBehaviour
{
	private int ox = 20;

	private int oy = 100;

	private void OnGUI()
	{
		GUI.Label(new Rect(ox, oy + 10, 500f, 100f), "Load demo scene:");
		if (GUI.Button(new Rect(ox, oy + 30, 120f, 20f), "Welcome"))
		{
			Application.LoadLevel("Welcome");
		}
		if (GUI.Button(new Rect(ox, oy + 60, 120f, 20f), "Colors"))
		{
			Application.LoadLevel("Colors");
		}
		if (GUI.Button(new Rect(ox, oy + 90, 120f, 20f), "Transparency"))
		{
			Application.LoadLevel("Transparency");
		}
		if (GUI.Button(new Rect(ox, oy + 120, 120f, 20f), "Occluders"))
		{
			Application.LoadLevel("Occluders");
		}
		if (GUI.Button(new Rect(ox, oy + 150, 120f, 20f), "Scripting"))
		{
			Application.LoadLevel("Scripting");
		}
		if (GUI.Button(new Rect(ox, oy + 180, 120f, 20f), "Compound"))
		{
			Application.LoadLevel("Compound");
		}
	}
}
