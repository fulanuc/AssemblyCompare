using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004FF RID: 1279
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PlayerCharacterMasterController))]
	public class PlayerStatsComponent : NetworkBehaviour
	{
		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001D0C RID: 7436 RVA: 0x00015566 File Offset: 0x00013766
		// (set) Token: 0x06001D0D RID: 7437 RVA: 0x0001556E File Offset: 0x0001376E
		public CharacterMaster characterMaster { get; private set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001D0E RID: 7438 RVA: 0x00015577 File Offset: 0x00013777
		// (set) Token: 0x06001D0F RID: 7439 RVA: 0x0001557F File Offset: 0x0001377F
		public PlayerCharacterMasterController playerCharacterMasterController { get; private set; }

		// Token: 0x06001D10 RID: 7440 RVA: 0x0008E140 File Offset: 0x0008C340
		private void Awake()
		{
			this.playerCharacterMasterController = base.GetComponent<PlayerCharacterMasterController>();
			this.characterMaster = base.GetComponent<CharacterMaster>();
			PlayerStatsComponent.instancesList.Add(this);
			this.currentStats = StatSheet.New();
			if (NetworkClient.active)
			{
				this.recordedStats = StatSheet.New();
				this.clientDeltaStatsBuffer = StatSheet.New();
			}
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x00015588 File Offset: 0x00013788
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				this.SendUpdateToClient();
			}
			PlayerStatsComponent.instancesList.Remove(this);
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x000155A3 File Offset: 0x000137A3
		public static StatSheet FindBodyStatSheet(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatSheet(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x000155BA File Offset: 0x000137BA
		public static StatSheet FindBodyStatSheet(CharacterBody characterBody)
		{
			if (characterBody == null)
			{
				return null;
			}
			CharacterMaster master = characterBody.master;
			if (master == null)
			{
				return null;
			}
			PlayerStatsComponent component = master.GetComponent<PlayerStatsComponent>();
			if (component == null)
			{
				return null;
			}
			return component.currentStats;
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x000155DD File Offset: 0x000137DD
		public static PlayerStatsComponent FindBodyStatsComponent(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatsComponent(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x000155F4 File Offset: 0x000137F4
		public static PlayerStatsComponent FindBodyStatsComponent(CharacterBody characterBody)
		{
			if (characterBody == null)
			{
				return null;
			}
			CharacterMaster master = characterBody.master;
			if (master == null)
			{
				return null;
			}
			return master.GetComponent<PlayerStatsComponent>();
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x0001560C File Offset: 0x0001380C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				if (NetworkServer.active)
				{
					PlayerStatsComponent playerStatsComponent = PlayerStatsComponent.FindBodyStatsComponent(damageReport.victim.gameObject);
					if (playerStatsComponent)
					{
						playerStatsComponent.serverTransmitTimer = 0f;
					}
				}
			};
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x00015632 File Offset: 0x00013832
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00015641 File Offset: 0x00013841
		[Server]
		public void ForceNextTransmit()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ForceNextTransmit()' called on client");
				return;
			}
			this.serverTransmitTimer = 0f;
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x0008E198 File Offset: 0x0008C398
		[Server]
		private void ServerFixedUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ServerFixedUpdate()' called on client");
				return;
			}
			float num = 0f;
			float runTime = 0f;
			if (Run.instance && !Run.instance.isRunStopwatchPaused)
			{
				num = Time.fixedDeltaTime;
				runTime = Run.instance.GetRunStopwatch();
			}
			StatManager.CharacterUpdateEvent e = default(StatManager.CharacterUpdateEvent);
			e.statsComponent = this;
			e.runTime = runTime;
			GameObject bodyObject = this.characterMaster.GetBodyObject();
			if (bodyObject != this.cachedBodyObject)
			{
				this.cachedBodyObject = bodyObject;
				this.cachedBodyObject = bodyObject;
				this.cachedBodyTransform = ((bodyObject != null) ? bodyObject.transform : null);
				if (this.cachedBodyTransform)
				{
					this.previousBodyPosition = this.cachedBodyTransform.position;
				}
				this.cachedCharacterBody = ((bodyObject != null) ? bodyObject.GetComponent<CharacterBody>() : null);
				this.cachedBodyCharacterMotor = ((bodyObject != null) ? bodyObject.GetComponent<CharacterMotor>() : null);
			}
			if (this.cachedBodyTransform)
			{
				Vector3 position = this.cachedBodyTransform.position;
				e.additionalDistanceTraveled = Vector3.Distance(position, this.previousBodyPosition);
				this.previousBodyPosition = position;
			}
			if (this.characterMaster.alive)
			{
				e.additionalTimeAlive += num;
			}
			if (this.cachedCharacterBody)
			{
				e.level = (int)this.cachedCharacterBody.level;
			}
			StatManager.PushCharacterUpdateEvent(e);
			this.serverTransmitTimer -= Time.fixedDeltaTime;
			if (this.serverTransmitTimer <= 0f)
			{
				this.serverTransmitTimer = this.serverTransmitInterval;
				this.SendUpdateToClient();
			}
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x0008E32C File Offset: 0x0008C52C
		[Server]
		private void SendUpdateToClient()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::SendUpdateToClient()' called on client");
				return;
			}
			NetworkUser networkUser = this.playerCharacterMasterController.networkUser;
			if (networkUser)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(58);
				networkWriter.Write(base.gameObject);
				this.currentStats.Write(networkWriter);
				networkWriter.FinishMessage();
				networkUser.connectionToClient.SendWriter(networkWriter, this.GetNetworkChannel());
			}
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x0008E3A4 File Offset: 0x0008C5A4
		[NetworkMessageHandler(client = true, msgType = 58)]
		private static void HandleStatsUpdate(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (gameObject)
			{
				PlayerStatsComponent component = gameObject.GetComponent<PlayerStatsComponent>();
				if (component)
				{
					component.InstanceHandleStatsUpdate(netMsg.reader);
				}
			}
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x00015663 File Offset: 0x00013863
		[Client]
		private void InstanceHandleStatsUpdate(NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stats.PlayerStatsComponent::InstanceHandleStatsUpdate(UnityEngine.Networking.NetworkReader)' called on server");
				return;
			}
			if (!NetworkServer.active)
			{
				this.currentStats.Read(reader);
			}
			this.FlushStatsToUserProfile();
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x0008E3E0 File Offset: 0x0008C5E0
		[Client]
		private void FlushStatsToUserProfile()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stats.PlayerStatsComponent::FlushStatsToUserProfile()' called on server");
				return;
			}
			StatSheet.GetDelta(this.clientDeltaStatsBuffer, this.currentStats, this.recordedStats);
			StatSheet.Copy(this.currentStats, this.recordedStats);
			NetworkUser networkUser = this.playerCharacterMasterController.networkUser;
			LocalUser localUser = (networkUser != null) ? networkUser.localUser : null;
			if (localUser == null)
			{
				return;
			}
			UserProfile userProfile = localUser.userProfile;
			if (userProfile == null)
			{
				return;
			}
			userProfile.ApplyDeltaStatSheet(this.clientDeltaStatsBuffer);
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x0008E45C File Offset: 0x0008C65C
		[ConCommand(commandName = "print_stats", flags = ConVarFlags.None, helpText = "Prints all current stats of the sender.")]
		private static void CCPrintStats(ConCommandArgs args)
		{
			GameObject senderMasterObject = args.senderMasterObject;
			StatSheet statSheet;
			if (senderMasterObject == null)
			{
				statSheet = null;
			}
			else
			{
				PlayerStatsComponent component = senderMasterObject.GetComponent<PlayerStatsComponent>();
				statSheet = ((component != null) ? component.currentStats : null);
			}
			StatSheet statSheet2 = statSheet;
			if (statSheet2 == null)
			{
				return;
			}
			string[] array = new string[statSheet2.fields.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = string.Format("[\"{0}\"]={1}", statSheet2.fields[i].name, statSheet2.fields[i].ToString());
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001EE6 RID: 7910
		public static readonly List<PlayerStatsComponent> instancesList = new List<PlayerStatsComponent>();

		// Token: 0x04001EE9 RID: 7913
		private float serverTransmitTimer;

		// Token: 0x04001EEA RID: 7914
		private float serverTransmitInterval = 10f;

		// Token: 0x04001EEB RID: 7915
		private Vector3 previousBodyPosition;

		// Token: 0x04001EEC RID: 7916
		private GameObject cachedBodyObject;

		// Token: 0x04001EED RID: 7917
		private CharacterBody cachedCharacterBody;

		// Token: 0x04001EEE RID: 7918
		private CharacterMotor cachedBodyCharacterMotor;

		// Token: 0x04001EEF RID: 7919
		private Transform cachedBodyTransform;

		// Token: 0x04001EF0 RID: 7920
		public StatSheet currentStats;

		// Token: 0x04001EF1 RID: 7921
		private StatSheet clientDeltaStatsBuffer;

		// Token: 0x04001EF2 RID: 7922
		private StatSheet recordedStats;
	}
}
