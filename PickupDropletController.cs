using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038B RID: 907
	public class PickupDropletController : NetworkBehaviour
	{
		// Token: 0x060012F8 RID: 4856 RVA: 0x0006AEE0 File Offset: 0x000690E0
		public static void CreatePickupDroplet(PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PickupDroplet"), position, Quaternion.identity);
			gameObject.GetComponent<PickupDropletController>().NetworkpickupIndex = pickupIndex;
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = velocity;
			component.AddTorque(UnityEngine.Random.Range(150f, 120f) * UnityEngine.Random.onUnitSphere);
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0006AF40 File Offset: 0x00069140
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

		// Token: 0x060012FA RID: 4858 RVA: 0x0006AFA4 File Offset: 0x000691A4
		private void Start()
		{
			GameObject pickupDropletDisplayPrefab = this.pickupIndex.GetPickupDropletDisplayPrefab();
			if (pickupDropletDisplayPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(pickupDropletDisplayPrefab, base.transform);
			}
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060012FD RID: 4861 RVA: 0x0006AFD4 File Offset: 0x000691D4
		// (set) Token: 0x060012FE RID: 4862 RVA: 0x0000E89A File Offset: 0x0000CA9A
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

		// Token: 0x060012FF RID: 4863 RVA: 0x0006AFE8 File Offset: 0x000691E8
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

		// Token: 0x06001300 RID: 4864 RVA: 0x0006B054 File Offset: 0x00069254
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

		// Token: 0x040016B9 RID: 5817
		[SyncVar]
		[NonSerialized]
		public PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040016BA RID: 5818
		private bool alive = true;
	}
}
