using System;
using RoR2.Orbs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000387 RID: 903
	public class PaladinBarrierController : MonoBehaviour, IBarrier
	{
		// Token: 0x060012E2 RID: 4834 RVA: 0x0006A948 File Offset: 0x00068B48
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

		// Token: 0x060012E3 RID: 4835 RVA: 0x0000E71B File Offset: 0x0000C91B
		public void EnableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(true);
			this.barrierIsOn = true;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0000E735 File Offset: 0x0000C935
		public void DisableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(false);
			this.barrierIsOn = false;
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0000E74F File Offset: 0x0000C94F
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.DisableBarrier();
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0000E77B File Offset: 0x0000C97B
		private void Update()
		{
			if (this.barrierIsOn)
			{
				this.barrierPivotTransform.rotation = Util.QuaternionSafeLookRotation(this.inputBank.aimDirection);
			}
		}

		// Token: 0x04001699 RID: 5785
		public float blockLaserDamageCoefficient;

		// Token: 0x0400169A RID: 5786
		public float blockLaserProcCoefficient;

		// Token: 0x0400169B RID: 5787
		public float blockLaserDistance;

		// Token: 0x0400169C RID: 5788
		private float totalDamageBlocked;

		// Token: 0x0400169D RID: 5789
		private CharacterBody characterBody;

		// Token: 0x0400169E RID: 5790
		private InputBankTest inputBank;

		// Token: 0x0400169F RID: 5791
		private TeamComponent teamComponent;

		// Token: 0x040016A0 RID: 5792
		private bool barrierIsOn;

		// Token: 0x040016A1 RID: 5793
		public Transform barrierPivotTransform;
	}
}
