using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TPS/ProjectileScript")]
public class ProjectileScript : MonoBehaviour
{
    private const float enableColliderTime = 0.3f;

    protected GameObject resObject;

    protected GameObject explodeObject;

    protected GameObject smallExplodeObject;

    protected GameObject laserHitObject;

    public Transform targetTransform;

    public Vector3 targetPos;

    protected Transform proTransform;

    protected Transform trackingTarget;

    public Enemy trackingEnemy;

    public Player trackingPlayer;

    protected WeaponType gunType;

    protected EVehicleWeaponType vehicleWeaponType;

    public Vector3 dir;

    public float hitForce;

    public float explodeRadius;

    public float flySpeed;

    public Vector3 speed;

    public float life = 2f;

    public int damage;

    public bool isLocal = true;

    public byte bagIndex;

    public bool isPenerating;

    public bool isReflecting;

    public bool isSpliting;

    public bool isSplited;

    public bool isMine;

    public bool isUseGravity;

    public bool isTrackingPos;

    protected float createdTime;

    protected float lastTriggerTime;

    protected float gravity = Global.PROJECTILE_GRAVITY;

    protected float downSpeed;

    protected float deltaTime;

    protected float initAngel = 50f;

    protected float trackingAngelRotateTime = 2f;

    protected float trackingFlyAcceleration = 3f;

    protected float trackingDuration = 5.1f;

    protected float trackingUpdateTargetPosInterval = 0.5f;

    protected Timer trackingUpdateTargetPosTimer = new Timer();

    protected Vector3 trackingPos;

    protected float trackingLastUpdatetargetPosTime;

    protected HashSet<PenerateHitInfo> penetratingTargets = new HashSet<PenerateHitInfo>();

    protected HashSet<Enemy> enemyhitSet = new HashSet<Enemy>();

    public Weapon weapon;

    public VehicleWeapon vehicleWeapon;

    private Timer enableColliderTimer = new Timer();

    public bool destroyed;

    public WeaponType GunType
    {
        set
        {
            gunType = value;
        }
    }

    public EVehicleWeaponType VehicleWeaponType
    {
        set
        {
            vehicleWeaponType = value;
        }
    }

    public void Start()
    {
        destroyed = false;
        proTransform = base.transform;
        createdTime = Time.time;
        trackingLastUpdatetargetPosTime = createdTime;
        if (GameApp.GetInstance().GetGameMode().IsVSMode() && (gunType == WeaponType.RPG || gunType == WeaponType.TwinRPG || gunType == WeaponType.TrackingRPG || gunType == WeaponType.BowSword || gunType == WeaponType.Fist || gunType == WeaponType.QuadrupleFist || (gunType == WeaponType.NoGun && vehicleWeaponType == EVehicleWeaponType.ATV_MINI_RPG)))
        {
            flySpeed *= VSMath.RPG_FLY_BOOTH;
        }
        trackingUpdateTargetPosTimer.SetTimer(trackingUpdateTargetPosInterval, true);
        base.gameObject.GetComponent<Collider>().enabled = true;
        base.transform.localScale = Vector3.one;
    }

    public void OnEnable()
    {
        destroyed = false;
        isPenerating = false;
        isReflecting = false;
        isSpliting = false;
        isSplited = false;
        isMine = false;
        isUseGravity = false;
        downSpeed = 0f;
        enableColliderTimer.SetTimer(0.3f, false);
    }

