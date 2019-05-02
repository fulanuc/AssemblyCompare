using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000532 RID: 1330
	[RequireComponent(typeof(HealthComponent))]
	public class AssignTeamFilterToTeamComponent : MonoBehaviour
	{
		// Token: 0x06001DE3 RID: 7651 RVA: 0x00093380 File Offset: 0x00091580
		private void Start()
		{
			if (NetworkServer.active)
			{
				TeamComponent component = base.GetComponent<TeamComponent>();
				TeamFilter component2 = base.GetComponent<TeamFilter>();
				if (component2 && component)
				{
					component.teamIndex = component2.teamIndex;
				}
			}
		}
	}
}
