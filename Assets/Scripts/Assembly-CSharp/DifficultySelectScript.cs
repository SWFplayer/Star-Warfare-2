using UnityEngine;

public class DifficultySelectScript : MonoBehaviour
{
	public UILabel Label;

	public UISprite[] Icons;

	public MapPanelScript m_ParentMapPanel;

	private void OnClick()
	{
		UISprite[] icons = Icons;
		foreach (UISprite uISprite in icons)
		{
			uISprite.gameObject.SetActive(false);
		}
		int difficultMode = (int)MissionSelectUIScript.m_Instance.m_difficultMode;
		difficultMode = (difficultMode + 1) % 2;
		if (difficultMode < Icons.Length)
		{
			Icons[difficultMode].gameObject.SetActive(true);
			Label.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_" + (difficultMode + 1));
			if (MissionSelectUIScript.m_Instance != null)
			{
				m_ParentMapPanel.ChangeDifficulty((EDifficultMode)difficultMode);
			}
		}
	}

	private void OnEnable()
	{
		UISprite[] icons = Icons;
		foreach (UISprite uISprite in icons)
		{
			uISprite.gameObject.SetActive(false);
		}
		if (MissionSelectUIScript.m_Instance != null)
		{
			int difficultMode = (int)MissionSelectUIScript.m_Instance.m_difficultMode;
			Icons[difficultMode].gameObject.SetActive(true);
			Label.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_" + (difficultMode + 1));
		}
	}
}
