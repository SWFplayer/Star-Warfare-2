using System;
using UnityEngine;

public class OptionUIScript : MonoBehaviour
{
	public GameObject ExitButton;

	public GameObject MainPage;

	public GameObject Tip;

	public GameObject TipLabel;

	public UIAtlas Atlas;

	public static OptionUIScript mInstance;

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
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(Tip);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (ItemTween.GetInstance() != null)
			{
				ItemTween.GetInstance().Fresh();
			}
			if (JoystickHide.GetInstance() != null)
			{
				JoystickHide.GetInstance().Fresh();
			}
			if (JoystickManager.GetInstance() != null)
			{
				JoystickManager.GetInstance().ChangeSide();
			}
			if (BossStickManager.GetInstance() != null)
			{
				BossStickManager.GetInstance().ChangeSide();
			}
			if (GameApp.GetInstance().GetGameWorld() != null)
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.InputController.ResetSensitivity(GameApp.GetInstance().GetUserState().TouchInputSensitivity);
			}
			if (StateAim.mInstance != null)
			{
				StateAim.mInstance.RefreshToggle();
			}
			GameApp.GetInstance().Save();
			Resources.UnloadAsset(Atlas.spriteMaterial);
			UnityEngine.Object.Destroy(base.gameObject);
			Resources.UnloadUnusedAssets();
		}
		else if (go == Tip)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			Tip.SetActive(false);
		}
	}

	public OptionMainPageScript GetMainPageScript()
	{
		return MainPage.GetComponent<OptionMainPageScript>();
	}

	public void SetIsInMainMenu(bool isMainMenu)
	{
		GetMainPageScript().SetEnable(isMainMenu);
	}

	public void SetTip(string loc)
	{
		TipLabel.GetComponent<UILabel>().text = loc;
		TipLabel.GetComponent<UILocalizationX>().Refresh();
		Tip.SetActive(true);
	}

	public void Exit()
	{
		OnButtonClick(ExitButton);
	}
}
