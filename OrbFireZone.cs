using System;
using System.Collections.Generic;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200037D RID: 893
	public class OrbFireZone : MonoBehaviour
	{
		// Token: 0x060012B2 RID: 4786 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Awake()
		{
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00069D18 File Offset: 0x00067F18
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

		// Token: 0x060012B4 RID: 4788 RVA: 0x00069DD8 File Offset: 0x00067FD8
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

		// Token: 0x0400164A RID: 5706
		public float baseDamage;

		// Token: 0x0400164B RID: 5707
		public float procCoefficient;

		// Token: 0x0400164C RID: 5708
		public float orbRemoveFromBottomOfListFrequency;

		// Token: 0x0400164D RID: 5709
		public float orbResetListFrequency;

		// Token: 0x0400164E RID: 5710
		private List<Collider> previousColliderList = new List<Collider>();

		// Token: 0x0400164F RID: 5711
		private float resetStopwatch;

		// Token: 0x04001650 RID: 5712
		private float removeFromBottomOfListStopwatch;
	}
}
