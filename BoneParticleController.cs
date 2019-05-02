using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026E RID: 622
	public class BoneParticleController : MonoBehaviour
	{
		// Token: 0x06000BB0 RID: 2992 RVA: 0x0004C750 File Offset: 0x0004A950
		private void Start()
		{
			this.bonesList = new List<Transform>();
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name.IndexOf("IK", StringComparison.OrdinalIgnoreCase) == -1 && transform.name.IndexOf("Root", StringComparison.OrdinalIgnoreCase) == -1 && transform.name.IndexOf("Base", StringComparison.OrdinalIgnoreCase) == -1)
				{
					Debug.LogFormat("added bone {0}", new object[]
					{
						transform
					});
					this.bonesList.Add(transform);
				}
			}
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0004C7E4 File Offset: 0x0004A9E4
		private void Update()
		{
			if (this.skinnedMeshRenderer)
			{
				this.stopwatch += Time.deltaTime;
				if (this.stopwatch > 1f / this.spawnFrequency)
				{
					this.stopwatch -= 1f / this.spawnFrequency;
					int count = this.bonesList.Count;
					Transform transform = this.bonesList[UnityEngine.Random.Range(0, count)];
					if (transform)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.childParticlePrefab, transform.transform.position, Quaternion.identity, transform);
					}
				}
			}
		}

		// Token: 0x04000F96 RID: 3990
		public GameObject childParticlePrefab;

		// Token: 0x04000F97 RID: 3991
		public float spawnFrequency;

		// Token: 0x04000F98 RID: 3992
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04000F99 RID: 3993
		private float stopwatch;

		// Token: 0x04000F9A RID: 3994
		private List<Transform> bonesList;
	}
}
