using System;
using System.Linq;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000387 RID: 903
	public class PingerController : NetworkBehaviour
	{
		// Token: 0x060012E3 RID: 4835 RVA: 0x0006AE3C File Offset: 0x0006903C
		private void RebuildPing(PingerController.PingInfo pingInfo)
		{
			if (!pingInfo.active && this.pingIndicator != null)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
				this.pingIndicator = null;
				return;
			}
			if (!this.pingIndicator)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/PingIndicator"));
				this.pingIndicator = gameObject.GetComponent<PingIndicator>();
				this.pingIndicator.pingOwner = base.gameObject;
			}
			this.pingIndicator.pingOrigin = pingInfo.origin;
			this.pingIndicator.pingNormal = pingInfo.normal;
			this.pingIndicator.pingTarget = pingInfo.targetGameObject;
			this.pingIndicator.RebuildPing();
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0000E723 File Offset: 0x0000C923
		private void OnDestroy()
		{
			if (this.pingIndicator)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
			}
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0000E742 File Offset: 0x0000C942
		private void OnSyncCurrentPing(PingerController.PingInfo newPingInfo)
		{
			if (base.hasAuthority)
			{
				return;
			}
			this.SetCurrentPing(newPingInfo);
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0000E754 File Offset: 0x0000C954
		private void SetCurrentPing(PingerController.PingInfo newPingInfo)
		{
			this.NetworkcurrentPing = newPingInfo;
			this.RebuildPing(this.currentPing);
			if (base.hasAuthority)
			{
				this.CallCmdPing(this.currentPing);
			}
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0000E77D File Offset: 0x0000C97D
		[Command]
		private void CmdPing(PingerController.PingInfo incomingPing)
		{
			this.NetworkcurrentPing = incomingPing;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0006AEF0 File Offset: 0x000690F0
		private void FixedUpdate()
		{
			if (base.hasAuthority)
			{
				this.pingRechargeStopwatch -= Time.fixedDeltaTime;
				if (this.pingRechargeStopwatch <= 0f)
				{
					this.pingStock = Mathf.Min(this.pingStock + 1, 3);
					this.pingRechargeStopwatch = 1.5f;
				}
			}
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0006AF44 File Offset: 0x00069144
		public void AttemptPing(Ray aimRay, GameObject bodyObject)
		{
			if (this.pingStock <= 0)
			{
				Chat.AddMessage(Language.GetString("PLAYER_PING_COOLDOWN"));
				return;
			}
			PingerController.PingInfo pingInfo = new PingerController.PingInfo
			{
				active = true
			};
			this.pingStock--;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.maxDistanceFilter = 1000f;
			bullseyeSearch.maxAngleFilter = 10f;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(bodyObject);
			HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			RaycastHit raycastHit;
			if (hurtBox && hurtBox.healthComponent)
			{
				Transform transform = hurtBox.healthComponent.transform;
				pingInfo.origin = transform.position;
				pingInfo.targetNetworkIdentity = hurtBox.healthComponent.GetComponent<NetworkIdentity>();
			}
			else if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Collide))
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				NetworkIdentity networkIdentity = gameObject.GetComponentInParent<NetworkIdentity>();
				if (!networkIdentity)
				{
					EntityLocator entityLocator = gameObject.transform.parent ? gameObject.transform.parent.GetComponentInChildren<EntityLocator>() : gameObject.GetComponent<EntityLocator>();
					if (entityLocator)
					{
						gameObject = entityLocator.entity;
						networkIdentity = gameObject.GetComponent<NetworkIdentity>();
					}
				}
				pingInfo.origin = raycastHit.point;
				pingInfo.normal = raycastHit.normal;
				pingInfo.targetNetworkIdentity = networkIdentity;
			}
			if (pingInfo.targetNetworkIdentity != null && pingInfo.targetNetworkIdentity == this.currentPing.targetNetworkIdentity)
			{
				pingInfo = PingerController.emptyPing;
				this.pingStock++;
			}
			this.SetCurrentPing(pingInfo);
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0000E795 File Offset: 0x0000C995
		static PingerController()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(PingerController), PingerController.kCmdCmdPing, new NetworkBehaviour.CmdDelegate(PingerController.InvokeCmdCmdPing));
			NetworkCRC.RegisterBehaviour("PingerController", 0);
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x0006B140 File Offset: 0x00069340
		// (set) Token: 0x060012EE RID: 4846 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
		public PingerController.PingInfo NetworkcurrentPing
		{
			get
			{
				return this.currentPing;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncCurrentPing(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PingerController.PingInfo>(value, ref this.currentPing, dirtyBit);
			}
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0000E80F File Offset: 0x0000CA0F
		protected static void InvokeCmdCmdPing(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdPing called on client.");
				return;
			}
			((PingerController)obj).CmdPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0006B154 File Offset: 0x00069354
		public void CallCmdPing(PingerController.PingInfo incomingPing)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdPing called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdPing(incomingPing);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)PingerController.kCmdCmdPing);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WritePingInfo_PingerController(networkWriter, incomingPing);
			base.SendCommandInternal(networkWriter, 0, "CmdPing");
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0006B1E0 File Offset: 0x000693E0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
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
				GeneratedNetworkCode._WritePingInfo_PingerController(writer, this.currentPing);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0006B24C File Offset: 0x0006944C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.currentPing = GeneratedNetworkCode._ReadPingInfo_PingerController(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncCurrentPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
			}
		}

		// Token: 0x0400169F RID: 5791
		private int pingStock = 3;

		// Token: 0x040016A0 RID: 5792
		private float pingRechargeStopwatch;

		// Token: 0x040016A1 RID: 5793
		private const int maximumPingStock = 3;

		// Token: 0x040016A2 RID: 5794
		private const float pingRechargeInterval = 1.5f;

		// Token: 0x040016A3 RID: 5795
		private static readonly PingerController.PingInfo emptyPing;

		// Token: 0x040016A4 RID: 5796
		private PingIndicator pingIndicator;

		// Token: 0x040016A5 RID: 5797
		[SyncVar(hook = "OnSyncCurrentPing")]
		public PingerController.PingInfo currentPing;

		// Token: 0x040016A6 RID: 5798
		private static int kCmdCmdPing = 1170265357;

		// Token: 0x02000388 RID: 904
		[Serializable]
		public struct PingInfo
		{
			// Token: 0x170001A4 RID: 420
			// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0000E838 File Offset: 0x0000CA38
			public GameObject targetGameObject
			{
				get
				{
					if (!this.targetNetworkIdentity)
					{
						return null;
					}
					return this.targetNetworkIdentity.gameObject;
				}
			}

			// Token: 0x040016A7 RID: 5799
			public bool active;

			// Token: 0x040016A8 RID: 5800
			public Vector3 origin;

			// Token: 0x040016A9 RID: 5801
			public Vector3 normal;

			// Token: 0x040016AA RID: 5802
			public NetworkIdentity targetNetworkIdentity;
		}
	}
}
