using UnityEngine;

public class StateCombo : MonoBehaviour
{
	public UILabel m_Number;

	public UISprite m_ComboSprite;

	private Player player;

	private int prevCombo;

	private float comboZoom;

	private float effectTime;

	private void Awake()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		m_Number.gameObject.SetActive(false);
		m_ComboSprite.gameObject.SetActive(false);
		prevCombo = 0;
		comboZoom = 1.8f;
		effectTime = 0.5f;
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (player == null)
		{
			return;
		}
		int combo = player.GetCombo();
		if (combo > 1)
		{
			m_Number.gameObject.SetActive(true);
			m_ComboSprite.gameObject.SetActive(true);
			m_Number.text = string.Format("{0:D2}", combo);
			if (combo >= 2 && prevCombo != combo)
			{
				GameObject gameObject = Object.Instantiate(m_Number.gameObject, m_Number.transform.position, m_Number.transform.rotation) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				TweenScale.Begin(gameObject, effectTime, comboZoom * Vector3.one);
				TweenAlpha.Begin(gameObject, effectTime, 0f);
				gameObject.AddComponent<AutoDestroyScript>();
				gameObject.GetComponent<AutoDestroyScript>().life = effectTime;
				GameObject gameObject2 = Object.Instantiate(m_ComboSprite.gameObject, m_ComboSprite.transform.position, m_ComboSprite.transform.rotation) as GameObject;
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.localScale = Vector3.one;
				TweenScale.Begin(gameObject2, effectTime, comboZoom * Vector3.one);
				TweenAlpha.Begin(gameObject2, effectTime, 0f);
				gameObject2.AddComponent<AutoDestroyScript>();
				gameObject2.GetComponent<AutoDestroyScript>().life = effectTime;
			}
			prevCombo = combo;
		}
		else
		{
			prevCombo = 0;
			m_Number.gameObject.SetActive(false);
			m_ComboSprite.gameObject.SetActive(false);
		}
	}
}
