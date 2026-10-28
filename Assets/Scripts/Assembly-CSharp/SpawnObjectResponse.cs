using UnityEngine;

public class SpawnObjectResponse : Response
{
	private byte type;

	private int OwnerId;

	private short skillId;

	private Vector3 position;

	private float eX;

	private float eY;

	private int targetId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		type = bytesBuffer.ReadByte();
		position.x = (float)bytesBuffer.ReadShort() / 10f;
		position.y = (float)bytesBuffer.ReadShort() / 10f;
		position.z = (float)bytesBuffer.ReadShort() / 10f;
		OwnerId = bytesBuffer.ReadInt();
		if (type == 4 || type == 9)
		{
			skillId = bytesBuffer.ReadShort();
		}
		else if (type == 8 || type == 10)
		{
			eX = (float)bytesBuffer.ReadShort() / 10f;
			eY = (float)bytesBuffer.ReadShort() / 10f;
		}
		if (type == 10)
		{
			targetId = bytesBuffer.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		float y = Global.FLOORHEIGHT;
		Ray ray = new Ray(position + new Vector3(0f, 1f, 0f), Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
		{
			y = hitInfo.point.y;
		}
		position.y = y;
		ESpawnObjectType eSpawnObjectType = (ESpawnObjectType)type;
		Player playerByUserID = GameApp.GetInstance().GetGameWorld().GetPlayerByUserID(OwnerId);
		GameObject gameObject = null;
		switch (eSpawnObjectType)
		{
		case ESpawnObjectType.BOMB:
		{
			Object original9 = Resources.Load(EffectName.SKILL_REVENGE_BOMB);
			position += new Vector3(0f, 1f, 0f);
			gameObject = Object.Instantiate(original9, position, Quaternion.identity) as GameObject;
			break;
		}
		case ESpawnObjectType.FLASH:
		{
			Object original8 = Resources.Load(EffectName.SKILL_REVENGE_FALSH);
			position += new Vector3(0f, 1f, 0f);
			gameObject = Object.Instantiate(original8, position, Quaternion.identity) as GameObject;
			break;
		}
		case ESpawnObjectType.MISSLE:
		{
			Object original7 = Resources.Load(EffectName.SKILL_MISSLE);
			gameObject = Object.Instantiate(original7, position, Quaternion.identity) as GameObject;
			break;
		}
		case ESpawnObjectType.NUCLEAR:
		{
			Object original6 = Resources.Load(EffectName.SKILL_NUCLEAR);
			gameObject = Object.Instantiate(original6, position, Quaternion.identity) as GameObject;
			break;
		}
		case ESpawnObjectType.SLOW_AREA:
		{
			Object original5 = Resources.Load(EffectName.SKILL_SLOW_MISSLE);
			gameObject = Object.Instantiate(original5, position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<GeneratedBomb>().SetSkill(skillId);
			break;
		}
		case ESpawnObjectType.SLOW_BOMB:
		{
			Object original4 = Resources.Load(EffectName.SKILL_REVENGE_SLOW_BOMB);
			position += new Vector3(0f, 1f, 0f);
			gameObject = Object.Instantiate(original4, position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<GeneratedBomb>().SetSkill(skillId);
			break;
		}
		case ESpawnObjectType.TRIPPLE_MISSLE:
		{
			Object original3 = Resources.Load(EffectName.SKILLL_TRIPLE_MISSLE);
			gameObject = Object.Instantiate(original3, position, Quaternion.identity) as GameObject;
			break;
		}
		case ESpawnObjectType.MINE:
			position += Vector3.up * 0.2f;
			gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.MINE).CreateObject(position, Vector3.zero, Quaternion.identity);
			break;
		case ESpawnObjectType.IMPACT_WAVE:
		{
			Object original2 = Resources.Load(EffectName.IMPACT_WAVE);
			position += new Vector3(0f, 1f, 0f);
			gameObject = Object.Instantiate(original2, position, Quaternion.Euler(eX, eY, 0f)) as GameObject;
			break;
		}
		case ESpawnObjectType.HOOK:
		{
			Object original = Resources.Load(EffectName.HOOK);
			position += new Vector3(0f, 1f, 0f);
			gameObject = Object.Instantiate(original, position, Quaternion.Euler(eX, eY, 0f)) as GameObject;
			break;
		}
		default:
			Debug.Log("Invalid Revenge Bomb Type!");
			break;
		}
		if (gameObject != null)
		{
			switch (eSpawnObjectType)
			{
			case ESpawnObjectType.IMPACT_WAVE:
			{
				ImpactWave component4 = gameObject.GetComponent<ImpactWave>();
				component4.owner = playerByUserID;
				break;
			}
			case ESpawnObjectType.MINE:
			{
				Mine component3 = gameObject.GetComponent<Mine>();
				component3.owner = playerByUserID;
				component3.Init();
				break;
			}
			case ESpawnObjectType.HOOK:
			{
				HookScript component2 = gameObject.GetComponent<HookScript>();
				component2.owner = playerByUserID;
				component2.SetBeginPos(position);
				component2.SetTargetUnit(targetId);
				break;
			}
			default:
			{
				GeneratedBomb component = gameObject.GetComponent<GeneratedBomb>();
				component.owner = playerByUserID;
				break;
			}
			}
		}
	}
}
