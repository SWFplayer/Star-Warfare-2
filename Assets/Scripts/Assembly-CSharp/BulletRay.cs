using System.Collections.Generic;
using UnityEngine;

public class BulletRay : BulletScript
{
	public void ShootDetect(DamageProperty damageProperty, List<Ray> rays, int criticalRate, ObjectPool sparksObjectPool, float range, float damageDeductRate, float pvpDamageRate, bool isPenerating)
	{
		List<Collider> list = new List<Collider>();
		List<Vector3> list2 = new List<Vector3>();
		foreach (Ray ray in rays)
		{
			int layerMask = (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.VOID_SHIELD) | (1 << PhysicsLayer.DESTRUCTIBLE);
			RaycastHit hitInfo;
			if (isPenerating)
			{
				RaycastHit[] array = Physics.RaycastAll(ray, 1000f, layerMask);
				float num = 1000f;
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i].collider.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET || array[i].collider.gameObject.layer == PhysicsLayer.TRANSPARENT_WALL || array[i].collider.gameObject.layer == PhysicsLayer.DESTRUCTIBLE || array[i].collider.gameObject.layer == PhysicsLayer.VOID_SHIELD) && array[i].distance < num)
					{
						num = array[i].distance;
						damageProperty.hitpoint = array[i].point;
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].distance <= num)
					{
						list.Add(array[j].collider);
						list2.Add(array[j].point);
					}
				}
			}
			else if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask))
			{
				list.Add(hitInfo.collider);
				list2.Add(hitInfo.point);
				damageProperty.hitpoint = hitInfo.point;
			}
		}
		DamageDetect(damageProperty, list, list2, criticalRate, sparksObjectPool, range, damageDeductRate, pvpDamageRate);
	}
}
