using System.Collections;
using System.IO;
using Prime31;
using UnityEngine;

public class ApplicationPauseManager : MonoBehaviour, UIMsgListener
{
	public enum EPostingType
	{
		PVE_SCORE = 0,
		PVP_SCORE = 1,
		CUSTOMIZE = 2
	}

	public static ApplicationPauseManager mInstance;

	private bool mIsPaused;

	public bool FacebookClicked;

	public bool TwitterClicked;

	public bool IsPostingToFacebook;

	public bool IsPostingToTwitter;

	public bool DebugTest;

	public EPostingType currentType;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void NewInstance()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		AntiCrackManager.GetInstance().SetGold(userState.GetCash());
		AntiCrackManager.GetInstance().SetMithril(userState.GetMithril());
	}

	private bool IsCracked()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		AntiCrackManager instance = AntiCrackManager.GetInstance();
		return instance.GetGold() != userState.GetCash() || instance.GetMithril() != userState.GetMithril();
	}

	public void OnApplicationPause(bool pause)
	{
		mIsPaused = pause;
		Debug.Log("OnApplicationPause  " + mIsPaused);
		if (!mIsPaused)
		{
			if (FacebookClicked && !GameApp.GetInstance().GetUserState().GetFollowFacebook())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_FBTT_REWARD"), 1, 8);
				FacebookClicked = false;
				GameApp.GetInstance().GetUserState().AddMithril(50);
				GameApp.GetInstance().GetUserState().AnalysisManager.mithrilInfo.ADMithril += 50;
				GameApp.GetInstance().GetUserState().SetFollowFacebook(true);
				ContactPageScript.mInstance.FacebookBtn.transform.Find("AddMithril").gameObject.SetActive(false);
			}
			else if (TwitterClicked && !GameApp.GetInstance().GetUserState().GetFollowTwitter())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_FBTT_REWARD"), 1, 8);
				TwitterClicked = false;
				GameApp.GetInstance().GetUserState().AddMithril(50);
				GameApp.GetInstance().GetUserState().AnalysisManager.mithrilInfo.ADMithril += 50;
				GameApp.GetInstance().GetUserState().SetFollowTwitter(true);
				ContactPageScript.mInstance.TwitterBtn.transform.Find("AddMithril").gameObject.SetActive(false);
			}
			if (IsPostingToFacebook)
			{
				IsPostingToFacebook = false;
				StartCoroutine(postToFacebook(currentType));
			}
			else if (IsPostingToTwitter)
			{
				IsPostingToTwitter = false;
				StartCoroutine(postToTwitter(currentType));
			}
		}
		GameApp.GetInstance().Save();
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 8)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void Update()
	{
		if (DebugTest)
		{
			DebugTest = false;
			OnApplicationPause(!mIsPaused);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		Time.timeScale = 1f;
	}

	public IEnumerator postToFacebook(EPostingType type)
	{
		string pathToImage = Application.persistentDataPath + "/tempscreens.png";
		string content2 = string.Empty;
		switch (type)
		{
		case EPostingType.PVE_SCORE:
			content2 = LocalizationManager.GetInstance().GetString("SNS_Content1");
			break;
		case EPostingType.PVP_SCORE:
			content2 = LocalizationManager.GetInstance().GetString("SNS_Content3");
			break;
		case EPostingType.CUSTOMIZE:
			content2 = LocalizationManager.GetInstance().GetString("SNS_Content2");
			break;
		}
		yield return null;
		if (FacebookTools.isSessionValid())
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("SNS_Post3"), 9);
			yield return new WaitForSeconds(1f);
			WWWForm wwwForm = new WWWForm();
			wwwForm.AddBinaryData("tempscreens.png", File.ReadAllBytes(pathToImage), "InteractiveConsole.png");
			wwwForm.AddField("message", content2);
			FacebookTools.Feed(wwwForm);
		}
		yield return 0;
	}

	public IEnumerator postToTwitter(EPostingType type)
	{
		string pathToImage = Application.persistentDataPath + "/tempscreens.png";
		string content = string.Empty;
		switch (type)
		{
		case EPostingType.PVE_SCORE:
			content = LocalizationManager.GetInstance().GetString("SNS_Content1");
			break;
		case EPostingType.PVP_SCORE:
			content = LocalizationManager.GetInstance().GetString("SNS_Content3");
			break;
		case EPostingType.CUSTOMIZE:
			content = LocalizationManager.GetInstance().GetString("SNS_Content2");
			break;
		}
		yield return null;
		yield return 0;
	}

	public static void completionHandler(string error, object result)
	{
		if (error != null)
		{
			Debug.LogError(error);
			UIMsgBox.instance.CloseMessage();
			UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("SNS_Post2") + ": " + error, 1, 9);
		}
		else
		{
			Utils.logObject(result);
			UIMsgBox.instance.CloseMessage();
			UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("SNS_Post1"), 1, 9);
		}
	}
}
