using UnityEngine;

public class EnemyShaderLightChange : MonoBehaviour
{
	public float Speed = 1f;

	private Material mat;

	private float value;

	private float randomBeginValue;

	private void Start()
	{
		mat = base.GetComponent<Renderer>().material;
		if (mat == null)
		{
			base.enabled = false;
		}
		else if (mat.HasProperty("_lightRate"))
		{
			mat.SetFloat("_lightRate", value);
			randomBeginValue = Random.RandomRange(0f, Speed);
		}
		else
		{
			Debug.Log("the enemy shader has no property : _lightRate ");
			base.enabled = false;
		}
	}

	private void Update()
	{
		value = Mathf.PingPong(Time.time * Speed + randomBeginValue, 1f);
		if (mat.HasProperty("_lightRate"))
		{
			mat.SetFloat("_lightRate", value);
		}
	}
}
