using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000381 RID: 897
	public class PaintTerrain : MonoBehaviour
	{
		// Token: 0x060012BE RID: 4798 RVA: 0x0006A2D4 File Offset: 0x000684D4
		private void Start()
		{
			this.snowfallDirection = this.snowfallDirection.normalized;
			this.terrain = base.GetComponent<Terrain>();
			this.data = this.terrain.terrainData;
			this.alphamaps = this.data.GetAlphamaps(0, 0, this.data.alphamapWidth, this.data.alphamapHeight);
			this.detailmapGrass = this.data.GetDetailLayer(0, 0, this.data.detailWidth, this.data.detailHeight, 0);
			this.UpdateAlphaMaps();
			this.UpdateDetailMaps();
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0006A370 File Offset: 0x00068570
		private void UpdateAlphaMaps()
		{
			for (int i = 0; i < this.data.alphamapHeight; i++)
			{
				for (int j = 0; j < this.data.alphamapWidth; j++)
				{
					float z = (float)j / (float)this.data.alphamapWidth * this.data.size.x;
					float x = (float)i / (float)this.data.alphamapHeight * this.data.size.z;
					float num = 0f;
					float num2 = 0f;
					float num3 = Mathf.Pow(Vector3.Dot(Vector3.up, this.data.GetInterpolatedNormal((float)i / (float)this.data.alphamapHeight, (float)j / (float)this.data.alphamapWidth)), this.splatSlopePower);
					float num4 = num3;
					float num5 = 1f - num3;
					float interpolatedHeight = this.data.GetInterpolatedHeight((float)i / (float)this.data.alphamapHeight, (float)j / (float)this.data.alphamapWidth);
					RaycastHit raycastHit;
					if (Physics.Raycast(new Vector3(x, interpolatedHeight, z), this.snowfallDirection, out raycastHit, this.splatRaycastLength, LayerIndex.world.mask))
					{
						float num6 = num4;
						float num7 = Mathf.Clamp01(Mathf.Pow(raycastHit.distance / this.splatHeightReference, this.heightPower));
						num4 = num7 * num6;
						num = (1f - num7) * num6;
					}
					this.alphamaps[j, i, 0] = num4;
					this.alphamaps[j, i, 1] = num;
					this.alphamaps[j, i, 2] = num5;
					this.alphamaps[j, i, 3] = num2;
				}
			}
			this.data.SetAlphamaps(0, 0, this.alphamaps);
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0006A544 File Offset: 0x00068744
		private void UpdateDetailMaps()
		{
			for (int i = 0; i < this.data.detailHeight; i++)
			{
				for (int j = 0; j < this.data.detailWidth; j++)
				{
					int num = 0;
					this.detailmapGrass[j, i] = num;
				}
			}
			this.data.SetDetailLayer(0, 0, 0, this.detailmapGrass);
		}

		// Token: 0x04001673 RID: 5747
		public float splatHeightReference = 60f;

		// Token: 0x04001674 RID: 5748
		public float splatRaycastLength = 200f;

		// Token: 0x04001675 RID: 5749
		public float splatSlopePower = 1f;

		// Token: 0x04001676 RID: 5750
		public float heightPower = 1f;

		// Token: 0x04001677 RID: 5751
		public Vector3 snowfallDirection = Vector3.up;

		// Token: 0x04001678 RID: 5752
		public Texture2D grassNoiseMap;

		// Token: 0x04001679 RID: 5753
		private Terrain terrain;

		// Token: 0x0400167A RID: 5754
		private TerrainData data;

		// Token: 0x0400167B RID: 5755
		private float[,,] alphamaps;

		// Token: 0x0400167C RID: 5756
		private int[,] detailmapGrass;
	}
}
