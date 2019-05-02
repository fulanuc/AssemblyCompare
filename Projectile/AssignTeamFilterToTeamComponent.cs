using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000541 RID: 1345
	[RequireComponent(typeof(HealthComponent))]
	public class AssignTeamFilterToTeamComponent : MonoBehaviour
	{
		// Token: 0x06001E4D RID: 7757 RVA: 0x0009409C File Offset: 0x0009229C
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
