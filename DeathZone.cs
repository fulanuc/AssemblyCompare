using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C6 RID: 710
	internal class DeathZone : MonoBehaviour
	{
		// Token: 0x06000E72 RID: 3698 RVA: 0x00058D9C File Offset: 0x00056F9C
		public void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active)
			{
				HealthComponent component = other.GetComponent<HealthComponent>();
				if (component)
				{
					component.TakeDamage(new DamageInfo
					{
						position = other.transform.position,
						attacker = null,
						inflictor = base.gameObject,
						damage = 999999f
					});
				}
			}
		}
	}
}
