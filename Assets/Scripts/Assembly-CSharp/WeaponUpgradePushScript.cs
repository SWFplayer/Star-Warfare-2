using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradePushScript : MonoBehaviour, UIMsgListener
{
	public GameObject ExitButton;

	public UIDraggablePanel PlanPanel;

	public UIGrid PlanGrid;

	public GameObject PlanTemplate;

	public UIGrid BarGrid;

	public GameObject BarTemplate;

	public UISprite LeftArrow;

	public UISprite RightArrow;

	public UICenterOnChild CenterOnChild;

	private Weapon mTargetWeapon;

	private WeaponUpgradePlanScript mSelectedPlan;

	private List<WeaponUpgradePlanScript> mPlanList = new List<WeaponUpgradePlanScript>();

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(LeftArrow.gameObject);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(RightArrow.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		CenterOnChild.onFinished = DragFinished;
		LeftArrow.gameObject.SetActive(false);
		RightArrow.gameObject.SetActive(false);
		mSelectedPlan = null;
	}

	public void Init(Weapon targetWeapon)
	{
		mTargetWeapon = targetWeapon;
		if (mTargetWeapon != null)
		{
			int num = Global.MAX_LEVEL_WEAPONW - mTargetWeapon.Level;
			int num2 = 0;
			for (int i = 0; i < Global.WEAPON_UPGRADE_PUSH_UP_LEVELS.Count; i++)
			{
				if (num >= Global.WEAPON_UPGRADE_PUSH_UP_LEVELS[i])
				{
					num2++;
				}
			}
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PlanTemplate) as GameObject;
					gameObject.name = "Plan0" + j;
					gameObject.transform.parent = PlanGrid.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(BarTemplate) as GameObject;
					gameObject2.name = "Bar0" + j;
					gameObject2.transform.parent = BarGrid.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.localScale = Vector3.one;
					WeaponUpgradePlanScript component = gameObject.GetComponent<WeaponUpgradePlanScript>();
					component.Init(mTargetWeapon, mTargetWeapon.Level, mTargetWeapon.Level + Global.WEAPON_UPGRADE_PUSH_UP_LEVELS[j], Global.WEAPON_UPGRADE_PUSH_OFF[j], this);
					component.Bar = gameObject2.GetComponent<UISprite>();
					mPlanList.Add(component);
				}
				PlanGrid.Reposition();
				BarGrid.Reposition();
				Vector3 localPosition = BarGrid.transform.localPosition;
				float num3 = BarGrid.cellWidth * (float)(mPlanList.Count - 1) + (float)mPlanList[mPlanList.Count - 1].Bar.width;
				BarGrid.transform.localPosition = new Vector3((0f - num3) / 2f, localPosition.y, localPosition.z);
				CenterOnChild.Recenter();
			}
			else
			{
				Close();
			}
		}
		else
		{
			Close();
		}
	}

	private void DragFinished()
	{
		if (!(CenterOnChild.centeredObject != null))
		{
			return;
		}
		WeaponUpgradePlanScript component = CenterOnChild.centeredObject.GetComponent<WeaponUpgradePlanScript>();
		if (component != null)
		{
			if (mSelectedPlan != null)
			{
				mSelectedPlan.Bar.spriteName = "move_n";
			}
			mSelectedPlan = component;
			mSelectedPlan.Bar.spriteName = "move_u";
			int num = mPlanList.IndexOf(mSelectedPlan);
			LeftArrow.gameObject.SetActive(num > 0);
			RightArrow.gameObject.SetActive(num < mPlanList.Count - 1);
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PROMO_DESC_12"), 2, 43);
		}
		else if (go == LeftArrow.gameObject)
		{
			PlanPanel.Scroll(-2.8f);
		}
		else if (go == RightArrow.gameObject)
		{
			PlanPanel.Scroll(2.8f);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 43)
		{
			UIMsgBox.instance.CloseMessage();
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Ok:
				Close();
				break;
			}
		}
		else if (whichMsg.EventId == 3)
		{
			UIMsgBox.instance.CloseMessage();
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
			}
		}
	}

	public void Close()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	private void OnDestroy()
	{
		GameApp.GetInstance().GetUserState().ClearWeaponUpgradeID();
	}
}
