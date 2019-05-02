using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000399 RID: 921
	public class PreGameRuleVoteController : NetworkBehaviour
	{
		// Token: 0x06001375 RID: 4981 RVA: 0x0006C8DC File Offset: 0x0006AADC
		public static PreGameRuleVoteController FindForUser(NetworkUser networkUser)
		{
			GameObject gameObject = networkUser.gameObject;
			foreach (PreGameRuleVoteController preGameRuleVoteController in PreGameRuleVoteController.instancesList)
			{
				if (preGameRuleVoteController.networkUserNetworkIdentity && preGameRuleVoteController.networkUserNetworkIdentity.gameObject == gameObject)
				{
					return preGameRuleVoteController;
				}
			}
			return null;
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001376 RID: 4982 RVA: 0x0006C958 File Offset: 0x0006AB58
		// (remove) Token: 0x06001377 RID: 4983 RVA: 0x0006C98C File Offset: 0x0006AB8C
		public static event Action onVotesUpdated;

		// Token: 0x06001378 RID: 4984 RVA: 0x0000EE9C File Offset: 0x0000D09C
		private void Start()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				this.SetVotesFromRuleBookForSinglePlayer();
			}
			if (NetworkServer.active)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			}
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x0006C9C0 File Offset: 0x0006ABC0
		private void Update()
		{
			if (NetworkServer.active && !this.networkUserNetworkIdentity)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.clientShouldTransmit)
			{
				this.clientShouldTransmit = false;
				this.ClientTransmitVotesToServer();
			}
			if (PreGameRuleVoteController.shouldUpdateGameVotes)
			{
				PreGameRuleVoteController.shouldUpdateGameVotes = false;
				PreGameRuleVoteController.UpdateGameVotes();
			}
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x0006CA14 File Offset: 0x0006AC14
		[Client]
		private void ClientTransmitVotesToServer()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.PreGameRuleVoteController::ClientTransmitVotesToServer()' called on server");
				return;
			}
			Debug.Log("PreGameRuleVoteController.ClientTransmitVotesToServer()");
			if (!this.networkUserNetworkIdentity)
			{
				Debug.Log("Can't transmit votes: No network user object.");
				return;
			}
			NetworkUser component = this.networkUserNetworkIdentity.GetComponent<NetworkUser>();
			if (!component)
			{
				Debug.Log("Can't transmit votes: No network user component.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(70);
			networkWriter.Write(base.gameObject);
			this.WriteVotes(networkWriter);
			networkWriter.FinishMessage();
			component.connectionToServer.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x0006CAB8 File Offset: 0x0006ACB8
		[NetworkMessageHandler(msgType = 70, client = false, server = true)]
		public static void ServerHandleClientVoteUpdate(NetworkMessage netMsg)
		{
			string format = "Received vote from {0}";
			object[] array = new object[1];
			int num = 0;
			NetworkUser networkUser = NetworkUser.readOnlyInstancesList.FirstOrDefault((NetworkUser v) => v.connectionToClient == netMsg.conn);
			array[num] = ((networkUser != null) ? networkUser.userName : null);
			Debug.LogFormat(format, array);
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!gameObject)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: preGameRuleVoteControllerObject=null");
				return;
			}
			PreGameRuleVoteController component = gameObject.GetComponent<PreGameRuleVoteController>();
			if (!component)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: preGameRuleVoteController=null");
				return;
			}
			NetworkIdentity networkIdentity = component.networkUserNetworkIdentity;
			if (!networkIdentity)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: No NetworkIdentity");
				return;
			}
			NetworkUser component2 = networkIdentity.GetComponent<NetworkUser>();
			if (!component2)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: No NetworkUser");
				return;
			}
			if (component2.connectionToClient != netMsg.conn)
			{
				Debug.LogFormat("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: {0}!={1}", new object[]
				{
					component.connectionToClient,
					netMsg.conn
				});
				return;
			}
			Debug.LogFormat("Accepting vote from {0}", new object[]
			{
				component2.userName
			});
			component.ReadVotes(netMsg.reader);
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x0006CBE4 File Offset: 0x0006ADE4
		public void SetVote(int ruleIndex, int choiceValue)
		{
			PreGameRuleVoteController.Vote vote = this.votes[ruleIndex];
			if (vote.choiceValue == choiceValue)
			{
				return;
			}
			this.votes[ruleIndex].choiceValue = choiceValue;
			if (!NetworkServer.active && this.networkUserNetworkIdentity && this.networkUserNetworkIdentity.isLocalPlayer)
			{
				this.clientShouldTransmit = true;
			}
			else
			{
				base.SetDirtyBit(2u);
			}
			PreGameRuleVoteController.shouldUpdateGameVotes = true;
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x0006CC54 File Offset: 0x0006AE54
		private static void UpdateGameVotes()
		{
			int i = 0;
			int choiceCount = RuleCatalog.choiceCount;
			while (i < choiceCount)
			{
				PreGameRuleVoteController.votesForEachChoice[i] = 0;
				i++;
			}
			int j = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (j < ruleCount)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(j);
				int count = ruleDef.choices.Count;
				foreach (PreGameRuleVoteController preGameRuleVoteController in PreGameRuleVoteController.instancesList)
				{
					PreGameRuleVoteController.Vote vote = preGameRuleVoteController.votes[j];
					if (vote.hasVoted && vote.choiceValue < count)
					{
						RuleChoiceDef ruleChoiceDef = ruleDef.choices[vote.choiceValue];
						PreGameRuleVoteController.votesForEachChoice[ruleChoiceDef.globalIndex]++;
					}
				}
				j++;
			}
			if (NetworkServer.active)
			{
				int k = 0;
				int ruleCount2 = RuleCatalog.ruleCount;
				while (k < ruleCount2)
				{
					RuleDef ruleDef2 = RuleCatalog.GetRuleDef(k);
					int count2 = ruleDef2.choices.Count;
					PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef2);
					int ruleChoiceIndex = -1;
					int num = 0;
					bool flag = false;
					for (int l = 0; l < count2; l++)
					{
						RuleChoiceDef ruleChoiceDef2 = ruleDef2.choices[l];
						int num2 = PreGameRuleVoteController.votesForEachChoice[ruleChoiceDef2.globalIndex];
						if (num2 == num)
						{
							flag = true;
						}
						else if (num2 > num)
						{
							ruleChoiceIndex = ruleChoiceDef2.globalIndex;
							num = num2;
							flag = false;
						}
					}
					if (num == 0)
					{
						ruleChoiceIndex = ruleDef2.choices[ruleDef2.defaultChoiceIndex].globalIndex;
					}
					if (!flag || num == 0)
					{
						PreGameController.instance.ApplyChoice(ruleChoiceIndex);
					}
					k++;
				}
			}
			Action action = PreGameRuleVoteController.onVotesUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0000EEB7 File Offset: 0x0000D0B7
		private void Awake()
		{
			PreGameRuleVoteController.instancesList.Add(this);
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x0006CE20 File Offset: 0x0006B020
		private void OnDestroy()
		{
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				this.SetVote(i, -1);
				i++;
			}
			PreGameRuleVoteController.instancesList.Remove(this);
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x0006CE54 File Offset: 0x0006B054
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 3u;
			}
			writer.Write((byte)num);
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			if (flag)
			{
				writer.Write(this.networkUserNetworkIdentity);
			}
			if (flag2)
			{
				this.WriteVotes(writer);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x0006CEA4 File Offset: 0x0006B0A4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			if (flag)
			{
				this.networkUserNetworkIdentity = reader.ReadNetworkIdentity();
			}
			if (flag2)
			{
				this.ReadVotes(reader);
			}
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x0000EEC4 File Offset: 0x0000D0C4
		private RuleChoiceDef GetDefaultChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef)];
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x0006CEDC File Offset: 0x0006B0DC
		private void SetVotesFromRuleBookForSinglePlayer()
		{
			for (int i = 0; i < this.votes.Length; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				this.votes[i].choiceValue = this.GetDefaultChoice(ruleDef).localIndex;
			}
			base.SetDirtyBit(2u);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0006CF28 File Offset: 0x0006B128
		private void WriteVotes(NetworkWriter writer)
		{
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				this.ruleMaskBuffer[i] = this.votes[i].hasVoted;
				i++;
			}
			writer.Write(this.ruleMaskBuffer);
			int j = 0;
			int ruleCount2 = RuleCatalog.ruleCount;
			while (j < ruleCount2)
			{
				if (this.votes[j].hasVoted)
				{
					PreGameRuleVoteController.Vote.Serialize(writer, this.votes[j]);
				}
				j++;
			}
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x0006CFA8 File Offset: 0x0006B1A8
		private void ReadVotes(NetworkReader reader)
		{
			reader.ReadRuleMask(this.ruleMaskBuffer);
			bool flag = !this.networkUserNetworkIdentity || !this.networkUserNetworkIdentity.isLocalPlayer;
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				PreGameRuleVoteController.Vote vote;
				if (this.ruleMaskBuffer[i])
				{
					vote = PreGameRuleVoteController.Vote.Deserialize(reader);
				}
				else
				{
					vote = default(PreGameRuleVoteController.Vote);
				}
				if (flag)
				{
					this.votes[i] = vote;
				}
				i++;
			}
			PreGameRuleVoteController.shouldUpdateGameVotes = (PreGameRuleVoteController.shouldUpdateGameVotes || flag);
			if (NetworkServer.active)
			{
				base.SetDirtyBit(2u);
			}
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x0000EEE1 File Offset: 0x0000D0E1
		public bool IsChoiceVoted(RuleChoiceDef ruleChoiceDef)
		{
			return this.votes[ruleChoiceDef.ruleDef.globalIndex].choiceValue == ruleChoiceDef.localIndex;
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x0000EF06 File Offset: 0x0000D106
		static PreGameRuleVoteController()
		{
			PreGameController.onServerRecalculatedModifierAvailability += delegate(PreGameController controller)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			};
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0400170D RID: 5901
		private static readonly List<PreGameRuleVoteController> instancesList = new List<PreGameRuleVoteController>();

		// Token: 0x0400170F RID: 5903
		private const byte networkUserIdentityDirtyBit = 1;

		// Token: 0x04001710 RID: 5904
		private const byte votesDirtyBit = 2;

		// Token: 0x04001711 RID: 5905
		private const byte allDirtyBits = 3;

		// Token: 0x04001712 RID: 5906
		private PreGameRuleVoteController.Vote[] votes = new PreGameRuleVoteController.Vote[RuleCatalog.ruleCount];

		// Token: 0x04001713 RID: 5907
		public static int[] votesForEachChoice = new int[RuleCatalog.choiceCount];

		// Token: 0x04001714 RID: 5908
		private bool clientShouldTransmit;

		// Token: 0x04001715 RID: 5909
		public NetworkIdentity networkUserNetworkIdentity;

		// Token: 0x04001716 RID: 5910
		private static bool shouldUpdateGameVotes;

		// Token: 0x04001717 RID: 5911
		private readonly RuleMask ruleMaskBuffer = new RuleMask();

		// Token: 0x0200039A RID: 922
		[Serializable]
		private struct Vote
		{
			// Token: 0x170001B9 RID: 441
			// (get) Token: 0x0600138A RID: 5002 RVA: 0x0000EF59 File Offset: 0x0000D159
			public bool hasVoted
			{
				get
				{
					return this.internalValue > 0;
				}
			}

			// Token: 0x170001BA RID: 442
			// (get) Token: 0x0600138B RID: 5003 RVA: 0x0000EF64 File Offset: 0x0000D164
			// (set) Token: 0x0600138C RID: 5004 RVA: 0x0000EF6E File Offset: 0x0000D16E
			public int choiceValue
			{
				get
				{
					return (int)(this.internalValue - 1);
				}
				set
				{
					this.internalValue = (byte)(value + 1);
				}
			}

			// Token: 0x0600138D RID: 5005 RVA: 0x0000EF7A File Offset: 0x0000D17A
			public static void Serialize(NetworkWriter writer, PreGameRuleVoteController.Vote vote)
			{
				writer.Write(vote.internalValue);
			}

			// Token: 0x0600138E RID: 5006 RVA: 0x0006D03C File Offset: 0x0006B23C
			public static PreGameRuleVoteController.Vote Deserialize(NetworkReader reader)
			{
				return new PreGameRuleVoteController.Vote
				{
					internalValue = reader.ReadByte()
				};
			}

			// Token: 0x04001718 RID: 5912
			[SerializeField]
			private byte internalValue;
		}
	}
}
