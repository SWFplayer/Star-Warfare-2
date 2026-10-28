using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Panel")]
public class UIPanel : MonoBehaviour
{
    public enum DebugInfo
    {
        None = 0,
        Gizmos = 1,
        Geometry = 2
    }

    public delegate void OnChangeDelegate();

    public static BetterList<UIPanel> list = new BetterList<UIPanel>();

    public OnChangeDelegate onChange;

    public bool showInPanelTool = true;

    public bool generateNormals;

    public bool widgetsAreStatic;

    public bool cullWhileDragging;

    [SerializeField]
    public Matrix4x4 worldToLocal = Matrix4x4.identity;

    [SerializeField]

    private float mAlpha = 1f;

    [SerializeField]

    private UIDrawCall.Clipping mClipping;

    [SerializeField]

    private Vector4 mClipRange = Vector4.zero;

    [SerializeField]

    private Vector2 mClipSoftness = new Vector2(40f, 40f);


    [SerializeField]
    private int mDepth;

    private static bool mFullRebuild = false;

    private static BetterList<Vector3> mVerts = new BetterList<Vector3>();

    private static BetterList<Vector3> mNorms = new BetterList<Vector3>();

    private static BetterList<Vector4> mTans = new BetterList<Vector4>();

    private static BetterList<Vector2> mUvs = new BetterList<Vector2>();

    private static BetterList<Color32> mCols = new BetterList<Color32>();

    private GameObject mGo;

    private Transform mTrans;

    private Camera mCam;

    private int mLayer = -1;

    private float mCullTime;

    private float mUpdateTime;

    private float mMatrixTime;

    private static float[] mTemp = new float[4];

    private Vector2 mMin = Vector2.zero;

    private Vector2 mMax = Vector2.zero;

    private UIPanel[] mChildPanels;

    public GameObject cachedGameObject
    {
        get
        {
            if (mGo == null)
            {
                mGo = base.gameObject;
            }
            return mGo;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (mTrans == null)
            {
                mTrans = base.transform;
            }
            return mTrans;
        }
    }

    public float alpha
    {
        get
        {
            return mAlpha;
        }
        set
        {
            float num = Mathf.Clamp01(value);
            if (mAlpha == num)
            {
                return;
            }
            mAlpha = num;
            for (int i = 0; i < UIDrawCall.list.size; i++)
            {
                UIDrawCall uIDrawCall = UIDrawCall.list[i];
                if (uIDrawCall != null && uIDrawCall.panel == this)
                {
                    uIDrawCall.isDirty = true;
                }
            }
            for (int j = 0; j < UIWidget.list.size; j++)
            {
                UIWidget uIWidget = UIWidget.list[j];
                if (uIWidget.panel == this)
                {
                    uIWidget.MarkAsChangedLite();
                }
            }
        }
    }

    public int depth
    {
        get
        {
            return mDepth;
        }
        set
        {
            if (mDepth == value)
            {
                return;
            }
            mDepth = value;
            mFullRebuild = true;
            for (int i = 0; i < UIDrawCall.list.size; i++)
            {
                UIDrawCall uIDrawCall = UIDrawCall.list[i];
                if (uIDrawCall != null)
                {
                    uIDrawCall.isDirty = true;
                }
            }
            for (int j = 0; j < UIWidget.list.size; j++)
            {
                UIWidget.list[j].MarkAsChangedLite();
            }
            list.Sort(CompareFunc);
        }
    }

    public int drawCallCount
    {
        get
        {
            int num = 0;
            for (int i = 0; i < UIDrawCall.list.size; i++)
            {
                UIDrawCall uIDrawCall = UIDrawCall.list[i];
                if (!(uIDrawCall.panel != this))
                {
                    num++;
                }
            }
            return num;
        }
    }

    public UIDrawCall.Clipping clipping
    {
        get
        {
            return mClipping;
        }
        set
        {
            if (mClipping != value)
            {
                mClipping = value;
                mMatrixTime = 0f;
                UpdateDrawcalls();
            }
        }
    }

