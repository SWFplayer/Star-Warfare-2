public abstract class GameUI : ComponentUI
{
	private GameUIStatus mUIStatus;

	private GameUIBundle mGameUIBundle;

	private GameSubUI mGameSubUI;

	private GameSubUI mNextGameSubUI;

	private bool mIsRemoveSubUI;

	private GameUIListener mGameUIListener;

	public GameUIStatus UIStatus
	{
		get
		{
			return mUIStatus;
		}
	}

	public void InitUI(GameUIStatus uiStatus)
	{
		mIsRemoveSubUI = false;
		mUIStatus = uiStatus;
	}

	public void SetListener(GameUIListener listener)
	{
		mGameUIListener = listener;
	}

	public void SetGameUIBundle(GameUIBundle bundle)
	{
		OnInit(bundle);
	}

	protected virtual void OnInit(GameUIBundle bundle)
	{
	}

	public void LoadSubUI(GameSubUI gameSubUI)
	{
		if (gameSubUI != null)
		{
			mNextGameSubUI = gameSubUI;
		}
	}

	public void RemoveSubUI()
	{
		mIsRemoveSubUI = true;
	}

	private void Update()
	{
		if (mIsRemoveSubUI)
		{
			if (mGameSubUI != null)
			{
				mGameSubUI.Destroy();
			}
			mIsRemoveSubUI = false;
		}
		if (mNextGameSubUI != null)
		{
			if (mGameSubUI != null)
			{
				mGameSubUI.Destroy();
			}
			mGameSubUI = mNextGameSubUI;
			mGameSubUI.Create(this);
			mNextGameSubUI = null;
		}
		else if (mGameSubUI != null)
		{
			mGameSubUI.Update();
		}
		OnUIUpdate();
	}

	protected virtual void OnUIUpdate()
	{
	}

	private void Destroy()
	{
		if (mGameSubUI != null)
		{
			mGameSubUI.Destroy();
		}
		OnUIDestroy();
		if (mGameUIListener != null)
		{
			mGameUIListener.OnMenuClose(mUIStatus);
		}
	}

	protected virtual void OnUIDestroy()
	{
	}

	private void OnEnable()
	{
		if (mGameSubUI != null)
		{
			mGameSubUI.Show();
		}
		OnShow();
	}

	protected virtual void OnShow()
	{
	}

	private void OnDisable()
	{
		if (mGameSubUI != null)
		{
			mGameSubUI.Hide();
		}
		OnHide();
	}

	protected virtual void OnHide()
	{
	}
}
