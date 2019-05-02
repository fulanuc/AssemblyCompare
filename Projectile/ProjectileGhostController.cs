using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000558 RID: 1368
	public class ProjectileGhostController : MonoBehaviour
	{
		// Token: 0x06001EB5 RID: 7861 RVA: 0x000166E0 File Offset: 0x000148E0
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001EB6 RID: 7862 RVA: 0x0009612C File Offset: 0x0009432C
		private void Update()
		{
			if (this.authorityTransform ^ this.predictionTransform)
			{
				this.CopyTransform(this.authorityTransform ? this.authorityTransform : this.predictionTransform);
				return;
			}
			if (this.authorityTransform)
			{
				this.LerpTransform(this.predictionTransform, this.authorityTransform, this.migration);
				if (this.migration == 1f)
				{
					this.predictionTransform = null;
					return;
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x000166EE File Offset: 0x000148EE
		private void LerpTransform(Transform a, Transform b, float t)
		{
			this.transform.position = Vector3.LerpUnclamped(a.position, b.position, t);
			this.transform.rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t);
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x0001672A File Offset: 0x0001492A
		private void CopyTransform(Transform src)
		{
			this.transform.position = src.position;
			this.transform.rotation = src.rotation;
		}

		// Token: 0x040020F7 RID: 8439
		private new Transform transform;

		// Token: 0x040020F8 RID: 8440
		private float migration;

		// Token: 0x040020F9 RID: 8441
		[HideInInspector]
		public Transform authorityTransform;

		// Token: 0x040020FA RID: 8442
		[HideInInspector]
		public Transform predictionTransform;
	}
}
