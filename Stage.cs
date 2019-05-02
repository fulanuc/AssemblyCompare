using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.ConVar;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003EB RID: 1003
	public class Stage : NetworkBehaviour
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x000106BF File Offset: 0x0000E8BF
		// (set) Token: 0x060015E5 RID: 5605 RVA: 0x000106C6 File Offset: 0x0000E8C6
		public static Stage instance { get; private set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x000106CE File Offset: 0x0000E8CE
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x000106D6 File Offset: 0x0000E8D6
		public SceneDef sceneDef { get; private set; }

		// Token: 0x060015E8 RID: 5608 RVA: 0x00075334 File Offset: 0x00073534
		private void Start()
		{
			this.sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
			if (NetworkServer.active)
			{
				this.NetworkstartRunTime = Run.instance.fixedTime;
				this.stageSpawnPosition = this.SampleNodeGraphForSpawnPosition();
				ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
				Transform playerSpawnTransform = this.GetPlayerSpawnTransform();
				for (int i = 0; i < readOnlyInstancesList.Count; i++)
				{
					CharacterMaster characterMaster = readOnlyInstancesList[i];
					if (characterMaster && !characterMaster.GetComponent<PlayerCharacterMasterController>() && !characterMaster.GetBodyObject() && characterMaster.gameObject.scene.buildIndex == -1)
					{
						Vector3 vector = Vector3.zero;
						Quaternion rotation = Quaternion.identity;
						if (playerSpawnTransform)
						{
							vector = playerSpawnTransform.position;
							rotation = playerSpawnTransform.rotation;
							BaseAI component = readOnlyInstancesList[i].GetComponent<BaseAI>();
							CharacterBody component2 = readOnlyInstancesList[i].bodyPrefab.GetComponent<CharacterBody>();
							if (component && component2)
							{
								NodeGraph nodeGraph = component.GetNodeGraph();
								if (nodeGraph)
								{
									List<NodeGraph.NodeIndex> list = nodeGraph.FindNodesInRange(vector, 10f, 100f, (HullMask)(1 << (int)component2.hullClassification));
									if ((float)list.Count > 0f)
									{
										nodeGraph.GetNodePosition(list[UnityEngine.Random.Range(0, list.Count)], out vector);
									}
								}
							}
						}
						readOnlyInstancesList[i].Respawn(vector, rotation);
					}
				}
				this.BeginServer();
			}
			if (NetworkClient.active)
			{
				this.RespawnLocalPlayers();
			}
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x000754C4 File Offset: 0x000736C4
		[Client]
		public void RespawnLocalPlayers()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stage::RespawnLocalPlayers()' called on server");
				return;
			}
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				NetworkUser networkUser = readOnlyInstancesList[i];
				CharacterMaster characterMaster = null;
				if (networkUser.isLocalPlayer && networkUser.masterObject)
				{
					characterMaster = networkUser.masterObject.GetComponent<CharacterMaster>();
				}
				if (characterMaster)
				{
					characterMaster.CallCmdRespawn("");
				}
			}
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x000106DF File Offset: 0x0000E8DF
		private void OnEnable()
		{
			Stage.instance = SingletonHelper.Assign<Stage>(Stage.instance, this);
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x000106F1 File Offset: 0x0000E8F1
		private void OnDisable()
		{
			Stage.instance = SingletonHelper.Unassign<Stage>(Stage.instance, this);
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x0007553C File Offset: 0x0007373C
		private Vector3 SampleNodeGraphForSpawnPosition()
		{
			Vector3 zero = Vector3.zero;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeFlags requiredFlags = this.usePod ? NodeFlags.NoCeiling : NodeFlags.None;
			List<NodeGraph.NodeIndex> activeNodesForHullMaskWithFlagConditions = groundNodes.GetActiveNodesForHullMaskWithFlagConditions(HullMask.BeetleQueen, requiredFlags, NodeFlags.None);
			if (activeNodesForHullMaskWithFlagConditions.Count < 0)
			{
				Debug.LogWarning("No spawn points available in scene!");
				return Vector3.zero;
			}
			NodeGraph.NodeIndex nodeIndex = activeNodesForHullMaskWithFlagConditions[Run.instance.spawnRng.RangeInt(0, activeNodesForHullMaskWithFlagConditions.Count)];
			groundNodes.GetNodePosition(nodeIndex, out zero);
			return zero;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x000755B4 File Offset: 0x000737B4
		[Server]
		public Transform GetPlayerSpawnTransform()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'UnityEngine.Transform RoR2.Stage::GetPlayerSpawnTransform()' called on client");
				return null;
			}
			SpawnPoint spawnPoint = SpawnPoint.ConsumeSpawnPoint();
			if (spawnPoint)
			{
				return spawnPoint.transform;
			}
			return null;
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x000755F8 File Offset: 0x000737F8
		[Server]
		public void RespawnCharacter(CharacterMaster characterMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::RespawnCharacter(RoR2.CharacterMaster)' called on client");
				return;
			}
			if (!characterMaster)
			{
				return;
			}
			Transform playerSpawnTransform = this.GetPlayerSpawnTransform();
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (playerSpawnTransform)
			{
				vector = playerSpawnTransform.position;
				quaternion = playerSpawnTransform.rotation;
			}
			characterMaster.Respawn(vector, quaternion);
			if (characterMaster.GetComponent<PlayerCharacterMasterController>())
			{
				this.spawnedAnyPlayer = true;
			}
			if (this.usePod)
			{
				Run.instance.HandlePlayerFirstEntryAnimation(characterMaster.GetBody(), vector, quaternion);
			}
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00010703 File Offset: 0x0000E903
		[Server]
		public void BeginAdvanceStage(string destinationStage)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::BeginAdvanceStage(System.String)' called on client");
				return;
			}
			this.NetworkstageAdvanceTime = Run.instance.fixedTime + 0.75f;
			this.nextStage = destinationStage;
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060015F0 RID: 5616 RVA: 0x00010737 File Offset: 0x0000E937
		// (set) Token: 0x060015F1 RID: 5617 RVA: 0x0001073F File Offset: 0x0000E93F
		public bool completed { get; private set; }

		// Token: 0x060015F2 RID: 5618 RVA: 0x00010748 File Offset: 0x0000E948
		[Server]
		private void BeginServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::BeginServer()' called on client");
				return;
			}
			Action<Stage> action = Stage.onServerStageBegin;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x0001076F File Offset: 0x0000E96F
		[Server]
		public void CompleteServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::CompleteServer()' called on client");
				return;
			}
			if (this.completed)
			{
				return;
			}
			this.completed = true;
			Action<Stage> action = Stage.onServerStageComplete;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060015F4 RID: 5620 RVA: 0x00075684 File Offset: 0x00073884
		// (remove) Token: 0x060015F5 RID: 5621 RVA: 0x000756B8 File Offset: 0x000738B8
		public static event Action<Stage> onServerStageBegin;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x060015F6 RID: 5622 RVA: 0x000756EC File Offset: 0x000738EC
		// (remove) Token: 0x060015F7 RID: 5623 RVA: 0x00075720 File Offset: 0x00073920
		public static event Action<Stage> onServerStageComplete;

		// Token: 0x060015F8 RID: 5624 RVA: 0x00075754 File Offset: 0x00073954
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (!string.IsNullOrEmpty(this.nextStage) && this.stageAdvanceTime <= Run.instance.fixedTime)
				{
					string nextSceneName = this.nextStage;
					this.nextStage = null;
					Run.instance.AdvanceStage(nextSceneName);
				}
				if (this.spawnedAnyPlayer && float.IsInfinity(this.stageAdvanceTime) && !Run.instance.isGameOverServer)
				{
					ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
					bool flag = false;
					for (int i = 0; i < instances.Count; i++)
					{
						PlayerCharacterMasterController playerCharacterMasterController = instances[i];
						if (playerCharacterMasterController.isConnected && playerCharacterMasterController.preventGameOver)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Run.instance.BeginGameOver(GameResultType.Lost);
					}
				}
			}
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060015FC RID: 5628 RVA: 0x0007586C File Offset: 0x00073A6C
		// (set) Token: 0x060015FD RID: 5629 RVA: 0x000107C2 File Offset: 0x0000E9C2
		public float NetworkstartRunTime
		{
			get
			{
				return this.startRunTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.startRunTime, 1u);
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x00075880 File Offset: 0x00073A80
		// (set) Token: 0x060015FF RID: 5631 RVA: 0x000107D6 File Offset: 0x0000E9D6
		public float NetworkstageAdvanceTime
		{
			get
			{
				return this.stageAdvanceTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.stageAdvanceTime, 2u);
			}
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x00075894 File Offset: 0x00073A94
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.startRunTime);
				writer.Write(this.stageAdvanceTime);
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
				writer.Write(this.startRunTime);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.stageAdvanceTime);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x00075940 File Offset: 0x00073B40
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.startRunTime = reader.ReadSingle();
				this.stageAdvanceTime = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.startRunTime = reader.ReadSingle();
			}
			if ((num & 2) != 0)
			{
				this.stageAdvanceTime = reader.ReadSingle();
			}
		}

		// Token: 0x04001936 RID: 6454
		[SyncVar]
		public float startRunTime;

		// Token: 0x04001938 RID: 6456
		private Vector3 stageSpawnPosition = Vector3.zero;

		// Token: 0x04001939 RID: 6457
		private bool spawnedAnyPlayer;

		// Token: 0x0400193A RID: 6458
		[NonSerialized]
		public bool usePod = Run.instance && Run.instance.stageClearCount == 0 && Stage.stage1PodConVar.value;

		// Token: 0x0400193B RID: 6459
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x0400193C RID: 6460
		[SyncVar]
		public float stageAdvanceTime = float.PositiveInfinity;

		// Token: 0x0400193D RID: 6461
		public const float stageAdvanceTransitionDuration = 0.5f;

		// Token: 0x0400193E RID: 6462
		public const float stageAdvanceTransitionDelay = 0.75f;

		// Token: 0x0400193F RID: 6463
		private string nextStage = "";
	}
}
