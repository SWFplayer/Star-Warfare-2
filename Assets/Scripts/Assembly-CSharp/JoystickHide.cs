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

        if (GameApp.GetInstance() != null)
        {
            state = GameApp.GetInstance().GetUserState();
        }
    }

    public static JoystickHide GetInstance()
    {
        return mInstance;
    }

    private void OnDestroy()
    {
        if (mInstance == this)
        {
            mInstance = null;
        }
    }

    private void Start()
    {
        if (inner != null) innerAlpha = inner.GetComponent<TweenAlpha>();
        if (outer != null) outerAlpha = outer.GetComponent<TweenAlpha>();

        if (innerAlpha != null) innerAlpha.duration = 2f;
        if (outerAlpha != null) outerAlpha.duration = 2f;

        pressTimer.SetTimer(1f, false);
        pressTimer.Pause();

        if (state == null && GameApp.GetInstance() != null)
        {
            state = GameApp.GetInstance().GetUserState();
        }

        if (state != null)
        {
            enableHide = state.GetAutoHide();
        }
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
        if (state == null && GameApp.GetInstance() != null)
        {
            state = GameApp.GetInstance().GetUserState();
        }

        if (state != null)
        {
            enableHide = state.GetAutoHide();
        }
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