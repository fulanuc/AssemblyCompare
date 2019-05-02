using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003D5 RID: 981
	public class SceneInfo : MonoBehaviour
	{
		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06001560 RID: 5472 RVA: 0x0001028E File Offset: 0x0000E48E
		public static SceneInfo instance
		{
			get
			{
				return SceneInfo._instance;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06001561 RID: 5473 RVA: 0x00010295 File Offset: 0x0000E495
		// (set) Token: 0x06001562 RID: 5474 RVA: 0x0001029D File Offset: 0x0000E49D
		public SceneDef sceneDef { get; private set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06001563 RID: 5475 RVA: 0x000102A6 File Offset: 0x0000E4A6
		public bool countsAsStage
		{
			get
			{
				return this.sceneDef && this.sceneDef.sceneType == SceneType.Stage;
			}
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00072E50 File Offset: 0x00071050
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

		// Token: 0x06001565 RID: 5477 RVA: 0x000102C5 File Offset: 0x0000E4C5
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.groundNodes);
			UnityEngine.Object.Destroy(this.airNodes);
			UnityEngine.Object.Destroy(this.railNodes);
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x000102E8 File Offset: 0x0000E4E8
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

		// Token: 0x06001567 RID: 5479 RVA: 0x00010314 File Offset: 0x0000E514
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

		// Token: 0x06001568 RID: 5480 RVA: 0x00010340 File Offset: 0x0000E540
		public void SetGateState(string gateName, bool gateEnabled)
		{
			this.groundNodes.SetGateState(gateName, gateEnabled);
			this.airNodes.SetGateState(gateName, gateEnabled);
			if (this.railNodes)
			{
				this.railNodes.SetGateState(gateName, gateEnabled);
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00010376 File Offset: 0x0000E576
		private void OnEnable()
		{
			if (!SceneInfo._instance)
			{
				SceneInfo._instance = this;
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0001038A File Offset: 0x0000E58A
		private void OnDisable()
		{
			if (SceneInfo._instance == this)
			{
				SceneInfo._instance = null;
			}
		}

		// Token: 0x0400188D RID: 6285
		private static SceneInfo _instance;

		// Token: 0x0400188E RID: 6286
		[FormerlySerializedAs("groundNodes")]
		public MapNodeGroup groundNodeGroup;

		// Token: 0x0400188F RID: 6287
		[FormerlySerializedAs("airNodes")]
		public MapNodeGroup airNodeGroup;

		// Token: 0x04001890 RID: 6288
		[FormerlySerializedAs("railNodes")]
		public MapNodeGroup railNodeGroup;

		// Token: 0x04001891 RID: 6289
		[NonSerialized]
		public NodeGraph groundNodes;

		// Token: 0x04001892 RID: 6290
		[NonSerialized]
		public NodeGraph airNodes;

		// Token: 0x04001893 RID: 6291
		[NonSerialized]
		public NodeGraph railNodes;
	}
}
