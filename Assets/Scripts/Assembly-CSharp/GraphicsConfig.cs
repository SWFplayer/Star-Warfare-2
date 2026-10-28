using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class GraphicsConfig : MonoBehaviour
{
	public GameObject shadowCamera;

	public GameObject mainCamera;

	public GraphicsParameter[] m_MeshList;

	public LevelName levelName;

	private Dictionary<string, Texture> textureFiles = new Dictionary<string, Texture>();

	private static GraphicsConfig sInstance;

	public static bool IsBossLevel()
	{
		if (sInstance != null)
		{
			return sInstance.levelName == LevelName.Campaign01_boss || sInstance.levelName == LevelName.Campaign02_boss || sInstance.levelName == LevelName.Campaign03_boss || sInstance.levelName == LevelName.Campaign04_boss || sInstance.levelName == LevelName.Campaign05_boss;
		}
		return false;
	}

	private void Start()
	{
		sInstance = this;
		if (!GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			shadowCamera.SetActive(true);
			GraphicsParameter[] improvedMesh = GetImprovedMesh(levelName);
			m_MeshList = improvedMesh;
			if (m_MeshList != null)
			{
				GraphicsParameter[] meshList = m_MeshList;
				foreach (GraphicsParameter graphicsParameter in meshList)
				{
					if (!(graphicsParameter.meshObj != null))
					{
						continue;
					}
					if (graphicsParameter.improveType == EImproveType.Apply_Shadow_Only)
					{
						if (graphicsParameter.meshObj.GetComponent<ShadowReceiver>() == null)
						{
							graphicsParameter.meshObj.AddComponent<ShadowReceiver>();
						}
						graphicsParameter.meshObj.AddComponent<ShadowReceiver>().ChangeTexture();
					}
					else if (graphicsParameter.improveType == EImproveType.Apply_Shadow_POM)
					{
						POMShadowReceiver pOMShadowReceiver = graphicsParameter.meshObj.GetComponent<POMShadowReceiver>();
						if (pOMShadowReceiver == null)
						{
							pOMShadowReceiver = graphicsParameter.meshObj.AddComponent<POMShadowReceiver>();
						}
						pOMShadowReceiver.normalMap = LoadTexture(graphicsParameter.normalMapFile);
						pOMShadowReceiver.heightMap = LoadTexture(graphicsParameter.heightMapFile);
						pOMShadowReceiver.lightDir = graphicsParameter.lightDir;
						pOMShadowReceiver.ChangeTexture();
					}
					else if (graphicsParameter.improveType == EImproveType.Apply_Shadow_POM_Material)
					{
						POMShadowReceiver pOMShadowReceiver2 = graphicsParameter.meshObj.GetComponent<POMShadowReceiver>();
						if (pOMShadowReceiver2 == null)
						{
							pOMShadowReceiver2 = graphicsParameter.meshObj.AddComponent<POMShadowReceiver>();
						}
						pOMShadowReceiver2.lightDir = graphicsParameter.lightDir;
						pOMShadowReceiver2.ChangeMaterial(graphicsParameter.normalMapFile);
					}
					else
					{
						Debug.LogWarning("Not supported right now!");
					}
				}
			}
		}
		else
		{
			UnityEngine.Object.Destroy(shadowCamera);
		}
		if (GameApp.GetInstance().GetUserState().UsingImageEffects())
		{
			if (!(mainCamera != null))
			{
				return;
			}
			GameObject gameObject = null;
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				int num = GameApp.GetInstance().GetUserState().GetSelectedStageIndexGeneral();
				if (num == 99)
				{
					num = 3;
				}
				Debug.Log("GetSelectedStageIndexGeneral = " + num);
				gameObject = Resources.Load("Graphics/Camera_" + num) as GameObject;
			}
			else
			{
				int num2 = GameApp.GetInstance().GetUserState().GetSelectedStageIndexGeneral() + 1;
				gameObject = Resources.Load("Graphics/Camera_" + num2) as GameObject;
			}
			if (gameObject != null)
			{
				ApplyImageEffects(gameObject);
				gameObject = null;
			}
		}
		else
		{
			FastBloom component = mainCamera.GetComponent<FastBloom>();
			if (component != null)
			{
				component.enabled = false;
			}
			GlowEffect component2 = mainCamera.GetComponent<GlowEffect>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
			ColorCorrectionCurves component3 = mainCamera.GetComponent<ColorCorrectionCurves>();
			if (component3 != null)
			{
				component3.enabled = false;
			}
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		sInstance = null;
	}

	private Texture LoadTexture(string name)
	{
		if (name == null || name.Length == 0)
		{
			Debug.LogWarning("POM texture missing!");
			return null;
		}
		name = name.Replace("\\", "/");
		if (textureFiles.ContainsKey(name))
		{
			return textureFiles[name];
		}
		Texture texture = Resources.Load(name) as Texture;
		if (texture == null)
		{
			Debug.LogWarning("Failed to load POM texture : " + name);
		}
		textureFiles.Add(name, texture);
		return texture;
	}

	private void ApplyImageEffects(GameObject cameraObj)
	{
		FastBloom component = cameraObj.GetComponent<FastBloom>();
		if (component != null && GameApp.GetInstance().GetUserState().UsingBloom())
		{
			FastBloom fastBloom = mainCamera.GetComponent<FastBloom>();
			if (fastBloom == null)
			{
				fastBloom = mainCamera.AddComponent<FastBloom>();
			}
			fastBloom.intensity = component.intensity;
			fastBloom.threshhold = component.threshhold;
			fastBloom.resolution = component.resolution;
			fastBloom.blurIterations = component.blurIterations;
			fastBloom.blurSize = component.blurSize;
			fastBloom.blurType = component.blurType;
			fastBloom.fastBloomShader = component.fastBloomShader;
		}
		else
		{
			FastBloom component2 = mainCamera.GetComponent<FastBloom>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}
		GlowEffect component3 = cameraObj.GetComponent<GlowEffect>();
		if (component3 != null && GameApp.GetInstance().GetUserState().UsingBloom())
		{
			GlowEffect glowEffect = mainCamera.GetComponent<GlowEffect>();
			if (glowEffect == null)
			{
				glowEffect = mainCamera.AddComponent<GlowEffect>();
			}
			glowEffect.glowIntensity = component3.glowIntensity;
			glowEffect.blurIterations = component3.blurIterations;
			glowEffect.blurSpread = component3.blurSpread;
			glowEffect.glowTint = component3.glowTint;
			glowEffect.compositeShader = component3.compositeShader;
			glowEffect.downsampleShader = component3.downsampleShader;
			glowEffect.blurShader = component3.blurShader;
		}
		else
		{
			GlowEffect component4 = mainCamera.GetComponent<GlowEffect>();
			if (component4 != null)
			{
				component4.enabled = false;
			}
		}
		ColorCorrectionCurves component5 = cameraObj.GetComponent<ColorCorrectionCurves>();
		if (component5 != null && GameApp.GetInstance().GetUserState().UsingColorCorrection())
		{
			ColorCorrectionCurves colorCorrectionCurves = mainCamera.GetComponent<ColorCorrectionCurves>();
			if (colorCorrectionCurves == null)
			{
				colorCorrectionCurves = mainCamera.AddComponent<ColorCorrectionCurves>();
			}
			colorCorrectionCurves.saturation = component5.saturation;
			colorCorrectionCurves.mode = component5.mode;
			colorCorrectionCurves.redChannel = component5.redChannel;
			colorCorrectionCurves.blueChannel = component5.blueChannel;
			colorCorrectionCurves.greenChannel = component5.greenChannel;
			colorCorrectionCurves.selectiveCc = component5.selectiveCc;
			colorCorrectionCurves.zCurve = component5.zCurve;
			colorCorrectionCurves.depthRedChannel = component5.depthRedChannel;
			colorCorrectionCurves.depthBlueChannel = component5.depthBlueChannel;
			colorCorrectionCurves.depthGreenChannel = component5.depthGreenChannel;
			colorCorrectionCurves.simpleColorCorrectionCurvesShader = component5.simpleColorCorrectionCurvesShader;
			colorCorrectionCurves.colorCorrectionCurvesShader = component5.colorCorrectionCurvesShader;
			colorCorrectionCurves.colorCorrectionSelectiveShader = component5.colorCorrectionSelectiveShader;
		}
		else
		{
			ColorCorrectionCurves component6 = mainCamera.GetComponent<ColorCorrectionCurves>();
			if (component6 != null)
			{
				component6.enabled = false;
			}
		}
	}

	public static string GetConfigFolder()
	{
		return Application.dataPath + "/Resources/Graphics/";
	}

	public static GraphicsParameter[] GetImprovedMesh(LevelName levelName)
	{
		try
		{
			List<GraphicsParameter> list = new List<GraphicsParameter>();
			TextAsset textAsset = Resources.Load("Graphics/" + levelName, typeof(TextAsset)) as TextAsset;
			if (null == textAsset)
			{
				Debug.LogError("Graphics/" + levelName.ToString() + " does not exist!");
				return null;
			}
			XmlReader xmlReader = XmlReader.Create(new MemoryStream(textAsset.bytes));
			while (xmlReader.Read())
			{
				if (!xmlReader.IsStartElement())
				{
					continue;
				}
				if ("mesh" == xmlReader.Name)
				{
					string value = xmlReader["improveType"];
					string text = xmlReader["meshObj"];
					string normalMapFile = xmlReader["normalMapFile"];
					string heightMapFile = xmlReader["heightMapFile"];
					string text2 = xmlReader["lightDir"];
					GraphicsParameter graphicsParameter = new GraphicsParameter();
					graphicsParameter.improveType = (EImproveType)(int)Enum.Parse(typeof(EImproveType), value);
					graphicsParameter.heightMapFile = heightMapFile;
					graphicsParameter.normalMapFile = normalMapFile;
					string[] array = text2.Split(':');
					graphicsParameter.lightDir = new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
					GameObject gameObject = GameObject.Find(text);
					if (null != gameObject)
					{
						MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
						if (component != null)
						{
							component.castShadows = false;
							component.receiveShadows = false;
						}
					}
					else
					{
						Debug.LogError("Could not find mesh : " + text);
					}
					graphicsParameter.meshObj = gameObject;
					list.Add(graphicsParameter);
				}
				else if (!("root" == xmlReader.Name))
				{
				}
			}
			xmlReader.Close();
			return list.ToArray();
		}
		catch (Exception ex)
		{
			Debug.Log("Failed to load GraphicsConfig: " + ex);
		}
		return null;
	}
}
