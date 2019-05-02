using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003ED RID: 1005
	public class SmoothTrailMesh : MonoBehaviour
	{
		// Token: 0x0600160F RID: 5647 RVA: 0x00075378 File Offset: 0x00073578
		private void Awake()
		{
			this.mesh = new Mesh();
			this.mesh.MarkDynamic();
			GameObject gameObject = new GameObject("SmoothTrailMeshRenderer");
			this.meshFilter = gameObject.AddComponent<MeshFilter>();
			this.meshFilter.mesh = this.mesh;
			this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
			this.meshRenderer.sharedMaterials = this.sharedMaterials;
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x000753E0 File Offset: 0x000735E0
		private void AddCurrentPoint()
		{
			float time = Time.time;
			Vector3 position = base.transform.position;
			Vector3 b = base.transform.up * this.width * 0.5f;
			this.pointsQueue.Enqueue(new SmoothTrailMesh.Point
			{
				vertex1 = position + b,
				vertex2 = position - b,
				time = time
			});
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x000109D2 File Offset: 0x0000EBD2
		private void OnEnable()
		{
			this.AddCurrentPoint();
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x000109DA File Offset: 0x0000EBDA
		private void OnDisable()
		{
			this.pointsQueue.Clear();
			this.mesh.Clear();
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x000109F2 File Offset: 0x0000EBF2
		private void OnDestroy()
		{
			if (this.meshFilter)
			{
				this.meshFilter.mesh = null;
				UnityEngine.Object.Destroy(this.meshFilter.gameObject);
			}
			UnityEngine.Object.Destroy(this.mesh);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00075458 File Offset: 0x00073658
		private void Simulate()
		{
			float time = Time.time;
			Vector3 position = base.transform.position;
			Vector3 b = base.transform.up * this.width * 0.5f;
			float num = time - this.previousTime;
			if (num > 0f)
			{
				float num2 = 1f / num;
				for (float num3 = this.previousTime; num3 <= time; num3 += this.timeStep)
				{
					float t = (num3 - this.previousTime) * num2;
					Vector3 a = Vector3.LerpUnclamped(this.previousPosition, position, t);
					Vector3 b2 = Vector3.SlerpUnclamped(this.previousUp, b, t);
					this.pointsQueue.Enqueue(new SmoothTrailMesh.Point
					{
						vertex1 = a + b2,
						vertex2 = a - b2,
						time = num3
					});
				}
			}
			float num4 = time - this.trailLifetime;
			while (this.pointsQueue.Count > 0 && this.pointsQueue.Peek().time < num4)
			{
				this.pointsQueue.Dequeue();
			}
			this.previousTime = time;
			this.previousPosition = position;
			this.previousUp = b;
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00010A28 File Offset: 0x0000EC28
		private void LateUpdate()
		{
			this.Simulate();
			this.GenerateMesh();
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x0007558C File Offset: 0x0007378C
		private void GenerateMesh()
		{
			Vector3[] array = new Vector3[this.pointsQueue.Count * 2];
			Vector2[] array2 = new Vector2[this.pointsQueue.Count * 2];
			Color[] array3 = new Color[this.pointsQueue.Count * 2];
			float num = 1f / (float)this.pointsQueue.Count;
			int num2 = 0;
			if (this.pointsQueue.Count > 0)
			{
				float time = this.pointsQueue.Peek().time;
				float time2 = Time.time;
				float num3 = time2 - time;
				float num4 = 1f / num3;
				foreach (SmoothTrailMesh.Point point in this.pointsQueue)
				{
					float num5 = (time2 - point.time) * num4;
					array[num2] = point.vertex1;
					array2[num2] = new Vector2(1f, num5);
					array3[num2] = new Color(1f, 1f, 1f, this.fadeVertexAlpha ? (1f - num5) : 1f);
					num2++;
					array[num2] = point.vertex2;
					array2[num2] = new Vector2(0f, num5);
					num2++;
				}
			}
			int num6 = this.pointsQueue.Count - 1;
			int[] array4 = new int[num6 * 2 * 3];
			int num7 = 0;
			int num8 = 0;
			for (int i = 0; i < num6; i++)
			{
				array4[num7] = num8;
				array4[num7 + 1] = num8 + 1;
				array4[num7 + 2] = num8 + 2;
				array4[num7 + 3] = num8 + 3;
				array4[num7 + 4] = num8 + 1;
				array4[num7 + 5] = num8 + 2;
				num7 += 6;
				num8 += 2;
			}
			this.mesh.Clear();
			this.mesh.vertices = array;
			this.mesh.uv = array2;
			this.mesh.triangles = array4;
			this.mesh.colors = array3;
			this.mesh.RecalculateBounds();
			this.mesh.UploadMeshData(false);
		}

		// Token: 0x04001948 RID: 6472
		private MeshFilter meshFilter;

		// Token: 0x04001949 RID: 6473
		private MeshRenderer meshRenderer;

		// Token: 0x0400194A RID: 6474
		private Mesh mesh;

		// Token: 0x0400194B RID: 6475
		public float timeStep = 0.00555555569f;

		// Token: 0x0400194C RID: 6476
		public float width = 1f;

		// Token: 0x0400194D RID: 6477
		public Material[] sharedMaterials;

		// Token: 0x0400194E RID: 6478
		public float trailLifetime = 1f;

		// Token: 0x0400194F RID: 6479
		public bool fadeVertexAlpha = true;

		// Token: 0x04001950 RID: 6480
		private Vector3 previousPosition;

		// Token: 0x04001951 RID: 6481
		private Vector3 previousUp;

		// Token: 0x04001952 RID: 6482
		private float previousTime;

		// Token: 0x04001953 RID: 6483
		private Queue<SmoothTrailMesh.Point> pointsQueue = new Queue<SmoothTrailMesh.Point>();

		// Token: 0x020003EE RID: 1006
		[Serializable]
		private struct Point
		{
			// Token: 0x04001954 RID: 6484
			public Vector3 vertex1;

			// Token: 0x04001955 RID: 6485
			public Vector3 vertex2;

			// Token: 0x04001956 RID: 6486
			public float time;
		}
	}
}
