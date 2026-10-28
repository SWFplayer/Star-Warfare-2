using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetSelection : MonoBehaviour
{
	public UILabel Value;

	public GameObject SelectionObj;

	public UISprite BlackBack;

	public GameObject RoomSetTemplateObj;

	public GameObject SelectionGrid;

	public int DefaultValue;

	private int mBackWidthStep = 70;

	private List<RoomSetTemplate> mSelections = new List<RoomSetTemplate>();

	private RoomSetTemplate mCurrentSelection;

	private int mSelectedValue;

	private bool mIsSelectionOpen;

	private void Awake()
	{
		mSelectedValue = DefaultValue;
		Value.text = mSelectedValue.ToString();
		SelectionObj.SetActive(false);
	}

	public void Init(int currentValue)
	{
		mIsSelectionOpen = false;
		SelectionObj.SetActive(false);
		foreach (RoomSetTemplate mSelection in mSelections)
		{
			UIEventListener uIEventListener = UIEventListener.Get(mSelection.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UnityEngine.Object.DestroyImmediate(mSelection.gameObject);
		}
		mSelections.Clear();
		mCurrentSelection = null;
		mSelectedValue = currentValue;
		Value.text = mSelectedValue.ToString();
	}

	public void OpenSelection(List<int> selections)
	{
		SelectionObj.SetActive(true);
		BlackBack.height = selections.Count * mBackWidthStep;
		if (selections.Count % 2 == 0)
		{
			BlackBack.transform.localPosition = Vector3.zero + Vector3.up * mBackWidthStep / 2f;
		}
		else
		{
			BlackBack.transform.localPosition = Vector3.zero;
		}
		foreach (RoomSetTemplate mSelection in mSelections)
		{
			UIEventListener uIEventListener = UIEventListener.Get(mSelection.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UnityEngine.Object.DestroyImmediate(mSelection.gameObject);
		}
		mSelections.Clear();
		mCurrentSelection = null;
		int num = selections.Count / 2;
		for (int i = 0; i < selections.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(RoomSetTemplateObj) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = SelectionGrid.transform;
			gameObject.transform.localPosition = Vector3.zero - Vector3.up * (i - num) * mBackWidthStep;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIEventListener uIEventListener2 = UIEventListener.Get(gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			RoomSetTemplate component = gameObject.GetComponent<RoomSetTemplate>();
			component.Init(selections[i]);
			if (selections[i] == mSelectedValue)
			{
				component.SetHighlight(true);
				mCurrentSelection = component;
			}
			mSelections.Add(component);
		}
		mIsSelectionOpen = true;
	}

	private void OnButtonClick(GameObject go)
	{
		foreach (RoomSetTemplate mSelection in mSelections)
		{
			if (!(go == mSelection.gameObject))
			{
				continue;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mSelection == mCurrentSelection)
			{
				ConfirmCurrent();
				break;
			}
			if (mCurrentSelection != null)
			{
				mCurrentSelection.SetHighlight(false);
			}
			mCurrentSelection = mSelection;
			mCurrentSelection.SetHighlight(true);
			break;
		}
	}

	public void ConfirmCurrent()
	{
		if (mCurrentSelection != null)
		{
			mSelectedValue = mCurrentSelection.GetValue();
		}
		Value.text = mSelectedValue.ToString();
		SelectionObj.SetActive(false);
		mIsSelectionOpen = false;
	}

	public int GetValue()
	{
		return mSelectedValue;
	}
}
