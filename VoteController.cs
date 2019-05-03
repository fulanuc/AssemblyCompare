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
	// Token: 0x02000419 RID: 1049
	public class VoteController : NetworkBehaviour
	{
		// Token: 0x0600175A RID: 5978 RVA: 0x0001170F File Offset: 0x0000F90F
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

		// Token: 0x0600175B RID: 5979 RVA: 0x00011742 File Offset: 0x0000F942
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

		// Token: 0x0600175C RID: 5980 RVA: 0x0007A2A8 File Offset: 0x000784A8
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

		// Token: 0x0600175D RID: 5981 RVA: 0x0007A38C File Offset: 0x0007858C
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

		// Token: 0x0600175E RID: 5982 RVA: 0x0007A41C File Offset: 0x0007861C
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

		// Token: 0x0600175F RID: 5983 RVA: 0x0001176C File Offset: 0x0000F96C
		private void OnServerConnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0001176C File Offset: 0x0000F96C
		private void OnServerDisconnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0001177C File Offset: 0x0000F97C
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.AddUserToVoters;
			GameNetworkManager.onServerConnectGlobal -= this.OnServerConnectGlobal;
			GameNetworkManager.onServerDisconnectGlobal -= this.OnServerDisconnectGlobal;
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x000117B1 File Offset: 0x0000F9B1
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.InitializeVoters();
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0007A48C File Offset: 0x0007868C
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

		// Token: 0x06001764 RID: 5988 RVA: 0x000117BF File Offset: 0x0000F9BF
		private void Update()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0007A558 File Offset: 0x00078758
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

		// Token: 0x06001766 RID: 5990 RVA: 0x0007A6C8 File Offset: 0x000788C8
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

		// Token: 0x06001767 RID: 5991 RVA: 0x000117CE File Offset: 0x0000F9CE
		public int GetVoteCount()
		{
			return (int)this.votes.Count;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x000117DB File Offset: 0x0000F9DB
		public VoteController.UserVote GetVote(int i)
		{
			return this.votes[i];
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x0007A7CC File Offset: 0x000789CC
		// (set) Token: 0x0600176C RID: 5996 RVA: 0x00011820 File Offset: 0x0000FA20
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

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x0007A7E0 File Offset: 0x000789E0
		// (set) Token: 0x0600176E RID: 5998 RVA: 0x00011834 File Offset: 0x0000FA34
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

		// Token: 0x0600176F RID: 5999 RVA: 0x00011848 File Offset: 0x0000FA48
		protected static void InvokeSyncListvotes(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList votes called on server.");
				return;
			}
			((VoteController)obj).votes.HandleMsg(reader);
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x00011871 File Offset: 0x0000FA71
		static VoteController()
		{
			NetworkBehaviour.RegisterSyncListDelegate(typeof(VoteController), VoteController.kListvotes, new NetworkBehaviour.CmdDelegate(VoteController.InvokeSyncListvotes));
			NetworkCRC.RegisterBehaviour("VoteController", 0);
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0007A7F4 File Offset: 0x000789F4
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

		// Token: 0x06001772 RID: 6002 RVA: 0x0007A8E0 File Offset: 0x00078AE0
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

		// Token: 0x04001A7B RID: 6779
		[Tooltip("Whether or not users must be participating in the run to be allowed to vote.")]
		public bool onlyAllowParticipatingPlayers = true;

		// Token: 0x04001A7C RID: 6780
		[Tooltip("Whether or not to add new players to the voting pool when they connect.")]
		public bool addNewPlayers;

		// Token: 0x04001A7D RID: 6781
		[Tooltip("Whether or not users are allowed to change their choice after submitting it.")]
		public bool canChangeVote;

		// Token: 0x04001A7E RID: 6782
		[Tooltip("Whether or not users are allowed to revoke their vote entirely after submitting it.")]
		public bool canRevokeVote;

		// Token: 0x04001A7F RID: 6783
		[Tooltip("If set, the vote cannot be completed early by all users submitting, and the timeout must occur.")]
		public bool mustTimeOut;

		// Token: 0x04001A80 RID: 6784
		[Tooltip("Whether or not this vote must reset and be unvotable while someone is connecting or disconnecting.")]
		public bool resetOnConnectionsChanged;

		// Token: 0x04001A81 RID: 6785
		[Tooltip("How long it takes for the vote to forcibly complete once the timer begins.")]
		public float timeoutDuration = 15f;

		// Token: 0x04001A82 RID: 6786
		[Tooltip("How long it takes for action to be taken after the vote is complete.")]
		public float minimumTimeBeforeProcessing = 3f;

		// Token: 0x04001A83 RID: 6787
		[Tooltip("What causes the timer to start counting down.")]
		public VoteController.TimerStartCondition timerStartCondition;

		// Token: 0x04001A84 RID: 6788
		[Tooltip("An array of functions to be called based on the user vote.")]
		public UnityEvent[] choices;

		// Token: 0x04001A85 RID: 6789
		[Tooltip("The choice to use when nobody votes or everybody who can vote quits.")]
		public int defaultChoiceIndex;

		// Token: 0x04001A86 RID: 6790
		[Tooltip("Whether or not to destroy the attached GameObject when the vote completes.")]
		public bool destroyGameObjectOnComplete = true;

		// Token: 0x04001A87 RID: 6791
		private VoteController.SyncListUserVote votes = new VoteController.SyncListUserVote();

		// Token: 0x04001A88 RID: 6792
		[SyncVar]
		public bool timerIsActive;

		// Token: 0x04001A89 RID: 6793
		[SyncVar]
		public float timer;

		// Token: 0x04001A8A RID: 6794
		private static int kListvotes = 458257089;

		// Token: 0x0200041A RID: 1050
		public enum TimerStartCondition
		{
			// Token: 0x04001A8C RID: 6796
			Immediate,
			// Token: 0x04001A8D RID: 6797
			OnAnyVoteReceived,
			// Token: 0x04001A8E RID: 6798
			WhileAnyVoteReceived,
			// Token: 0x04001A8F RID: 6799
			WhileAllVotesReceived,
			// Token: 0x04001A90 RID: 6800
			Never
		}

		// Token: 0x0200041B RID: 1051
		[Serializable]
		public struct UserVote
		{
			// Token: 0x17000224 RID: 548
			// (get) Token: 0x06001773 RID: 6003 RVA: 0x000118AC File Offset: 0x0000FAAC
			public bool receivedVote
			{
				get
				{
					return this.voteChoiceIndex >= 0;
				}
			}

			// Token: 0x04001A91 RID: 6801
			public GameObject networkUserObject;

			// Token: 0x04001A92 RID: 6802
			public int voteChoiceIndex;
		}

		// Token: 0x0200041C RID: 1052
		public class SyncListUserVote : SyncListStruct<VoteController.UserVote>
		{
			// Token: 0x06001775 RID: 6005 RVA: 0x000118C2 File Offset: 0x0000FAC2
			public override void SerializeItem(NetworkWriter writer, VoteController.UserVote item)
			{
				writer.Write(item.networkUserObject);
				writer.WritePackedUInt32((uint)item.voteChoiceIndex);
			}

			// Token: 0x06001776 RID: 6006 RVA: 0x0007A96C File Offset: 0x00078B6C
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
