using UnityEngine;

public class DominationHUD : MonoBehaviour
{
	public static DominationHUD mInstance;

	public UILabel BlueScore;

	public UILabel RedScore;

	public UISprite[] HillProcess;

	public UISprite[] HillProcessBackground;

	public UISprite[] BalanceHighlight;

	public Transform[] InitHighlightPos;

	public UISprite SelfRedIcon;

	public UISprite SelfBlueIcon;

	private bool mResetComplete;

	private PointScript[] mPoint;

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
		for (int i = 0; i < HillProcess.Length; i++)
		{
			if (!(mPoint[i] != null))
			{
				continue;
			}
			if (mPoint[i].isNeutral)
			{
				float num = mPoint[i].mBalance;
				if (mPoint[i].GetAttackNetualTeam() == 0)
				{
					if (mPoint[i].mBalance < 0f)
					{
						HillProcess[i].spriteName = "red_point1";
						num = 0f - num;
					}
					else
					{
						HillProcess[i].spriteName = "blue_point1";
					}
				}
				else if (mPoint[i].GetAttackNetualTeam() == 1)
				{
					if (mPoint[i].mBalance < 0f)
					{
						HillProcess[i].spriteName = "blue_point1";
						num = 0f - num;
					}
					else
					{
						HillProcess[i].spriteName = "red_point1";
					}
				}
				HillProcess[i].fillAmount = num / PointScript.PointControlNeedValue;
				HillProcessBackground[i].spriteName = "point_empty";
			}
			else
			{
				if (mPoint[i].GetController() == TeamName.Blue)
				{
					HillProcess[i].spriteName = "red_point1";
					HillProcessBackground[i].spriteName = "blue_point";
				}
				else
				{
					HillProcess[i].spriteName = "blue_point1";
					HillProcessBackground[i].spriteName = "red_point";
				}
				HillProcess[i].fillAmount = mPoint[i].mBalance / PointScript.PointControlNeedValue;
			}
			if (HillProcess[i].fillAmount > 0.05f && HillProcess[i].fillAmount < 0.95f)
			{
				if (!BalanceHighlight[i].gameObject.activeSelf)
				{
					BalanceHighlight[i].gameObject.SetActive(true);
				}
				BalanceHighlight[i].transform.localPosition = new Vector3(InitHighlightPos[i].localPosition.x - (1f - HillProcess[i].fillAmount) * (float)mBalanceBarMaxWidth, BalanceHighlight[i].transform.localPosition.y, 0f);
			}
			else if (BalanceHighlight[i].gameObject.activeSelf)
			{
				BalanceHighlight[i].gameObject.SetActive(false);
			}
		}
		BlueScore.text = GameApp.GetInstance().GetGameWorld().VsBattleLogic.GetScore(0) + "/" + 4000;
		RedScore.text = GameApp.GetInstance().GetGameWorld().VsBattleLogic.GetScore(1) + "/" + 4000;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void Reset()
	{
		mPoint = new PointScript[HillProcess.Length];
		for (int i = 0; i < HillProcess.Length; i++)
		{
			HillProcess[i].fillAmount = 0f;
			HillProcessBackground[i].spriteName = "point_empty";
			mPoint[i] = (GameApp.GetInstance().GetGameWorld().VsBattleLogic as DominationModeLogic).GetPoint(i);
		}
		BlueScore.text = 4000.ToString();
		RedScore.text = 4000.ToString();
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

	public string SetScoreToRestTime(int score)
	{
		string empty = string.Empty;
		int num = 4000 - score;
		int num2 = num / 60;
		empty = ((num2 < 10) ? ("0" + num2) : num2.ToString()) + ":";
		int num3 = num % 60;
		return empty + ((num3 < 10) ? ("0" + num3) : num3.ToString());
	}
}
