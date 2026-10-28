using UnityEngine;

public class SwVoidExplodeCollider : MonoBehaviour
{
	public SwVoidDemoman mSwVoid;

	public bool isLocal;

	private void Awake()
	{
		base.gameObject.layer = PhysicsLayer.PROJECTILE;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isLocal && other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			return;
		}
		if (!isLocal && other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
		{
			if (!(other.gameObject == mSwVoid.GetPlayer().GetObject()))
			{
			}
		}
		else
		{
			mSwVoid.ColliderTouched(other.gameObject);
			base.enabled = false;
		}
	}
}
