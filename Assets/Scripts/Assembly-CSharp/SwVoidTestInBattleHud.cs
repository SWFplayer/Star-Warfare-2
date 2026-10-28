using UnityEngine;

public class SwVoidTestInBattleHud : MonoBehaviour
{
	private Player player;

	private UserState userState;

	private int index;

	private Timer checkTimer;

	private void Awake()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		userState = GameApp.GetInstance().GetUserState();
		if (userState.SwVoidIdInUsing == 0)
		{
			base.gameObject.SetActive(false);
		}
		index = -1;
		checkTimer = new Timer();
		checkTimer.SetTimer(0.2f, true);
		base.gameObject.SetActive(false);
	}

	private void OnClick()
	{
		index++;
		if (index > 1)
		{
			index = 0;
		}
		if (index != 1)
		{
			GameApp.GetInstance().GetGameWorld().TrySpawnSwVoid(player);
		}
		else
		{
			player.mSwVoid.Destroy();
		}
	}

	private void Update()
	{
		if (Input.GetButton("SwVoidChange") && checkTimer.Ready())
		{
			checkTimer.Do();
			OnClick();
		}
	}
}
