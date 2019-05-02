using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F9 RID: 1017
	[RequireComponent(typeof(ItemFollower))]
	public class TalismanAnimator : MonoBehaviour
	{
		// Token: 0x0600166A RID: 5738 RVA: 0x00076944 File Offset: 0x00074B44
		private void Start()
		{
			this.itemFollower = base.GetComponent<ItemFollower>();
			if (this.itemFollower.followerInstance)
			{
				this.killEffects = this.itemFollower.followerInstance.GetComponentsInChildren<ParticleSystem>();
			}
			CharacterModel componentInParent = base.GetComponentInParent<CharacterModel>();
			if (componentInParent)
			{
				CharacterBody body = componentInParent.body;
				if (body)
				{
					this.equipmentSlot = body.equipmentSlot;
				}
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x000769B0 File Offset: 0x00074BB0
		private void FixedUpdate()
		{
			if (this.equipmentSlot)
			{
				float cooldownTimer = this.equipmentSlot.cooldownTimer;
				if (this.lastCooldownTimer - cooldownTimer >= 0.5f)
				{
					ParticleSystem[] array = this.killEffects;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
				}
				this.lastCooldownTimer = cooldownTimer;
			}
		}

		// Token: 0x0400199B RID: 6555
		private float lastCooldownTimer;

		// Token: 0x0400199C RID: 6556
		private EquipmentSlot equipmentSlot;

		// Token: 0x0400199D RID: 6557
		private ItemFollower itemFollower;

		// Token: 0x0400199E RID: 6558
		private ParticleSystem[] killEffects;
	}
}
