using UnityEngine;

public class MutantHUD : MonoBehaviour
{
	public static MutantHUD mInstance;

	public GameObject BossState;

	public UISprite BossProcess;

	public UISprite ProcessHighlight;

	public Transform InitHighlightPos;

	public UILabel Description;

	public UILabel CountDown;

	private bool mResetComplete;

	protected int mProcessBarMaxWidth = 280;

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
		GiantBetrayalModeLogic giantBetrayalModeLogic = GameApp.GetInstance().GetGameWorld().VsBattleLogic as GiantBetrayalModeLogic;
		if (giantBetrayalModeLogic.GetPlayerBoss() != null)
		{
			CountDown.text = GetRestToWinTime(giantBetrayalModeLogic.GetPlayerBossTimeToWin());
			BossProcess.fillAmount = (float)giantBetrayalModeLogic.GetPlayerBoss().Hp / (float)giantBetrayalModeLogic.GetPlayerBoss().MaxHp;
		}
		if (BossProcess.fillAmount > 0.01f && BossProcess.fillAmount < 0.99f)
		{
			if (!ProcessHighlight.gameObject.activeSelf)
			{
				ProcessHighlight.gameObject.SetActive(true);
			}
			ProcessHighlight.transform.localPosition = new Vector3(InitHighlightPos.localPosition.x - (1f - BossProcess.fillAmount) * (float)mProcessBarMaxWidth, ProcessHighlight.transform.localPosition.y, 0f);
		}
		else if (ProcessHighlight.gameObject.activeSelf)
		{
			ProcessHighlight.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void Reset()
	{
		BossProcess.fillAmount = 1f;
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.IsBoss)
		{
			Debug.Log("SHOW___IsBoss________________________");
			BossState.SetActive(false);
			HUDBase.instance.m_JoystickManager.gameObject.SetActive(false);
			HUDBase.instance.m_BossStickmanager.gameObject.SetActive(true);
			HUDBase.instance.m_SkillHUDManager.gameObject.SetActive(false);
			HUDBase.instance.m_StateItem.gameObject.SetActive(false);
			HUDBase.instance.m_Weapon.gameObject.SetActive(false);
			if (StateBuff.mInstance != null)
			{
				StateBuff.mInstance.gameObject.SetActive(false);
			}
			Description.text = LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD1");
		}
		else
		{
			Debug.Log("SHOW___IsPlayer________________________");
			BossState.SetActive(true);
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                HUDBase.instance.m_JoystickManager.gameObject.SetActive(true);
            }
            HUDBase.instance.m_BossStickmanager.gameObject.SetActive(false);
			HUDBase.instance.m_SkillHUDManager.gameObject.SetActive(true);
			HUDBase.instance.m_StateItem.gameObject.SetActive(true);
			HUDBase.instance.m_Weapon.gameObject.SetActive(true);
			if (StateBuff.mInstance != null)
			{
				StateBuff.mInstance.gameObject.SetActive(true);
			}
			Description.text = LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD2");
		}
		CountDown.text = GetRestToWinTime((GameApp.GetInstance().GetGameWorld().VsBattleLogic as GiantBetrayalModeLogic).GetPlayerBossWinTime());
	}

	public string GetRestToWinTime(float restSeconds)
	{
		string empty = string.Empty;
		int num = Mathf.CeilToInt(restSeconds);
		if (num < 0)
		{
			return "--:--";
		}
		int num2 = num / 60;
		empty = ((num2 < 10) ? ("0" + num2) : num2.ToString()) + ":";
		int num3 = num % 60;
		return empty + ((num3 < 10) ? ("0" + num3) : num3.ToString());
	}
}
