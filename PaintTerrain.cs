using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000386 RID: 902
	public class PaintTerrain : MonoBehaviour
	{
		// Token: 0x060012DE RID: 4830 RVA: 0x0006A678 File Offset: 0x00068878
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

		// Token: 0x060012DF RID: 4831 RVA: 0x0006A714 File Offset: 0x00068914
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

		// Token: 0x060012E0 RID: 4832 RVA: 0x0006A8E8 File Offset: 0x00068AE8
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

		// Token: 0x0400168F RID: 5775
		public float splatHeightReference = 60f;

		// Token: 0x04001690 RID: 5776
		public float splatRaycastLength = 200f;

		// Token: 0x04001691 RID: 5777
		public float splatSlopePower = 1f;

		// Token: 0x04001692 RID: 5778
		public float heightPower = 1f;

		// Token: 0x04001693 RID: 5779
		public Vector3 snowfallDirection = Vector3.up;

		// Token: 0x04001694 RID: 5780
		public Texture2D grassNoiseMap;

		// Token: 0x04001695 RID: 5781
		private Terrain terrain;

		// Token: 0x04001696 RID: 5782
		private TerrainData data;

		// Token: 0x04001697 RID: 5783
		private float[,,] alphamaps;

		// Token: 0x04001698 RID: 5784
		private int[,] detailmapGrass;
	}
}
