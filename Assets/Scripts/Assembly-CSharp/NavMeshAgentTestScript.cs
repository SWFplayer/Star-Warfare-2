using UnityEngine;

public class NavMeshAgentTestScript : MonoBehaviour
{
	public Transform Target;

	private Timer mCheckTimer = new Timer();

	private UnityEngine.AI.NavMeshAgent mAgent;

	private void Start()
	{
		mCheckTimer.SetTimer(1f, false);
		mAgent = base.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	private void Update()
	{
		if (mCheckTimer.Ready())
		{
			mCheckTimer.Do();
			mAgent.SetDestination(Target.position);
		}
	}
}
