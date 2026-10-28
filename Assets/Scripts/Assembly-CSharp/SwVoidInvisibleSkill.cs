using UnityEngine;

public class SwVoidInvisibleSkill : MonoBehaviour
{
	public UISprite m_Button;

	public GameObject m_Background;

	private SwVoid m_SwVoid;

	private ESwVoidType m_Type;

	private Timer checkTimer;

	private void OnClick()
	{
		m_SwVoid.ActiveSkill();
	}

	private void Start()
	{
		checkTimer = new Timer();
		checkTimer.SetTimer(0.2f, true);
	}

	private void Update()
	{
		if (m_SwVoid.IsCoolDown())
		{
			base.GetComponent<Collider>().enabled = true;
			if (Input.GetButton("SwVoidSkill") && checkTimer.Ready())
			{
				checkTimer.Do();
				OnClick();
			}
			return;
		}
		base.GetComponent<Collider>().enabled = false;
		Timer timer = m_SwVoid.GetTimer(1);
		if (!timer.Ready())
		{
			m_Button.fillAmount = 1f - timer.GetTimeSpan() / timer.GetInterval();
			return;
		}
		Timer timer2 = m_SwVoid.GetTimer(0);
		if (!timer2.Ready())
		{
			m_Button.fillAmount = (timer2.GetTimeSpan() - timer.GetTimeSpan()) / (timer2.GetInterval() - timer.GetInterval());
		}
	}

	public void SetSwVoid(SwVoid swVoid)
	{
		m_SwVoid = swVoid;
	}
}
