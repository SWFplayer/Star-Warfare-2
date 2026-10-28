using UnityEngine;

public class UIMsg : MonoBehaviour
{
	public enum UIMsgButton
	{
		Ok = 0,
		Cancel = 1,
		Never = 2
	}

	private class EmptyListener : UIMsgListener
	{
		public void OnMsgButtonClick(UIMsg whichMsg, UIMsgButton buttonId)
		{
		}
	}

	public UILabel m_Information;

	public TweenWidth m_BackTweenWidth;

	public TweenHeight m_BackTweenHight;

	private UIMsgListener listener;

	private BoxCollider m_block;

	private static UIMsgListener mEmpty;

	public static UIMsgListener EMPTY
	{
		get
		{
			return mEmpty;
		}
	}

	public byte EventId { get; set; }

	public bool IgnoreOthers { get; set; }

	public byte Type { get; set; }

	private void Awake()
	{
		mEmpty = new EmptyListener();
		if (listener == null)
		{
			listener = mEmpty;
		}
	}

	private void Start()
	{
		m_block = base.gameObject.GetComponent<Collider>() as BoxCollider;
		m_block.size = new Vector3(Screen.width, Screen.height, 0f);
	}

	private void OnEnable()
	{
		if (m_BackTweenWidth != null)
		{
			m_BackTweenWidth.Reset();
			m_BackTweenWidth.PlayForward();
		}
		if (m_BackTweenHight != null)
		{
			m_BackTweenHight.Reset();
			m_BackTweenHight.PlayForward();
		}
	}

	public void SetListener(UIMsgListener listener)
	{
		if (listener != null)
		{
			this.listener = null;
			this.listener = listener;
		}
	}

	public void HandleEvent(UIMsgButton eventID)
	{
		listener.OnMsgButtonClick(this, eventID);
	}
}
