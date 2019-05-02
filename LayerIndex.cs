using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000451 RID: 1105
	public struct LayerIndex
	{
		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600189E RID: 6302 RVA: 0x000126FD File Offset: 0x000108FD
		public LayerMask mask
		{
			get
			{
				return (this.intVal >= 0) ? (1 << this.intVal) : this.intVal;
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x00080A9C File Offset: 0x0007EC9C
		static LayerIndex()
		{
			for (int i = 0; i < 32; i++)
			{
				string text = LayerMask.LayerToName(i);
				if (text != "" && (LayerIndex.assignedLayerMask & 1u << i) == 0u)
				{
					Debug.LogWarningFormat("Layer \"{0}\" is defined in this project's \"Tags and Layers\" settings but is not defined in LayerIndex!", new object[]
					{
						text
					});
				}
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060018A0 RID: 6304 RVA: 0x00012720 File Offset: 0x00010920
		public LayerMask collisionMask
		{
			get
			{
				return LayerIndex.collisionMasks[this.intVal];
			}
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x00080C30 File Offset: 0x0007EE30
		private static LayerIndex GetLayerIndex(string layerName)
		{
			LayerIndex layerIndex = new LayerIndex
			{
				intVal = LayerMask.NameToLayer(layerName)
			};
			if (layerIndex.intVal == LayerIndex.invalidLayer.intVal)
			{
				Debug.LogErrorFormat("Layer \"{0}\" is not defined in this project's \"Tags and Layers\" settings.", new object[]
				{
					layerName
				});
			}
			else
			{
				LayerIndex.assignedLayerMask |= 1u << layerIndex.intVal;
			}
			return layerIndex;
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x00080C94 File Offset: 0x0007EE94
		private static LayerMask[] CalcCollisionMasks()
		{
			LayerMask[] array = new LayerMask[32];
			for (int i = 0; i < 32; i++)
			{
				LayerMask layerMask = default(LayerMask);
				for (int j = 0; j < 32; j++)
				{
					if (!Physics.GetIgnoreLayerCollision(i, j))
					{
						layerMask |= 1 << j;
					}
				}
				array[i] = layerMask;
			}
			return array;
		}

		// Token: 0x04001C13 RID: 7187
		public int intVal;

		// Token: 0x04001C14 RID: 7188
		private static uint assignedLayerMask = 0u;

		// Token: 0x04001C15 RID: 7189
		public static readonly LayerIndex invalidLayer = new LayerIndex
		{
			intVal = -1
		};

		// Token: 0x04001C16 RID: 7190
		public static readonly LayerIndex defaultLayer = LayerIndex.GetLayerIndex("Default");

		// Token: 0x04001C17 RID: 7191
		public static readonly LayerIndex transparentFX = LayerIndex.GetLayerIndex("TransparentFX");

		// Token: 0x04001C18 RID: 7192
		public static readonly LayerIndex ignoreRaycast = LayerIndex.GetLayerIndex("Ignore Raycast");

		// Token: 0x04001C19 RID: 7193
		public static readonly LayerIndex water = LayerIndex.GetLayerIndex("Water");

		// Token: 0x04001C1A RID: 7194
		public static readonly LayerIndex ui = LayerIndex.GetLayerIndex("UI");

		// Token: 0x04001C1B RID: 7195
		public static readonly LayerIndex fakeActor = LayerIndex.GetLayerIndex("FakeActor");

		// Token: 0x04001C1C RID: 7196
		public static readonly LayerIndex noCollision = LayerIndex.GetLayerIndex("NoCollision");

		// Token: 0x04001C1D RID: 7197
		public static readonly LayerIndex pickups = LayerIndex.GetLayerIndex("Pickups");

		// Token: 0x04001C1E RID: 7198
		public static readonly LayerIndex world = LayerIndex.GetLayerIndex("World");

		// Token: 0x04001C1F RID: 7199
		public static readonly LayerIndex entityPrecise = LayerIndex.GetLayerIndex("EntityPrecise");

		// Token: 0x04001C20 RID: 7200
		public static readonly LayerIndex debris = LayerIndex.GetLayerIndex("Debris");

		// Token: 0x04001C21 RID: 7201
		public static readonly LayerIndex projectile = LayerIndex.GetLayerIndex("Projectile");

		// Token: 0x04001C22 RID: 7202
		public static readonly LayerIndex manualRender = LayerIndex.GetLayerIndex("ManualRender");

		// Token: 0x04001C23 RID: 7203
		public static readonly LayerIndex background = LayerIndex.GetLayerIndex("Background");

		// Token: 0x04001C24 RID: 7204
		public static readonly LayerIndex ragdoll = LayerIndex.GetLayerIndex("Ragdoll");

		// Token: 0x04001C25 RID: 7205
		public static readonly LayerIndex noDraw = LayerIndex.GetLayerIndex("NoDraw");

		// Token: 0x04001C26 RID: 7206
		public static readonly LayerIndex prefabBrush = LayerIndex.GetLayerIndex("PrefabBrush");

		// Token: 0x04001C27 RID: 7207
		public static readonly LayerIndex postProcess = LayerIndex.GetLayerIndex("PostProcess");

		// Token: 0x04001C28 RID: 7208
		public static readonly LayerIndex uiWorldSpace = LayerIndex.GetLayerIndex("UI, WorldSpace");

		// Token: 0x04001C29 RID: 7209
		private static readonly LayerMask[] collisionMasks = LayerIndex.CalcCollisionMasks();
	}
}
