using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000383 RID: 899
	public class OrbitalLaserController : MonoBehaviour
	{
		// Token: 0x060012D6 RID: 4822 RVA: 0x0000E68A File Offset: 0x0000C88A
		private void Start()
		{
			this.chargeEffect.SetActive(true);
			this.chargeEffect.GetComponent<ObjectScaleCurve>().timeMax = this.chargeDuration;
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0006A27C File Offset: 0x0006847C
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			float maxSpeed = this.chargeMaxVelocity;
			switch (this.orbitalLaserState)
			{
			case OrbitalLaserController.OrbitalLaserState.Charging:
				maxSpeed = this.chargeMaxVelocity;
				if (this.stopwatch >= this.chargeDuration)
				{
					this.orbitalLaserState = OrbitalLaserController.OrbitalLaserState.Firing;
					this.stopwatch = 0f;
					this.chargeEffect.SetActive(false);
					this.fireEffect.SetActive(true);
				}
				break;
			case OrbitalLaserController.OrbitalLaserState.Firing:
				maxSpeed = this.fireMaxVelocity;
				this.bulletAttackTimer -= Time.fixedDeltaTime;
				if (this.ownerBody && this.bulletAttackTimer < 0f)
				{
					this.bulletAttackTimer += 1f / this.fireFrequency;
					new BulletAttack
					{
						owner = this.ownerBody.gameObject,
						origin = base.transform.position + Vector3.up * 600f,
						maxDistance = 1200f,
						aimVector = Vector3.down,
						minSpread = 0f,
						maxSpread = 0f,
						damage = Mathf.Lerp(this.damageCoefficientInitial, this.damageCoefficientFinal, this.stopwatch / this.fireDuration) * this.ownerBody.damage / this.fireFrequency,
						force = this.force,
						tracerEffectPrefab = this.tracerEffectPrefab,
						muzzleName = "",
						hitEffectPrefab = this.hitEffectPrefab,
						isCrit = Util.CheckRoll(this.ownerBody.crit, this.ownerBody.master),
						stopperMask = LayerIndex.world.mask,
						damageColorIndex = DamageColorIndex.Item,
						procCoefficient = this.procCoefficient / this.fireFrequency,
						radius = 2f
					}.Fire();
				}
				if (this.stopwatch >= this.fireDuration || !this.ownerBody)
				{
					this.orbitalLaserState = OrbitalLaserController.OrbitalLaserState.Decaying;
					this.stopwatch = 0f;
					this.fireEffect.SetActive(false);
				}
				break;
			case OrbitalLaserController.OrbitalLaserState.Decaying:
				maxSpeed = 0f;
				if (this.stopwatch >= this.decayDuration)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			}
			Vector3 target = base.transform.position;
			if (this.ownerBody)
			{
				this.ownerInputBank = this.ownerBody.GetComponent<InputBankTest>();
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray
				{
					direction = this.ownerInputBank.aimDirection,
					origin = this.ownerInputBank.aimOrigin
				}, out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					target = raycastHit.point;
				}
				base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref this.refVelocity, this.smoothDampTime, maxSpeed);
			}
		}

		// Token: 0x0400166D RID: 5741
		public CharacterBody ownerBody;

		// Token: 0x0400166E RID: 5742
		public GameObject chargeEffect;

		// Token: 0x0400166F RID: 5743
		public GameObject fireEffect;

		// Token: 0x04001670 RID: 5744
		public float chargeDuration = 3f;

		// Token: 0x04001671 RID: 5745
		public float fireDuration = 6f;

		// Token: 0x04001672 RID: 5746
		public float decayDuration = 1.5f;

		// Token: 0x04001673 RID: 5747
		public float chargeMaxVelocity = 20f;

		// Token: 0x04001674 RID: 5748
		public float fireMaxVelocity = 1f;

		// Token: 0x04001675 RID: 5749
		public float smoothDampTime = 0.3f;

		// Token: 0x04001676 RID: 5750
		public float fireFrequency = 5f;

		// Token: 0x04001677 RID: 5751
		public float damageCoefficientInitial = 6f;

		// Token: 0x04001678 RID: 5752
		public float damageCoefficientFinal = 6f;

		// Token: 0x04001679 RID: 5753
		public float procCoefficient = 0.5f;

		// Token: 0x0400167A RID: 5754
		public float force;

		// Token: 0x0400167B RID: 5755
		public GameObject tracerEffectPrefab;

		// Token: 0x0400167C RID: 5756
		public GameObject hitEffectPrefab;

		// Token: 0x0400167D RID: 5757
		private float stopwatch;

		// Token: 0x0400167E RID: 5758
		private float bulletAttackTimer;

		// Token: 0x0400167F RID: 5759
		private OrbitalLaserController.OrbitalLaserState orbitalLaserState;

		// Token: 0x04001680 RID: 5760
		private Vector3 refVelocity;

		// Token: 0x04001681 RID: 5761
		private Transform chargeEffectTransform;

		// Token: 0x04001682 RID: 5762
		private InputBankTest ownerInputBank;

		// Token: 0x02000384 RID: 900
		public enum OrbitalLaserState
		{
			// Token: 0x04001684 RID: 5764
			Charging,
			// Token: 0x04001685 RID: 5765
			Firing,
			// Token: 0x04001686 RID: 5766
			Decaying
		}
	}
}
