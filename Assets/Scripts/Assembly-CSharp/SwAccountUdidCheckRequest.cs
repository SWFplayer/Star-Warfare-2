using UnityEngine;

public class SwAccountUdidCheckRequest : SwLoginRequest
{
	protected bool mIsFromOption;

	protected bool mHasRegistered;

	protected bool mHasGuestAccount;

	protected string mReceiveEmail;

	protected string mReceiveName;

	protected int mReceiveUserId;

	public SwAccountUdidCheckRequest(bool isFromOption)
	{
		mIsFromOption = isFromOption;
	}

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		UserState userState = GameApp.GetInstance().GetUserState();
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountUdidCheck?udid=" + ToBase64String(uUID) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode != SwErrorCode.SUCCESS)
		{
			return;
		}
		mHasRegistered = bb.ReadBool();
		if (mHasRegistered)
		{
			mReceiveEmail = bb.ReadStringShortLength();
			mReceiveName = bb.ReadStringShortLength();
			return;
		}
		mHasGuestAccount = bb.ReadBool();
		if (mHasGuestAccount)
		{
			mReceiveUserId = bb.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			if (mHasRegistered)
			{
				if (mIsFromOption)
				{
					UIMsgBox.instance.ShowMessage(OptionMainPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_SPHINT6", mReceiveEmail, mReceiveName), 11, 62);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_SPHINT6", mReceiveEmail, mReceiveName), 11, 62);
				}
				TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.REGISTER_TUTORIAL, true);
				return;
			}
			UIMsgBox.instance.CloseMessage();
			GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/QuickRegisterUI") as GameObject;
			GameObject gameObject = (GameObject)Object.Instantiate(original, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			QuickRegisterScript component = gameObject.GetComponent<QuickRegisterScript>();
			if (null != component)
			{
				component.HasGuestAccount = mHasGuestAccount;
				component.UserId = mReceiveUserId;
				component.IsFromOption = mIsFromOption;
			}
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.REGISTER_TUTORIAL);
		}
		else if (mReceiveErrorCode == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			if (mIsFromOption)
			{
				if (OptionMainPageScript.mInstance != null)
				{
					UIMsgBox.instance.ShowErrorCodeMessage(OptionMainPageScript.mInstance, mReceiveErrorCode, 16);
				}
			}
			else if (MainMenuScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(MainMenuScript.mInstance, mReceiveErrorCode, 16);
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(MainMenu2DScript.mInstance, mReceiveErrorCode, 16);
			}
		}
		else if (mIsFromOption)
		{
			if (OptionMainPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(OptionMainPageScript.mInstance, mReceiveErrorCode, 17);
			}
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(MainMenuScript.mInstance, mReceiveErrorCode, 17);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(MainMenu2DScript.mInstance, mReceiveErrorCode, 17);
		}
	}

	public override void OnTimeOut()
	{
		if (mIsFromOption)
		{
			if (OptionMainPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(OptionMainPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
			}
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
		}
	}

	public override void OnResponseError()
	{
		if (mIsFromOption)
		{
			if (OptionMainPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(OptionMainPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
			}
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
		}
	}
}
