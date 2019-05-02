using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061D RID: 1565
	public class NotificationQueue : MonoBehaviour
	{
		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06002382 RID: 9090 RVA: 0x00019E4D File Offset: 0x0001804D
		public static ReadOnlyCollection<NotificationQueue> readOnlyInstancesList
		{
			get
			{
				return NotificationQueue._readOnlyInstancesList;
			}
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x00019E54 File Offset: 0x00018054
		private void OnEnable()
		{
			NotificationQueue.instancesList.Add(this);
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x00019E61 File Offset: 0x00018061
		private void OnDisable()
		{
			NotificationQueue.instancesList.Remove(this);
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000A943C File Offset: 0x000A763C
		private void OnItemPickup(CharacterMaster characterMaster, ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			if (itemDef == null || itemDef.hidden)
			{
				return;
			}
			if (this.hud.targetMaster == characterMaster)
			{
				this.notificationQueue.Enqueue(new NotificationQueue.NotificationInfo
				{
					notificationType = NotificationQueue.NotificationType.ItemPickup,
					itemIndex = itemIndex
				});
			}
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x00019E6F File Offset: 0x0001806F
		private void OnEquipmentPickup(CharacterMaster characterMaster, EquipmentIndex equipmentIndex)
		{
			if (this.hud.targetMaster == characterMaster)
			{
				this.notificationQueue.Enqueue(new NotificationQueue.NotificationInfo
				{
					notificationType = NotificationQueue.NotificationType.EquipmentPickup,
					equipmentIndex = equipmentIndex
				});
			}
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x000A9490 File Offset: 0x000A7690
		public void OnPickup(CharacterMaster characterMaster, PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex >= ItemIndex.Syringe)
			{
				this.OnItemPickup(characterMaster, itemIndex);
				return;
			}
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			if (equipmentIndex >= EquipmentIndex.CommandMissile)
			{
				this.OnEquipmentPickup(characterMaster, equipmentIndex);
			}
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x000A94C8 File Offset: 0x000A76C8
		public void Update()
		{
			if (!this.currentNotification && this.notificationQueue.Count > 0)
			{
				NotificationQueue.NotificationInfo notificationInfo = this.notificationQueue.Dequeue();
				this.currentNotification = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NotificationPanel2")).GetComponent<GenericNotification>();
				NotificationQueue.NotificationType notificationType = notificationInfo.notificationType;
				if (notificationType != NotificationQueue.NotificationType.ItemPickup)
				{
					if (notificationType == NotificationQueue.NotificationType.EquipmentPickup)
					{
						this.currentNotification.SetEquipment(notificationInfo.equipmentIndex);
					}
				}
				else
				{
					this.currentNotification.SetItem(notificationInfo.itemIndex);
				}
				this.currentNotification.GetComponent<RectTransform>().SetParent(base.GetComponent<RectTransform>(), false);
			}
		}

		// Token: 0x0400262B RID: 9771
		private static List<NotificationQueue> instancesList = new List<NotificationQueue>();

		// Token: 0x0400262C RID: 9772
		private static ReadOnlyCollection<NotificationQueue> _readOnlyInstancesList = new ReadOnlyCollection<NotificationQueue>(NotificationQueue.instancesList);

		// Token: 0x0400262D RID: 9773
		public HUD hud;

		// Token: 0x0400262E RID: 9774
		private Queue<NotificationQueue.NotificationInfo> notificationQueue = new Queue<NotificationQueue.NotificationInfo>();

		// Token: 0x0400262F RID: 9775
		private GenericNotification currentNotification;

		// Token: 0x0200061E RID: 1566
		private enum NotificationType
		{
			// Token: 0x04002631 RID: 9777
			ItemPickup,
			// Token: 0x04002632 RID: 9778
			EquipmentPickup
		}

		// Token: 0x0200061F RID: 1567
		private class NotificationInfo
		{
			// Token: 0x04002633 RID: 9779
			public NotificationQueue.NotificationType notificationType;

			// Token: 0x04002634 RID: 9780
			public ItemIndex itemIndex = ItemIndex.None;

			// Token: 0x04002635 RID: 9781
			public EquipmentIndex equipmentIndex = EquipmentIndex.None;
		}
	}
}
