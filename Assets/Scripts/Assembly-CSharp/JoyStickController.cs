using System.Collections;
using UnityEngine;

public class JoyStickController : MonoBehaviour
{
	public static bool IsControllerConnected;

	public static JoyStickController mInstance;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private IEnumerator CheckForControllers()
	{
		while (true)
		{
			string[] names = Input.GetJoystickNames();
			if (!IsControllerConnected && names.Length > 0)
			{
				IsControllerConnected = true;
				string joystickName = string.Empty;
				string[] array = names;
				foreach (string name in array)
				{
					joystickName += name;
				}
				UIMsgBox.instance.ShowConfirmMessage("Joystick " + joystickName + " Connected!");
			}
			else if (IsControllerConnected && names.Length == 0)
			{
				IsControllerConnected = false;
				UIMsgBox.instance.ShowConfirmMessage("Joystick Disconnected!");
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
