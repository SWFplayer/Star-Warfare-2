using System;
using UnityEngine;

public class EditNamePageScript : MonoBehaviour, UIMsgListener
{
	public GameObject ConfirmBtn;

	public GameObject CancelBtn;

	public UIInput Input_RegisterNickName;

	public UILabel Register_NickNameResult;

	public UILabel Nickname_Description;

	public UILabel OldNameLabel;

	public static EditNamePageScript mInstance;

	private bool mCanSubmit = true;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(ConfirmBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(CancelBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(Input_RegisterNickName.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	private void OnEnable()
	{
		mCanSubmit = true;
		OldNameLabel.text = Lobby.GetInstance().GetUserName();
		Register_NickNameResult.text = string.Empty;
		Nickname_Description.color = Color.white;
		Input_RegisterNickName.value = string.Empty;
	}

	private void Update()
	{
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == CancelBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			PlayerInfoUI.mLocalInstance.AvatarFrame.SetActive(true);
			PlayerInfoUI.mLocalInstance.ShowModels();
			base.gameObject.SetActive(false);
		}
		else if (go == Input_RegisterNickName.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			Register_NickNameResult.text = string.Empty;
			Nickname_Description.color = Color.white;
			mCanSubmit = true;
		}
		else
		{
			if (!(go == ConfirmBtn))
			{
				return;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mCanSubmit)
			{
				if (GameApp.GetInstance().GetUserState().GetMithril() >= 100)
				{
					UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_SPHINT_4", 100), 2, 64);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_2"), 2, 3);
				}
			}
		}
	}

	public void Register_CheckNickName()
	{
		if (Input_RegisterNickName.value.Length > 0)
		{
			SwAccountNameCheckRequest request = new SwAccountNameCheckRequest(Input_RegisterNickName.value);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_14");
			Register_NickNameResult.color = Color.white;
			Nickname_Description.color = Color.white;
			mCanSubmit = false;
		}
		else
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.red;
			mCanSubmit = false;
		}
	}

	public void Register_CheckNickNameResult(int code)
	{
		if (code == SwErrorCode.SUCCESS)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_16");
			Register_NickNameResult.color = Color.green;
			Nickname_Description.color = Color.green;
			mCanSubmit = true;
		}
		else if (code == SwErrorCode.RENAME_NAME_EXIST || code == SwErrorCode.ACCOUNT_NAME_EXIST)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_17");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.white;
			mCanSubmit = false;
		}
		else if (code == SwErrorCode.RENAME_NAME_INVALID || code == SwErrorCode.ACCOUNT_NAME_INVALID)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.red;
			mCanSubmit = false;
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 64)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				string value = Input_RegisterNickName.value;
				SwAccountRenameRequest request = new SwAccountRenameRequest(value);
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_12"), 7);
				UIMsgBox.instance.CloseMessage();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 3)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
	}
}
