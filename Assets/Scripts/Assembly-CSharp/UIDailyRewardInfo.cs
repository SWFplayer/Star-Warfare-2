using UnityEngine;

public class UIDailyRewardInfo : MonoBehaviour
{
	public UISprite iapIcon;

	public UISprite shareIcon;

	public UISprite weaponIcon;

	public UILabel amountLabel;

	public UILabel nameLabel;

	public UISprite rarity;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(byte rewardType, short rewardId, int rewardValue)
	{
		DailyBonusManager.GetInstance().SetInfoToUIRewardInfo(this, rewardType, rewardId, rewardValue);
	}

	public void SetIcon(string iconName, byte rewardType)
	{
		switch (rewardType)
		{
		case 1:
		case 2:
			iapIcon.gameObject.SetActive(true);
			shareIcon.gameObject.SetActive(false);
			weaponIcon.gameObject.SetActive(false);
			iapIcon.spriteName = iconName;
			break;
		case 3:
		case 6:
			iapIcon.gameObject.SetActive(false);
			shareIcon.gameObject.SetActive(true);
			weaponIcon.gameObject.SetActive(false);
			shareIcon.spriteName = iconName;
			break;
		default:
			iapIcon.gameObject.SetActive(false);
			shareIcon.gameObject.SetActive(false);
			weaponIcon.gameObject.SetActive(true);
			weaponIcon.spriteName = iconName;
			weaponIcon.MakePixelPerfect();
			break;
		}
	}

	public void SetRarity(bool isShow, string spriteName)
	{
		if (isShow)
		{
			rarity.gameObject.SetActive(true);
			rarity.spriteName = spriteName;
		}
		else
		{
			rarity.gameObject.SetActive(false);
		}
	}

	public void SetValue(int amount)
	{
		amountLabel.text = "X" + amount;
	}

	public void SetName(string rewardName)
	{
		nameLabel.text = rewardName;
	}
}
