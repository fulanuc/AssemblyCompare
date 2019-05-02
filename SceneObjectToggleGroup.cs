using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D7 RID: 983
	public class SceneObjectToggleGroup : NetworkBehaviour
	{
		// Token: 0x0600156C RID: 5484 RVA: 0x0001039F File Offset: 0x0000E59F
		static SceneObjectToggleGroup()
		{
			GameNetworkManager.onServerSceneChangedGlobal += SceneObjectToggleGroup.OnServerSceneChanged;
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00072EE4 File Offset: 0x000710E4
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

		// Token: 0x0600156E RID: 5486 RVA: 0x00072F1C File Offset: 0x0007111C
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

		// Token: 0x0600156F RID: 5487 RVA: 0x000103BC File Offset: 0x0000E5BC
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!NetworkServer.active)
			{
				this.ApplyActivations();
			}
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x0007304C File Offset: 0x0007124C
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

		// Token: 0x06001571 RID: 5489 RVA: 0x00073124 File Offset: 0x00071324
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

		// Token: 0x06001572 RID: 5490 RVA: 0x000731B4 File Offset: 0x000713B4
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

		// Token: 0x06001573 RID: 5491 RVA: 0x00073224 File Offset: 0x00071424
		private void ApplyActivations()
		{
			for (int i = 0; i < this.allToggleableObjects.Length; i++)
			{
				this.allToggleableObjects[i].SetActive(this.activations[i]);
			}
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x04001898 RID: 6296
		public GameObjectToggleGroup[] toggleGroups;

		// Token: 0x04001899 RID: 6297
		private const byte enabledObjectsDirtyBit = 1;

		// Token: 0x0400189A RID: 6298
		private const byte initialStateMask = 1;

		// Token: 0x0400189B RID: 6299
		private static readonly Queue<SceneObjectToggleGroup> activationsQueue = new Queue<SceneObjectToggleGroup>();

		// Token: 0x0400189C RID: 6300
		private GameObject[] allToggleableObjects;

		// Token: 0x0400189D RID: 6301
		private bool[] activations;

		// Token: 0x0400189E RID: 6302
		private SceneObjectToggleGroup.ToggleGroupRange[] internalToggleGroups;

		// Token: 0x020003D8 RID: 984
		private struct ToggleGroupRange
		{
			// Token: 0x0400189F RID: 6303
			public int start;

			// Token: 0x040018A0 RID: 6304
			public int count;

			// Token: 0x040018A1 RID: 6305
			public int minEnabled;

			// Token: 0x040018A2 RID: 6306
			public int maxEnabled;
		}
	}
}
