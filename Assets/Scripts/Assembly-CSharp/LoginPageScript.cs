using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class LoginPageScript : MonoBehaviour, UIMsgListener
{
	public static LoginPageScript mInstance;

	public static bool mTurnToRegister;

	public static bool mTurnToLogin;

	public static bool mTurnToAccount;

	public GameObject CatalogPage;

	public GameObject LoginPage;

	public GameObject RegisterPage;

	public GameObject BindPage;

	public GameObject LoginButton;

	public GameObject RegisterButton;

	public GameObject LoginConfirmButton;

	public GameObject RegisterConfirmButton;

	public GameObject LoginReturnButton;

	public GameObject RegisterReturnButton;

	public UIInput Input_LoginID;

	public UIInput Input_LoginPassword;

	public UIInput Input_RegisterEmail;

	public UIInput Input_RegisterNickName;

	public UIInput Input_RegisterPassword;

	public UIInput Input_RegisterPasswordConfirm;

	public UILabel Register_EmailResult;

	public UILabel Register_NickNameResult;

	public UILabel Register_PasswordResult;

	public UILabel Register_PasswordConfirmResult;

	public UILabel Bind_Email;

	public UILabel Bind_NickName;

	public GameObject ChangePasswordBtn;

	private bool mRegisterEmailPassed;

	private bool mRegisterNickNamePassed;

	private bool mRegisterPasswordPassed;

	public GameObject FacebookLoginBtn;

	public GameObject TwitterLoginBtn;

	private bool facebookClicked;

	private bool twitterClicked;

	private bool facebookRefreshed;

	private bool twitterRefreshed;

	private float refreshInterval;

	public UITexture GCPhoto;

	public UITexture FBPhoto;

	public UITexture TTPhoto;

	public GameObject GCPhotoBtn;

	public GameObject FBPhotoBtn;

	public GameObject TTPhotoBtn;

	public GameObject Tip1;

	public GameObject Tip2;

	public GameObject Tip3;

	public GameObject Tip4;

	public GameObject TipPassword;

	public UILabel Email_Description;

	public UILabel Nickname_Description;

	private void Awake()
	{
		mInstance = this;
	}

	public void OnEnable()
	{
		facebookClicked = false;
		twitterClicked = false;
		facebookRefreshed = false;
		twitterRefreshed = false;
		if (!AndroidConstant.isFacebook())
		{
			FacebookLoginBtn.SetActive(false);
		}
		else if (FacebookTools.isSessionValid())
		{
			FacebookLoginBtn.transform.Find("LoginSprite").gameObject.SetActive(true);
			FacebookLoginBtn.transform.Find("LogoutSprite").gameObject.SetActive(false);
		}
		else
		{
			FacebookLoginBtn.transform.Find("LoginSprite").gameObject.SetActive(false);
			FacebookLoginBtn.transform.Find("LogoutSprite").gameObject.SetActive(true);
		}
		TwitterLoginBtn.SetActive(false);
		if (SocialManager.GetInstance().LocalSocialPlayer != null)
		{
			SocialPlayer localSocialPlayer = SocialManager.GetInstance().LocalSocialPlayer;
			if (localSocialPlayer.hasFacebookProfilePhoto)
			{
				FBPhoto.mainTexture = localSocialPlayer.facebookProfilePhoto;
				FBPhotoBtn.SetActive(true);
			}
			else
			{
				FBPhotoBtn.SetActive(false);
			}
			if (localSocialPlayer.hasGameCenterProfilePhoto)
			{
				GCPhoto.mainTexture = localSocialPlayer.gameCenterProfilePhoto;
				GCPhotoBtn.SetActive(true);
			}
			else
			{
				GCPhotoBtn.SetActive(false);
			}
			if (localSocialPlayer.hasTwitterProfilePhoto)
			{
				TTPhoto.mainTexture = localSocialPlayer.twitterProfilePhoto;
				TTPhotoBtn.SetActive(true);
			}
			else
			{
				TTPhotoBtn.SetActive(false);
			}
			FBPhotoBtn.transform.parent.GetComponent<UIGrid>().Reposition();
			switch (SocialManager.GetInstance().LocalSocialPlayer.friendType)
			{
			case FriendType.GameCenter:
				GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
				FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				break;
			case FriendType.Facebook:
				GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
				TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				break;
			case FriendType.Twitter:
				GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
				break;
			default:
				GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
				FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
				break;
			}
		}
		if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
		{
			CatalogPage.SetActive(true);
			LoginPage.SetActive(false);
			RegisterPage.SetActive(false);
			BindPage.SetActive(true);
			Tip1.SetActive(false);
			Tip2.SetActive(false);
			LoginButton.SetActive(false);
			RegisterButton.SetActive(false);
			Bind_Email.text = GameApp.GetInstance().GetUserState().GetOnlineEmail();
			Bind_NickName.text = GameApp.GetInstance().GetUserState().GetOnlineNickname();
		}
		else
		{
			CatalogPage.SetActive(true);
			LoginPage.SetActive(false);
			RegisterPage.SetActive(false);
			BindPage.SetActive(false);
		}
		if (mTurnToRegister)
		{
			OnButtonClick(RegisterButton);
		}
		if (mTurnToLogin)
		{
			OnButtonClick(LoginButton);
		}
	}

	public void Refresh()
	{
		OnEnable();
	}

	private void RefreshPortrait()
	{
		if (SocialManager.GetInstance().LocalSocialPlayer != null)
		{
			SocialPlayer localSocialPlayer = SocialManager.GetInstance().LocalSocialPlayer;
			if (localSocialPlayer.hasFacebookProfilePhoto)
			{
				FBPhoto.mainTexture = localSocialPlayer.facebookProfilePhoto;
				FBPhotoBtn.SetActive(true);
			}
			if (localSocialPlayer.hasGameCenterProfilePhoto)
			{
				GCPhoto.mainTexture = localSocialPlayer.gameCenterProfilePhoto;
				GCPhotoBtn.SetActive(true);
			}
			if (localSocialPlayer.hasTwitterProfilePhoto)
			{
				TTPhoto.mainTexture = localSocialPlayer.twitterProfilePhoto;
				TTPhotoBtn.SetActive(true);
			}
			FBPhotoBtn.transform.parent.GetComponent<UIGrid>().Reposition();
		}
	}

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(LoginButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(RegisterButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(ChangePasswordBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(LoginConfirmButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(RegisterConfirmButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(LoginReturnButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(RegisterReturnButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(FacebookLoginBtn);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(TwitterLoginBtn);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(GCPhotoBtn);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(FBPhotoBtn);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(TTPhotoBtn);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener13 = UIEventListener.Get(Tip1);
		uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener14 = UIEventListener.Get(Tip2);
		uIEventListener14.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener14.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener15 = UIEventListener.Get(Tip3);
		uIEventListener15.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener15.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener16 = UIEventListener.Get(Tip4);
		uIEventListener16.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener16.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener17 = UIEventListener.Get(TipPassword);
		uIEventListener17.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener17.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener18 = UIEventListener.Get(Input_RegisterEmail.gameObject);
		uIEventListener18.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener18.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener19 = UIEventListener.Get(Input_RegisterNickName.gameObject);
		uIEventListener19.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener19.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener20 = UIEventListener.Get(Input_RegisterPassword.gameObject);
		uIEventListener20.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener20.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener21 = UIEventListener.Get(Input_RegisterPasswordConfirm.gameObject);
		uIEventListener21.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener21.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		mRegisterEmailPassed = false;
		mRegisterNickNamePassed = false;
		mRegisterPasswordPassed = false;
	}

	private void Update()
	{
		if ((!facebookClicked || facebookRefreshed) && (!twitterClicked || twitterRefreshed))
		{
			return;
		}
		refreshInterval += Time.deltaTime;
		if (refreshInterval >= 0.5f && facebookClicked)
		{
			Debug.Log("Facebook login = " + FacebookTools.isSessionValid());
			refreshInterval -= 0.5f;
			if (FacebookTools.isSessionValid())
			{
				SocialManager.GetInstance().RefreshFacebook();
				facebookRefreshed = true;
			}
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
		mTurnToLogin = false;
		mTurnToRegister = false;
	}

	public void OnButtonClick(GameObject go)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (go == LoginButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (userState.GetOnlineRegisted())
			{
				CatalogPage.SetActive(true);
				LoginPage.SetActive(false);
				RegisterPage.SetActive(false);
				BindPage.SetActive(true);
				Bind_Email.text = userState.GetOnlineEmail();
				Bind_NickName.text = userState.GetOnlineNickname();
			}
			else
			{
				CatalogPage.SetActive(false);
				LoginPage.SetActive(true);
				RegisterPage.SetActive(false);
				BindPage.SetActive(false);
			}
		}
		else if (go == RegisterButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (userState.GetOnlineRegisted())
			{
				CatalogPage.SetActive(true);
				LoginPage.SetActive(false);
				RegisterPage.SetActive(false);
				BindPage.SetActive(true);
				Bind_Email.text = userState.GetOnlineEmail();
				Bind_NickName.text = userState.GetOnlineNickname();
			}
			else
			{
				CatalogPage.SetActive(false);
				LoginPage.SetActive(false);
				RegisterPage.SetActive(true);
				BindPage.SetActive(false);
				mRegisterEmailPassed = false;
				mRegisterNickNamePassed = false;
				mRegisterPasswordPassed = false;
			}
		}
		else if (go == ChangePasswordBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			SwModifyPasswordRequest request = new SwModifyPasswordRequest();
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
		}
		else if (go == LoginConfirmButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			string value = Input_LoginID.value;
			string value2 = Input_LoginPassword.value;
			SwAccountBindRequest request2 = new SwAccountBindRequest(value, value2);
			GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
		}
		else if (go == RegisterConfirmButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			string value3 = Input_RegisterEmail.value;
			string value4 = Input_RegisterNickName.value;
			string value5 = Input_RegisterPassword.value;
			string value6 = Input_RegisterPasswordConfirm.value;
			Register_CheckPasswordLocal();
			if (mRegisterPasswordPassed)
			{
				SwAccountRegistRequest request3 = new SwAccountRegistRequest(value3, value4, value5);
				GameApp.GetInstance().GetWWWManager().SendRequest(request3);
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_12"), 7);
			}
		}
		else if (go == LoginReturnButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (mTurnToLogin)
			{
				UnityEngine.Object.Destroy(base.gameObject.transform.root.gameObject);
				return;
			}
			CatalogPage.SetActive(true);
			LoginPage.SetActive(false);
			RegisterPage.SetActive(false);
			BindPage.SetActive(false);
		}
		else if (go == RegisterReturnButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (mTurnToRegister)
			{
				UnityEngine.Object.Destroy(base.gameObject.transform.root.gameObject);
				return;
			}
			CatalogPage.SetActive(true);
			LoginPage.SetActive(false);
			RegisterPage.SetActive(false);
			BindPage.SetActive(false);
		}
		else if (go == FacebookLoginBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			facebookClicked = true;
			FacebookTools.CallFBLogin();
		}
		else if (go == TwitterLoginBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			twitterClicked = true;
		}
		else if (go == GCPhotoBtn)
		{
			SocialManager.GetInstance().LocalSocialPlayer.friendType = FriendType.GameCenter;
			GameApp.GetInstance().GetUserState().SetFriendType(SocialManager.GetInstance().LocalSocialPlayer.friendType);
			GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
			FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
			TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
		}
		else if (go == FBPhotoBtn)
		{
			SocialManager.GetInstance().LocalSocialPlayer.friendType = FriendType.Facebook;
			GameApp.GetInstance().GetUserState().SetFriendType(SocialManager.GetInstance().LocalSocialPlayer.friendType);
			GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
			FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
			TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
		}
		else if (go == TTPhotoBtn)
		{
			SocialManager.GetInstance().LocalSocialPlayer.friendType = FriendType.Twitter;
			GameApp.GetInstance().GetUserState().SetFriendType(SocialManager.GetInstance().LocalSocialPlayer.friendType);
			GCPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
			FBPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(false);
			TTPhotoBtn.transform.Find("ChooseSprite").gameObject.SetActive(true);
		}
		else if (go == Tip1)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_ACCOUNT_TIPS_1");
		}
		else if (go == Tip2)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_ACCOUNT_TIPS_2");
		}
		else if (go == Tip3)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_ACCOUNT_TIPS_3");
		}
		else if (go == Tip4)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_ACCOUNT_TIPS_4");
		}
		else if (go == TipPassword)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_CHANGE_PW_07");
		}
		else if (go == Input_RegisterEmail.gameObject)
		{
			Register_EmailResult.text = string.Empty;
			Email_Description.color = Color.white;
		}
		else if (go == Input_RegisterNickName.gameObject)
		{
			Register_NickNameResult.text = string.Empty;
			Nickname_Description.color = Color.white;
		}
		else if (go == Input_RegisterPassword.gameObject || go == Input_RegisterPasswordConfirm.gameObject)
		{
			Register_PasswordResult.text = string.Empty;
			Register_PasswordConfirmResult.text = string.Empty;
		}
	}

	public void Register_CheckEmail()
	{
		if (Input_RegisterEmail.value.Length > 0)
		{
			if (Input_RegisterEmail.value.Contains("@") && Input_RegisterEmail.value.Contains("."))
			{
				SwAccountEmailCheckRequest request = new SwAccountEmailCheckRequest(Input_RegisterEmail.value);
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
				Register_EmailResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_14");
				Register_EmailResult.color = Color.white;
				Email_Description.color = Color.white;
			}
			else
			{
				Register_EmailResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
				Register_EmailResult.color = Color.red;
				Email_Description.color = Color.red;
				mRegisterEmailPassed = false;
			}
		}
	}

	public void Register_CheckEmailResult(int code)
	{
		if (code == SwErrorCode.SUCCESS)
		{
			Register_EmailResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_16");
			Register_EmailResult.color = Color.green;
			Email_Description.color = Color.green;
			mRegisterEmailPassed = true;
		}
		else if (code == SwErrorCode.ACCOUNT_EMAIL_EXIST)
		{
			Register_EmailResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_17");
			Register_EmailResult.color = Color.red;
			Email_Description.color = Color.white;
			mRegisterEmailPassed = false;
		}
		else if (code == SwErrorCode.ACCOUNT_EMAIL_INVALID)
		{
			Register_EmailResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_EmailResult.color = Color.red;
			Email_Description.color = Color.red;
			mRegisterEmailPassed = false;
		}
		else if (code == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(this, code, 16);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(code);
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
		}
		else
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.red;
			mRegisterNickNamePassed = false;
		}
	}

	public void Register_CheckNickNameResult(int code)
	{
		if (code == SwErrorCode.SUCCESS)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_16");
			Register_NickNameResult.color = Color.green;
			Nickname_Description.color = Color.green;
			mRegisterNickNamePassed = true;
		}
		else if (code == SwErrorCode.ACCOUNT_NAME_EXIST)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_17");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.white;
			mRegisterNickNamePassed = false;
		}
		else if (code == SwErrorCode.ACCOUNT_NAME_INVALID)
		{
			Register_NickNameResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_NickNameResult.color = Color.red;
			Nickname_Description.color = Color.red;
			mRegisterNickNamePassed = false;
		}
		else if (code == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(this, code, 16);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(code);
		}
	}

	public void Register_CheckPassWord()
	{
		if (Input_RegisterPassword.value == Input_RegisterPasswordConfirm.value && Input_RegisterPassword.value.Length > 0)
		{
			SwAccountPasswordCheckRequest request = new SwAccountPasswordCheckRequest(Input_RegisterPassword.value);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_14");
			Register_PasswordResult.color = Color.white;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_14");
			Register_PasswordConfirmResult.color = Color.white;
		}
		else if (Input_RegisterPassword.value == Input_RegisterPasswordConfirm.value)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordResult.color = Color.red;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordConfirmResult.color = Color.red;
			mRegisterPasswordPassed = false;
		}
		else
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_18");
			Register_PasswordResult.color = Color.red;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_18");
			Register_PasswordConfirmResult.color = Color.red;
			mRegisterPasswordPassed = false;
		}
	}

	public void Register_CheckPasswordLocal()
	{
		if (Input_RegisterPassword.value != Input_RegisterPasswordConfirm.value)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_18");
			Register_PasswordResult.color = Color.red;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_18");
			Register_PasswordConfirmResult.color = Color.red;
			mRegisterPasswordPassed = false;
		}
		else if (Input_RegisterPassword.value.Length == 0 || Input_RegisterPasswordConfirm.value.Length == 0)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordResult.color = Color.red;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordConfirmResult.color = Color.red;
			mRegisterPasswordPassed = false;
		}
		else
		{
			mRegisterPasswordPassed = true;
		}
	}

	public void Register_CheckPasswordResult(int code)
	{
		if (code == SwErrorCode.SUCCESS)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_16");
			Register_PasswordResult.color = Color.green;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_16");
			Register_PasswordConfirmResult.color = Color.green;
			mRegisterNickNamePassed = true;
		}
		else if (code == SwErrorCode.ACCOUNT_PASSWORD_INVALID)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordResult.color = Color.red;
			Register_PasswordConfirmResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordConfirmResult.color = Color.red;
			mRegisterNickNamePassed = false;
		}
		else if (code == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(this, code, 16);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(code);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 16)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				Refresh();
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				switch (AndroidConstant.version)
				{
				case AndroidConstant.Version.GooglePlay:
					AndroidPluginScript.ShowAppUrl(0);
					break;
				case AndroidConstant.Version.Kindle:
					AndroidPluginScript.ShowAppUrl(1);
					break;
				}
			}
		}
		else if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void FetchFacebookPhoto()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_22"), 7);
		StartCoroutine(_FetchFacebookPhoto());
	}

	private IEnumerator _FetchFacebookPhoto()
	{
		SocialPlayer localSocial = SocialManager.GetInstance().LocalSocialPlayer;
		string photoURL = "https://graph.facebook.com/" + localSocial.faAccountId + "/picture";
		WWW wwwTexture = new WWW(photoURL);
		float fetchStartTime = Time.time;
		bool isFetchTimeOut = false;
		while (!wwwTexture.isDone)
		{
			if (Time.time - fetchStartTime > 15f)
			{
				isFetchTimeOut = true;
				break;
			}
			yield return 0;
		}
		if (!isFetchTimeOut)
		{
			File.WriteAllBytes(bytes: wwwTexture.texture.EncodeToPNG(), path: localSocial.facebookProfilePhotoPath);
			UIMsgBox.instance.CloseMessage();
			RefreshPortrait();
		}
		else
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_27"), 1);
		}
		wwwTexture.Dispose();
	}

	public void FetchTwitterPhoto()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_22"), 7);
		StartCoroutine(_FetchTwitterPhoto());
	}

	private IEnumerator _FetchTwitterPhoto()
	{
		SocialPlayer localSocial = SocialManager.GetInstance().LocalSocialPlayer;
		string photoURL = "http://twitter.com/api/users/profile_image/" + localSocial.ttAccountId;
		WWW wwwTexture = new WWW(photoURL);
		float fetchStartTime = Time.time;
		bool isFetchTimeOut = false;
		while (!wwwTexture.isDone)
		{
			if (Time.time - fetchStartTime > 15f)
			{
				isFetchTimeOut = true;
				break;
			}
			yield return 0;
		}
		if (!isFetchTimeOut)
		{
			File.WriteAllBytes(bytes: wwwTexture.texture.EncodeToPNG(), path: localSocial.twitterProfilePhotoPath);
			UIMsgBox.instance.CloseMessage();
			RefreshPortrait();
		}
		else
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_27"), 1);
		}
		wwwTexture.Dispose();
	}
}
