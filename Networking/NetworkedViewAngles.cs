using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000582 RID: 1410
	public class NetworkedViewAngles : NetworkBehaviour
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001FCC RID: 8140 RVA: 0x0001720A File Offset: 0x0001540A
		// (set) Token: 0x06001FCD RID: 8141 RVA: 0x00017212 File Offset: 0x00015412
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001FCE RID: 8142 RVA: 0x0001721B File Offset: 0x0001541B
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x0009B538 File Offset: 0x00099738
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

		// Token: 0x06001FD0 RID: 8144 RVA: 0x00017229 File Offset: 0x00015429
		public override float GetNetworkSendInterval()
		{
			return this.sendRate;
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x00017231 File Offset: 0x00015431
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.viewAngles.intVal;
		}

		// Token: 0x06001FD2 RID: 8146 RVA: 0x0009B59C File Offset: 0x0009979C
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

		// Token: 0x06001FD3 RID: 8147 RVA: 0x0001723D File Offset: 0x0001543D
		[Command(channel = 5)]
		public void CmdUpdateViewAngles(float pitch, float yaw)
		{
			this.networkDesiredViewAngles = new PitchYawPair(pitch, yaw);
		}

		// Token: 0x06001FD4 RID: 8148 RVA: 0x0001724C File Offset: 0x0001544C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.networkDesiredViewAngles);
			return !initialState;
		}

		// Token: 0x06001FD5 RID: 8149 RVA: 0x0009B62C File Offset: 0x0009982C
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

		// Token: 0x06001FD7 RID: 8151 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x00017287 File Offset: 0x00015487
		protected static void InvokeCmdCmdUpdateViewAngles(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateViewAngles called on client.");
				return;
			}
			((NetworkedViewAngles)obj).CmdUpdateViewAngles(reader.ReadSingle(), reader.ReadSingle());
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x0009B668 File Offset: 0x00099868
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

		// Token: 0x06001FDA RID: 8154 RVA: 0x000172B8 File Offset: 0x000154B8
		static NetworkedViewAngles()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkedViewAngles), NetworkedViewAngles.kCmdCmdUpdateViewAngles, new NetworkBehaviour.CmdDelegate(NetworkedViewAngles.InvokeCmdCmdUpdateViewAngles));
			NetworkCRC.RegisterBehaviour("NetworkedViewAngles", 0);
		}

		// Token: 0x04002204 RID: 8708
		public PitchYawPair viewAngles;

		// Token: 0x04002205 RID: 8709
		private PitchYawPair networkDesiredViewAngles;

		// Token: 0x04002206 RID: 8710
		private PitchYawPair velocity;

		// Token: 0x04002207 RID: 8711
		private NetworkIdentity networkIdentity;

		// Token: 0x04002209 RID: 8713
		public float sendRate = 0.05f;

		// Token: 0x0400220A RID: 8714
		public float bufferMultiplier = 3f;

		// Token: 0x0400220B RID: 8715
		public float maxSmoothVelocity = 1440f;

		// Token: 0x0400220C RID: 8716
		private float sendTimer;

		// Token: 0x0400220D RID: 8717
		private static int kCmdCmdUpdateViewAngles = -1684781536;
	}
}
