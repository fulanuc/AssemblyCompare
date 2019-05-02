using System;
using System.Collections.ObjectModel;
using RoR2.Networking;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FE RID: 766
	public class GenericPickupController : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000F90 RID: 3984 RVA: 0x0000BF80 File Offset: 0x0000A180
		private void SyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.NetworkpickupIndex = newPickupIndex;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0005D448 File Offset: 0x0005B648
		[Server]
		private static void SendPickupMessage(CharacterMaster master, PickupIndex pickupIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::SendPickupMessage(RoR2.CharacterMaster,RoR2.PickupIndex)' called on client");
				return;
			}
			uint pickupQuantity = 1u;
			if (master.inventory)
			{
				ItemIndex itemIndex = pickupIndex.itemIndex;
				if (itemIndex != ItemIndex.None)
				{
					pickupQuantity = (uint)master.inventory.GetItemCount(itemIndex);
				}
			}
			GenericPickupController.PickupMessage msg = new GenericPickupController.PickupMessage
			{
				masterGameObject = master.gameObject,
				pickupIndex = pickupIndex,
				pickupQuantity = pickupQuantity
			};
			NetworkServer.SendByChannelToAll(57, msg, QosChannelIndex.chat.intVal);
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0005D4C8 File Offset: 0x0005B6C8
		[NetworkMessageHandler(msgType = 57, client = true)]
		private static void HandlePickupMessage(NetworkMessage netMsg)
		{
			Debug.Log("GenericPickupController.HandlePickupMessage: Received pickup message.");
			ReadOnlyCollection<NotificationQueue> readOnlyInstancesList = NotificationQueue.readOnlyInstancesList;
			GenericPickupController.PickupMessage pickupMessage = GenericPickupController.pickupMessageInstance;
			netMsg.ReadMessage<GenericPickupController.PickupMessage>(pickupMessage);
			GameObject masterGameObject = pickupMessage.masterGameObject;
			PickupIndex pickupIndex = pickupMessage.pickupIndex;
			uint pickupQuantity = pickupMessage.pickupQuantity;
			pickupMessage.Reset();
			if (!masterGameObject)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! masterObject is not valid.");
				return;
			}
			CharacterMaster component = masterGameObject.GetComponent<CharacterMaster>();
			if (!component)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! master component is not valid.");
				return;
			}
			PlayerCharacterMasterController component2 = component.GetComponent<PlayerCharacterMasterController>();
			if (component2)
			{
				NetworkUser networkUser = component2.networkUser;
				if (networkUser)
				{
					LocalUser localUser = networkUser.localUser;
					if (localUser != null)
					{
						localUser.userProfile.DiscoverPickup(pickupIndex);
					}
				}
			}
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				readOnlyInstancesList[i].OnPickup(component, pickupIndex);
			}
			CharacterBody body = component.GetBody();
			if (!body)
			{
				Debug.Log("GenericPickupController.HandlePickupMessage: failed! characterBody is not valid.");
			}
			ItemDef itemDef = ItemCatalog.GetItemDef(pickupIndex.itemIndex);
			if (itemDef != null && itemDef.hidden)
			{
				Debug.LogFormat("GenericPickupController.HandlePickupMessage: skipped item {0}, marked hidden.", new object[]
				{
					itemDef.nameToken
				});
				return;
			}
			Chat.AddPickupMessage(body, pickupIndex.GetPickupNameToken(), pickupIndex.GetPickupColor(), pickupQuantity);
			if (body)
			{
				Util.PlaySound("Play_UI_item_pickup", body.gameObject);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0000BF8F File Offset: 0x0000A18F
		private float stopWatch
		{
			get
			{
				return Run.instance.fixedTime - this.waitStartTime;
			}
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0005D628 File Offset: 0x0005B828
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.stopWatch >= this.waitDuration && !this.consumed)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					ItemIndex itemIndex = this.pickupIndex.itemIndex;
					if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Lunar)
					{
						return;
					}
					EquipmentIndex equipmentIndex = this.pickupIndex.equipmentIndex;
					if (equipmentIndex != EquipmentIndex.None)
					{
						if (EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar)
						{
							return;
						}
						if (component.inventory && component.inventory.currentEquipmentIndex != EquipmentIndex.None)
						{
							return;
						}
					}
					if (this.pickupIndex.coinIndex != -1)
					{
						return;
					}
					if (GenericPickupController.BodyHasPickupPermission(component))
					{
						this.AttemptGrant(component);
					}
				}
			}
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0005D6E4 File Offset: 0x0005B8E4
		public string GetContextString(Interactor activator)
		{
			string token = "";
			if (this.pickupIndex.itemIndex != ItemIndex.None)
			{
				token = "ITEM_PICKUP_CONTEXT";
			}
			if (this.pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				token = "EQUIPMENT_PICKUP_CONTEXT";
			}
			if (this.pickupIndex.coinIndex != -1)
			{
				token = "LUNAR_COIN_PICKUP_CONTEXT";
			}
			return string.Format(Language.GetString(token), this.GetDisplayName());
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0005D744 File Offset: 0x0005B944
		private void UpdatePickupDisplay()
		{
			if (!this.pickupDisplay)
			{
				return;
			}
			this.pickupDisplay.SetPickupIndex(this.pickupIndex, false);
			if (this.pickupDisplay.modelRenderer)
			{
				Highlight component = base.GetComponent<Highlight>();
				if (component)
				{
					component.targetRenderer = this.pickupDisplay.modelRenderer;
				}
			}
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0005D7A4 File Offset: 0x0005B9A4
		[Server]
		private void GrantItem(CharacterBody body, Inventory inventory)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantItem(RoR2.CharacterBody,RoR2.Inventory)' called on client");
				return;
			}
			inventory.GiveItem(this.pickupIndex.itemIndex, 1);
			GenericPickupController.SendPickupMessage(inventory.GetComponent<CharacterMaster>(), this.pickupIndex);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0005D7F4 File Offset: 0x0005B9F4
		[Server]
		private void GrantEquipment(CharacterBody body, Inventory inventory)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantEquipment(RoR2.CharacterBody,RoR2.Inventory)' called on client");
				return;
			}
			this.waitStartTime = Run.instance.fixedTime;
			EquipmentIndex currentEquipmentIndex = inventory.currentEquipmentIndex;
			EquipmentIndex equipmentIndex = this.pickupIndex.equipmentIndex;
			inventory.SetEquipmentIndex(equipmentIndex);
			this.NetworkpickupIndex = new PickupIndex(currentEquipmentIndex);
			this.consumed = false;
			GenericPickupController.SendPickupMessage(inventory.GetComponent<CharacterMaster>(), new PickupIndex(equipmentIndex));
			if (this.pickupIndex == PickupIndex.none)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.selfDestructIfPickupIndexIsNotIdeal && this.pickupIndex != PickupIndex.Find(this.idealPickupIndex.pickupName))
			{
				PickupDropletController.CreatePickupDroplet(this.pickupIndex, base.transform.position, new Vector3(UnityEngine.Random.Range(-4f, 4f), 20f, UnityEngine.Random.Range(-4f, 4f)));
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0005D8F0 File Offset: 0x0005BAF0
		[Server]
		private void GrantLunarCoin(CharacterBody body, uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::GrantLunarCoin(RoR2.CharacterBody,System.UInt32)' called on client");
				return;
			}
			CharacterMaster master = body.master;
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(body);
			if (networkUser)
			{
				if (master)
				{
					GenericPickupController.SendPickupMessage(master, this.pickupIndex);
				}
				networkUser.AwardLunarCoins(count);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0005D950 File Offset: 0x0005BB50
		[Server]
		private void AttemptGrant(CharacterBody body)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::AttemptGrant(RoR2.CharacterBody)' called on client");
				return;
			}
			TeamComponent component = body.GetComponent<TeamComponent>();
			if (component && component.teamIndex == TeamIndex.Player)
			{
				Inventory inventory = body.inventory;
				if (inventory)
				{
					this.consumed = true;
					if (this.pickupIndex.itemIndex != ItemIndex.None)
					{
						this.GrantItem(body, inventory);
					}
					if (this.pickupIndex.equipmentIndex != EquipmentIndex.None)
					{
						this.GrantEquipment(body, inventory);
					}
					if (this.pickupIndex.coinIndex != -1)
					{
						this.GrantLunarCoin(body, 1u);
					}
				}
			}
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0000BFD3 File Offset: 0x0000A1D3
		private void Start()
		{
			this.waitStartTime = Run.instance.fixedTime;
			this.consumed = false;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0005D9E4 File Offset: 0x0005BBE4
		public Interactability GetInteractability(Interactor activator)
		{
			if (!base.enabled)
			{
				return Interactability.Disabled;
			}
			if (this.stopWatch < this.waitDuration || this.consumed)
			{
				return Interactability.Disabled;
			}
			CharacterBody component = activator.GetComponent<CharacterBody>();
			if (!component)
			{
				return Interactability.Disabled;
			}
			if (!GenericPickupController.BodyHasPickupPermission(component))
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0000BFF2 File Offset: 0x0000A1F2
		public void OnInteractionBegin(Interactor activator)
		{
			this.AttemptGrant(activator.GetComponent<CharacterBody>());
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0000C000 File Offset: 0x0000A200
		public string GetDisplayName()
		{
			return Language.GetString(this.pickupIndex.GetPickupNameToken());
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0005DA30 File Offset: 0x0005BC30
		public void SetPickupIndexFromString(string pickupString)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			PickupIndex networkpickupIndex = PickupIndex.Find(pickupString);
			this.NetworkpickupIndex = networkpickupIndex;
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x0005DA54 File Offset: 0x0005BC54
		// (set) Token: 0x06000FA6 RID: 4006 RVA: 0x0000C03C File Offset: 0x0000A23C
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0005DA68 File Offset: 0x0005BC68
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0005DAD4 File Offset: 0x0005BCD4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SyncPickupIndex(GeneratedNetworkCode._ReadPickupIndex_None(reader));
			}
		}

		// Token: 0x04001395 RID: 5013
		public PickupDisplay pickupDisplay;

		// Token: 0x04001396 RID: 5014
		[SyncVar(hook = "SyncPickupIndex")]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x04001397 RID: 5015
		public bool selfDestructIfPickupIndexIsNotIdeal;

		// Token: 0x04001398 RID: 5016
		public SerializablePickupIndex idealPickupIndex;

		// Token: 0x04001399 RID: 5017
		private static readonly GenericPickupController.PickupMessage pickupMessageInstance = new GenericPickupController.PickupMessage();

		// Token: 0x0400139A RID: 5018
		public float waitDuration = 0.5f;

		// Token: 0x0400139B RID: 5019
		private float waitStartTime;

		// Token: 0x0400139C RID: 5020
		private bool consumed;

		// Token: 0x0400139D RID: 5021
		public const string pickupSoundString = "Play_UI_item_pickup";

		// Token: 0x020002FF RID: 767
		private class PickupMessage : MessageBase
		{
			// Token: 0x06000FA9 RID: 4009 RVA: 0x0000C07B File Offset: 0x0000A27B
			public void Reset()
			{
				this.masterGameObject = null;
				this.pickupIndex = PickupIndex.none;
				this.pickupQuantity = 0u;
			}

			// Token: 0x06000FAB RID: 4011 RVA: 0x0000C096 File Offset: 0x0000A296
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.masterGameObject);
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000FAC RID: 4012 RVA: 0x0000C0BC File Offset: 0x0000A2BC
			public override void Deserialize(NetworkReader reader)
			{
				this.masterGameObject = reader.ReadGameObject();
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x0400139E RID: 5022
			public GameObject masterGameObject;

			// Token: 0x0400139F RID: 5023
			public PickupIndex pickupIndex;

			// Token: 0x040013A0 RID: 5024
			public uint pickupQuantity;
		}
	}
}
