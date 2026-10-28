public class BattleSubHUD : GameSubUI
{
	private HUDBase mHUDManager;

	protected override void OnCreate()
	{
		mHUDManager = GetGameUI() as HUDBase;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		UIDialogData uIDialogData = UserStateUI.GetInstance().FightDialogManager.PopFightDialogData();
		if (uIDialogData != null && mHUDManager is HUDManager)
		{
			(mHUDManager as HUDManager).m_FightDialog.ShowDialog(uIDialogData);
		}
	}
}
