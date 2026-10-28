using UnityEngine;

public class CTFHUD : MonoBehaviour
{
	public static CTFHUD mInstance;

	public UILabel BlueScore;

	public UILabel RedScore;

	public UISprite FlagProcess;

	public UISprite RedFlag;

	public UISprite BlueFlag;

	public UISprite SelfRedIcon;

	public UISprite SelfBlueIcon;

	private bool mResetComplete;

	private void Start()
	{
		mInstance = this;
		mResetComplete = false;
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			mResetComplete = true;
			Reset();
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Update()
	{
		if (!mResetComplete)
		{
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() == null)
			{
				return;
			}
			mResetComplete = true;
			Reset();
		}
		UpdateFlagInfo();
	}

	private void UpdateFlagInfo()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		int num = GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[0];
		int num2 = GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[1];
		BlueScore.text = num.ToString() + "/" + Lobby.GetInstance().WinValue;
		RedScore.text = num2.ToString() + "/" + Lobby.GetInstance().WinValue;
		int flagInPlayerID = gameWorld.FlagInPlayerID;
		Player player = null;
		player = ((flagInPlayerID != gameWorld.GetLocalPlayer().GetChannelID()) ? ((Player)gameWorld.GetRemotePlayerByUserID(flagInPlayerID)) : ((Player)gameWorld.GetLocalPlayer()));
		if (player != null)
		{
			if (player.Team == TeamName.Blue)
			{
				FlagProcess.spriteName = "point_blue";
				RedFlag.gameObject.SetActive(false);
				BlueFlag.gameObject.SetActive(true);
			}
			else
			{
				FlagProcess.spriteName = "point_red";
				RedFlag.gameObject.SetActive(true);
				BlueFlag.gameObject.SetActive(false);
			}
		}
		else
		{
			RedFlag.gameObject.SetActive(false);
			BlueFlag.gameObject.SetActive(false);
		}
		if (gameWorld.FlagInPlayerID == -1 && gameWorld.LastFlagInPlayerID == -1)
		{
			FlagProcess.fillAmount = 0f;
			return;
		}
		int num3 = 40 - gameWorld.FlagClock.GetCurrentTimeSeconds();
		if (num3 < 0)
		{
			num3 = 0;
		}
		float fillAmount = (float)num3 / 40f;
		FlagProcess.fillAmount = fillAmount;
	}

	public void Reset()
	{
		FlagProcess.fillAmount = 0f;
		BlueScore.text = "0";
		RedScore.text = "0";
		RedFlag.gameObject.SetActive(false);
		BlueFlag.gameObject.SetActive(false);
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.Team == TeamName.Blue)
		{
			SelfBlueIcon.gameObject.SetActive(true);
			SelfRedIcon.gameObject.SetActive(false);
		}
		else
		{
			SelfBlueIcon.gameObject.SetActive(false);
			SelfRedIcon.gameObject.SetActive(true);
		}
	}
}
