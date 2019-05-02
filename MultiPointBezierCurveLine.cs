using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000364 RID: 868
	[RequireComponent(typeof(LineRenderer))]
	[ExecuteAlways]
	public class MultiPointBezierCurveLine : MonoBehaviour
	{
		// Token: 0x060011E9 RID: 4585 RVA: 0x0000DA83 File Offset: 0x0000BC83
		private void Start()
		{
			this.lineRenderer = base.GetComponent<LineRenderer>();
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x000676C0 File Offset: 0x000658C0
		private void LateUpdate()
		{
			for (int i = 0; i < this.linePositionList.Length; i++)
			{
				float globalT = (float)i / (float)(this.linePositionList.Length - 1);
				this.linePositionList[i] = this.EvaluateBezier(globalT);
			}
			this.lineRenderer.SetPositions(this.linePositionList);
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x00067714 File Offset: 0x00065914
		private Vector3 EvaluateBezier(float globalT)
		{
			int num = this.vertexList.Length - 1;
			int num3;
			int num2 = Mathf.Min((num3 = Mathf.FloorToInt((float)num * globalT)) + 1, num);
			MultiPointBezierCurveLine.Vertex vertex = this.vertexList[num3];
			MultiPointBezierCurveLine.Vertex vertex2 = this.vertexList[num2];
			Vector3 vector = vertex.vertexTransform ? vertex.vertexTransform.position : vertex.position;
			Vector3 a = vertex2.vertexTransform ? vertex2.vertexTransform.position : vertex2.position;
			Vector3 b = vertex.vertexTransform ? vertex.vertexTransform.TransformVector(vertex.localVelocity) : vertex.localVelocity;
			Vector3 b2 = vertex2.vertexTransform ? vertex2.vertexTransform.TransformVector(vertex2.localVelocity) : vertex2.localVelocity;
			if (num3 == num2)
			{
				return vector;
			}
			float inMin = (float)num3 / (float)num;
			float inMax = (float)num2 / (float)num;
			float num4 = Util.Remap(globalT, inMin, inMax, 0f, 1f);
			Vector3 a2 = Vector3.Lerp(vector, vector + b, num4);
			Vector3 b3 = Vector3.Lerp(a, a + b2, 1f - num4);
			return Vector3.Lerp(a2, b3, num4);
		}

		// Token: 0x040015F4 RID: 5620
		public MultiPointBezierCurveLine.Vertex[] vertexList;

		// Token: 0x040015F5 RID: 5621
		public Vector3[] linePositionList;

		// Token: 0x040015F6 RID: 5622
		[HideInInspector]
		public LineRenderer lineRenderer;

		// Token: 0x02000365 RID: 869
		[Serializable]
		public struct Vertex
		{
			// Token: 0x040015F7 RID: 5623
			public Transform vertexTransform;

			// Token: 0x040015F8 RID: 5624
			public Vector3 position;

			// Token: 0x040015F9 RID: 5625
			public Vector3 localVelocity;
		}
	}
}
