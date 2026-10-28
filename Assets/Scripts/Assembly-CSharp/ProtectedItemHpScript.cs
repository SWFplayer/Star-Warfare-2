using UnityEngine;

public class ProtectedItemHpScript : MonoBehaviour
{
	public UISprite m_CurrentHp;

	private void Awake()
	{
		m_CurrentHp.fillAmount = 1f;
	}

	public void SetHpFillAmount(float amount)
	{
		m_CurrentHp.fillAmount = amount;
	}
}
