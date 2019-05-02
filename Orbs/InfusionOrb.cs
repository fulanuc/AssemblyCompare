using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000511 RID: 1297
	public class InfusionOrb : Orb
	{
		// Token: 0x06001D53 RID: 7507 RVA: 0x0008FEDC File Offset: 0x0008E0DC
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 30f;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), effectData, true);
			HurtBox component = this.target.GetComponent<HurtBox>();
			CharacterBody characterBody = (component != null) ? component.healthComponent.GetComponent<CharacterBody>() : null;
			if (characterBody)
			{
				this.targetInventory = characterBody.inventory;
			}
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x0008FF6C File Offset: 0x0008E16C
		public override void OnArrival()
		{
			if (this.targetInventory)
			{
				this.targetInventory.AddInfusionBonus((uint)this.maxHpValue);
				HurtBox component = this.target.GetComponent<HurtBox>();
				HealthComponent healthComponent = (component != null) ? component.healthComponent : null;
				if (healthComponent)
				{
					healthComponent.Heal((float)this.maxHpValue, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x04001F73 RID: 8051
		private const float speed = 30f;

		// Token: 0x04001F74 RID: 8052
		public int maxHpValue;

		// Token: 0x04001F75 RID: 8053
		private Inventory targetInventory;
	}
}
