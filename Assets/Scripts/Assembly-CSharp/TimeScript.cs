using UnityEngine;

public class TimeScript : MonoBehaviour
{
	private UserState userState;

	private void Start()
	{
		userState = GameApp.GetInstance().GetUserState();
	}

	private void Update()
	{
	}
}
