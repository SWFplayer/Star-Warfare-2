using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightableObject : MonoBehaviour
{
    private Material highlightingMaterial
    {
        get
        {
            return (!this.zWrite) ? this.opaqueMaterial : this.opaqueZMaterial;
        }
    }

    private Material opaqueMaterial
    {
        get
        {
            if (this._opaqueMaterial == null)
            {
                this._opaqueMaterial = new Material(HighlightableObject.opaqueShader);
                this._opaqueMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return this._opaqueMaterial;
        }
    }

    private Material opaqueZMaterial
    {
        get
        {
            if (this._opaqueZMaterial == null)
            {
                this._opaqueZMaterial = new Material(HighlightableObject.opaqueZShader);
                this._opaqueZMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return this._opaqueZMaterial;
        }
    }

    private static Shader opaqueShader
    {
        get
        {
            if (HighlightableObject._opaqueShader == null)
            {
                HighlightableObject._opaqueShader = Shader.Find("Hidden/Highlighted/StencilOpaque");
            }
            return HighlightableObject._opaqueShader;
        }
    }

    public static Shader transparentShader
    {
        get
        {
            if (HighlightableObject._transparentShader == null)
            {
                HighlightableObject._transparentShader = Shader.Find("Hidden/Highlighted/StencilTransparent");
            }
            return HighlightableObject._transparentShader;
        }
    }

    private static Shader opaqueZShader
    {
        get
        {
            if (HighlightableObject._opaqueZShader == null)
            {
                HighlightableObject._opaqueZShader = Shader.Find("Hidden/Highlighted/StencilOpaqueZ");
            }
            return HighlightableObject._opaqueZShader;
        }
    }

    private static Shader transparentZShader
    {
        get
        {
            if (HighlightableObject._transparentZShader == null)
            {
                HighlightableObject._transparentZShader = Shader.Find("Hidden/Highlighted/StencilTransparentZ");
            }
            return HighlightableObject._transparentZShader;
        }
    }

    private void OnEnable()
    {
        base.StartCoroutine(this.EndOfFrame());
        HighlightingEffect.highlightingEvent += this.UpdateEventHandler;
    }

    private void OnDisable()
    {
        base.StopAllCoroutines();
        HighlightingEffect.highlightingEvent -= this.UpdateEventHandler;
        if (this.highlightableRenderers != null)
        {
            this.highlightableRenderers.Clear();
        }
        this.layersCache = null;
        this.materialsIsDirty = true;
        this.currentState = false;
        this.currentColor = Color.clear;
        this.transitionActive = false;
        this.transitionValue = 0f;
        this.once = false;
        this.flashing = false;
        this.constantly = false;
        this.occluder = false;
        this.zWrite = false;
        if (this._opaqueMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this._opaqueMaterial);
        }
        if (this._opaqueZMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this._opaqueZMaterial);
        }
    }

    public void ReinitMaterials()
    {
        this.materialsIsDirty = true;
    }

    public void RestoreMaterials()
    {
        Debug.LogWarning("HighlightingSystem : RestoreMaterials() is obsolete. Please use ReinitMaterials() instead.");
        this.ReinitMaterials();
    }

    public void OnParams(Color color)
    {
        this.onceColor = color;
    }

    public void On()
    {
        this.once = true;
    }

    public void On(Color color)
    {
        this.onceColor = color;
        this.On();
    }

    public void FlashingParams(Color color1, Color color2, float freq)
    {
        this.flashingColorMin = color1;
        this.flashingColorMax = color2;
        this.flashingFreq = freq;
    }

    public void FlashingOn()
    {
        this.flashing = true;
    }

    public void FlashingOn(Color color1, Color color2)
    {
        this.flashingColorMin = color1;
        this.flashingColorMax = color2;
        this.FlashingOn();
    }

    public void FlashingOn(Color color1, Color color2, float freq)
    {
        this.flashingFreq = freq;
        this.FlashingOn(color1, color2);
    }

    public void FlashingOn(float freq)
    {
        this.flashingFreq = freq;
        this.FlashingOn();
    }

    public void FlashingOff()
    {
        this.flashing = false;
    }

    public void FlashingSwitch()
    {
        this.flashing = !this.flashing;
    }

    public void ConstantParams(Color color)
    {
        this.constantColor = color;
    }

    public void ConstantOn()
    {
        this.constantly = true;
        this.transitionActive = true;
    }

    public void ConstantOn(Color color)
    {
        this.constantColor = color;
        this.ConstantOn();
    }

    public void ConstantOff()
    {
        this.constantly = false;
        this.transitionActive = true;
    }

    public void ConstantSwitch()
    {
        this.constantly = !this.constantly;
        this.transitionActive = true;
    }

    public void ConstantOnImmediate()
    {
        this.constantly = true;
        this.transitionValue = 1f;
        this.transitionActive = false;
    }

    public void ConstantOnImmediate(Color color)
    {
        this.constantColor = color;
        this.ConstantOnImmediate();
    }

    public void ConstantOffImmediate()
    {
        this.constantly = false;
        this.transitionValue = 0f;
        this.transitionActive = false;
    }

    public void ConstantSwitchImmediate()
    {
        this.constantly = !this.constantly;
        this.transitionValue = ((!this.constantly) ? 0f : 1f);
        this.transitionActive = false;
    }

    public void OccluderOn()
    {
        this.occluder = true;
    }

    public void OccluderOff()
    {
        this.occluder = false;
    }

    public void OccluderSwitch()
    {
        this.occluder = !this.occluder;
    }

    public void Off()
    {
        this.once = false;
        this.flashing = false;
        this.constantly = false;
        this.transitionValue = 0f;
        this.transitionActive = false;
    }

    public void Die()
    {
        UnityEngine.Object.Destroy(this);
    }

    private void InitMaterials(bool writeDepth)
    {
        this.currentState = false;
        this.zWrite = writeDepth;
        this.highlightableRenderers = new List<HighlightableObject.HighlightingRendererCache>();
        MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
        this.CacheRenderers(componentsInChildren);
        SkinnedMeshRenderer[] componentsInChildren2 = base.GetComponentsInChildren<SkinnedMeshRenderer>();
        this.CacheRenderers(componentsInChildren2);

        this.currentState = false;
        this.materialsIsDirty = false;
        this.currentColor = Color.clear;
    }

    private void CacheRenderers(Renderer[] renderers)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] sharedMaterials = renderers[i].sharedMaterials;
            if (sharedMaterials != null)
            {
                this.highlightableRenderers.Add(new HighlightableObject.HighlightingRendererCache(renderers[i], sharedMaterials, this.highlightingMaterial, this.zWrite));
            }
        }
    }

    private void SetColor(Color c)
    {
        if (this.currentColor == c)
        {
            return;
        }
        if (this.zWrite)
        {
            this.opaqueZMaterial.SetColor("_Outline", c);
        }
        else
        {
            this.opaqueMaterial.SetColor("_Outline", c);
        }
        for (int i = 0; i < this.highlightableRenderers.Count; i++)
        {
            this.highlightableRenderers[i].SetColorForTransparent(c);
        }
        this.currentColor = c;
    }

    private void UpdateColors()
    {
        if (!this.currentState)
        {
            return;
        }
        if (this.occluder)
        {
            this.SetColor(this.occluderColor);
            return;
        }
        if (this.once)
        {
            this.SetColor(this.onceColor);
            return;
        }
        if (this.flashing)
        {
            Color color = Color.Lerp(this.flashingColorMin, this.flashingColorMax, 0.5f * Mathf.Sin(Time.realtimeSinceStartup * this.flashingFreq * 6.2831855f) + 0.5f);
            this.SetColor(color);
            return;
        }
        if (this.transitionActive)
        {
            Color color2 = new Color(this.constantColor.r, this.constantColor.g, this.constantColor.b, this.constantColor.a * this.transitionValue);
            this.SetColor(color2);
            return;
        }
        if (this.constantly)
        {
            this.SetColor(this.constantColor);
            return;
        }
    }

    private void PerformTransition()
    {
        if (!this.transitionActive)
        {
            return;
        }
        float num = (!this.constantly) ? 0f : 1f;
        if (this.transitionValue == num)
        {
            this.transitionActive = false;
            return;
        }
        if (Time.timeScale != 0f)
        {
            float num2 = Time.deltaTime / Time.timeScale;
            this.transitionValue += ((!this.constantly) ? (-HighlightableObject.constantOffSpeed) : HighlightableObject.constantOnSpeed) * num2;
            this.transitionValue = Mathf.Clamp01(this.transitionValue);
            return;
        }
    }

    private void UpdateEventHandler(bool trigger, bool writeDepth)
    {
        if (trigger)
        {
            if (this.zWrite != writeDepth)
            {
                this.materialsIsDirty = true;
            }
            if (this.materialsIsDirty)
            {
                this.InitMaterials(writeDepth);
            }
            this.currentState = (this.once || this.flashing || this.constantly || this.transitionActive || this.occluder);
            if (this.currentState)
            {
                this.UpdateColors();
                this.PerformTransition();
                if (this.highlightableRenderers != null)
                {
                    this.layersCache = new int[this.highlightableRenderers.Count];
                    for (int i = 0; i < this.highlightableRenderers.Count; i++)
                    {
                        GameObject goCached = this.highlightableRenderers[i].goCached;
                        this.layersCache[i] = goCached.layer;
                        goCached.layer = HighlightableObject.highlightingLayer;
                        this.highlightableRenderers[i].SetState(true);
                    }
                }
            }
        }
        else if (this.currentState && this.highlightableRenderers != null)
        {
            for (int j = 0; j < this.highlightableRenderers.Count; j++)
            {
                this.highlightableRenderers[j].goCached.layer = this.layersCache[j];
                this.highlightableRenderers[j].SetState(false);
            }
        }
    }

    private IEnumerator EndOfFrame()
    {
        while (base.enabled)
        {
            yield return new WaitForEndOfFrame();
            this.once = false;
        }
        yield break;
    }

    private const float doublePI = 6.2831855f;

    public static int highlightingLayer = 7;

    private static float constantOnSpeed = 4.5f;

    private static float constantOffSpeed = 4f;

    private static float transparentCutoff = 0.5f;

    private List<HighlightableObject.HighlightingRendererCache> highlightableRenderers;

    private int[] layersCache;

    private bool materialsIsDirty = true;

    private bool currentState;

    private Color currentColor;

    private bool transitionActive;

    private float transitionValue;

    private float flashingFreq = 2f;

    private bool once;

    private Color onceColor = Color.red;

    private bool flashing;

    private Color flashingColorMin = new Color(0f, 1f, 1f, 0f);

    private Color flashingColorMax = new Color(0f, 1f, 1f, 1f);

    private bool constantly;

    private Color constantColor = Color.yellow;

    private bool occluder;

    private bool zWrite;

    private readonly Color occluderColor = new Color(0f, 0f, 0f, 0.005f);

    private Material _opaqueMaterial;

    private Material _opaqueZMaterial;

    private static Shader _opaqueShader;

    private static Shader _transparentShader;

    private static Shader _opaqueZShader;

    private static Shader _transparentZShader;

    private class HighlightingRendererCache
    {
        public HighlightingRendererCache(Renderer rend, Material[] mats, Material sharedOpaqueMaterial, bool writeDepth)
        {
            this.rendererCached = rend;
            this.goCached = rend.gameObject;
            this.sourceMaterials = mats;
            this.replacementMaterials = new Material[mats.Length];
            this.transparentMaterialIndexes = new List<int>();
            for (int i = 0; i < mats.Length; i++)
            {
                Material material = mats[i];
                if (!(material == null))
                {
                    string tag = material.GetTag("RenderType", true);
                    if (tag == "Transparent" || tag == "TransparentCutout")
                    {
                        Material material2 = new Material((!writeDepth) ? HighlightableObject.transparentShader : HighlightableObject.transparentZShader);
                        if (material.HasProperty("_MainTex"))
                        {
                            material2.SetTexture("_MainTex", material.mainTexture);
                            material2.SetTextureOffset("_MainTex", material.mainTextureOffset);
                            material2.SetTextureScale("_MainTex", material.mainTextureScale);
                        }
                        material2.SetFloat("_Cutoff", (!material.HasProperty("_Cutoff")) ? HighlightableObject.transparentCutoff : material.GetFloat("_Cutoff"));
                        this.replacementMaterials[i] = material2;
                        this.transparentMaterialIndexes.Add(i);
                    }
                    else
                    {
                        this.replacementMaterials[i] = sharedOpaqueMaterial;
                    }
                }
            }
        }

        public void SetState(bool state)
        {
            this.rendererCached.sharedMaterials = ((!state) ? this.sourceMaterials : this.replacementMaterials);
        }

        public void SetColorForTransparent(Color clr)
        {
            for (int i = 0; i < this.transparentMaterialIndexes.Count; i++)
            {
                this.replacementMaterials[this.transparentMaterialIndexes[i]].SetColor("_Outline", clr);
            }
        }

        public Renderer rendererCached;

        public GameObject goCached;

        private Material[] sourceMaterials;

        private Material[] replacementMaterials;

        private List<int> transparentMaterialIndexes;
    }
}
