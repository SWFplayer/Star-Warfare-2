using UnityEngine;
using UnityEngine.Rendering;

public class ShadowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 direction = new Vector3(1f, 1f, 0f);
    public float distance = 5f;

    private static ShadowCamera sInstance;
    private Transform localPlayerTransform;
    private Shader renderShader;

    private Camera shadowCam;
    private CommandBuffer cmd;
    private Renderer playerRenderer;
    private Material shadowMaterial;

    private int GetRTSize()
    {
        int num = Mathf.Min(Screen.width, Screen.height);
        int[] array = new int[9] { 4, 8, 16, 32, 64, 128, 256, 512, 1024 };
        if (num >= 1024) return 1024;
        for (int num2 = array.Length - 1; num2 > 0; num2--)
        {
            if (num >= array[num2]) return array[num2];
        }
        return 1;
    }

    private void Awake()
    {
        if (GameApp.GetInstance().GetUserState().IsLowQuality())
        {
            Object.Destroy(base.gameObject);
            return;
        }

        shadowCam = GetComponent<Camera>();
        shadowCam.cullingMask |= (1 << PhysicsLayer.VEHICLE) | (1 << PhysicsLayer.BOSS_BODY);

        renderShader = Shader.Find("Custom/RenderShadow");
        if (renderShader != null)
        {
            shadowMaterial = new Material(renderShader);
        }

        if (GraphicsConfig.IsBossLevel())
        {
            distance *= 3f;
        }
        sInstance = this;
        
        cmd = new CommandBuffer();
        cmd.name = "RenderSkinnedShadow";
        
        cmd.ClearRenderTarget(true, true, Color.black);
        
        shadowCam.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);

        shadowCam.nearClipPlane = 0.1f;
        shadowCam.farClipPlane = 100f;
        shadowCam.orthographic = false;
        shadowCam.fieldOfView = 60f;
    }

    private void Destroy()
    {
        sInstance = null;
    }

    public static ShadowCamera GetInstance()
    {
        return sInstance;
    }

    private void Start()
    {
        if (GameApp.GetInstance().GetGameWorld() != null)
        {
            LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
            if (localPlayer != null)
            {
                localPlayerTransform = localPlayer.GetTransform();
                
                if (localPlayerTransform != null)
                {
                    playerRenderer = localPlayerTransform.GetComponentInChildren<Renderer>();
                }
            }
        }
        FollowLocalPlayer();
    }

    public void OnPreRender()
    {
        
        GL.Clear(false, true, Color.black, 1f);

    }

    private void Update()
    {
        FollowLocalPlayer();
    }

    private void FollowLocalPlayer()
    {
        if (null != target && null == localPlayerTransform)
        {
            localPlayerTransform = target;
        }

        if (null != localPlayerTransform)
        {
            base.transform.position = localPlayerTransform.position + direction * distance;
            base.transform.LookAt(localPlayerTransform.position);
            
            if (playerRenderer == null)
            {
                playerRenderer = localPlayerTransform.GetComponentInChildren<Renderer>();
            }
        }
        else if (GameApp.GetInstance().GetGameWorld() != null)
        {
            LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
            if (localPlayer != null)
            {
                localPlayerTransform = localPlayer.GetTransform();
            }
        }
    }

    private void LateUpdate()
    {
        
        FollowLocalPlayer();
        
        if (cmd != null && localPlayerTransform != null && shadowMaterial != null)
        {
            cmd.Clear();
           
            cmd.ClearRenderTarget(true, true, Color.black);
            
            GameObject[] allPlayerRoots = GameObject.FindGameObjectsWithTag("PlayerRoot");
            foreach (GameObject pRoot in allPlayerRoots)
            {
                Renderer[] playerRenderers = pRoot.GetComponentsInChildren<Renderer>();
                DrawRenderersToCommandBuffer(playerRenderers);
            }

           
            GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
            if (gameWorld != null && gameWorld.GetEnemies() != null)
            {
                foreach (Enemy enemy in gameWorld.GetEnemies().Values)
                {
                    
                    if (enemy != null && enemy.entityTransform != null && enemy.InPlayingState())
                    {
                        Renderer[] enemyRenderers = enemy.entityTransform.GetComponentsInChildren<Renderer>();
                        DrawRenderersToCommandBuffer(enemyRenderers);
                    }
                }
            }
        }
    }

    private void DrawRenderersToCommandBuffer(Renderer[] renderers)
    {
        foreach (Renderer r in renderers)
        {
            
            if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;

            
            if (r.GetComponent<ShadowReceiver>() != null) continue;

            if (!r.enabled || !r.isVisible) continue;

            if (r.gameObject.name.ToLower() == "shadow" || r.transform.parent.name.ToLower() == "shadow")
            {
                continue;
            }
            

            if (r.gameObject.name.ToLower() == "shandiantiao_sw3" || r.transform.parent.name.ToLower() == "shandiantiao_sw3")
            {
                continue;
            }

            Material[] materials = r.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                
                if (materials[i] != null)
                {
                    cmd.DrawRenderer(r, shadowMaterial, i, 0);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (shadowCam != null && cmd != null)
        {
            shadowCam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
        }
    }
}