    public Vector4 clipRange
    {
        get
        {
            return mClipRange;
        }
        set
        {
            if (mClipRange != value)
            {
                mCullTime = ((mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f);
                mClipRange = value;
                mMatrixTime = 0f;
                UpdateDrawcalls();
            }
        }
    }

    public Vector2 clipSoftness
    {
        get
        {
            return mClipSoftness;
        }
        set
        {
            if (mClipSoftness != value)
            {
                mClipSoftness = value;
                UpdateDrawcalls();
            }
        }
    }

    public static int CompareFunc(UIPanel a, UIPanel b)
    {
        if (a != null && b != null)
        {
            if (a.mDepth < b.mDepth)
            {
                return -1;
            }
            if (a.mDepth > b.mDepth)
            {
                return 1;
            }
        }
        return 0;
    }

    public void SetAlphaRecursive(float val, bool rebuildList)
    {
        if (rebuildList || mChildPanels == null)
        {
            mChildPanels = GetComponentsInChildren<UIPanel>(true);
        }
        int i = 0;
        for (int num = mChildPanels.Length; i < num; i++)
        {
            mChildPanels[i].alpha = val;
        }
    }

    private bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        UpdateTransformMatrix();
        a = worldToLocal.MultiplyPoint3x4(a);
        b = worldToLocal.MultiplyPoint3x4(b);
        c = worldToLocal.MultiplyPoint3x4(c);
        d = worldToLocal.MultiplyPoint3x4(d);
        mTemp[0] = a.x;
        mTemp[1] = b.x;
        mTemp[2] = c.x;
        mTemp[3] = d.x;
        float num = Mathf.Min(mTemp);
        float num2 = Mathf.Max(mTemp);
        mTemp[0] = a.y;
        mTemp[1] = b.y;
        mTemp[2] = c.y;
        mTemp[3] = d.y;
        float num3 = Mathf.Min(mTemp);
        float num4 = Mathf.Max(mTemp);
        if (num2 < mMin.x)
        {
            return false;
        }
        if (num4 < mMin.y)
        {
            return false;
        }
        if (num > mMax.x)
        {
            return false;
        }
        if (num3 > mMax.y)
        {
            return false;
        }
        return true;
    }

