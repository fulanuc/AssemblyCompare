using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D1 RID: 977
	public class SceneObjectToggleGroup : NetworkBehaviour
	{
		// Token: 0x0600153E RID: 5438 RVA: 0x00010118 File Offset: 0x0000E318
		static SceneObjectToggleGroup()
		{
			GameNetworkManager.onServerSceneChangedGlobal += SceneObjectToggleGroup.OnServerSceneChanged;
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x00072A78 File Offset: 0x00070C78
		private static void OnServerSceneChanged(string sceneName)
		{
			while (SceneObjectToggleGroup.activationsQueue.Count > 0)
			{
				SceneObjectToggleGroup sceneObjectToggleGroup = SceneObjectToggleGroup.activationsQueue.Dequeue();
				if (sceneObjectToggleGroup)
				{
					sceneObjectToggleGroup.ApplyActivations();
				}
			}
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00072AB0 File Offset: 0x00070CB0
		private void Awake()
		{
			SceneObjectToggleGroup.activationsQueue.Enqueue(this);
			int num = 0;
			for (int i = 0; i < this.toggleGroups.Length; i++)
			{
				num += this.toggleGroups[i].objects.Length;
			}
			this.allToggleableObjects = new GameObject[num];
			this.activations = new bool[num];
			this.internalToggleGroups = new SceneObjectToggleGroup.ToggleGroupRange[this.toggleGroups.Length];
			int start = 0;
			for (int j = 0; j < this.toggleGroups.Length; j++)
			{
				GameObject[] objects = this.toggleGroups[j].objects;
				SceneObjectToggleGroup.ToggleGroupRange toggleGroupRange = default(SceneObjectToggleGroup.ToggleGroupRange);
				toggleGroupRange.start = start;
				toggleGroupRange.count = objects.Length;
				toggleGroupRange.minEnabled = this.toggleGroups[j].minEnabled;
				toggleGroupRange.maxEnabled = this.toggleGroups[j].maxEnabled;
				this.internalToggleGroups[j] = toggleGroupRange;
				foreach (GameObject gameObject in objects)
				{
					this.allToggleableObjects[start++] = gameObject;
				}
			}
			if (NetworkServer.active)
			{
				this.Generate();
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x00010135 File Offset: 0x0000E335
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!NetworkServer.active)
			{
				this.ApplyActivations();
			}
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00072BE0 File Offset: 0x00070DE0
		[Server]
		private void Generate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SceneObjectToggleGroup::Generate()' called on client");
				return;
			}
			for (int i = 0; i < this.internalToggleGroups.Length; i++)
			{
				SceneObjectToggleGroup.ToggleGroupRange toggleGroupRange = this.internalToggleGroups[i];
				int num = Run.instance.stageRng.RangeInt(toggleGroupRange.minEnabled, toggleGroupRange.maxEnabled + 1);
				List<int> list = SceneObjectToggleGroup.<Generate>g__RangeList|12_0(toggleGroupRange.start, toggleGroupRange.count);
				Util.ShuffleList<int>(list, Run.instance.stageRng);
				for (int j = num - 1; j >= 0; j--)
				{
					this.activations[list[j]] = true;
					list.RemoveAt(j);
				}
				for (int k = 0; k < list.Count; k++)
				{
					this.activations[list[k]] = false;
				}
			}
			base.SetDirtyBit(1u);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00072CB8 File Offset: 0x00070EB8
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = initialState ? 1u : base.syncVarDirtyBits;
			writer.Write((byte)num);
			if ((num & 1u) != 0u)
			{
				int num2 = 0;
				int num3 = (this.activations.Length - 1 >> 3) + 1;
				for (int i = 0; i < num3; i++)
				{
					byte b = 0;
					int num4 = 0;
					while (num4 < 8 && num2 < this.activations.Length)
					{
						if (this.activations[num2])
						{
							b |= (byte)(1 << num4);
						}
						num4++;
						num2++;
					}
					writer.Write(b);
				}
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00072D48 File Offset: 0x00070F48
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) != 0)
			{
				int num = 0;
				int num2 = (this.activations.Length - 1 >> 3) + 1;
				for (int i = 0; i < num2; i++)
				{
					byte b = reader.ReadByte();
					int num3 = 0;
					while (num3 < 8 && num < this.activations.Length)
					{
						this.activations[num] = ((b & (byte)(1 << num3)) > 0);
						num3++;
						num++;
					}
				}
			}
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x00072DB8 File Offset: 0x00070FB8
		private void ApplyActivations()
		{
			for (int i = 0; i < this.allToggleableObjects.Length; i++)
			{
				this.allToggleableObjects[i].SetActive(this.activations[i]);
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x04001876 RID: 6262
		public GameObjectToggleGroup[] toggleGroups;

		// Token: 0x04001877 RID: 6263
		private const byte enabledObjectsDirtyBit = 1;

		// Token: 0x04001878 RID: 6264
		private const byte initialStateMask = 1;

		// Token: 0x04001879 RID: 6265
		private static readonly Queue<SceneObjectToggleGroup> activationsQueue = new Queue<SceneObjectToggleGroup>();

		// Token: 0x0400187A RID: 6266
		private GameObject[] allToggleableObjects;

		// Token: 0x0400187B RID: 6267
		private bool[] activations;

		// Token: 0x0400187C RID: 6268
		private SceneObjectToggleGroup.ToggleGroupRange[] internalToggleGroups;

		// Token: 0x020003D2 RID: 978
		private struct ToggleGroupRange
		{
			// Token: 0x0400187D RID: 6269
			public int start;

			// Token: 0x0400187E RID: 6270
			public int count;

			// Token: 0x0400187F RID: 6271
			public int minEnabled;

			// Token: 0x04001880 RID: 6272
			public int maxEnabled;
		}
	}
}
