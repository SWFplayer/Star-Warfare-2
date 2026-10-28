using System;
using UnityEngine;

public class ItemTween : MonoBehaviour
{
	public GameObject ItemBarBtn;

	public GameObject Items;

	public GameObject KillStreaks;

	private static ItemTween mInstance;

	private bool enableHide;

	private UserState state;

	private BoxCollider[] itemColliders;

	private BoxCollider[] killStreakColliders;

	private TweenPosition tween;

	private Vector3 position;

	private Timer tweenTimer = new Timer();

	private void Awake()
	{
		mInstance = this;
	}

	private void Start()
	{
		tween = base.gameObject.GetComponent<TweenPosition>();
		position = tween.to;
		UIEventListener uIEventListener = UIEventListener.Get(ItemBarBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		EventDelegate.Add(tween.onFinished, OnFinished);
		tweenTimer.SetTimer(5f, false);
		itemColliders = Items.GetComponentsInChildren<BoxCollider>();
		killStreakColliders = new BoxCollider[0];
		state = GameApp.GetInstance().GetUserState();
		enableHide = state.GetItemHide() && !GameApp.GetInstance().GetGameMode().IsVSMode();
	}

	public void RefreshAfterAddKillStreak()
	{
		killStreakColliders = KillStreaks.GetComponentsInChildren<BoxCollider>();
		tween.PlayForward();
	}

	public void Fresh()
	{
		enableHide = state.GetItemHide() && !GameApp.GetInstance().GetGameMode().IsVSMode();
	}

	private void Update()
	{
		if (tweenTimer.Ready() && base.gameObject.transform.localPosition == position && enableHide)
		{
			tween.PlayReverse();
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public static ItemTween GetInstance()
	{
		return mInstance;
	}

	private void OnFinished()
	{
		tweenTimer.Do();
		if (base.gameObject.transform.localPosition == tween.from)
		{
			ItemBarBtn.SetActive(true);
			BoxCollider[] array = itemColliders;
			foreach (BoxCollider boxCollider in array)
			{
				if (boxCollider != null)
				{
					boxCollider.enabled = false;
				}
			}
			BoxCollider[] array2 = killStreakColliders;
			foreach (BoxCollider boxCollider2 in array2)
			{
				if (boxCollider2 != null)
				{
					boxCollider2.enabled = false;
				}
			}
			return;
		}
		ItemBarBtn.SetActive(false);
		BoxCollider[] array3 = itemColliders;
		foreach (BoxCollider boxCollider3 in array3)
		{
			if (boxCollider3 != null)
			{
				boxCollider3.enabled = true;
			}
		}
		BoxCollider[] array4 = killStreakColliders;
		foreach (BoxCollider boxCollider4 in array4)
		{
			if (boxCollider4 != null)
			{
				boxCollider4.enabled = true;
			}
		}
	}

	private void OnClick(GameObject go)
	{
		if (go == ItemBarBtn)
		{
			tween.PlayForward();
		}
	}

	public void AutoPopupWhenLowHp()
	{
		ItemButton[] componentsInChildren = Items.GetComponentsInChildren<ItemButton>();
		ItemButton[] array = componentsInChildren;
		foreach (ItemButton itemButton in array)
		{
			Item item = GameApp.GetInstance().GetUserState().GetItems()[itemButton.ItemID];
			if (item.IsHpRestore())
			{
				tween.PlayForward();
				break;
			}
		}
	}
}
