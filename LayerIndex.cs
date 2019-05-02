using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045C RID: 1116
	public struct LayerIndex
	{
		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060018FA RID: 6394 RVA: 0x00012C0A File Offset: 0x00010E0A
		public LayerMask mask
		{
			get
			{
				return (this.intVal >= 0) ? (1 << this.intVal) : this.intVal;
			}
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x0008143C File Offset: 0x0007F63C
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

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060018FC RID: 6396 RVA: 0x00012C2D File Offset: 0x00010E2D
		public LayerMask collisionMask
		{
			get
			{
				return LayerIndex.collisionMasks[this.intVal];
			}
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x000815D0 File Offset: 0x0007F7D0
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

		// Token: 0x060018FE RID: 6398 RVA: 0x00081634 File Offset: 0x0007F834
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

		// Token: 0x04001C47 RID: 7239
		public int intVal;

		// Token: 0x04001C48 RID: 7240
		private static uint assignedLayerMask = 0u;

		// Token: 0x04001C49 RID: 7241
		public static readonly LayerIndex invalidLayer = new LayerIndex
		{
			intVal = -1
		};

		// Token: 0x04001C4A RID: 7242
		public static readonly LayerIndex defaultLayer = LayerIndex.GetLayerIndex("Default");

		// Token: 0x04001C4B RID: 7243
		public static readonly LayerIndex transparentFX = LayerIndex.GetLayerIndex("TransparentFX");

		// Token: 0x04001C4C RID: 7244
		public static readonly LayerIndex ignoreRaycast = LayerIndex.GetLayerIndex("Ignore Raycast");

		// Token: 0x04001C4D RID: 7245
		public static readonly LayerIndex water = LayerIndex.GetLayerIndex("Water");

		// Token: 0x04001C4E RID: 7246
		public static readonly LayerIndex ui = LayerIndex.GetLayerIndex("UI");

		// Token: 0x04001C4F RID: 7247
		public static readonly LayerIndex fakeActor = LayerIndex.GetLayerIndex("FakeActor");

		// Token: 0x04001C50 RID: 7248
		public static readonly LayerIndex noCollision = LayerIndex.GetLayerIndex("NoCollision");

		// Token: 0x04001C51 RID: 7249
		public static readonly LayerIndex pickups = LayerIndex.GetLayerIndex("Pickups");

		// Token: 0x04001C52 RID: 7250
		public static readonly LayerIndex world = LayerIndex.GetLayerIndex("World");

		// Token: 0x04001C53 RID: 7251
		public static readonly LayerIndex entityPrecise = LayerIndex.GetLayerIndex("EntityPrecise");

		// Token: 0x04001C54 RID: 7252
		public static readonly LayerIndex debris = LayerIndex.GetLayerIndex("Debris");

		// Token: 0x04001C55 RID: 7253
		public static readonly LayerIndex projectile = LayerIndex.GetLayerIndex("Projectile");

		// Token: 0x04001C56 RID: 7254
		public static readonly LayerIndex manualRender = LayerIndex.GetLayerIndex("ManualRender");

		// Token: 0x04001C57 RID: 7255
		public static readonly LayerIndex background = LayerIndex.GetLayerIndex("Background");

		// Token: 0x04001C58 RID: 7256
		public static readonly LayerIndex ragdoll = LayerIndex.GetLayerIndex("Ragdoll");

		// Token: 0x04001C59 RID: 7257
		public static readonly LayerIndex noDraw = LayerIndex.GetLayerIndex("NoDraw");

		// Token: 0x04001C5A RID: 7258
		public static readonly LayerIndex prefabBrush = LayerIndex.GetLayerIndex("PrefabBrush");

		// Token: 0x04001C5B RID: 7259
		public static readonly LayerIndex postProcess = LayerIndex.GetLayerIndex("PostProcess");

		// Token: 0x04001C5C RID: 7260
		public static readonly LayerIndex uiWorldSpace = LayerIndex.GetLayerIndex("UI, WorldSpace");

		// Token: 0x04001C5D RID: 7261
		private static readonly LayerMask[] collisionMasks = LayerIndex.CalcCollisionMasks();
	}
}