    public bool IsVisible(Vector3 worldPos)
    {
        if (mAlpha < 0.001f)
        {
            return false;
        }
        if (mClipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        UpdateTransformMatrix();
        Vector3 vector = worldToLocal.MultiplyPoint3x4(worldPos);
        if (vector.x < mMin.x)
        {
            return false;
        }
        if (vector.y < mMin.y)
        {
            return false;
        }
        if (vector.x > mMax.x)
        {
            return false;
        }
        if (vector.y > mMax.y)
        {
            return false;
        }
        return true;
    }

    public bool IsVisible(UIWidget w)
    {
        if (mAlpha < 0.001f)
        {
            return false;
        }
        if (!w.enabled || !NGUITools.GetActive(w.cachedGameObject) || w.alpha < 0.001f)
        {
            return false;
        }
        if (mClipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        Vector3[] worldCorners = w.worldCorners;
        return IsVisible(worldCorners[0], worldCorners[1], worldCorners[2], worldCorners[3]);
    }

    public static void SetDirty()
    {
        mFullRebuild = true;
    }

    private UIDrawCall GetDrawCall(int index, Material mat)
    {
        if (index < UIDrawCall.list.size)
        {
            UIDrawCall uIDrawCall = UIDrawCall.list.buffer[index];
            if (uIDrawCall != null && uIDrawCall.panel == this && uIDrawCall.baseMaterial == mat && uIDrawCall.mainTexture == mat.mainTexture)
            {
                return uIDrawCall;
            }
            int num = UIDrawCall.list.size;
            while (num > index)
            {
                UIDrawCall dc = UIDrawCall.list.buffer[--num];
                DestroyDrawCall(dc, num);
            }
        }
        
        GameObject gameObject = new GameObject("_UIDrawCall [" + mat.name + "]");

        if (Application.isPlaying)
        {
            Object.DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        gameObject.layer = cachedGameObject.layer;
        UIDrawCall uIDrawCall2 = gameObject.AddComponent<UIDrawCall>();
        uIDrawCall2.baseMaterial = mat;
        uIDrawCall2.renderQueue = UIDrawCall.list.size;
        uIDrawCall2.panel = this;
        UIDrawCall.list.Add(uIDrawCall2);
        return uIDrawCall2;
    }

    private void Awake()
    {
        mGo = base.gameObject;
        mTrans = base.transform;
    }

    private void Start()
    {
        mLayer = mGo.layer;
        UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
        mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
    }

    private void OnEnable()
    {
        mFullRebuild = true;
        list.Add(this);
        list.Sort(CompareFunc);

        if (Application.isPlaying)
        {
            Refresh();
        }

    }

    private void OnDisable()
    {
        int num = UIDrawCall.list.size;
        while (num > 0)
        {
            UIDrawCall uIDrawCall = UIDrawCall.list.buffer[--num];
            if (uIDrawCall != null && uIDrawCall.panel == this)
            {
                DestroyDrawCall(uIDrawCall, num);
            }
        }
        list.Remove(this);
    }

    private void UpdateTransformMatrix()
    {
        if (mUpdateTime != 0f && mMatrixTime == mUpdateTime)
        {
            return;
        }
        mMatrixTime = mUpdateTime;
        worldToLocal = cachedTransform.worldToLocalMatrix;
        if (mClipping != UIDrawCall.Clipping.None)
        {
            Vector2 vector = new Vector2(mClipRange.z, mClipRange.w);
            if (vector.x == 0f)
            {
                vector.x = ((!(mCam == null)) ? mCam.pixelWidth : ((float)Screen.width));
            }
            if (vector.y == 0f)
            {
                vector.y = ((!(mCam == null)) ? mCam.pixelHeight : ((float)Screen.height));
            }
            vector *= 0.5f;
            mMin.x = mClipRange.x - vector.x;
            mMin.y = mClipRange.y - vector.y;
            mMax.x = mClipRange.x + vector.x;
            mMax.y = mClipRange.y + vector.y;
        }
    }

    private void UpdateDrawcalls()
    {
        Vector4 vector = Vector4.zero;
        if (mClipping != UIDrawCall.Clipping.None)
        {
            vector = new Vector4(mClipRange.x, mClipRange.y, mClipRange.z * 0.5f, mClipRange.w * 0.5f);
        }
        if (vector.z == 0f)
        {
            vector.z = (float)Screen.width * 0.5f;
        }
        if (vector.w == 0f)
        {
            vector.w = (float)Screen.height * 0.5f;
        }
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
        {
            vector.x -= 0.5f;
            vector.y += 0.5f;
        }
        Transform transform = cachedTransform;
        int num = 0;
        while (num < UIDrawCall.list.size)
        {
            UIDrawCall uIDrawCall = UIDrawCall.list.buffer[num];
            if (uIDrawCall == null)
            {
                UIDrawCall.list.RemoveAt(num);
                continue;
            }
            if (uIDrawCall.panel == this)
            {
                uIDrawCall.clipping = mClipping;
                uIDrawCall.clipRange = vector;
                uIDrawCall.clipSoftness = mClipSoftness;
                Transform transform2 = uIDrawCall.transform;
                transform2.position = transform.position;
                transform2.rotation = transform.rotation;
                transform2.localScale = transform.lossyScale;
            }
            num++;
        }
    }

    private void LateUpdate()
    {
        if (list[0] != this)
        {
            return;
        }
        for (int i = 0; i < list.size; i++)
        {
            UIPanel uIPanel = list[i];
            uIPanel.mUpdateTime = RealTime.time;
            uIPanel.UpdateTransformMatrix();
            uIPanel.UpdateLayers();
            uIPanel.UpdateWidgets();
        }
        if (mFullRebuild)
        {
            UIWidget.list.Sort(UIWidget.CompareFunc);
            Fill();
        }
        else
        {
            int num = 0;
            while (num < UIDrawCall.list.size)
            {
                UIDrawCall uIDrawCall = UIDrawCall.list[num];
                if (uIDrawCall.isDirty && !Fill(uIDrawCall))
                {
                    DestroyDrawCall(uIDrawCall, num);
                }
                else
                {
                    num++;
                }
            }
        }
        for (int j = 0; j < list.size; j++)
        {
            UIPanel uIPanel2 = list[j];
            uIPanel2.UpdateDrawcalls();
        }
        mFullRebuild = false;
    }

    private static void DestroyDrawCall(UIDrawCall dc, int index)
    {
        if (dc != null)
        {
            UIDrawCall.list.RemoveAt(index);
            NGUITools.DestroyImmediate(dc.gameObject);
        }
    }

    private void UpdateLayers()
    {
        if (mLayer == cachedGameObject.layer)
        {
            return;
        }
        mLayer = mGo.layer;
        UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
        mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
        SetChildLayer(cachedTransform, mLayer);
        int i = 0;
        for (int size = UIDrawCall.list.size; i < size; i++)
        {
            UIDrawCall uIDrawCall = UIDrawCall.list[i];
            if (uIDrawCall != null && uIDrawCall.panel == this)
            {
                uIDrawCall.gameObject.layer = mLayer;
            }
        }
    }

    private void UpdateWidgets()
    {
        bool forceVisible = !cullWhileDragging && (clipping == UIDrawCall.Clipping.None || mCullTime > mUpdateTime);
        bool flag = false;
        int i = 0;
        for (int size = UIWidget.list.size; i < size; i++)
        {
            UIWidget uIWidget = UIWidget.list[i];
            if (!uIWidget.enabled || !(uIWidget.panel == this) || !uIWidget.UpdateGeometry(this, forceVisible))
            {
                continue;
            }
            flag = true;
            if (!mFullRebuild)
            {
                UIDrawCall drawCall = uIWidget.drawCall;
                if (drawCall != null)
                {
                    drawCall.isDirty = true;
                }
                else
                {
                    mFullRebuild = true;
                }
            }
        }
        if (flag && onChange != null)
        {
            onChange();
        }
    }

    public void Refresh()
    {
        mFullRebuild = true;
        if (list.size > 0)
        {
            list[0].LateUpdate();
        }
    }

    public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
    {
        float num = clipRange.z * 0.5f;
        float num2 = clipRange.w * 0.5f;
        Vector2 minRect = new Vector2(min.x, min.y);
        Vector2 maxRect = new Vector2(max.x, max.y);
        Vector2 minArea = new Vector2(clipRange.x - num, clipRange.y - num2);
        Vector2 maxArea = new Vector2(clipRange.x + num, clipRange.y + num2);
        if (clipping == UIDrawCall.Clipping.SoftClip)
        {
            minArea.x += clipSoftness.x;
            minArea.y += clipSoftness.y;
            maxArea.x -= clipSoftness.x;
            maxArea.y -= clipSoftness.y;
        }
        return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
    }

    public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
    {
        Vector3 vector = CalculateConstrainOffset(targetBounds.min, targetBounds.max);
        if (vector.magnitude > 0f)
        {
            if (immediate)
            {
                target.localPosition += vector;
                targetBounds.center += vector;
                SpringPosition component = target.GetComponent<SpringPosition>();
                if (component != null)
                {
                    component.enabled = false;
                }
            }
            else
            {
                SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + vector, 13f);
                springPosition.ignoreTimeScale = true;
                springPosition.worldSpace = false;
            }
            return true;
        }
        return false;
    }

    public bool ConstrainTargetToBounds(Transform target, bool immediate)
    {
        Bounds targetBounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
        return ConstrainTargetToBounds(target, ref targetBounds, immediate);
    }

    private static void SetChildLayer(Transform t, int layer)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            if (child.GetComponent<UIPanel>() == null)
            {
                if (child.GetComponent<UIWidget>() != null)
                {
                    child.gameObject.layer = layer;
                }
                SetChildLayer(child, layer);
            }
        }
    }

