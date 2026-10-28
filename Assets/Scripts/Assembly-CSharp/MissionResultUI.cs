using System.Text.RegularExpressions;
using UnityEngine;

public class MissionResultUI : MonoBehaviour
{
	public UILabel Description;

	public UISprite StarBackgroundIcon;

	public UISprite StarForewardIcon;

	public UISprite RewardCurrency;

	public UILabel RewardAmount;

	public UISprite ClearedMark;

	public UISprite Background;

	public GameObject RewardObj;

	private bool mIsFirstTimeComplete;

	public Mission mMission { get; set; }

	private void Awake()
	{
		StarBackgroundIcon.gameObject.SetActive(false);
		RewardObj.SetActive(false);
	}

	public void Init(Mission mission, bool isFirstTimeComplete)
	{
		mMission = mission;
		mIsFirstTimeComplete = isFirstTimeComplete;
		int rewardCash = mMission.GetRewardCash();
		int rewardMithril = mMission.GetRewardMithril();
		if (mission.IsMainMission)
		{
			Description.text = "- " + Regex.Replace(mMission.SummaryText, "[\\n\\r]", string.Empty);
		}
		else
		{
			Description.text = "- " + mMission.SummaryText;
		}
		if (rewardCash > 0)
		{
			RewardCurrency.gameObject.SetActive(true);
			RewardCurrency.spriteName = "icon_dollar1";
			RewardCurrency.MakePixelPerfect();
			if (isFirstTimeComplete)
			{
				RewardAmount.text = UIConstant.CURRENCY_COLOR_GOLD_PREFIX + rewardCash + UIConstant.COLOR_POSTFIX;
				ClearedMark.gameObject.SetActive(false);
			}
			else
			{
				RewardAmount.gameObject.SetActive(false);
				ClearedMark.gameObject.SetActive(true);
			}
		}
		else if (rewardMithril > 0)
		{
			RewardCurrency.gameObject.SetActive(true);
			RewardCurrency.spriteName = "icon_mithril1";
			RewardCurrency.MakePixelPerfect();
			if (isFirstTimeComplete)
			{
				RewardAmount.text = UIConstant.CURRENCY_COLOR_MITHRIL_PREFIX + rewardMithril + UIConstant.COLOR_POSTFIX;
				ClearedMark.gameObject.SetActive(false);
			}
			else
			{
				RewardAmount.gameObject.SetActive(false);
				ClearedMark.gameObject.SetActive(true);
			}
		}
		if (mMission.IsComplete())
		{
			if (!isFirstTimeComplete)
			{
				StarBackgroundIcon.spriteName = "star_u";
			}
		}
		else
		{
			Description.color = new Color(0.63f, 0.63f, 0.63f);
			Description.alpha = 0.5f;
			Background.spriteName = "mission_bk2";
			Background.alpha = 0.5f;
		}
	}

	public void ShowReward()
	{
		RewardObj.SetActive(true);
	}

	public bool IsFirstTimeCompleted()
	{
		return mIsFirstTimeComplete;
	}
}
