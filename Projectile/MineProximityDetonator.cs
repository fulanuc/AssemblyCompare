using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053B RID: 1339
	public class MineProximityDetonator : MonoBehaviour
	{
		// Token: 0x06001E06 RID: 7686 RVA: 0x00094310 File Offset: 0x00092510
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

		// Token: 0x0400204E RID: 8270
		public TeamFilter myTeamFilter;

		// Token: 0x0400204F RID: 8271
		public UnityEvent triggerEvents;
	}
}
