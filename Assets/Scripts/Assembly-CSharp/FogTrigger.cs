using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FogTrigger : MonoBehaviour
{
	public float m_fadeDuration = 2f;

	public bool m_fog = true;

	public bool m_fogChange;

	public Color32 m_fogColor = Color.white;

	public FogMode m_fogMode;

	public float m_fogDensity = 0.001f;

	public float m_linearFogStart;

	public float m_linerFogEnd = 300f;

	private Bounds mVolumeBounds;

	public bool changeDirectionLight;

	public Color mDirectionLightColor;

	public bool changeAmbientColor;

	public Color mAmbientColor;

	public List<GameObject> thingsToHide;

	public List<GameObject> thingsToShow;

	private bool hasChecked;

	public bool isOutRestore;

	public Color m_originDircetionalColor;

	public Color m_originAmbientColor;

	private void Awake()
	{
		base.gameObject.layer = PhysicsLayer.TRIGGER;
		mVolumeBounds = base.GetComponent<Collider>().bounds;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != PhysicsLayer.PLAYER)
		{
			return;
		}
		if (m_fogChange)
		{
			FogRender.GetInstance().SetTargetFog(this);
		}
		if (changeDirectionLight)
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeDirectionalLightColor(mDirectionLightColor);
		}
		foreach (GameObject item in thingsToHide)
		{
			if (item != null)
			{
				item.SetActive(false);
			}
		}
		foreach (GameObject item2 in thingsToShow)
		{
			if (item2 != null)
			{
				item2.SetActive(true);
			}
		}
		if (changeAmbientColor)
		{
			FogRender.GetInstance().SetAmbientColor(mAmbientColor);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (isOutRestore && other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			if (changeDirectionLight)
			{
				GameApp.GetInstance().GetGameWorld().GetCamera()
					.ChangeDirectionalLightColor(m_originDircetionalColor);
			}
			if (changeAmbientColor)
			{
				FogRender.GetInstance().SetAmbientColor(m_originAmbientColor);
			}
		}
	}

	private void OnDisable()
	{
		FogRender.GetInstance().Disable();
	}
}