    public static UIPanel Find(Transform trans, bool createIfMissing)
    {
        Transform transform = trans;
        UIPanel uIPanel = null;
        while (uIPanel == null && trans != null)
        {
            uIPanel = trans.GetComponent<UIPanel>();
            if (uIPanel != null || trans.parent == null)
            {
                break;
            }
            trans = trans.parent;
        }
        if (createIfMissing && uIPanel == null && trans != transform)
        {
            mFullRebuild = true;
            uIPanel = trans.gameObject.AddComponent<UIPanel>();
            SetChildLayer(uIPanel.cachedTransform, uIPanel.cachedGameObject.layer);
        }
        return uIPanel;
    }

    public static UIPanel Find(Transform trans)
    {
        return Find(trans, true);
    }

    private static void Fill()
    {
        int num = UIDrawCall.list.size;
        while (num > 0)
        {
            DestroyDrawCall(UIDrawCall.list[--num], num);
        }
        int num2 = 0;
        UIPanel uIPanel = null;
        Material material = null;
        UIDrawCall uIDrawCall = null;
        int num3 = 0;
        while (num3 < UIWidget.list.size)
        {
            UIWidget uIWidget = UIWidget.list[num3];
            if (uIWidget == null)
            {
                UIWidget.list.RemoveAt(num3);
                continue;
            }
            if (uIWidget.isVisible && uIWidget.hasVertices)
            {
                if (uIPanel != uIWidget.panel || material != uIWidget.material)
                {
                    if (uIPanel != null && material != null && mVerts.size != 0)
                    {
                        uIPanel.SubmitDrawCall(uIDrawCall);
                        uIDrawCall = null;
                    }
                    uIPanel = uIWidget.panel;
                    material = uIWidget.material;
                }
                if (uIPanel != null && material != null)
                {
                    if (uIDrawCall == null)
                    {
                        uIDrawCall = uIPanel.GetDrawCall(num2++, material);
                    }
                    uIWidget.drawCall = uIDrawCall;
                    if (uIPanel.generateNormals)
                    {
                        uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
                    }
                    else
                    {
                        uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
                    }
                }
            }
            else
            {
                uIWidget.drawCall = null;
            }
            num3++;
        }
        if (mVerts.size != 0)
        {
            uIPanel.SubmitDrawCall(uIDrawCall);
        }
    }

