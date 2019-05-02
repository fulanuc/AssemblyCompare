using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000595 RID: 1429
	public class NetworkedViewAngles : NetworkBehaviour
	{
		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x0600205D RID: 8285 RVA: 0x0001791A File Offset: 0x00015B1A
		// (set) Token: 0x0600205E RID: 8286 RVA: 0x00017922 File Offset: 0x00015B22
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x0600205F RID: 8287 RVA: 0x0001792B File Offset: 0x00015B2B
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x06002060 RID: 8288 RVA: 0x0009CA40 File Offset: 0x0009AC40
		private void Update()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
			if (this.hasEffectiveAuthority)
			{
				this.networkDesiredViewAngles = this.viewAngles;
				return;
			}
			this.viewAngles = PitchYawPair.SmoothDamp(this.viewAngles, this.networkDesiredViewAngles, ref this.velocity, this.GetNetworkSendInterval() * this.bufferMultiplier, this.maxSmoothVelocity);
		}

		// Token: 0x06002061 RID: 8289 RVA: 0x00017939 File Offset: 0x00015B39
		public override float GetNetworkSendInterval()
		{
			return this.sendRate;
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x00017941 File Offset: 0x00015B41
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.viewAngles.intVal;
		}

		// Token: 0x06002063 RID: 8291 RVA: 0x0009CAA4 File Offset: 0x0009ACA4
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1u);
			}
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
			if (this.hasEffectiveAuthority)
			{
				this.networkDesiredViewAngles = this.viewAngles;
				if (!NetworkServer.active)
				{
					this.sendTimer -= Time.deltaTime;
					if (this.sendTimer <= 0f)
					{
						this.CallCmdUpdateViewAngles(this.viewAngles.pitch, this.viewAngles.yaw);
						this.sendTimer = this.GetNetworkSendInterval();
					}
				}
			}
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x0001794D File Offset: 0x00015B4D
		[Command(channel = 5)]
		public void CmdUpdateViewAngles(float pitch, float yaw)
		{
			this.networkDesiredViewAngles = new PitchYawPair(pitch, yaw);
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x0001795C File Offset: 0x00015B5C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.networkDesiredViewAngles);
			return !initialState;
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x0009CB34 File Offset: 0x0009AD34
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			PitchYawPair pitchYawPair = reader.ReadPitchYawPair();
			if (this.hasEffectiveAuthority)
			{
				return;
			}
			this.networkDesiredViewAngles = pitchYawPair;
			if (initialState)
			{
				this.viewAngles = pitchYawPair;
				this.velocity = PitchYawPair.zero;
			}
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00017997 File Offset: 0x00015B97
		protected static void InvokeCmdCmdUpdateViewAngles(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateViewAngles called on client.");
				return;
			}
			((NetworkedViewAngles)obj).CmdUpdateViewAngles(reader.ReadSingle(), reader.ReadSingle());
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x0009CB70 File Offset: 0x0009AD70
		public void CallCmdUpdateViewAngles(float pitch, float yaw)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdUpdateViewAngles called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdUpdateViewAngles(pitch, yaw);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkedViewAngles.kCmdCmdUpdateViewAngles);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(pitch);
			networkWriter.Write(yaw);
			base.SendCommandInternal(networkWriter, 5, "CmdUpdateViewAngles");
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x000179C8 File Offset: 0x00015BC8
		static NetworkedViewAngles()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkedViewAngles), NetworkedViewAngles.kCmdCmdUpdateViewAngles, new NetworkBehaviour.CmdDelegate(NetworkedViewAngles.InvokeCmdCmdUpdateViewAngles));
			NetworkCRC.RegisterBehaviour("NetworkedViewAngles", 0);
		}

		// Token: 0x0400225B RID: 8795
		public PitchYawPair viewAngles;

		// Token: 0x0400225C RID: 8796
		private PitchYawPair networkDesiredViewAngles;

		// Token: 0x0400225D RID: 8797
		private PitchYawPair velocity;

		// Token: 0x0400225E RID: 8798
		private NetworkIdentity networkIdentity;

		// Token: 0x04002260 RID: 8800
		public float sendRate = 0.05f;

		// Token: 0x04002261 RID: 8801
		public float bufferMultiplier = 3f;

		// Token: 0x04002262 RID: 8802
		public float maxSmoothVelocity = 1440f;

		// Token: 0x04002263 RID: 8803
		private float sendTimer;

		// Token: 0x04002264 RID: 8804
		private static int kCmdCmdUpdateViewAngles = -1684781536;
	}
}
