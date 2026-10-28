using UnityEngine;

public class IAPRestoreScript : MonoBehaviour
{
	public void RestoreIAPContent(string id)
	{
		if (id == "com.ifreyr.starwarfare.rookie099")
		{
			GameApp.GetInstance().GetUserState().DeliverIAPItem(IAPName.ROOKIE);
			Debug.Log("Deliver Rookie.");
		}
		else if (id == "com.ifreyr.starwarfare.sergeant299")
		{
			GameApp.GetInstance().GetUserState().DeliverIAPItem(IAPName.SERGEANT);
			Debug.Log("Deliver SERGEANT.");
		}
	}
}
