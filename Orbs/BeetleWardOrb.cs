using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Orbs
{
	// Token: 0x0200050B RID: 1291
	public class BeetleWardOrb : Orb
	{
		// Token: 0x06001D41 RID: 7489 RVA: 0x0008F7DC File Offset: 0x0008D9DC
		public override void Begin()
		{
			base.duration = base.distanceToTarget / this.speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BeetleWardOrbEffect"), effectData, true);
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0008F83C File Offset: 0x0008DA3C
		public override void OnArrival()
		{
			if (this.target)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BeetleWard"), this.target.transform.position, Quaternion.identity);
				gameObject.GetComponent<TeamFilter>().teamIndex = this.target.teamIndex;
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x04001F50 RID: 8016
		public float speed;
	}
}
