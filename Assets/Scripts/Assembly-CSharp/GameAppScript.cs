using System.Collections;
using UnityEngine;

public class GameAppScript : MonoBehaviour
{
	public static bool IsControllerConnected;

	private void Start()
	{
		StartCoroutine(CheckForControllers());
	}

	private void Update()
	{
		GameApp.GetInstance().Loop(Time.deltaTime);
	}

	private void LateUpdate()
	{
		GameApp.GetInstance().LateLoop(Time.deltaTime);
	}

	private IEnumerator CheckForControllers()
	{
		while (true)
		{
			string[] names = Input.GetJoystickNames();
			if (!IsControllerConnected && names.Length > 0)
			{
				IsControllerConnected = true;
				string[] array = names;
				foreach (string n in array)
				{
					Debug.Log(n + " Connected!");
				}
			}
			else if (IsControllerConnected && names.Length == 0)
			{
				IsControllerConnected = false;
				Debug.Log("Disconnected");
			}
			yield return new WaitForSeconds(3f);
		}
	}
}
