using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054A RID: 1354
	public class MineProximityDetonator : MonoBehaviour
	{
		// Token: 0x06001E70 RID: 7792 RVA: 0x0009502C File Offset: 0x0009322C
		public void OnTriggerEnter(Collider collider)
		{
			if (NetworkServer.active)
			{
				if (collider)
				{
					HurtBox component = collider.GetComponent<HurtBox>();
					if (component)
					{
						TeamComponent component2 = component.healthComponent.GetComponent<TeamComponent>();
						if (component2 && component2.teamIndex == this.myTeamFilter.teamIndex)
						{
							return;
						}
						UnityEvent unityEvent = this.triggerEvents;
						if (unityEvent == null)
						{
							return;
						}
						unityEvent.Invoke();
					}
				}
				return;
			}
		}

		// Token: 0x0400208C RID: 8332
		public TeamFilter myTeamFilter;

		// Token: 0x0400208D RID: 8333
		public UnityEvent triggerEvents;
	}
}
