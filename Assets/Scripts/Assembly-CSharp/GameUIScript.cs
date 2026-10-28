using System.Collections;
using UnityEngine;

public class GameUIScript : MonoBehaviour
{
	private NetworkManager networkMgr;

	public Texture2D reticle;

	protected float frames;

	protected float updateInterval = 2f;

	protected float timeLeft;

	protected string fpsStr;

	protected float accum;

	protected Timer timer = new Timer();

	protected bool showUI;

	protected bool showPopUI;

	private IEnumerator Start()
	{
		timer.SetTimer(10f, false);
		timer.Do();
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		yield return 1;
	}

	private void Update()
	{
		timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if (timeLeft <= 0f)
		{
			fpsStr = "FPS:" + accum / frames;
			frames = 0f;
			accum = 0f;
			timeLeft = updateInterval;
		}
	}

	public void OnGUI()
	{
		GUI.Label(new Rect(10f, 130f, 200f, 100f), fpsStr);
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null)
		{
			GUI.Label(new Rect(170f, 20f, 100f, 100f), localPlayer.Hp + "/" + localPlayer.MaxHp);
			if (localPlayer.GetWeapon() != null)
			{
				GUI.Label(new Rect(870f, 70f, 100f, 100f), localPlayer.GetWeapon().Name + ":" + localPlayer.GetWeapon().Damage);
			}
		}
		GUI.Label(new Rect(10f, 160f, 200f, 100f), "DifficultyLevel " + gameWorld.DifficultyLevel);
		VSClock missionClock = gameWorld.MissionClock;
		if (missionClock != null)
		{
			string text = string.Format("{0:D2} : {1:D2}", (int)missionClock.GetTimeLeft() / 60, (int)missionClock.GetTimeLeft() % 60);
			GUI.Label(new Rect(280f, 40f, 100f, 100f), text);
		}
	}

	private void LostFunction(int id)
	{
	}
}
