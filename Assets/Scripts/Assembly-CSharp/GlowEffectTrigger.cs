using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GlowEffectTrigger : MonoBehaviour
{
	public GlowEffectParameter m_Parameters;

	public bool m_StartAtFirst;

	private void Awake()
	{
		base.gameObject.layer = PhysicsLayer.TRIGGER;
		if (m_StartAtFirst)
		{
			PostProcessRender.GetInstance().SetTargetGlowEffect(m_Parameters);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			PostProcessRender.GetInstance().SetTargetGlowEffect(m_Parameters);
		}
	}
}
