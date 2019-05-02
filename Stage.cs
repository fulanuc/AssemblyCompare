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
	// Token: 0x020003F1 RID: 1009
	public class Stage : NetworkBehaviour
	{
		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001621 RID: 5665 RVA: 0x00010AC8 File Offset: 0x0000ECC8
		// (set) Token: 0x06001622 RID: 5666 RVA: 0x00010ACF File Offset: 0x0000ECCF
		public static Stage instance { get; private set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001623 RID: 5667 RVA: 0x00010AD7 File Offset: 0x0000ECD7
		// (set) Token: 0x06001624 RID: 5668 RVA: 0x00010ADF File Offset: 0x0000ECDF
		public SceneDef sceneDef { get; private set; }

		// Token: 0x06001625 RID: 5669 RVA: 0x0007596C File Offset: 0x00073B6C
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
						readOnlyInstancesList[i].Respawn(vector, rotation, false);
					}
				}
				this.BeginServer();
			}
			if (NetworkClient.active)
			{
				this.RespawnLocalPlayers();
			}
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00075AFC File Offset: 0x00073CFC
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

		// Token: 0x06001627 RID: 5671 RVA: 0x00010AE8 File Offset: 0x0000ECE8
		private void OnEnable()
		{
			Stage.instance = SingletonHelper.Assign<Stage>(Stage.instance, this);
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00010AFA File Offset: 0x0000ECFA
		private void OnDisable()
		{
			Stage.instance = SingletonHelper.Unassign<Stage>(Stage.instance, this);
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x00075B74 File Offset: 0x00073D74
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

		// Token: 0x0600162A RID: 5674 RVA: 0x00075BEC File Offset: 0x00073DEC
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

		// Token: 0x0600162B RID: 5675 RVA: 0x00075C30 File Offset: 0x00073E30
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
			characterMaster.Respawn(vector, quaternion, true);
			if (characterMaster.GetComponent<PlayerCharacterMasterController>())
			{
				this.spawnedAnyPlayer = true;
			}
			if (this.usePod)
			{
				Run.instance.HandlePlayerFirstEntryAnimation(characterMaster.GetBody(), vector, quaternion);
			}
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x00010B0C File Offset: 0x0000ED0C
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

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600162D RID: 5677 RVA: 0x00010B40 File Offset: 0x0000ED40
		// (set) Token: 0x0600162E RID: 5678 RVA: 0x00010B48 File Offset: 0x0000ED48
		public bool completed { get; private set; }

		// Token: 0x0600162F RID: 5679 RVA: 0x00010B51 File Offset: 0x0000ED51
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

		// Token: 0x06001630 RID: 5680 RVA: 0x00010B78 File Offset: 0x0000ED78
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

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06001631 RID: 5681 RVA: 0x00075CBC File Offset: 0x00073EBC
		// (remove) Token: 0x06001632 RID: 5682 RVA: 0x00075CF0 File Offset: 0x00073EF0
		public static event Action<Stage> onServerStageBegin;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06001633 RID: 5683 RVA: 0x00075D24 File Offset: 0x00073F24
		// (remove) Token: 0x06001634 RID: 5684 RVA: 0x00075D58 File Offset: 0x00073F58
		public static event Action<Stage> onServerStageComplete;

		// Token: 0x06001635 RID: 5685 RVA: 0x00075D8C File Offset: 0x00073F8C
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

		// Token: 0x06001638 RID: 5688 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001639 RID: 5689 RVA: 0x00075EA4 File Offset: 0x000740A4
		// (set) Token: 0x0600163A RID: 5690 RVA: 0x00010BCB File Offset: 0x0000EDCB
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

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x0600163B RID: 5691 RVA: 0x00075EB8 File Offset: 0x000740B8
		// (set) Token: 0x0600163C RID: 5692 RVA: 0x00010BDF File Offset: 0x0000EDDF
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

		// Token: 0x0600163D RID: 5693 RVA: 0x00075ECC File Offset: 0x000740CC
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

		// Token: 0x0600163E RID: 5694 RVA: 0x00075F78 File Offset: 0x00074178
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

		// Token: 0x0400195F RID: 6495
		[SyncVar]
		public float startRunTime;

		// Token: 0x04001961 RID: 6497
		private Vector3 stageSpawnPosition = Vector3.zero;

		// Token: 0x04001962 RID: 6498
		private bool spawnedAnyPlayer;

		// Token: 0x04001963 RID: 6499
		[NonSerialized]
		public bool usePod = Run.instance && Run.instance.stageClearCount == 0 && Stage.stage1PodConVar.value;

		// Token: 0x04001964 RID: 6500
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x04001965 RID: 6501
		[SyncVar]
		public float stageAdvanceTime = float.PositiveInfinity;

		// Token: 0x04001966 RID: 6502
		public const float stageAdvanceTransitionDuration = 0.5f;

		// Token: 0x04001967 RID: 6503
		public const float stageAdvanceTransitionDelay = 0.75f;

		// Token: 0x04001968 RID: 6504
		private string nextStage = "";
	}
}
