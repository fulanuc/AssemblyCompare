using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004F9 RID: 1273
	public class WireMeshBuilder : IDisposable
	{
		// Token: 0x06001CFE RID: 7422 RVA: 0x0008DE70 File Offset: 0x0008C070
		private int GetVertexIndex(WireMeshBuilder.LineVertex vertex)
		{
			int num;
			if (!this.uniqueVertexToIndex.TryGetValue(vertex, out num))
			{
				int num2 = this.uniqueVertexCount;
				this.uniqueVertexCount = num2 + 1;
				num = num2;
				this.positions.Add(vertex.position);
				this.colors.Add(vertex.color);
				this.uniqueVertexToIndex.Add(vertex, num);
			}
			return num;
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x0008DED0 File Offset: 0x0008C0D0
		public void AddLine(Vector3 p1, Color c1, Vector3 p2, Color c2)
		{
			WireMeshBuilder.LineVertex vertex = new WireMeshBuilder.LineVertex
			{
				position = p1,
				color = c1
			};
			WireMeshBuilder.LineVertex vertex2 = new WireMeshBuilder.LineVertex
			{
				position = p2,
				color = c2
			};
			int vertexIndex = this.GetVertexIndex(vertex);
			int vertexIndex2 = this.GetVertexIndex(vertex2);
			this.indices.Add(vertexIndex);
			this.indices.Add(vertexIndex2);
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x00015470 File Offset: 0x00013670
		public Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.SetVertices(this.positions);
			mesh.SetColors(this.colors);
			mesh.SetIndices(this.indices.ToArray(), MeshTopology.Lines, 0);
			return mesh;
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x000154A2 File Offset: 0x000136A2
		public void Dispose()
		{
			this.uniqueVertexToIndex = null;
			this.indices = null;
			this.positions = null;
			this.colors = null;
		}

		// Token: 0x04001ECE RID: 7886
		private int uniqueVertexCount;

		// Token: 0x04001ECF RID: 7887
		private Dictionary<WireMeshBuilder.LineVertex, int> uniqueVertexToIndex = new Dictionary<WireMeshBuilder.LineVertex, int>();

		// Token: 0x04001ED0 RID: 7888
		private List<int> indices = new List<int>();

		// Token: 0x04001ED1 RID: 7889
		private List<Vector3> positions = new List<Vector3>();

		// Token: 0x04001ED2 RID: 7890
		private List<Color> colors = new List<Color>();

		// Token: 0x020004FA RID: 1274
		private struct LineVertex
		{
			// Token: 0x04001ED3 RID: 7891
			public Vector3 position;

			// Token: 0x04001ED4 RID: 7892
			public Color color;
		}
	}
}
