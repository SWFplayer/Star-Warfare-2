using UnityEngine;

public class GetBattleStateResponse : Response
{
	protected IBattleState[] battleStates;

	public override void ReadData(byte[] data)
	{
		BytesBuffer buffer = new BytesBuffer(data);
		battleStates = new IBattleState[3];
		battleStates[0] = BattleStateFactory.GetInstance().Create(GMBattleState.GM_COOP_STATE);
		battleStates[1] = BattleStateFactory.GetInstance().Create(GMBattleState.GM_TDM_STATE);
		battleStates[2] = BattleStateFactory.GetInstance().Create(GMBattleState.GM_FFA_STATE);
		for (int i = 0; i < battleStates.Length; i++)
		{
			battleStates[i].ReadFromBuffer(buffer);
		}
	}

	public override void ProcessLogic()
	{
		GameObject gameObject = GameObject.Find("StartMenu");
		if (!(gameObject != null))
		{
		}
	}

	public void ResetBattleStateFromNet()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
	}
}
