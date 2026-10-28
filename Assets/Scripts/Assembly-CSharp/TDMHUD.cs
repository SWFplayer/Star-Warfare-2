using UnityEngine;

public class TDMHUD : MonoBehaviour
{
	public static TDMHUD mInstance;

	public UILabel BlueScore;

	public UILabel RedScore;

	public UILabel CenterTitle;

	public UILabel CenterInfo;

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
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
			{
				mResetComplete = true;
				Reset();
			}
			return;
		}
		int num = GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[0];
		int num2 = GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[1];
		BlueScore.text = num.ToString();
		RedScore.text = num2.ToString();
		if (Lobby.GetInstance().IsScoreWinCondition == 1)
		{
			CenterTitle.text = "SCORE";
			int num3 = num;
			if (num3 < num2)
			{
				num3 = num2;
				CenterTitle.color = UIConstant.COLOR_RED_TEAM;
				CenterInfo.color = UIConstant.COLOR_RED_TEAM;
			}
			else
			{
				CenterTitle.color = UIConstant.COLOR_BLUE_TEAM;
				CenterInfo.color = UIConstant.COLOR_BLUE_TEAM;
			}
			CenterInfo.text = num3 + "/" + Lobby.GetInstance().WinValue;
		}
		else
		{
			CenterTitle.text = "TIME";
			int num4 = Lobby.GetInstance().WinValue * 60 - Lobby.GetInstance().GetVSClock().GetCurrentTimeSeconds();
			string empty = string.Empty;
			int num5 = num4 / 60;
			empty = ((num5 < 10) ? ("0" + num5) : num5.ToString()) + ":";
			int num6 = num4 % 60;
			empty += ((num6 < 10) ? ("0" + num6) : num6.ToString());
			CenterInfo.text = empty;
		}
	}

	public void Reset()
	{
		BlueScore.text = "0";
		RedScore.text = "0";
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
