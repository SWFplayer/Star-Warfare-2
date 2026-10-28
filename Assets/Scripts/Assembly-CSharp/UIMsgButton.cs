using UnityEngine;

public class UIMsgButton : MonoBehaviour
{
	public UIMsg.UIMsgButton eventID;

	public UIMsg m_UIMsg;

	private void OnClick()
	{
		if (eventID == UIMsg.UIMsgButton.Ok)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		else if (eventID == UIMsg.UIMsgButton.Cancel || eventID == UIMsg.UIMsgButton.Never)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		}
		if (m_UIMsg != null)
		{
			m_UIMsg.HandleEvent(eventID);
		}
	}
}
