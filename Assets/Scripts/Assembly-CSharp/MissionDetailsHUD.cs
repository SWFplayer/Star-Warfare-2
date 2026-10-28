using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MissionDetailsHUD : GameUI
{
	private const int WAITING_RESULT_STATE_INIT = 0;

	private const int WAITING_RESULT_STATE_SWEEP_1 = 1;

	private const int WAITING_RESULT_STATE_SWEEP_2 = 2;

	private const int WAITING_RESULT_STATE_BLINKING = 3;

	private const int WAITING_RESULT_STATE_FAIL_MOVE = 4;

	private const int WAITING_RESULT_STATE_TWEEN_OUT = 5;

	public static MissionDetailsHUD m_Instance;

	public GameObject m_DetailsButton;

	public GameObject m_MainBackground;

	public GameObject m_SubBackground;

	public UITweenPlayer m_SubMissionTween;

	public UILabel[] m_MissionSummaryArray;

	public UISprite[] m_MissionProgressArray;

	public GameObject m_Boss2Obj;

	public UISprite m_Boss2HPSprite;

	public GameObject m_MainMissionScrollObject;

	public UILabel m_ScrollLabelOnSubComplete;

	public UISprite m_ClockBackground;

	public UILabel m_MissionClock;

	public UISprite m_ProgressBarForward;

	public GameObject m_ProgressBarBackground;

	public GameObject m_Win;

	public GameObject m_Fail;

	public GameObject m_Retreat;

	private bool mSubMissionActive;

	private float mLastPressTime;

	private float mStayTime;

	private Mission[] mMissionArray;

	private bool[] mMissionCompleteArray;

	private EMissionProcess[] mMissionProcessArray;

	private Timer mMissionStatusTimer = new Timer();

	private bool mIsTimeBlinking;

	private float mMainMissionScrollDuration = 0.5f;

	private float mMainMissionScrollSpace = 28f;

	private int mMainMissionScrollCount;

	private List<string> mCompleteScrollSubName = new List<string>();

	private float mMainMissionScrollTime = 2.5f;

	private float mLastScrollTime = 999999f;

	private Timer mMissionRefreshEffectTimer;

	public UISprite[] m_WinSweepSprites1;

	public UISprite[] m_WinSweepSprites2;

	public UISprite[] m_LoseSweepSprites1;

	public UISprite[] m_LoseSweepSprites2;

	public TweenAlpha m_WinBlink;

	public TweenAlpha m_LoseBlink;

	private int mWaitingResultState;

	private bool mIsWaitingResultState_Win;

	private bool mIsWaitingResultState_Lose;

	private float mWaitingResultStateTime;

	private float mWaitingForSweepEffect1Time = 1f;

	private float mWaitingForSweepEffect2Time = 0.3f;

	private float mWaitingForBlinkingTime = 0.2f;

	private float mWaitingForFailMoveTime = 0.3f;

	private float mWaitingForTweenOutTime = 1.2f;

	private void Awake()
	{
		mMissionRefreshEffectTimer = new Timer(3f, false);
		m_Instance = this;
		UIEventListener uIEventListener = UIEventListener.Get(m_DetailsButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickDetailButton));
		uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressDetailButton));
		m_Boss2Obj.SetActive(false);
		mSubMissionActive = true;
		mLastPressTime = Time.time;
		mStayTime = 4f;
		mMissionArray = new Mission[m_MissionSummaryArray.Length];
		mMissionCompleteArray = new bool[m_MissionSummaryArray.Length];
		mMissionProcessArray = new EMissionProcess[m_MissionSummaryArray.Length];
		for (int i = 0; i < mMissionArray.Length; i++)
		{
			if (i >= Global.MISSION_SIZE)
			{
				continue;
			}
			mMissionArray[i] = MissionManager.GetInstance().GetCurrentStageMission(i);
			if (mMissionArray[i] != null)
			{
				if (i == 0)
				{
					m_MissionSummaryArray[i].text = Regex.Replace(mMissionArray[i].SummaryText, "[\\n\\r]", string.Empty);
				}
				else
				{
					m_MissionSummaryArray[i].text = mMissionArray[i].SummaryText;
				}
				mMissionCompleteArray[i] = mMissionArray[i].IsComplete();
				mMissionProcessArray[i] = mMissionArray[i].Process;
				if (mMissionArray[i].IsComplete())
				{
					if (i == 0)
					{
						m_MissionSummaryArray[i].color = UIConstant.COLOR_MISSION_SUMMARY_SUCCESS;
					}
					else
					{
						m_MissionSummaryArray[i].color = UIConstant.COLOR_SUB_MISSION_SUMMARY_SUCCESS;
					}
				}
				else
				{
					m_MissionSummaryArray[i].color = UIConstant.COLOR_MISSION_SUMMARY_IN_PROGRESS;
				}
				if (mMissionArray[i].IsMainMission && mMissionArray[i].Type == MissionType.BOSS)
				{
					m_MissionProgressArray[i].spriteName = "bar_51_full";
					Debug.Log("GameApp.GetInstance().GetUserState().Boss2Type" + GameApp.GetInstance().GetUserState().Boss2Type);
					m_Boss2Obj.SetActive(GameApp.GetInstance().GetUserState().Boss2Type != EnemyType.NA);
				}
			}
			else
			{
				Debug.LogError("Mission is NULL, mission id = " + (i + 1));
			}
		}
		if (mMissionArray[0].Type == MissionType.BOSS)
		{
			mMissionStatusTimer.SetTimer(0.1f, false);
		}
		else
		{
			mMissionStatusTimer.SetTimer(1f, false);
		}
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			VSClock missionClock = gameWorld.MissionClock;
			if (missionClock != null)
			{
				string empty = string.Empty;
				if (MissionManager.GetInstance().GetCurrentStageMission(0).Type == MissionType.SURVIVAL)
				{
					empty = string.Format("[00ffea]{0:D2} : {1:D2}", (int)missionClock.GetCurrentTime() / 60, (int)missionClock.GetCurrentTime() % 60);
					m_ClockBackground.spriteName = "clock_bk";
				}
				else
				{
					empty = string.Format("{0:D2} : {1:D2}", (int)missionClock.GetTimeLeft() / 60, (int)missionClock.GetTimeLeft() % 60);
					m_ClockBackground.spriteName = "clock_bk";
				}
				m_MissionClock.text = empty;
			}
			else
			{
				m_MissionClock.gameObject.SetActive(false);
				m_ClockBackground.gameObject.SetActive(false);
			}
		}
		else
		{
			m_MissionClock.gameObject.SetActive(false);
			m_ClockBackground.gameObject.SetActive(false);
		}
		m_ProgressBarForward.gameObject.SetActive(false);
		m_ProgressBarBackground.SetActive(false);
		m_Win.SetActive(false);
		m_Fail.SetActive(false);
		m_Retreat.SetActive(false);
		m_ScrollLabelOnSubComplete.text = string.Empty;
		mMissionRefreshEffectTimer.Pause();
	}

	private void Start()
	{
		mLastPressTime = Time.time - mStayTime;
	}

	protected override void OnUIUpdate()
	{
		if (mSubMissionActive && Time.time - mLastPressTime > mStayTime)
		{
			UILabel[] missionSummaryArray = m_MissionSummaryArray;
			foreach (UILabel uILabel in missionSummaryArray)
			{
				TweenScale component = uILabel.gameObject.GetComponent<TweenScale>();
				if (null != component)
				{
					component.enabled = false;
				}
			}
			mSubMissionActive = false;
			m_SubMissionTween.PlayReverse();
		}
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			VSClock missionClock = gameWorld.MissionClock;
			if (missionClock != null)
			{
				m_MissionClock.gameObject.SetActive(true);
				m_ClockBackground.gameObject.SetActive(true);
				if (MissionManager.GetInstance().GetCurrentStageMission(0).Type == MissionType.SURVIVAL)
				{
					m_MissionClock.text = string.Format("[00ffea]{0:D2} : {1:D2}", (int)missionClock.GetCurrentTime() / 60, (int)missionClock.GetCurrentTime() % 60);
				}
				else
				{
					m_MissionClock.text = string.Format("{0:D2} : {1:D2}", (int)missionClock.GetTimeLeft() / 60, (int)missionClock.GetTimeLeft() % 60);
					if (missionClock.GetTimeLeft() < 30f)
					{
						m_MissionClock.color = Color.yellow;
						if (!mIsTimeBlinking)
						{
							mIsTimeBlinking = true;
							TweenAlpha tweenAlpha = TweenAlpha.Begin(m_MissionClock.gameObject, 1f, 0f);
							tweenAlpha.style = UITweener.Style.PingPong;
						}
					}
				}
			}
			else
			{
				m_MissionClock.gameObject.SetActive(false);
				m_ClockBackground.gameObject.SetActive(false);
			}
		}
		else
		{
			m_MissionClock.gameObject.SetActive(false);
			m_ClockBackground.gameObject.SetActive(false);
		}
		if (mMissionStatusTimer.Ready())
		{
			mMissionStatusTimer.Do();
			for (int j = 0; j < mMissionArray.Length; j++)
			{
				if (j >= Global.MISSION_SIZE)
				{
					continue;
				}
				if (mMissionArray[j].IsMainMission)
				{
					m_MissionSummaryArray[j].text = Regex.Replace(mMissionArray[j].GetProgressText(), "[\\n\\r]", string.Empty);
				}
				if (!mMissionCompleteArray[j])
				{
					mMissionCompleteArray[j] = mMissionArray[j].IsAtLeastHalfComplete();
					if (mMissionProcessArray[j] != mMissionArray[j].Process)
					{
						mMissionProcessArray[j] = mMissionArray[j].Process;
						if (mMissionArray[j].Process == EMissionProcess.SUCCESS)
						{
							m_MissionSummaryArray[j].color = UIConstant.COLOR_MISSION_SUMMARY_SUCCESS;
						}
						else if (mMissionArray[j].Process == EMissionProcess.FAIL)
						{
							m_MissionSummaryArray[j].color = UIConstant.COLOR_MISSION_SUMMARY_FAIL;
						}
					}
				}
				m_MissionProgressArray[j].fillAmount = mMissionArray[j].GetProgressPercentage();
				if (mMissionArray[j].IsMainMission && mMissionArray[j].Type == MissionType.BOSS && GameApp.GetInstance().GetUserState().Boss1Type != EnemyType.NA)
				{
					m_Boss2HPSprite.fillAmount = (mMissionArray[j] as MissionBoss).GetBoss2ProgressPercentage();
				}
			}
		}
		if (mMainMissionScrollCount > 0)
		{
			if (RealTime.time - mLastScrollTime > mMainMissionScrollTime)
			{
				mMainMissionScrollCount--;
				if (mCompleteScrollSubName.Count > 0)
				{
					mCompleteScrollSubName.RemoveAt(0);
				}
				OnSubMissionNoticeFinished();
			}
		}
		else
		{
			mLastScrollTime = RealTime.time + 1f;
		}
		if (mMissionRefreshEffectTimer.Ready())
		{
			mMissionRefreshEffectTimer.Pause();
			OnPressDetailButton(m_DetailsButton, false);
			m_DetailsButton.GetComponent<TweenScale>().style = UITweener.Style.Once;
			m_DetailsButton.GetComponent<TweenScale>().Reset();
		}
		if (!mIsWaitingResultState_Win && !mIsWaitingResultState_Lose)
		{
			return;
		}
		switch (mWaitingResultState)
		{
		case 0:
			mWaitingResultState = 1;
			break;
		case 1:
			if (!(mWaitingResultStateTime >= mWaitingForSweepEffect1Time))
			{
				break;
			}
			mWaitingResultStateTime = 0f;
			mWaitingResultState = 2;
			if (mIsWaitingResultState_Win)
			{
				UISprite[] winSweepSprites = m_WinSweepSprites1;
				foreach (UISprite uISprite in winSweepSprites)
				{
					uISprite.gameObject.SetActive(true);
				}
			}
			else if (mIsWaitingResultState_Lose)
			{
				UISprite[] loseSweepSprites = m_LoseSweepSprites1;
				foreach (UISprite uISprite2 in loseSweepSprites)
				{
					uISprite2.gameObject.SetActive(true);
				}
			}
			break;
		case 2:
			if (!(mWaitingResultStateTime >= mWaitingForSweepEffect2Time))
			{
				break;
			}
			mWaitingResultStateTime = 0f;
			mWaitingResultState = 3;
			if (mIsWaitingResultState_Win)
			{
				UISprite[] winSweepSprites2 = m_WinSweepSprites2;
				foreach (UISprite uISprite3 in winSweepSprites2)
				{
					uISprite3.gameObject.SetActive(true);
				}
			}
			else if (mIsWaitingResultState_Lose)
			{
				UISprite[] loseSweepSprites2 = m_LoseSweepSprites2;
				foreach (UISprite uISprite4 in loseSweepSprites2)
				{
					uISprite4.gameObject.SetActive(true);
				}
			}
			break;
		case 3:
			if (mWaitingResultStateTime >= mWaitingForBlinkingTime)
			{
				mWaitingResultStateTime = 0f;
				if (mIsWaitingResultState_Win)
				{
					mWaitingResultState = 5;
					m_WinBlink.Reset();
					m_WinBlink.PlayForward();
				}
				else if (mIsWaitingResultState_Lose)
				{
					mWaitingResultState = 4;
					m_LoseBlink.Reset();
					m_LoseBlink.PlayForward();
				}
			}
			break;
		case 4:
			if (mWaitingResultStateTime >= mWaitingForFailMoveTime)
			{
				mWaitingResultState = 5;
			}
			break;
		case 5:
			if (mWaitingResultStateTime >= mWaitingForTweenOutTime)
			{
				mWaitingResultState++;
				if (mIsWaitingResultState_Win)
				{
					TweenPosition.Begin(m_Win, 0.7f, new Vector3(-1280f, 0f, 0f));
				}
				else if (mIsWaitingResultState_Lose)
				{
					TweenPosition.Begin(m_Fail, 0.7f, new Vector3(0f, -768f, 0f));
				}
			}
			break;
		}
		mWaitingResultStateTime += RealTime.deltaTime;
	}

	private void OnClickDetailButton(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
		m_SubMissionTween.PlayForward();
		mLastPressTime = Time.time;
		mStayTime = 4f;
		mSubMissionActive = true;
		UILabel[] missionSummaryArray = m_MissionSummaryArray;
		foreach (UILabel uILabel in missionSummaryArray)
		{
			uILabel.transform.localScale = Vector3.one;
			TweenScale component = uILabel.gameObject.GetComponent<TweenScale>();
			if (null != component)
			{
				component.enabled = false;
			}
		}
	}

	private void OnPressDetailButton(GameObject go, bool isPress)
	{
		if (go == m_DetailsButton)
		{
			if (isPress)
			{
				m_MainBackground.GetComponent<UISprite>().spriteName = "bar_quest_u";
			}
			else
			{
				m_MainBackground.GetComponent<UISprite>().spriteName = "bar_quest";
			}
		}
	}

	public void OnSubMissionComplete(List<int> idList)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_PROGRESS);
		for (int i = 0; i < idList.Count; i++)
		{
			if (idList[i] < m_MissionSummaryArray.Length)
			{
				TweenScale tweenScale = TweenScale.Begin(m_MissionSummaryArray[idList[i]].gameObject, 0.5f, 1.5f * Vector3.one);
				tweenScale.style = UITweener.Style.PingPong;
				mCompleteScrollSubName.Add(m_MissionSummaryArray[idList[i]].text + "\n");
				mMainMissionScrollCount++;
			}
		}
		SetScrollLabelText();
		Vector3 pos = m_MainMissionScrollObject.transform.localPosition + Vector3.up * mMainMissionScrollSpace;
		TweenPosition tweenPosition = TweenPosition.Begin(m_MainMissionScrollObject, mMainMissionScrollDuration, pos);
		m_ScrollLabelOnSubComplete.gameObject.AddComponent<TweenAlpha>();
		TweenAlpha component = m_ScrollLabelOnSubComplete.GetComponent<TweenAlpha>();
		component.style = UITweener.Style.PingPong;
		component.delay = 0.5f;
		component.from = 1f;
		component.to = 0f;
		component.duration = 0.4f;
		mLastScrollTime = RealTime.time;
	}

	public void OnSubMissionNoticeFinished()
	{
		mLastScrollTime = RealTime.time;
		if (mMainMissionScrollCount <= 1 && mMainMissionScrollCount == 0)
		{
			Vector3 pos = m_MainMissionScrollObject.transform.localPosition - Vector3.up * mMainMissionScrollSpace;
			TweenPosition tweenPosition = TweenPosition.Begin(m_MainMissionScrollObject, mMainMissionScrollDuration, pos);
		}
		SetScrollLabelText();
	}

	public void SetScrollLabelText()
	{
		m_ScrollLabelOnSubComplete.text = string.Empty;
		for (int i = 0; i < mCompleteScrollSubName.Count; i++)
		{
			m_ScrollLabelOnSubComplete.text += mCompleteScrollSubName[i];
		}
	}

	public void SetProgressBarActive(bool active)
	{
		m_ProgressBarForward.gameObject.SetActive(active);
		m_ProgressBarBackground.SetActive(active);
	}

	public void SetProgressBar(float progress)
	{
		m_ProgressBarForward.fillAmount = progress;
	}

	public void OnWin()
	{
		mIsWaitingResultState_Win = true;
		mIsWaitingResultState_Lose = false;
		m_Win.SetActive(true);
	}

	public void OnFail()
	{
		mIsWaitingResultState_Lose = true;
		mIsWaitingResultState_Win = false;
		if (mMissionArray[0].Type == MissionType.INFINITE)
		{
			m_Retreat.SetActive(true);
		}
		else
		{
			m_Fail.SetActive(true);
		}
	}

	public void ClearMessage()
	{
		m_Win.SetActive(false);
		m_Fail.SetActive(false);
		m_Retreat.SetActive(false);
	}

	public void RefreshMission()
	{
		mMissionRefreshEffectTimer.Do();
		OnPressDetailButton(m_DetailsButton, true);
		m_DetailsButton.GetComponent<TweenScale>().to = Vector3.one * 1.2f;
		m_DetailsButton.GetComponent<TweenScale>().style = UITweener.Style.PingPong;
		m_DetailsButton.GetComponent<TweenScale>().PlayForward();
	}
}
