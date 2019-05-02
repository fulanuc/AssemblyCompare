using System;
using System.Globalization;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000386 RID: 902
	public class PickupDropletController : NetworkBehaviour
	{
		// Token: 0x060012D8 RID: 4824 RVA: 0x0006AB3C File Offset: 0x00068D3C
		public static void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PickupDroplet"), position, Quaternion.identity);
			gameObject.GetComponent<PickupDropletController>().NetworkpickupIndex = pickupIndex;
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = velocity;
			component.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0006AB9C File Offset: 0x00068D9C
		public void OnCollisionEnter(Collision collision)
		{
			if (NetworkServer.active && this.alive)
			{
				this.alive = false;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericPickup"), base.transform.position, Quaternion.identity);
				gameObject.GetComponent<GenericPickupController>().NetworkpickupIndex = this.pickupIndex;
				NetworkServer.Spawn(gameObject);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0006AC00 File Offset: 0x00068E00
		private void Start()
		{
			GameObject pickupDropletDisplayPrefab = this.pickupIndex.GetPickupDropletDisplayPrefab();
			if (pickupDropletDisplayPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(pickupDropletDisplayPrefab, base.transform);
			}
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0006AC30 File Offset: 0x00068E30
		[ConCommand(commandName = "create_all_pickups", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Creates all items and equipment.")]
		private static void CCCreateAllPickups(ConCommandArgs args)
		{
			for (int i = 0; i < 78; i++)
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				string format = "create_pickup ItemIndex.{0}";
				ItemIndex itemIndex = (ItemIndex)i;
				string cmd = string.Format(invariantCulture, format, itemIndex.ToString());
				Console.instance.SubmitCmd(args.sender, cmd, false);
			}
			for (int j = 0; j < 27; j++)
			{
				IFormatProvider invariantCulture2 = CultureInfo.InvariantCulture;
				string format2 = "create_pickup EquipmentIndex.{0}";
				EquipmentIndex equipmentIndex = (EquipmentIndex)j;
				string cmd2 = string.Format(invariantCulture2, format2, equipmentIndex.ToString());
				Console.instance.SubmitCmd(args.sender, cmd2, false);
			}
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0006ACBC File Offset: 0x00068EBC
		[ConCommand(commandName = "create_pickup", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Spawns the named pickup where the sender is looking.")]
		private static void CCCreatePickup(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			PickupIndex pickupIndex = PickupIndex.Find(args[0]);
			GameObject senderMasterObject = args.senderMasterObject;
			if (!senderMasterObject)
			{
				return;
			}
			CharacterMaster component = senderMasterObject.GetComponent<CharacterMaster>();
			if (!component)
			{
				return;
			}
			CharacterBody body = component.GetBody();
			if (!body)
			{
				return;
			}
			InputBankTest component2 = body.GetComponent<InputBankTest>();
			if (!component2)
			{
				return;
			}
			Ray ray = new Ray(component2.aimOrigin, component2.aimDirection);
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(body.gameObject, ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				PickupDropletController.CreatePickupDroplet(pickupIndex, raycastHit.point + Vector3.up, Vector3.zero);
			}
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060012DF RID: 4831 RVA: 0x0006AD78 File Offset: 0x00068F78
		// (set) Token: 0x060012E0 RID: 4832 RVA: 0x0000E70F File Offset: 0x0000C90F
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, 1u);
			}
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0006AD8C File Offset: 0x00068F8C
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

		// Token: 0x060012E2 RID: 4834 RVA: 0x0006ADF8 File Offset: 0x00068FF8
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
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
			}
		}

		// Token: 0x0400169D RID: 5789
		[SyncVar]
		[NonSerialized]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x0400169E RID: 5790
		private bool alive = true;
	}
}
