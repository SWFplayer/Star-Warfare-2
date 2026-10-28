using UnityEngine;

public class KOHHUD : MonoBehaviour
{
	public static KOHHUD mInstance;

	public UILabel BlueScore;

	public UILabel RedScore;

	public UISprite HillProcess;

	public UISprite HillProcessBackground;

	public UISprite BalanceHighlight;

	public Transform InitHighlightPos;

	public UISprite SelfRedIcon;

	public UISprite SelfBlueIcon;

	private bool mResetComplete;

	private PointScript mPoint;

	protected int mBalanceBarMaxWidth = 76;

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

	private void Update()
	{
		if (!mResetComplete && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			mResetComplete = true;
			Reset();
		}
		if (!(mPoint != null))
		{
			return;
		}
		if (mPoint.isNeutral)
		{
			float num = mPoint.mBalance;
			if (mPoint.GetAttackNetualTeam() == 0)
			{
				if (mPoint.mBalance < 0f)
				{
					HillProcess.spriteName = "red_point1";
					num = 0f - num;
				}
				else
				{
					HillProcess.spriteName = "blue_point1";
				}
			}
			else if (mPoint.GetAttackNetualTeam() == 1)
			{
				if (mPoint.mBalance < 0f)
				{
					HillProcess.spriteName = "blue_point1";
					num = 0f - num;
				}
				else
				{
					HillProcess.spriteName = "red_point1";
				}
			}
			HillProcess.fillAmount = num / PointScript.PointControlNeedValue;
			HillProcessBackground.spriteName = "point_empty";
		}
		else
		{
			if (mPoint.GetController() == TeamName.Blue)
			{
				HillProcess.spriteName = "red_point1";
				HillProcessBackground.spriteName = "blue_point";
			}
			else
			{
				HillProcess.spriteName = "blue_point1";
				HillProcessBackground.spriteName = "red_point";
			}
			HillProcess.fillAmount = mPoint.mBalance / PointScript.PointControlNeedValue;
		}
		if (HillProcess.fillAmount > 0.05f && HillProcess.fillAmount < 0.95f)
		{
			if (!BalanceHighlight.gameObject.activeSelf)
			{
				BalanceHighlight.gameObject.SetActive(true);
			}
			BalanceHighlight.transform.localPosition = new Vector3(InitHighlightPos.localPosition.x - (1f - HillProcess.fillAmount) * (float)mBalanceBarMaxWidth, BalanceHighlight.transform.localPosition.y, 0f);
		}
		else if (BalanceHighlight.gameObject.activeSelf)
		{
			BalanceHighlight.gameObject.SetActive(false);
		}
		BlueScore.text = GameApp.GetInstance().GetGameWorld().VsBattleLogic.GetScore(0) + "/" + 1500;
		RedScore.text = GameApp.GetInstance().GetGameWorld().VsBattleLogic.GetScore(1) + "/" + 1500;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void Reset()
	{
		HillProcess.fillAmount = 0f;
		HillProcessBackground.spriteName = "point_empty";
		BlueScore.text = 1500.ToString();
		RedScore.text = 1500.ToString();
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
		mPoint = (GameApp.GetInstance().GetGameWorld().VsBattleLogic as KOTHModeLogic).GetPoint();
	}

	public string SetScoreToRestTime(int score)
	{
		string empty = string.Empty;
		int num = 1500 - score;
		int num2 = num / 60;
		empty = ((num2 < 10) ? ("0" + num2) : num2.ToString()) + ":";
		int num3 = num % 60;
		return empty + ((num3 < 10) ? ("0" + num3) : num3.ToString());
	}
}
