using System;
using RoR2.Orbs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000382 RID: 898
	public class PaladinBarrierController : MonoBehaviour, IBarrier
	{
		// Token: 0x060012C2 RID: 4802 RVA: 0x0006A5A4 File Offset: 0x000687A4
		public void BlockedDamage(DamageInfo damageInfo, float actualDamageBlocked)
		{
			this.totalDamageBlocked += actualDamageBlocked;
			LightningOrb lightningOrb = new LightningOrb();
			lightningOrb.teamIndex = this.teamComponent.teamIndex;
			lightningOrb.origin = damageInfo.position;
			lightningOrb.damageValue = actualDamageBlocked * this.blockLaserDamageCoefficient;
			lightningOrb.bouncesRemaining = 0;
			lightningOrb.attacker = damageInfo.attacker;
			lightningOrb.procCoefficient = this.blockLaserProcCoefficient;
			lightningOrb.lightningType = LightningOrb.LightningType.PaladinBarrier;
			HurtBox hurtBox = lightningOrb.PickNextTarget(lightningOrb.origin);
			if (hurtBox)
			{
				lightningOrb.target = hurtBox;
				lightningOrb.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0000E590 File Offset: 0x0000C790
		public void EnableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(true);
			this.barrierIsOn = true;
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0000E5AA File Offset: 0x0000C7AA
		public void DisableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(false);
			this.barrierIsOn = false;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0000E5C4 File Offset: 0x0000C7C4
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.DisableBarrier();
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x0000E5F0 File Offset: 0x0000C7F0
		private void Update()
		{
			if (this.barrierIsOn)
			{
				this.barrierPivotTransform.rotation = Util.QuaternionSafeLookRotation(this.inputBank.aimDirection);
			}
		}

		// Token: 0x0400167D RID: 5757
		public float blockLaserDamageCoefficient;

		// Token: 0x0400167E RID: 5758
		public float blockLaserProcCoefficient;

		// Token: 0x0400167F RID: 5759
		public float blockLaserDistance;

		// Token: 0x04001680 RID: 5760
		private float totalDamageBlocked;

		// Token: 0x04001681 RID: 5761
		private CharacterBody characterBody;

		// Token: 0x04001682 RID: 5762
		private InputBankTest inputBank;

		// Token: 0x04001683 RID: 5763
		private TeamComponent teamComponent;

		// Token: 0x04001684 RID: 5764
		private bool barrierIsOn;

		// Token: 0x04001685 RID: 5765
		public Transform barrierPivotTransform;
	}
}
