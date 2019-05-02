using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060B RID: 1547
	public class NotificationQueue : MonoBehaviour
	{
		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060022F2 RID: 8946 RVA: 0x00019796 File Offset: 0x00017996
		public static ReadOnlyCollection<NotificationQueue> readOnlyInstancesList
		{
			get
			{
				return NotificationQueue._readOnlyInstancesList;
			}
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x0001979D File Offset: 0x0001799D
		private void OnEnable()
		{
			NotificationQueue.instancesList.Add(this);
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x000197AA File Offset: 0x000179AA
		private void OnDisable()
		{
			NotificationQueue.instancesList.Remove(this);
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000A7DC0 File Offset: 0x000A5FC0
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

		// Token: 0x060022F6 RID: 8950 RVA: 0x000197B8 File Offset: 0x000179B8
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

		// Token: 0x060022F7 RID: 8951 RVA: 0x000A7E14 File Offset: 0x000A6014
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

		// Token: 0x060022F8 RID: 8952 RVA: 0x000A7E4C File Offset: 0x000A604C
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

		// Token: 0x040025D0 RID: 9680
		private static List<NotificationQueue> instancesList = new List<NotificationQueue>();

		// Token: 0x040025D1 RID: 9681
		private static ReadOnlyCollection<NotificationQueue> _readOnlyInstancesList = new ReadOnlyCollection<NotificationQueue>(NotificationQueue.instancesList);

		// Token: 0x040025D2 RID: 9682
		public HUD hud;

		// Token: 0x040025D3 RID: 9683
		private Queue<NotificationQueue.NotificationInfo> notificationQueue = new Queue<NotificationQueue.NotificationInfo>();

		// Token: 0x040025D4 RID: 9684
		private GenericNotification currentNotification;

		// Token: 0x0200060C RID: 1548
		private enum NotificationType
		{
			// Token: 0x040025D6 RID: 9686
			ItemPickup,
			// Token: 0x040025D7 RID: 9687
			EquipmentPickup
		}

		// Token: 0x0200060D RID: 1549
		private class NotificationInfo
		{
			// Token: 0x040025D8 RID: 9688
			public NotificationQueue.NotificationType notificationType;

			// Token: 0x040025D9 RID: 9689
			public ItemIndex itemIndex = ItemIndex.None;

			// Token: 0x040025DA RID: 9690
			public EquipmentIndex equipmentIndex = EquipmentIndex.None;
		}
	}
}
