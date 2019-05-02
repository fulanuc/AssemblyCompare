using System;
using System.Linq;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038C RID: 908
	public class PingerController : NetworkBehaviour
	{
		// Token: 0x06001301 RID: 4865 RVA: 0x0006B098 File Offset: 0x00069298
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

		// Token: 0x06001302 RID: 4866 RVA: 0x0000E8AE File Offset: 0x0000CAAE
		private void OnDestroy()
		{
			if (this.pingIndicator)
			{
				UnityEngine.Object.Destroy(this.pingIndicator.gameObject);
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0000E8CD File Offset: 0x0000CACD
		private void OnSyncCurrentPing(PingerController.PingInfo newPingInfo)
		{
			if (base.hasAuthority)
			{
				return;
			}
			this.SetCurrentPing(newPingInfo);
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0000E8DF File Offset: 0x0000CADF
		private void SetCurrentPing(PingerController.PingInfo newPingInfo)
		{
			this.NetworkcurrentPing = newPingInfo;
			this.RebuildPing(this.currentPing);
			if (base.hasAuthority)
			{
				this.CallCmdPing(this.currentPing);
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0000E908 File Offset: 0x0000CB08
		[Command]
		private void CmdPing(PingerController.PingInfo incomingPing)
		{
			this.NetworkcurrentPing = incomingPing;
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0006B14C File Offset: 0x0006934C
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

		// Token: 0x06001307 RID: 4871 RVA: 0x0006B1A0 File Offset: 0x000693A0
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

		// Token: 0x06001309 RID: 4873 RVA: 0x0000E920 File Offset: 0x0000CB20
		static PingerController()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(PingerController), PingerController.kCmdCmdPing, new NetworkBehaviour.CmdDelegate(PingerController.InvokeCmdCmdPing));
			NetworkCRC.RegisterBehaviour("PingerController", 0);
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x0006B39C File Offset: 0x0006959C
		// (set) Token: 0x0600130C RID: 4876 RVA: 0x0000E95B File Offset: 0x0000CB5B
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

		// Token: 0x0600130D RID: 4877 RVA: 0x0000E99A File Offset: 0x0000CB9A
		protected static void InvokeCmdCmdPing(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdPing called on client.");
				return;
			}
			((PingerController)obj).CmdPing(GeneratedNetworkCode._ReadPingInfo_PingerController(reader));
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0006B3B0 File Offset: 0x000695B0
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

		// Token: 0x0600130F RID: 4879 RVA: 0x0006B43C File Offset: 0x0006963C
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

		// Token: 0x06001310 RID: 4880 RVA: 0x0006B4A8 File Offset: 0x000696A8
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

		// Token: 0x040016BB RID: 5819
		private int pingStock = 3;

		// Token: 0x040016BC RID: 5820
		private float pingRechargeStopwatch;

		// Token: 0x040016BD RID: 5821
		private const int maximumPingStock = 3;

		// Token: 0x040016BE RID: 5822
		private const float pingRechargeInterval = 1.5f;

		// Token: 0x040016BF RID: 5823
		private static readonly PingerController.PingInfo emptyPing;

		// Token: 0x040016C0 RID: 5824
		private PingIndicator pingIndicator;

		// Token: 0x040016C1 RID: 5825
		[SyncVar(hook = "OnSyncCurrentPing")]
		public PingerController.PingInfo currentPing;

		// Token: 0x040016C2 RID: 5826
		private static int kCmdCmdPing = 1170265357;

		// Token: 0x0200038D RID: 909
		[Serializable]
		public struct PingInfo
		{
			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x06001311 RID: 4881 RVA: 0x0000E9C3 File Offset: 0x0000CBC3
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

			// Token: 0x040016C3 RID: 5827
			public bool active;

			// Token: 0x040016C4 RID: 5828
			public Vector3 origin;

			// Token: 0x040016C5 RID: 5829
			public Vector3 normal;

			// Token: 0x040016C6 RID: 5830
			public NetworkIdentity targetNetworkIdentity;
		}
	}
}
