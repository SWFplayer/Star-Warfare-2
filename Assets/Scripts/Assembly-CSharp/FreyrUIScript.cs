using UnityEngine;

public class FreyrUIScript : UIStateManager, UIHandler
{
	private UIImage splashImg;

	private float startTime;

	protected Timer fadeTimer = new Timer();

	private void Awake()
	{
	}

	private void Start()
	{
		Time.timeScale = 1f;
		InitUIManager();
		FrGoToPhase(0, false, true, true);
	}

	private void InitUIManager()
	{
		Res2DManager.GetInstance().Init(2);
		m_UIManager = base.gameObject.AddComponent<UIManager>() as UIManager;
		m_UIManager.SetParameter(24, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIPopupManager = GetPopup();
		m_UIPopupManager.SetParameter(24, 3, false);
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject == null)
		{
			GameObject original = Resources.Load(AudioName.BGM_MENU_SINGLE) as GameObject;
			gameObject = Object.Instantiate(original, new Vector3(-0.5f, 1f, 0f), Quaternion.identity) as GameObject;
			gameObject.name = AudioName.MENU_MUSIC_NAME;
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MenuMusicScript>();
		}
	}

	protected override void FrInit(int phase)
	{
		switch (phase)
		{
		case 0:
		{
			m_UIManager.RemoveAll();
			m_UIManager.transform.position = Vector3.up * 10000f;
			UnitUI ui = Res2DManager.GetInstance().vUI[2];
			splashImg = new UIImage();
			splashImg.AddObject(ui, 0, 0);
			splashImg.Rect = splashImg.GetObjectRect();
			splashImg.Enable = true;
			m_UIManager.Add(splashImg);
			FadeAnimationScript.GetInstance().FadeOutBlack();
			fadeTimer.Name = "Logo";
			fadeTimer.SetTimer(0.5f, false);
			break;
		}
		case 19:
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.Name = "Logo";
			fadeTimer.SetTimer(0.5f, false);
			break;
		}
	}

	protected override void FrClose(int phase)
	{
		switch (phase)
		{
		}
	}

	protected override void FrUpdate(int phase)
	{
		switch (phase)
		{
		case 0:
			FrGoToPhase(18, false, false, false);
			break;
		case 1:
			if (Res2DManager.GetInstance().LoadResPro())
			{
				FrGoToPhase(FrGetPreviousPhase(), true, false, false);
			}
			break;
		case 18:
		{
			if (!FadeAnimationScript.GetInstance().FadeOutComplete())
			{
				break;
			}
			UITouchInner[] array = iPhoneInputMgr.MockTouches();
			foreach (UITouchInner touch in array)
			{
				if (!(m_UIManager != null) || m_UIManager.HandleInput(touch))
				{
				}
			}
			break;
		}
		case 19:
			if (FadeAnimationScript.GetInstance().FadeInComplete())
			{
				FrFree();
				Application.LoadLevel("MainMenu");
			}
			break;
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == splashImg && command == 0)
		{
			FrGoToPhase(19, false, false, false);
		}
	}

	public void StartLoading(byte mode, int nMax)
	{
		Res2DManager.GetInstance().LoadResInit(mode, nMax);
		FrGoToPhase(1, false, true, false);
	}

	public override void FrFree()
	{
		Res2DManager.GetInstance().FreeUI(2, true);
		m_UIManager.RemoveAll();
		UnitUI ui = Res2DManager.GetInstance().vUI[22];
		UIImage uIImage = new UIImage();
		uIImage.AddObject(ui, 0, 0);
		uIImage.Rect = uIImage.GetObjectRect();
		uIImage.SetColor(Color.black);
		uIImage.SetSize(new Vector2(UIConstant.ScreenLocalWidth, UIConstant.ScreenLocalHeight));
		m_UIManager.Add(uIImage);
	}
}
