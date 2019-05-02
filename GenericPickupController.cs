using System;
using System.Collections.ObjectModel;
using RoR2.Networking;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000301 RID: 769
	public sealed class GenericPickupController : NetworkBehaviour, IInteractable, IDisplayNameProvider
	{
		// Token: 0x06000FA3 RID: 4003 RVA: 0x0000C05A File Offset: 0x0000A25A
		private void SyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.NetworkpickupIndex = newPickupIndex;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0005D668 File Offset: 0x0005B868
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

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0005D6E8 File Offset: 0x0005B8E8
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

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x0000C069 File Offset: 0x0000A269
		private float stopWatch
		{
			get
			{
				return Run.instance.fixedTime - this.waitStartTime;
			}
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0005D848 File Offset: 0x0005BA48
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

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0000C07C File Offset: 0x0000A27C
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00003696 File Offset: 0x00001896
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0005D904 File Offset: 0x0005BB04
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

		// Token: 0x06000FAB RID: 4011 RVA: 0x0005D964 File Offset: 0x0005BB64
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

		// Token: 0x06000FAC RID: 4012 RVA: 0x0005D9C4 File Offset: 0x0005BBC4
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

		// Token: 0x06000FAD RID: 4013 RVA: 0x0005DA14 File Offset: 0x0005BC14
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

		// Token: 0x06000FAE RID: 4014 RVA: 0x0005DB10 File Offset: 0x0005BD10
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

		// Token: 0x06000FAF RID: 4015 RVA: 0x0005DB70 File Offset: 0x0005BD70
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

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0000C0AD File Offset: 0x0000A2AD
		private void Start()
		{
			this.waitStartTime = Run.instance.fixedTime;
			this.consumed = false;
			this.UpdatePickupDisplay();
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x0000C0CC File Offset: 0x0000A2CC
		private void OnEnable()
		{
			InstanceTracker.Add<GenericPickupController>(this);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0000C0D4 File Offset: 0x0000A2D4
		private void OnDisable()
		{
			InstanceTracker.Remove<GenericPickupController>(this);
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0005DC04 File Offset: 0x0005BE04
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

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0000C0DC File Offset: 0x0000A2DC
		public void OnInteractionBegin(Interactor activator)
		{
			this.AttemptGrant(activator.GetComponent<CharacterBody>());
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x000038B4 File Offset: 0x00001AB4
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0000C0EA File Offset: 0x0000A2EA
		public string GetDisplayName()
		{
			return Language.GetString(this.pickupIndex.GetPickupNameToken());
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0005DC50 File Offset: 0x0005BE50
		public void SetPickupIndexFromString(string pickupString)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			PickupIndex networkpickupIndex = PickupIndex.Find(pickupString);
			this.NetworkpickupIndex = networkpickupIndex;
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000FBB RID: 4027 RVA: 0x0005DC74 File Offset: 0x0005BE74
		// (set) Token: 0x06000FBC RID: 4028 RVA: 0x0000C126 File Offset: 0x0000A326
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

		// Token: 0x06000FBD RID: 4029 RVA: 0x0005DC88 File Offset: 0x0005BE88
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

		// Token: 0x06000FBE RID: 4030 RVA: 0x0005DCF4 File Offset: 0x0005BEF4
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

		// Token: 0x040013AD RID: 5037
		public PickupDisplay pickupDisplay;

		// Token: 0x040013AE RID: 5038
		[SyncVar(hook = "SyncPickupIndex")]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040013AF RID: 5039
		public bool selfDestructIfPickupIndexIsNotIdeal;

		// Token: 0x040013B0 RID: 5040
		public SerializablePickupIndex idealPickupIndex;

		// Token: 0x040013B1 RID: 5041
		private static readonly GenericPickupController.PickupMessage pickupMessageInstance = new GenericPickupController.PickupMessage();

		// Token: 0x040013B2 RID: 5042
		public float waitDuration = 0.5f;

		// Token: 0x040013B3 RID: 5043
		private float waitStartTime;

		// Token: 0x040013B4 RID: 5044
		private bool consumed;

		// Token: 0x040013B5 RID: 5045
		public const string pickupSoundString = "Play_UI_item_pickup";

		// Token: 0x02000302 RID: 770
		private class PickupMessage : MessageBase
		{
			// Token: 0x06000FBF RID: 4031 RVA: 0x0000C165 File Offset: 0x0000A365
			public void Reset()
			{
				this.masterGameObject = null;
				this.pickupIndex = PickupIndex.none;
				this.pickupQuantity = 0u;
			}

			// Token: 0x06000FC1 RID: 4033 RVA: 0x0000C180 File Offset: 0x0000A380
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.masterGameObject);
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000FC2 RID: 4034 RVA: 0x0000C1A6 File Offset: 0x0000A3A6
			public override void Deserialize(NetworkReader reader)
			{
				this.masterGameObject = reader.ReadGameObject();
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x040013B6 RID: 5046
			public GameObject masterGameObject;

			// Token: 0x040013B7 RID: 5047
			public PickupIndex pickupIndex;

			// Token: 0x040013B8 RID: 5048
			public uint pickupQuantity;
		}
	}
}
