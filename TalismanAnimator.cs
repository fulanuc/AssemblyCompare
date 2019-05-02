using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F3 RID: 1011
	[RequireComponent(typeof(ItemFollower))]
	public class TalismanAnimator : MonoBehaviour
	{
		// Token: 0x0600162A RID: 5674 RVA: 0x0007630C File Offset: 0x0007450C
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

		// Token: 0x0600162B RID: 5675 RVA: 0x00076378 File Offset: 0x00074578
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

		// Token: 0x04001972 RID: 6514
		private float lastCooldownTimer;

		// Token: 0x04001973 RID: 6515
		private EquipmentSlot equipmentSlot;

		// Token: 0x04001974 RID: 6516
		private ItemFollower itemFollower;

		// Token: 0x04001975 RID: 6517
		private ParticleSystem[] killEffects;
	}
}
