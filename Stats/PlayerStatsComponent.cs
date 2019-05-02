using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004F0 RID: 1264
	[RequireComponent(typeof(CharacterMaster))]
	[RequireComponent(typeof(PlayerCharacterMasterController))]
	public class PlayerStatsComponent : NetworkBehaviour
	{
		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x000150B7 File Offset: 0x000132B7
		// (set) Token: 0x06001CA6 RID: 7334 RVA: 0x000150BF File Offset: 0x000132BF
		public CharacterMaster characterMaster { get; private set; }

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06001CA7 RID: 7335 RVA: 0x000150C8 File Offset: 0x000132C8
		// (set) Token: 0x06001CA8 RID: 7336 RVA: 0x000150D0 File Offset: 0x000132D0
		public PlayerCharacterMasterController playerCharacterMasterController { get; private set; }

		// Token: 0x06001CA9 RID: 7337 RVA: 0x0008D3E4 File Offset: 0x0008B5E4
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

		// Token: 0x06001CAA RID: 7338 RVA: 0x000150D9 File Offset: 0x000132D9
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				this.SendUpdateToClient();
			}
			PlayerStatsComponent.instancesList.Remove(this);
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x000150F4 File Offset: 0x000132F4
		public static StatSheet FindBodyStatSheet(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatSheet(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x0001510B File Offset: 0x0001330B
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

		// Token: 0x06001CAD RID: 7341 RVA: 0x0001512E File Offset: 0x0001332E
		public static PlayerStatsComponent FindBodyStatsComponent(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return null;
			}
			return PlayerStatsComponent.FindBodyStatsComponent(bodyObject.GetComponent<CharacterBody>());
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00015145 File Offset: 0x00013345
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

		// Token: 0x06001CAF RID: 7343 RVA: 0x0001515D File Offset: 0x0001335D
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

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00015183 File Offset: 0x00013383
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00015192 File Offset: 0x00013392
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

		// Token: 0x06001CB2 RID: 7346 RVA: 0x0008D43C File Offset: 0x0008B63C
		[Server]
		private void ServerUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stats.PlayerStatsComponent::ServerUpdate()' called on client");
				return;
			}
			StatManager.CharacterUpdateEvent e = default(StatManager.CharacterUpdateEvent);
			e.statsComponent = this;
			e.runTime = Run.FixedTimeStamp.now.t;
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
				e.additionalTimeAlive += Time.deltaTime;
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

		// Token: 0x06001CB3 RID: 7347 RVA: 0x0008D5A4 File Offset: 0x0008B7A4
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

		// Token: 0x06001CB4 RID: 7348 RVA: 0x0008D61C File Offset: 0x0008B81C
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

		// Token: 0x06001CB5 RID: 7349 RVA: 0x000151B4 File Offset: 0x000133B4
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

		// Token: 0x06001CB6 RID: 7350 RVA: 0x0008D658 File Offset: 0x0008B858
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

		// Token: 0x06001CB7 RID: 7351 RVA: 0x0008D6D4 File Offset: 0x0008B8D4
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

		// Token: 0x06001CBA RID: 7354 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001EA8 RID: 7848
		public static readonly List<PlayerStatsComponent> instancesList = new List<PlayerStatsComponent>();

		// Token: 0x04001EAB RID: 7851
		private float serverTransmitTimer;

		// Token: 0x04001EAC RID: 7852
		private float serverTransmitInterval = 10f;

		// Token: 0x04001EAD RID: 7853
		private Vector3 previousBodyPosition;

		// Token: 0x04001EAE RID: 7854
		private GameObject cachedBodyObject;

		// Token: 0x04001EAF RID: 7855
		private CharacterBody cachedCharacterBody;

		// Token: 0x04001EB0 RID: 7856
		private CharacterMotor cachedBodyCharacterMotor;

		// Token: 0x04001EB1 RID: 7857
		private Transform cachedBodyTransform;

		// Token: 0x04001EB2 RID: 7858
		public StatSheet currentStats;

		// Token: 0x04001EB3 RID: 7859
		private StatSheet clientDeltaStatsBuffer;

		// Token: 0x04001EB4 RID: 7860
		private StatSheet recordedStats;
	}
}
