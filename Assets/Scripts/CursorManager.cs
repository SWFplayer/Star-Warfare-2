using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static CursorManager _instance;

    public static CursorManager Instance
    {
        get
        {
            if (_instance == null) CreateInstance();
            return _instance;
        }
    }

    public bool isCursorLocked { get; private set; }
    
    public KeyCode toggleKey = KeyCode.F1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        CreateInstance();
    }

    private static void CreateInstance()
    {
        if (_instance != null) return;

        GameObject go = new GameObject("[CursorManager]");
        _instance = go.AddComponent<CursorManager>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
       
        if (Input.GetKeyDown(toggleKey))
        {
            if (isCursorLocked) UnlockCursor();
            else LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) UnlockCursor();
        else if (hasFocus && isCursorLocked) LockCursor();
    }
}
