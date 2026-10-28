using UnityEngine;

public class StateInfoBox : MonoBehaviour
{
	public UIInfoBox m_UIInfoBox;

	public UIInfoBox m_UIPrivateAchieveBox;

	public UIInfoBox m_UIBroadcastingBox;

	public UIInfoBox m_UIWarningBox;

	public UIInfoBox m_UIChangeTeamBox;

	public UIInfoBox m_UIMutantBox;

	private void Update()
	{
		UserStateHUD.InfoBoxHUD.InfoHUD infoHUD = UserStateHUD.GetInstance().InfoBox.PopInfo();
		if (infoHUD != null)
		{
			if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.PopInfo)
			{
				m_UIInfoBox.AddInfo(infoHUD.StrInfo1, infoHUD.StrInfo2, infoHUD.IconString);
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.PrivateAchieve)
			{
				m_UIPrivateAchieveBox.AddInfo(infoHUD.StrInfo1, infoHUD.NumberInfo.ToString());
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.PrivateAchieveWithoutNum)
			{
				m_UIPrivateAchieveBox.AddInfo(infoHUD.StrInfo1, string.Empty);
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.Broadcasting)
			{
				m_UIBroadcastingBox.AddInfo(infoHUD.StrInfo1, string.Empty);
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.Warning)
			{
				m_UIWarningBox.AddInfo(infoHUD.StrInfo1, string.Empty);
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.ChangeTeam)
			{
				m_UIChangeTeamBox.AddInfo(infoHUD.StrInfo1, string.Empty);
			}
			else if (infoHUD.GetType == UserStateHUD.InfoBoxHUD.InfoHUD.InfoType.Mutant)
			{
				m_UIMutantBox.AddInfo(infoHUD.StrInfo1, string.Empty);
			}
		}
	}
}