    public void Update()
    {
        deltaTime = Time.deltaTime;
        if (trackingEnemy != null)
        {
            flySpeed += Time.deltaTime * trackingFlyAcceleration;
            trackingTarget = trackingEnemy.GetTransform();
            if (trackingTarget != null)
            {
                if (trackingEnemy.GetState() != Enemy.DEAD_STATE)
                {
                    if (Time.time - createdTime < trackingDuration)
                    {
                        if (trackingUpdateTargetPosTimer.Ready())
                        {
                            trackingPos = trackingEnemy.GetColliderCenterPosition();
                            trackingLastUpdatetargetPosTime = Time.time;
                            trackingUpdateTargetPosTimer.Do();
                        }
                        Vector3 normalized = (trackingPos - proTransform.position).normalized;
                        Quaternion to = Quaternion.LookRotation(normalized);
                        proTransform.rotation = Quaternion.Lerp(proTransform.rotation, to, (Time.time - trackingLastUpdatetargetPosTime) / trackingAngelRotateTime);
                        dir = proTransform.forward;
                    }
                }
                else
                {
                    trackingEnemy = null;
                }
            }
        }
        if (trackingPlayer != null)
        {
            flySpeed += Time.deltaTime * trackingFlyAcceleration;
            trackingTarget = trackingPlayer.GetTransform();
            if (trackingTarget != null)
            {
                if (trackingPlayer.InPlayingState())
                {
                    if (Time.time - createdTime < trackingDuration)
                    {
                        if (trackingUpdateTargetPosTimer.Ready())
                        {
                            trackingPos = trackingPlayer.GetTransform().position + Vector3.up * 1f;
                            trackingLastUpdatetargetPosTime = Time.time;
                            trackingUpdateTargetPosTimer.Do();
                        }
                        Vector3 normalized2 = (trackingPos - proTransform.position).normalized;
                        Quaternion to2 = Quaternion.LookRotation(normalized2);
                        proTransform.rotation = Quaternion.Lerp(proTransform.rotation, to2, (Time.time - trackingLastUpdatetargetPosTime) / trackingAngelRotateTime);
                        dir = proTransform.forward;
                    }
                }
                else
                {
                    trackingPlayer = null;
                }
            }
        }
        if (isTrackingPos)
        {
            Vector3 target = targetPos - base.transform.position;
            dir = Vector3.RotateTowards(dir, target, 0.9424779f * Time.deltaTime, 0f);
        }
        proTransform.Translate(flySpeed * dir * deltaTime, Space.World);
        if (isUseGravity)
        {
            downSpeed += gravity * deltaTime;
            proTransform.Translate(Vector3.down * downSpeed * deltaTime, Space.World);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isLocal)
        {
            return;
        }
        if (weapon != null)
        {
            DamageProperty damageProperty = weapon.CreateDamageProperty();
            BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile, weapon.player) as BulletProjectile;
            if (bulletProjectile != null)
            {
                bulletProjectile.ExplosionDetect(damageProperty, other, isPenerating, 80, penetratingTargets, proTransform.position, 0f, false);
            }
        }
        else if (vehicleWeapon != null)
        {
            DamageProperty damageProperty2 = vehicleWeapon.CreateDamageProperty();
            BulletProjectile bulletProjectile2 = BulletScript.CreateBullet(BulletType.Projectile, vehicleWeapon.GetPlayer()) as BulletProjectile;
            if (bulletProjectile2 != null)
            {
                bulletProjectile2.ExplosionDetect(damageProperty2, other, isPenerating, 80, penetratingTargets, proTransform.position, 0f, false);
            }
        }
        else
        {
            Debug.LogError("һ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd weapon\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd null !!! \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdҹ\ufffdһ\ufffd\ufffd");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyed || (isSplited && !enableColliderTimer.Ready()))
        {
            return;
        }
        GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
        Player localPlayer = gameWorld.GetLocalPlayer();
        Player player = null;
        if (weapon != null)
        {
            player = weapon.player;
        }
        if (vehicleWeapon != null)
        {
            player = vehicleWeapon.GetPlayer();
        }
        bool penerateExplode = false;
        if (isLocal && other.gameObject.layer == PhysicsLayer.PLAYER && !isMine)
        {
            return;
        }
        string path = "_Old/_Effect/update_effect/effect_explosion_004";
        EffectPoolType type = EffectPoolType.NONE;
        if (gunType == WeaponType.RPG || gunType == WeaponType.TwinRPG || gunType == WeaponType.TrackingRPG || gunType == WeaponType.SplitRPG || isMine)
        {
            type = EffectPoolType.W3_ROCKET_BOOM_5;
            if (explodeRadius > 6f)
            {
                type = EffectPoolType.W3_ROCKET_BOOM_7;
            }
            if (explodeRadius > 8f)
            {
                type = EffectPoolType.W3_ROCKET_BOOM_9;
            }
        }
        else if (gunType == WeaponType.Fist || (gunType == WeaponType.NoGun && vehicleWeaponType == EVehicleWeaponType.ATV_MINI_RPG) || gunType == WeaponType.BowSword)
        {
            type = EffectPoolType.W8_FIST_BOOM;
        }
        else if (gunType == WeaponType.QuadrupleFist)
        {
            type = EffectPoolType.W8_FIST_BOOM_EXP;
        }
        else if (gunType == WeaponType.Myth370Fist)
        {
            type = EffectPoolType.W31_ROCKET_BOOM;
        }
        else if (gunType == WeaponType.ChargeDoublePistol)
        {
            type = EffectPoolType.W31_ROCKET_BOOM;
        }
        if (isReflecting && (other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET || other.gameObject.layer == PhysicsLayer.TRANSPARENT_WALL || other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.VOID_SHIELD))
        {
            if (ReflectBack())
            {
                return;
            }
            GameObject original = Resources.Load(path) as GameObject;
            Object.Instantiate(original, proTransform.position, Quaternion.identity);
            Object.DestroyObject(base.gameObject);
            penerateExplode = true;
            destroyed = true;
        }
        if (isPenerating)
        {
            if (!isReflecting)
            {
                if (other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET || other.gameObject.layer == PhysicsLayer.TRANSPARENT_WALL || other.gameObject.layer == PhysicsLayer.FLOOR)
                {
                    Object.DestroyObject(base.gameObject);
                    destroyed = true;
                }
                else if (other.gameObject.layer == PhysicsLayer.VOID_SHIELD)
                {
                    ShieldScript component = other.transform.root.GetComponent<ShieldScript>();
                    if (!localPlayer.IsFriendForce(component.ownedPlayer) && localPlayer != component.ownedPlayer)
                    {
                        Object.DestroyObject(base.gameObject);
                        destroyed = true;
                    }
                }
            }
        }
        else if (other.gameObject.layer == PhysicsLayer.VOID_SHIELD)
        {
            ShieldScript component2 = other.transform.root.GetComponent<ShieldScript>();
            if (player != null && !player.IsFriendForce(component2.ownedPlayer) && player != component2.ownedPlayer)
            {
                gameWorld.GetEffectPool(type).CreateObject(proTransform.position, Vector3.zero, Quaternion.identity);
                base.gameObject.SetActive(false);
                destroyed = true;
                if (isSpliting)
                {
                    Split();
                }
            }
        }
        else
        {
            gameWorld.GetEffectPool(type).CreateObject(proTransform.position, Vector3.zero, Quaternion.identity);
            base.gameObject.SetActive(false);
            destroyed = true;
            if (isSpliting)
            {
                Split();
            }
        }
        if (!isLocal)
        {
            return;
        }
        if (weapon != null)
        {
            DamageProperty damageProperty = weapon.CreateDamageProperty();
            if (weapon.GetWeaponType() == WeaponType.SplitRPG && isSplited)
            {
                damageProperty.damage = (int)((float)damageProperty.damage / 2f);
                damageProperty.damageFactor = 1;
            }
            if (GameApp.GetInstance().GetGameMode().IsVSMode() && localPlayer != null && localPlayer.GetSkills().HasSkill(SkillsType.RPG_DIRECT_HIT_DAMAGE_INCREASE))
            {
                int userID = 0;
                if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
                {
                    if (other.gameObject.tag == TagName.PLAYER_ROOT)
                    {
                        userID = int.Parse(other.gameObject.name);
                    }
                    RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(userID);
                    damageProperty.directHitUnit = remotePlayerByUserID;
                }
            }
            BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile, weapon.player) as BulletProjectile;
            if (bulletProjectile != null)
            {
                bulletProjectile.ExplosionDetect(damageProperty, other, isPenerating, 80, penetratingTargets, proTransform.position, explodeRadius, penerateExplode);
            }
        }
        else if (vehicleWeapon != null)
        {
            DamageProperty damageProperty2 = vehicleWeapon.CreateDamageProperty();
            BulletProjectile bulletProjectile2 = BulletScript.CreateBullet(BulletType.Projectile, vehicleWeapon.GetPlayer()) as BulletProjectile;
            if (bulletProjectile2 != null)
            {
                bulletProjectile2.ExplosionDetect(damageProperty2, other, isPenerating, 80, penetratingTargets, proTransform.position, explodeRadius, penerateExplode);
            }
        }
        else if (isMine && isLocal)
        {
            Player localPlayer2 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
            DamageProperty damageProperty3 = new DamageProperty(localPlayer2, damage);
            damageProperty3.weaponid = 23;
            damageProperty3.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
            damageProperty3.isLocal = true;
            BulletProjectile bulletProjectile3 = BulletScript.CreateBullet(BulletType.Projectile, localPlayer2) as BulletProjectile;
            if (bulletProjectile3 != null)
            {
                bulletProjectile3.ExplosionDetect(damageProperty3, other, isPenerating, 80, penetratingTargets, proTransform.position, explodeRadius, penerateExplode);
            }
        }
        else
        {
            Debug.LogError("һ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd weapon\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd null !!! \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdҹ\ufffdһ\ufffd\ufffd");
        }
        if (weapon != null)
        {
            AudioManager.GetInstance().PlaySoundAt(weapon.ExplodeSoundName, base.transform.position);
        }
        else if (vehicleWeapon != null)
        {
            AudioManager.GetInstance().PlaySoundAt(vehicleWeapon.ExplodeSoundName, base.transform.position);
        }
    }

    protected bool ReflectBack()
    {
        Ray ray = new Ray(base.transform.position, dir);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 2f, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR)))
        {
            Vector3 normal = hitInfo.normal;
            float num = Vector3.Angle(dir, normal);
            if (num < 155f)
            {
                dir = Vector3.Reflect(dir, normal).normalized;
                proTransform.LookAt(proTransform.position + dir * 2f);
                int num2 = Random.Range(1, 5);
                AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/diablo/black_disk_bounce0" + num2, proTransform.position);
                return true;
            }
        }
        return false;
    }

    protected void Split()
    {
        if (weapon != null && weapon.player.IsLocal() && weapon.GetWeaponType() == WeaponType.SplitRPG)
        {
            SplitRPG splitRPG = weapon as SplitRPG;
            splitRPG.Split(base.transform.position, base.transform.forward);
        }
    }
}
