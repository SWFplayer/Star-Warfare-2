using UnityEngine;

public class HookForceScript : MonoBehaviour
{
	private Vector3 beginPos;

	private float life = 3f;

	private float beginTime;

	private GameUnit owner;

	private float forceSpeed = 50f;

	private GameObject effect;

	public void Init(Vector3 pos, GameUnit target)
	{
		owner = target;
		pos.y--;
		beginPos = pos;
		beginTime = Time.time;
		float magnitude = (beginPos - owner.GetPosition()).magnitude;
		life = Mathf.Min(life, magnitude / (forceSpeed - owner.Speed));
		Object original = Resources.Load(EffectName.VD5_RUSH);
		effect = Object.Instantiate(original, base.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity) as GameObject;
		effect.transform.LookAt(beginPos);
		effect.transform.parent = base.gameObject.transform;
	}

	private void Update()
	{
		if (owner == null || !owner.InPlayingState())
		{
			Object.Destroy(effect);
			Object.Destroy(this);
			return;
		}
		if (Time.time - beginTime > life)
		{
			Object.Destroy(effect);
			Object.Destroy(this);
			return;
		}
		Vector3 speed = (beginPos - owner.GetPosition()).normalized * forceSpeed;
		owner.SimpleMove(speed);
		if ((owner.GetPosition() - beginPos).magnitude < 1f)
		{
			Object.Destroy(effect);
			Object.Destroy(this);
		}
	}
}
