using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004EA RID: 1258
	public class WireMeshBuilder : IDisposable
	{
		// Token: 0x06001C97 RID: 7319 RVA: 0x0008D114 File Offset: 0x0008B314
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

		// Token: 0x06001C98 RID: 7320 RVA: 0x0008D174 File Offset: 0x0008B374
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

		// Token: 0x06001C99 RID: 7321 RVA: 0x00014FC1 File Offset: 0x000131C1
		public Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.SetVertices(this.positions);
			mesh.SetColors(this.colors);
			mesh.SetIndices(this.indices.ToArray(), MeshTopology.Lines, 0);
			return mesh;
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x00014FF3 File Offset: 0x000131F3
		public void Dispose()
		{
			this.uniqueVertexToIndex = null;
			this.indices = null;
			this.positions = null;
			this.colors = null;
		}

		// Token: 0x04001E90 RID: 7824
		private int uniqueVertexCount;

		// Token: 0x04001E91 RID: 7825
		private Dictionary<WireMeshBuilder.LineVertex, int> uniqueVertexToIndex = new Dictionary<WireMeshBuilder.LineVertex, int>();

		// Token: 0x04001E92 RID: 7826
		private List<int> indices = new List<int>();

		// Token: 0x04001E93 RID: 7827
		private List<Vector3> positions = new List<Vector3>();

		// Token: 0x04001E94 RID: 7828
		private List<Color> colors = new List<Color>();

		// Token: 0x020004EB RID: 1259
		private struct LineVertex
		{
			// Token: 0x04001E95 RID: 7829
			public Vector3 position;

			// Token: 0x04001E96 RID: 7830
			public Color color;
		}
	}
}
