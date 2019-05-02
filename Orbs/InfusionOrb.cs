using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000520 RID: 1312
	public class InfusionOrb : Orb
	{
		// Token: 0x06001DBB RID: 7611 RVA: 0x00090CB4 File Offset: 0x0008EEB4
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

		// Token: 0x06001DBC RID: 7612 RVA: 0x00015C19 File Offset: 0x00013E19
		public override void OnArrival()
		{
			if (this.targetInventory)
			{
				this.targetInventory.AddInfusionBonus((uint)this.maxHpValue);
			}
		}

		// Token: 0x04001FB1 RID: 8113
		private const float speed = 30f;

		// Token: 0x04001FB2 RID: 8114
		public int maxHpValue;

		// Token: 0x04001FB3 RID: 8115
		private Inventory targetInventory;
	}
}
