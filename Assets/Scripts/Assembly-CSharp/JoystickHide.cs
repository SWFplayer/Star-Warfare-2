using UnityEngine;

public class JoystickHide : MonoBehaviour
{
	public GameObject inner;

	public GameObject outer;

	private TweenAlpha innerAlpha;

	private TweenAlpha outerAlpha;

	private Timer pressTimer = new Timer();

	private UserState state;

	private bool enableHide;

	private static JoystickHide mInstance;

	private void Awake()
	{
		mInstance = this;
	}

	public static JoystickHide GetInstance()
	{
		return mInstance;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		innerAlpha = inner.GetComponent<TweenAlpha>();
		outerAlpha = outer.GetComponent<TweenAlpha>();
		innerAlpha.duration = 2f;
		outerAlpha.duration = 2f;
		pressTimer.SetTimer(1f, false);
		pressTimer.Pause();
		state = GameApp.GetInstance().GetUserState();
		enableHide = state.GetAutoHide();
	}

	private void OnPress(bool isPressed)
	{
		if (!isPressed)
		{
			innerAlpha.PlayForward();
			outerAlpha.PlayForward();
		}
		else if (enableHide)
		{
			pressTimer.Do();
		}
	}

	public void Fresh()
	{
		enableHide = state.GetAutoHide();
	}

	private void Update()
	{
		if (pressTimer.Ready() && innerAlpha != null && outerAlpha != null && enableHide)
		{
			innerAlpha.PlayReverse();
			outerAlpha.PlayReverse();
			pressTimer.Pause();
		}
	}
}
