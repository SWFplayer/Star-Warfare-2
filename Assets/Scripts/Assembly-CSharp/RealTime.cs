using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RealTime : MonoBehaviour
{
    private static RealTime mInst;

    private float mRealTime;
    private float mRealDelta;

    private float CurrentRealtime
    {
        get
        {
#if UNITY_EDITOR
            
            return Application.isPlaying ? Time.realtimeSinceStartup : (float)UnityEditor.EditorApplication.timeSinceStartup;
#else
            
            return Time.realtimeSinceStartup;
#endif
        }
    }

    public static float time
    {
        get
        {
            if (mInst == null) Spawn();
            return mInst.mRealTime;
        }
    }

    public static float deltaTime
    {
        get
        {
            if (mInst == null) Spawn();
            return mInst.mRealDelta;
        }
    }

    private static void Spawn()
    {
        GameObject go = new GameObject("_RealTime");

        if (Application.isPlaying)
        {
            Object.DontDestroyOnLoad(go);
        }
        else
        {
            go.hideFlags = HideFlags.HideAndDontSave;
        }

        mInst = go.AddComponent<RealTime>();
        mInst.mRealTime = mInst.CurrentRealtime;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            float current = CurrentRealtime;
            mRealDelta = current - mRealTime;
            mRealTime = current;
            return;
        }

        float realtimeSinceStartup = Time.realtimeSinceStartup;
        mRealDelta = realtimeSinceStartup - mRealTime;
        mRealTime = realtimeSinceStartup;
    }

    private void OnDestroy()
    {
        if (mInst == this)
        {
            mInst = null;
        }
    }
}

