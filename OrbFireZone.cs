using System;
using System.Collections.Generic;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000382 RID: 898
	public class OrbFireZone : MonoBehaviour
	{
		// Token: 0x060012D2 RID: 4818 RVA: 0x000025DA File Offset: 0x000007DA
		private void Awake()
		{
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0006A0BC File Offset: 0x000682BC
		private void FixedUpdate()
		{
			if (this.previousColliderList.Count > 0)
			{
				this.resetStopwatch += Time.fixedDeltaTime;
				this.removeFromBottomOfListStopwatch += Time.fixedDeltaTime;
				if (this.removeFromBottomOfListStopwatch > 1f / this.orbRemoveFromBottomOfListFrequency)
				{
					this.removeFromBottomOfListStopwatch -= 1f / this.orbRemoveFromBottomOfListFrequency;
					this.previousColliderList.RemoveAt(this.previousColliderList.Count - 1);
				}
				if (this.resetStopwatch > 1f / this.orbResetListFrequency)
				{
					this.resetStopwatch -= 1f / this.orbResetListFrequency;
					this.previousColliderList.Clear();
				}
			}
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x0006A17C File Offset: 0x0006837C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active)
			{
				if (this.previousColliderList.Contains(other))
				{
					return;
				}
				this.previousColliderList.Add(other);
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component && component.mainHurtBox)
				{
					DamageOrb damageOrb = new DamageOrb();
					damageOrb.attacker = null;
					damageOrb.damageOrbType = DamageOrb.DamageOrbType.ClayGooOrb;
					damageOrb.procCoefficient = this.procCoefficient;
					damageOrb.damageValue = this.baseDamage * Run.instance.teamlessDamageCoefficient;
					damageOrb.target = component.mainHurtBox;
					damageOrb.teamIndex = TeamIndex.None;
					RaycastHit raycastHit;
					if (Physics.Raycast(damageOrb.target.transform.position + UnityEngine.Random.insideUnitSphere * 3f, Vector3.down, out raycastHit, 1000f, LayerIndex.world.mask))
					{
						damageOrb.origin = raycastHit.point;
						OrbManager.instance.AddOrb(damageOrb);
					}
				}
			}
		}

		// Token: 0x04001666 RID: 5734
		public float baseDamage;

		// Token: 0x04001667 RID: 5735
		public float procCoefficient;

		// Token: 0x04001668 RID: 5736
		public float orbRemoveFromBottomOfListFrequency;

		// Token: 0x04001669 RID: 5737
		public float orbResetListFrequency;

		// Token: 0x0400166A RID: 5738
		private List<Collider> previousColliderList = new List<Collider>();

		// Token: 0x0400166B RID: 5739
		private float resetStopwatch;

		// Token: 0x0400166C RID: 5740
		private float removeFromBottomOfListStopwatch;
	}
}
