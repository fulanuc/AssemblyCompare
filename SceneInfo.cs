using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003CF RID: 975
	public class SceneInfo : MonoBehaviour
	{
		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x00010007 File Offset: 0x0000E207
		public static SceneInfo instance
		{
			get
			{
				return SceneInfo._instance;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06001533 RID: 5427 RVA: 0x0001000E File Offset: 0x0000E20E
		// (set) Token: 0x06001534 RID: 5428 RVA: 0x00010016 File Offset: 0x0000E216
		public SceneDef sceneDef { get; private set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x0001001F File Offset: 0x0000E21F
		public bool countsAsStage
		{
			get
			{
				return this.sceneDef && this.sceneDef.sceneType == SceneType.Stage;
			}
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x000729E4 File Offset: 0x00070BE4
		private void Awake()
		{
			if (this.groundNodeGroup)
			{
				this.groundNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.groundNodeGroup.nodeGraph);
			}
			if (this.airNodeGroup)
			{
				this.airNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.airNodeGroup.nodeGraph);
			}
			if (this.railNodeGroup)
			{
				this.railNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.railNodeGroup.nodeGraph);
			}
			this.sceneDef = SceneCatalog.GetSceneDefFromSceneName(base.gameObject.scene.name);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x0001003E File Offset: 0x0000E23E
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.groundNodes);
			UnityEngine.Object.Destroy(this.airNodes);
			UnityEngine.Object.Destroy(this.railNodes);
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00010061 File Offset: 0x0000E261
		public MapNodeGroup GetNodeGroup(MapNodeGroup.GraphType nodeGraphType)
		{
			switch (nodeGraphType)
			{
			case MapNodeGroup.GraphType.Ground:
				return this.groundNodeGroup;
			case MapNodeGroup.GraphType.Air:
				return this.airNodeGroup;
			case MapNodeGroup.GraphType.Rail:
				return this.railNodeGroup;
			default:
				return null;
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0001008D File Offset: 0x0000E28D
		public NodeGraph GetNodeGraph(MapNodeGroup.GraphType nodeGraphType)
		{
			switch (nodeGraphType)
			{
			case MapNodeGroup.GraphType.Ground:
				return this.groundNodes;
			case MapNodeGroup.GraphType.Air:
				return this.airNodes;
			case MapNodeGroup.GraphType.Rail:
				return this.railNodes;
			default:
				return null;
			}
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x000100B9 File Offset: 0x0000E2B9
		public void SetGateState(string gateName, bool gateEnabled)
		{
			this.groundNodes.SetGateState(gateName, gateEnabled);
			this.airNodes.SetGateState(gateName, gateEnabled);
			if (this.railNodes)
			{
				this.railNodes.SetGateState(gateName, gateEnabled);
			}
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x000100EF File Offset: 0x0000E2EF
		private void OnEnable()
		{
			if (!SceneInfo._instance)
			{
				SceneInfo._instance = this;
			}
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x00010103 File Offset: 0x0000E303
		private void OnDisable()
		{
			if (SceneInfo._instance == this)
			{
				SceneInfo._instance = null;
			}
		}

		// Token: 0x0400186B RID: 6251
		private static SceneInfo _instance;

		// Token: 0x0400186C RID: 6252
		[FormerlySerializedAs("groundNodes")]
		public MapNodeGroup groundNodeGroup;

		// Token: 0x0400186D RID: 6253
		[FormerlySerializedAs("airNodes")]
		public MapNodeGroup airNodeGroup;

		// Token: 0x0400186E RID: 6254
		[FormerlySerializedAs("railNodes")]
		public MapNodeGroup railNodeGroup;

		// Token: 0x0400186F RID: 6255
		[NonSerialized]
		public NodeGraph groundNodes;

		// Token: 0x04001870 RID: 6256
		[NonSerialized]
		public NodeGraph airNodes;

		// Token: 0x04001871 RID: 6257
		[NonSerialized]
		public NodeGraph railNodes;
	}
}
