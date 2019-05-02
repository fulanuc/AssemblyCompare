using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026E RID: 622
	public class BoneParticleController : MonoBehaviour
	{
		// Token: 0x06000BA7 RID: 2983 RVA: 0x0004C544 File Offset: 0x0004A744
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

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0004C5D8 File Offset: 0x0004A7D8
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

		// Token: 0x04000F90 RID: 3984
		public GameObject childParticlePrefab;

		// Token: 0x04000F91 RID: 3985
		public float spawnFrequency;

		// Token: 0x04000F92 RID: 3986
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04000F93 RID: 3987
		private float stopwatch;

		// Token: 0x04000F94 RID: 3988
		private List<Transform> bonesList;
	}
}
