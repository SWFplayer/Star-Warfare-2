using UnityEngine;

public class AchievementSlotScript : MonoBehaviour
{
	public GameObject AchievementObject;

	public GameObject CompleteLabel;

	public GameObject ProgressLabel;

	public GameObject NameLabel;

	public GameObject DescriptionLabel;

	public UISlider Slider;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetComplete(int Id)
	{
		if (Id != 0)
		{
			CompleteLabel.GetComponent<UILabel>().text = GameApp.GetInstance().GetUserState().TitleDictionary[Id];
		}
		CompleteLabel.SetActive(true);
		ProgressLabel.SetActive(false);
		Slider.value = 1f;
	}

	public void SetProgress(int[] progress, int Id)
	{
		if (progress[0] < progress[1])
		{
			CompleteLabel.SetActive(false);
			ProgressLabel.SetActive(true);
			ProgressLabel.GetComponent<UILabel>().text = progress[0].ToString() + '/' + progress[1].ToString();
			Slider.value = (float)progress[0] / (float)progress[1];
		}
		else
		{
			SetComplete(Id);
		}
	}

	public void HideAchievement()
	{
		AchievementObject.SetActive(false);
	}

	public void ShowAchievement()
	{
		AchievementObject.SetActive(true);
	}
}
