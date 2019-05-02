using System;
using System.Collections.Generic;
using EntityStates;
using EntityStates.Barrel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000299 RID: 665
	public class ChestBehavior : NetworkBehaviour
	{
		// Token: 0x06000D91 RID: 3473 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x00055124 File Offset: 0x00053324
		[Server]
		private void PickFromList(List<PickupIndex> dropList)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::PickFromList(System.Collections.Generic.List`1<RoR2.PickupIndex>)' called on client");
				return;
			}
			this.dropPickup = PickupIndex.none;
			if (dropList != null && dropList.Count > 0)
			{
				this.dropPickup = dropList[Run.instance.treasureRng.RangeInt(0, dropList.Count)];
			}
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00055180 File Offset: 0x00053380
		[Server]
		public void RollItem()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::RollItem()' called on client");
				return;
			}
			WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
			weightedSelection.AddChoice(Run.instance.availableTier1DropList, this.tier1Chance);
			weightedSelection.AddChoice(Run.instance.availableTier2DropList, this.tier2Chance);
			weightedSelection.AddChoice(Run.instance.availableTier3DropList, this.tier3Chance);
			weightedSelection.AddChoice(Run.instance.availableLunarDropList, this.lunarChance);
			List<PickupIndex> dropList = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
			this.PickFromList(dropList);
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x0000A952 File Offset: 0x00008B52
		[Server]
		public void RollEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::RollEquipment()' called on client");
				return;
			}
			this.PickFromList(Run.instance.availableEquipmentDropList);
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x0000A979 File Offset: 0x00008B79
		private void Start()
		{
			if (NetworkServer.active)
			{
				if (this.dropRoller != null)
				{
					this.dropRoller.Invoke();
					return;
				}
				Debug.LogFormat("Chest {0} has no item roller assigned!", Array.Empty<object>());
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0005521C File Offset: 0x0005341C
		[Server]
		public void Open()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::Open()' called on client");
				return;
			}
			EntityStateMachine component = base.GetComponent<EntityStateMachine>();
			if (component)
			{
				component.SetNextState(EntityState.Instantiate(this.openState));
			}
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x00055260 File Offset: 0x00053460
		[Server]
		public void ItemDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::ItemDrop()' called on client");
				return;
			}
			if (this.dropPickup == PickupIndex.none)
			{
				return;
			}
			PickupDropletController.CreatePickupDroplet(this.dropPickup, base.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + base.transform.forward * 2f);
			this.dropPickup = PickupIndex.none;
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400117A RID: 4474
		private PickupIndex dropPickup = PickupIndex.none;

		// Token: 0x0400117B RID: 4475
		public float tier1Chance = 0.8f;

		// Token: 0x0400117C RID: 4476
		public float tier2Chance = 0.2f;

		// Token: 0x0400117D RID: 4477
		public float tier3Chance = 0.01f;

		// Token: 0x0400117E RID: 4478
		public float lunarChance;

		// Token: 0x0400117F RID: 4479
		public UnityEvent dropRoller;

		// Token: 0x04001180 RID: 4480
		public SerializableEntityStateType openState = new SerializableEntityStateType(typeof(Opening));
	}
}
