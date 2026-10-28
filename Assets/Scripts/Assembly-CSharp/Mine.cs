using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
	private const float hideTime = 0.5f;

	private const float waitTime = 1.5f;

	private const float triggerRadius = 30f;

	public const float flySpeed = 50f;

	public const float bombRadius = 6f;

	private const string animationString = "open";

	public Player owner;

	public GameObject mineEntity;

	private Material mat;

	private GameWorld gameWorld;

	private Player localPlayer;

	private bool IsReadyForExp;

	private GameUnit target;

	private Vector3 originalPosition;

	public void Init()
	{
		mat = mineEntity.GetComponent<Renderer>().material;
		mat.SetFloat("_Alpha", 1f);
		gameWorld = GameApp.GetInstance().GetGameWorld();
		localPlayer = gameWorld.GetLocalPlayer();
		base.gameObject.GetComponent<SphereCollider>().radius = 30f;
		IsReadyForExp = false;
		target = null;
		originalPosition = base.transform.position;
		StopAllCoroutines();
		if (owner != null)
		{
			if (owner.IsLocal() || owner.IsFriendForce(localPlayer))
			{
				StartCoroutine(Blink());
			}
			else
			{
				StartCoroutine(Hide());
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (owner != null)
		{
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
				{
					RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(int.Parse(other.gameObject.name));
					if (remotePlayerByUserID != null && remotePlayerByUserID != owner && !remotePlayerByUserID.IsFriendForce(owner) && IsReadyForExp && !remotePlayerByUserID.GetSkills().HasSkill(SkillsType.FLY))
					{
						IsReadyForExp = false;
						StopAllCoroutines();
						target = remotePlayerByUserID;
						StartCoroutine(Appear());
					}
				}
				else if (other.gameObject.layer == PhysicsLayer.PLAYER)
				{
					if (!owner.IsLocal() && !owner.IsFriendForce(localPlayer) && IsReadyForExp && !localPlayer.GetSkills().HasSkill(SkillsType.FLY))
					{
						IsReadyForExp = false;
						StopAllCoroutines();
						target = localPlayer;
						StartCoroutine(Appear());
					}
				}
				else
				{
					if (other.gameObject.layer != PhysicsLayer.ENEMY)
					{
						return;
					}
					GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(other);
					if (rootObjectByCollider.tag == TagName.PLAYER_ROOT)
					{
						int userID = int.Parse(rootObjectByCollider.name);
						RemotePlayer remotePlayerByUserID2 = gameWorld.GetRemotePlayerByUserID(userID);
						if (remotePlayerByUserID2 != null && IsReadyForExp)
						{
							IsReadyForExp = false;
							StopAllCoroutines();
							target = remotePlayerByUserID2;
							StartCoroutine(Appear());
						}
					}
				}
			}
			else if (other.gameObject.layer == PhysicsLayer.ENEMY && IsReadyForExp)
			{
				IsReadyForExp = false;
				StopAllCoroutines();
				GameObject rootObjectByCollider2 = Enemy.GetRootObjectByCollider(other);
				if (rootObjectByCollider2.tag == TagName.ENEMY_ROOT)
				{
					target = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider2.name);
				}
				StartCoroutine(Appear());
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private IEnumerator Hide()
	{
		base.GetComponent<Animation>()["open"].speed = -1f;
		base.GetComponent<Animation>()["open"].normalizedTime = 1f;
		base.GetComponent<Animation>().Play("open");
		while (base.GetComponent<Animation>()["open"].normalizedTime > 0.1f)
		{
			yield return null;
		}
		while (mat.GetFloat("_Alpha") > 0f)
		{
			float alpha = mat.GetFloat("_Alpha");
			mat.SetFloat("_Alpha", alpha - Time.deltaTime / 0.5f);
			base.transform.Translate((originalPosition - base.transform.position) * Time.deltaTime / 0.5f, Space.World);
			yield return null;
		}
		IsReadyForExp = true;
		yield return null;
	}

	private void CreateProjectile()
	{
		Vector3 normalized = (target.GetPosition() - base.transform.position).normalized;
		Vector3 position = base.transform.position;
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.MINE_PROJECTILE).CreateObject(position, normalized, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		AutoDestroyScript component = gameObject.GetComponent<AutoDestroyScript>();
		if (component != null)
		{
			component.enabled = false;
		}
		FireLineScript component2 = gameObject.GetComponent<FireLineScript>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		ProjectileScript component3 = gameObject.GetComponent<ProjectileScript>();
		component3.enabled = true;
		component3.isLocal = true;
		component3.GunType = WeaponType.NoGun;
		component3.dir = normalized;
		component3.flySpeed = 50f;
		component3.explodeRadius = 6f;
		component3.hitForce = 0f;
		component3.life = 10f;
		component3.damage = owner.GetSkills().GetSpecialSkillDamage(AdvancedSkill.ESpecialSkillType.MINE);
		component3.targetPos = target.GetPosition();
		component3.isMine = true;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(AdvancedSkill.ESpecialSkillType.MINE, position, normalized);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	private IEnumerator Appear()
	{
		base.GetComponent<Animation>()["open"].speed = 1f / 3f;
		base.GetComponent<Animation>()["open"].normalizedTime = 0f;
		base.GetComponent<Animation>().Play("open");
		mat.SetFloat("_Alpha", 0f);
		while (mat.GetFloat("_Alpha") < 1f)
		{
			float alpha = mat.GetFloat("_Alpha");
			mat.SetFloat("_Alpha", alpha + Time.deltaTime / 1.5f);
			base.transform.Translate((originalPosition + Vector3.up - base.transform.position) * Time.deltaTime / 1.5f, Space.World);
			yield return null;
		}
		if (owner != null)
		{
			if (target != null && target.InPlayingState())
			{
				if (owner.IsLocal())
				{
					CreateProjectile();
				}
				base.gameObject.SetActive(false);
			}
			else if (owner.IsLocal() || owner.IsFriendForce(localPlayer))
			{
				StartCoroutine(Blink());
			}
			else
			{
				StartCoroutine(Hide());
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		yield return null;
	}

	private IEnumerator Blink()
	{
		base.GetComponent<Animation>()["open"].speed = -1f;
		base.GetComponent<Animation>()["open"].normalizedTime = 1f;
		base.GetComponent<Animation>().Play("open");
		while (base.GetComponent<Animation>()["open"].normalizedTime > 0.1f)
		{
			yield return null;
		}
		while (mat.GetFloat("_Alpha") > 0f)
		{
			float alpha = mat.GetFloat("_Alpha");
			mat.SetFloat("_Alpha", alpha - Time.deltaTime / 0.5f);
			base.transform.Translate((originalPosition - base.transform.position) * Time.deltaTime / 0.5f, Space.World);
			yield return null;
		}
		IsReadyForExp = true;
		while (true)
		{
			mat.SetFloat("_Alpha", Mathf.PingPong(Time.time, 0.75f));
			yield return null;
		}
	}
}
