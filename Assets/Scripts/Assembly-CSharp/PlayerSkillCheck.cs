using UnityEngine;

public class PlayerSkillCheck : MonoBehaviour
{
	public bool checkLocalPlayer;

	private PlayerSkill playerSkill;

	private void Start()
	{
		playerSkill = GameApp.GetInstance().GetUserState().playerSkill;
	}

	private void Update()
	{
		if (checkLocalPlayer)
		{
			checkLocalPlayer = false;
			Debug.Log(playerSkill.GetInfoString());
		}
	}
}
