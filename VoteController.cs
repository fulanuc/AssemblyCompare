using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200041F RID: 1055
	public class VoteController : NetworkBehaviour
	{
		// Token: 0x0600179D RID: 6045 RVA: 0x00011B3B File Offset: 0x0000FD3B
		[Server]
		private void StartTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::StartTimer()' called on client");
				return;
			}
			if (this.timerIsActive)
			{
				return;
			}
			this.NetworktimerIsActive = true;
			this.Networktimer = this.timeoutDuration;
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x00011B6E File Offset: 0x0000FD6E
		[Server]
		private void StopTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::StopTimer()' called on client");
				return;
			}
			this.NetworktimerIsActive = false;
			this.Networktimer = this.timeoutDuration;
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x0007A868 File Offset: 0x00078A68
		[Server]
		private void InitializeVoters()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::InitializeVoters()' called on client");
				return;
			}
			this.StopTimer();
			this.votes.Clear();
			IEnumerable<NetworkUser> source = NetworkUser.readOnlyInstancesList;
			if (this.onlyAllowParticipatingPlayers)
			{
				source = from v in source
				where v.isParticipating
				select v;
			}
			foreach (GameObject networkUserObject in from v in source
			select v.gameObject)
			{
				this.votes.Add(new VoteController.UserVote
				{
					networkUserObject = networkUserObject,
					voteChoiceIndex = -1
				});
			}
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x0007A94C File Offset: 0x00078B4C
		[Server]
		private void AddUserToVoters(NetworkUser networkUser)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::AddUserToVoters(RoR2.NetworkUser)' called on client");
				return;
			}
			if (this.onlyAllowParticipatingPlayers && !networkUser.isParticipating)
			{
				return;
			}
			if (this.votes.Any((VoteController.UserVote v) => v.networkUserObject == networkUser.gameObject))
			{
				return;
			}
			this.votes.Add(new VoteController.UserVote
			{
				networkUserObject = networkUser.gameObject,
				voteChoiceIndex = -1
			});
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x0007A9DC File Offset: 0x00078BDC
		private void Awake()
		{
			if (NetworkServer.active)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.Immediate)
				{
					this.StartTimer();
				}
				if (this.addNewPlayers)
				{
					NetworkUser.OnPostNetworkUserStart += this.AddUserToVoters;
				}
				GameNetworkManager.onServerConnectGlobal += this.OnServerConnectGlobal;
				GameNetworkManager.onServerDisconnectGlobal += this.OnServerDisconnectGlobal;
			}
			this.votes.InitializeBehaviour(this, VoteController.kListvotes);
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x00011B98 File Offset: 0x0000FD98
		private void OnServerConnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x00011B98 File Offset: 0x0000FD98
		private void OnServerDisconnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x00011BA8 File Offset: 0x0000FDA8
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.AddUserToVoters;
			GameNetworkManager.onServerConnectGlobal -= this.OnServerConnectGlobal;
			GameNetworkManager.onServerDisconnectGlobal -= this.OnServerDisconnectGlobal;
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x00011BDD File Offset: 0x0000FDDD
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.InitializeVoters();
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x0007AA4C File Offset: 0x00078C4C
		[Server]
		public void ReceiveUserVote(NetworkUser networkUser, int voteChoiceIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::ReceiveUserVote(RoR2.NetworkUser,System.Int32)' called on client");
				return;
			}
			if (this.resetOnConnectionsChanged && GameNetworkManager.singleton.GetConnectingClientCount() > 0)
			{
				return;
			}
			if (voteChoiceIndex < 0 && !this.canRevokeVote)
			{
				return;
			}
			if (voteChoiceIndex >= this.choices.Length)
			{
				return;
			}
			GameObject gameObject = networkUser.gameObject;
			for (int i = 0; i < (int)this.votes.Count; i++)
			{
				if (gameObject == this.votes[i].networkUserObject)
				{
					if (!this.canChangeVote && this.votes[i].receivedVote)
					{
						return;
					}
					this.votes[i] = new VoteController.UserVote
					{
						networkUserObject = gameObject,
						voteChoiceIndex = voteChoiceIndex
					};
				}
			}
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x00011BEB File Offset: 0x0000FDEB
		private void Update()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0007AB18 File Offset: 0x00078D18
		[Server]
		private void ServerUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::ServerUpdate()' called on client");
				return;
			}
			if (this.timerIsActive)
			{
				this.Networktimer = this.timer - Time.deltaTime;
				if (this.timer < 0f)
				{
					this.Networktimer = 0f;
				}
			}
			int num = 0;
			for (int i = (int)(this.votes.Count - 1); i >= 0; i--)
			{
				if (!this.votes[i].networkUserObject)
				{
					this.votes.RemoveAt(i);
				}
				else if (this.votes[i].receivedVote)
				{
					num++;
				}
			}
			bool flag = num > 0;
			bool flag2 = num == (int)this.votes.Count;
			if (flag)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.OnAnyVoteReceived || this.timerStartCondition == VoteController.TimerStartCondition.WhileAnyVoteReceived)
				{
					this.StartTimer();
				}
			}
			else if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAnyVoteReceived)
			{
				this.StopTimer();
			}
			if (flag2)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAllVotesReceived)
				{
					this.StartTimer();
				}
				else if (RoR2Application.isInSinglePlayer)
				{
					this.Networktimer = 0f;
				}
				else
				{
					this.Networktimer = Mathf.Min(this.timer, this.minimumTimeBeforeProcessing);
				}
			}
			else if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAllVotesReceived)
			{
				this.StopTimer();
			}
			if ((flag2 && !this.mustTimeOut) || (this.timerIsActive && this.timer <= 0f))
			{
				this.FinishVote();
			}
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x0007AC88 File Offset: 0x00078E88
		[Server]
		private void FinishVote()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::FinishVote()' called on client");
				return;
			}
			IGrouping<int, VoteController.UserVote> grouping = (from v in this.votes
			where v.receivedVote
			group v by v.voteChoiceIndex into v
			orderby v.Count<VoteController.UserVote>() descending
			select v).FirstOrDefault<IGrouping<int, VoteController.UserVote>>();
			int num = (grouping == null) ? this.defaultChoiceIndex : grouping.Key;
			if (num >= this.choices.Length)
			{
				num = this.defaultChoiceIndex;
			}
			if (num < this.choices.Length)
			{
				this.choices[num].Invoke();
			}
			base.enabled = false;
			this.NetworktimerIsActive = false;
			this.Networktimer = 0f;
			if (this.destroyGameObjectOnComplete)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x00011BFA File Offset: 0x0000FDFA
		public int GetVoteCount()
		{
			return (int)this.votes.Count;
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00011C07 File Offset: 0x0000FE07
		public VoteController.UserVote GetVote(int i)
		{
			return this.votes[i];
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x0007AD8C File Offset: 0x00078F8C
		// (set) Token: 0x060017AF RID: 6063 RVA: 0x00011C4C File Offset: 0x0000FE4C
		public bool NetworktimerIsActive
		{
			get
			{
				return this.timerIsActive;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.timerIsActive, 2u);
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060017B0 RID: 6064 RVA: 0x0007ADA0 File Offset: 0x00078FA0
		// (set) Token: 0x060017B1 RID: 6065 RVA: 0x00011C60 File Offset: 0x0000FE60
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 4u);
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x00011C74 File Offset: 0x0000FE74
		protected static void InvokeSyncListvotes(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList votes called on server.");
				return;
			}
			((VoteController)obj).votes.HandleMsg(reader);
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x00011C9D File Offset: 0x0000FE9D
		static VoteController()
		{
			NetworkBehaviour.RegisterSyncListDelegate(typeof(VoteController), VoteController.kListvotes, new NetworkBehaviour.CmdDelegate(VoteController.InvokeSyncListvotes));
			NetworkCRC.RegisterBehaviour("VoteController", 0);
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0007ADB4 File Offset: 0x00078FB4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteStructSyncListUserVote_VoteController(writer, this.votes);
				writer.Write(this.timerIsActive);
				writer.Write(this.timer);
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
				GeneratedNetworkCode._WriteStructSyncListUserVote_VoteController(writer, this.votes);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timerIsActive);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timer);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x0007AEA0 File Offset: 0x000790A0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				GeneratedNetworkCode._ReadStructSyncListUserVote_VoteController(reader, this.votes);
				this.timerIsActive = reader.ReadBoolean();
				this.timer = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				GeneratedNetworkCode._ReadStructSyncListUserVote_VoteController(reader, this.votes);
			}
			if ((num & 2) != 0)
			{
				this.timerIsActive = reader.ReadBoolean();
			}
			if ((num & 4) != 0)
			{
				this.timer = reader.ReadSingle();
			}
		}

		// Token: 0x04001AA4 RID: 6820
		[Tooltip("Whether or not users must be participating in the run to be allowed to vote.")]
		public bool onlyAllowParticipatingPlayers = true;

		// Token: 0x04001AA5 RID: 6821
		[Tooltip("Whether or not to add new players to the voting pool when they connect.")]
		public bool addNewPlayers;

		// Token: 0x04001AA6 RID: 6822
		[Tooltip("Whether or not users are allowed to change their choice after submitting it.")]
		public bool canChangeVote;

		// Token: 0x04001AA7 RID: 6823
		[Tooltip("Whether or not users are allowed to revoke their vote entirely after submitting it.")]
		public bool canRevokeVote;

		// Token: 0x04001AA8 RID: 6824
		[Tooltip("If set, the vote cannot be completed early by all users submitting, and the timeout must occur.")]
		public bool mustTimeOut;

		// Token: 0x04001AA9 RID: 6825
		[Tooltip("Whether or not this vote must reset and be unvotable while someone is connecting or disconnecting.")]
		public bool resetOnConnectionsChanged;

		// Token: 0x04001AAA RID: 6826
		[Tooltip("How long it takes for the vote to forcibly complete once the timer begins.")]
		public float timeoutDuration = 15f;

		// Token: 0x04001AAB RID: 6827
		[Tooltip("How long it takes for action to be taken after the vote is complete.")]
		public float minimumTimeBeforeProcessing = 3f;

		// Token: 0x04001AAC RID: 6828
		[Tooltip("What causes the timer to start counting down.")]
		public VoteController.TimerStartCondition timerStartCondition;

		// Token: 0x04001AAD RID: 6829
		[Tooltip("An array of functions to be called based on the user vote.")]
		public UnityEvent[] choices;

		// Token: 0x04001AAE RID: 6830
		[Tooltip("The choice to use when nobody votes or everybody who can vote quits.")]
		public int defaultChoiceIndex;

		// Token: 0x04001AAF RID: 6831
		[Tooltip("Whether or not to destroy the attached GameObject when the vote completes.")]
		public bool destroyGameObjectOnComplete = true;

		// Token: 0x04001AB0 RID: 6832
		private VoteController.SyncListUserVote votes = new VoteController.SyncListUserVote();

		// Token: 0x04001AB1 RID: 6833
		[SyncVar]
		public bool timerIsActive;

		// Token: 0x04001AB2 RID: 6834
		[SyncVar]
		public float timer;

		// Token: 0x04001AB3 RID: 6835
		private static int kListvotes = 458257089;

		// Token: 0x02000420 RID: 1056
		public enum TimerStartCondition
		{
			// Token: 0x04001AB5 RID: 6837
			Immediate,
			// Token: 0x04001AB6 RID: 6838
			OnAnyVoteReceived,
			// Token: 0x04001AB7 RID: 6839
			WhileAnyVoteReceived,
			// Token: 0x04001AB8 RID: 6840
			WhileAllVotesReceived,
			// Token: 0x04001AB9 RID: 6841
			Never
		}

		// Token: 0x02000421 RID: 1057
		[Serializable]
		public struct UserVote
		{
			// Token: 0x1700022D RID: 557
			// (get) Token: 0x060017B6 RID: 6070 RVA: 0x00011CD8 File Offset: 0x0000FED8
			public bool receivedVote
			{
				get
				{
					return this.voteChoiceIndex >= 0;
				}
			}

			// Token: 0x04001ABA RID: 6842
			public GameObject networkUserObject;

			// Token: 0x04001ABB RID: 6843
			public int voteChoiceIndex;
		}

		// Token: 0x02000422 RID: 1058
		public class SyncListUserVote : SyncListStruct<VoteController.UserVote>
		{
			// Token: 0x060017B8 RID: 6072 RVA: 0x00011CEE File Offset: 0x0000FEEE
			public override void SerializeItem(NetworkWriter writer, VoteController.UserVote item)
			{
				writer.Write(item.networkUserObject);
				writer.WritePackedUInt32((uint)item.voteChoiceIndex);
			}

			// Token: 0x060017B9 RID: 6073 RVA: 0x0007AF2C File Offset: 0x0007912C
			public override VoteController.UserVote DeserializeItem(NetworkReader reader)
			{
				return new VoteController.UserVote
				{
					networkUserObject = reader.ReadGameObject(),
					voteChoiceIndex = (int)reader.ReadPackedUInt32()
				};
			}
		}
	}
}
