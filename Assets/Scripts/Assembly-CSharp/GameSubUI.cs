public abstract class GameSubUI
{
	public GameUI mGameUI;

	public void Create(GameUI gameUI)
	{
		mGameUI = gameUI;
		OnCreate();
	}

	protected abstract void OnCreate();

	public void Destroy()
	{
		OnDestroy();
		mGameUI = null;
	}

	protected virtual void OnDestroy()
	{
	}

	public void Update()
	{
		OnUpdate();
	}

	protected virtual void OnUpdate()
	{
	}

	protected GameUI GetGameUI()
	{
		return mGameUI;
	}

	public void Show()
	{
		OnShow();
	}

	protected virtual void OnShow()
	{
	}

	public void Hide()
	{
		OnHide();
	}

	protected virtual void OnHide()
	{
	}
}
