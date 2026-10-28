using UnityEngine;

public class EnemyAnimationTest : MonoBehaviour
{
	public UnityEngine.AI.NavMeshAgent mAgent;

	public Animation mAnimation;

	public string mAnimationName;

	private float mLastCheckTime;

	private void Start()
	{
		mLastCheckTime = Time.time;
		mAnimation[mAnimationName].wrapMode = WrapMode.Loop;
	}

	private void Update()
	{
		mAnimation.CrossFade(mAnimationName);
		if (Time.time - mLastCheckTime > 0.5f)
		{
			mLastCheckTime = Time.time;
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if ((base.transform.position - localPlayer.GetTransform().position).sqrMagnitude > 25f)
			{
				mAgent.SetDestination(localPlayer.GetTransform().position);
			}
		}
	}
}
