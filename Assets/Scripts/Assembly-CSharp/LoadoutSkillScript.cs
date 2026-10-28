using UnityEngine;

public class LoadoutSkillScript : MonoBehaviour
{
	public UISprite Background;

	public UISprite Frame;

	public UISprite Icon;

	public UISprite Locker;

	public UILabel LabelId;

	public UISprite EquipedFrame;

	public PvpSkill Skill;

	public ELoadoutSkillIconType IconType;

	private void Start()
	{
	}

	public void SetSelected()
	{
		if (Frame != null)
		{
			Frame.gameObject.SetActive(true);
		}
	}

	public void SetUnselected()
	{
		if (Frame != null)
		{
			Frame.gameObject.SetActive(false);
		}
	}

	public bool IsSelected()
	{
		if (Frame != null)
		{
			return Frame.gameObject.activeSelf;
		}
		return false;
	}

	public bool IsLocked()
	{
		if (Locker != null)
		{
			return Locker.gameObject.activeSelf;
		}
		return false;
	}

	public void SetLocked()
	{
		if (Locker != null)
		{
			Locker.gameObject.SetActive(true);
		}
		if (Background != null)
		{
			Background.alpha = 0.5f;
		}
		if (Icon != null)
		{
			Icon.alpha = 0.5f;
		}
		if (LabelId != null)
		{
			LabelId.alpha = 0.5f;
		}
	}

	public void SetUnlocked()
	{
		if (Locker != null)
		{
			Locker.gameObject.SetActive(false);
		}
		if (Background != null)
		{
			Background.alpha = 1f;
		}
		if (Icon != null)
		{
			Icon.alpha = 1f;
		}
		if (LabelId != null)
		{
			LabelId.alpha = 1f;
		}
	}

	public void SetSkill(PvpSkill skill)
	{
		Skill = skill;
		if (skill == null || skill.GetId() == 0)
		{
			if (Icon != null)
			{
				Icon.gameObject.SetActive(false);
			}
			if (LabelId != null)
			{
				LabelId.gameObject.SetActive(true);
			}
			return;
		}
		if (Icon != null)
		{
			Icon.gameObject.SetActive(true);
			if (skill != null)
			{
				Icon.spriteName = skill.GetIconName();
			}
		}
		if (LabelId != null)
		{
			LabelId.gameObject.SetActive(false);
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public bool isEmpty()
	{
		if (Skill == null)
		{
			return true;
		}
		return false;
	}
}
