using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Orbs
{
	// Token: 0x0200051A RID: 1306
	public class BeetleWardOrb : Orb
	{
		// Token: 0x06001DA9 RID: 7593 RVA: 0x000905B4 File Offset: 0x0008E7B4
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

		// Token: 0x06001DAA RID: 7594 RVA: 0x00090614 File Offset: 0x0008E814
		public override void OnArrival()
		{
			if (this.target)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BeetleWard"), this.target.transform.position, Quaternion.identity);
				gameObject.GetComponent<TeamFilter>().teamIndex = this.target.teamIndex;
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x04001F8E RID: 8078
		public float speed;
	}
}