    private void SubmitDrawCall(UIDrawCall dc)
    {
        dc.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
        mVerts.Clear();
        mNorms.Clear();
        mTans.Clear();
        mUvs.Clear();
        mCols.Clear();
    }

    private static bool Fill(UIDrawCall dc)
    {
        if (dc != null)
        {
            dc.isDirty = false;
            int num = 0;
            while (num < UIWidget.list.size)
            {
                UIWidget uIWidget = UIWidget.list[num];
                if (uIWidget == null)
                {
                    UIWidget.list.RemoveAt(num);
                    continue;
                }
                if (uIWidget.drawCall == dc)
                {
                    if (uIWidget.isVisible && uIWidget.hasVertices)
                    {
                        if (dc.panel.generateNormals)
                        {
                            uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
                        }
                        else
                        {
                            uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
                        }
                    }
                    else
                    {
                        uIWidget.drawCall = null;
                    }
                }
                num++;
            }
            if (mVerts.size != 0)
            {
                dc.Set(mVerts, (!dc.panel.generateNormals) ? null : mNorms, (!dc.panel.generateNormals) ? null : mTans, mUvs, mCols);
                mVerts.Clear();
                mNorms.Clear();
                mTans.Clear();
                mUvs.Clear();
                mCols.Clear();
                return true;
            }
        }
        return false;
    }
}
