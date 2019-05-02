using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000549 RID: 1353
	public class ProjectileGhostController : MonoBehaviour
	{
		// Token: 0x06001E4B RID: 7755 RVA: 0x00016201 File Offset: 0x00014401
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001E4C RID: 7756 RVA: 0x00095410 File Offset: 0x00093610
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

		// Token: 0x06001E4D RID: 7757 RVA: 0x0001620F File Offset: 0x0001440F
		private void LerpTransform(Transform a, Transform b, float t)
		{
			this.transform.position = Vector3.LerpUnclamped(a.position, b.position, t);
			this.transform.rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t);
		}

		// Token: 0x06001E4E RID: 7758 RVA: 0x0001624B File Offset: 0x0001444B
		private void CopyTransform(Transform src)
		{
			this.transform.position = src.position;
			this.transform.rotation = src.rotation;
		}

		// Token: 0x040020B9 RID: 8377
		private new Transform transform;

		// Token: 0x040020BA RID: 8378
		private float migration;

		// Token: 0x040020BB RID: 8379
		[HideInInspector]
		public Transform authorityTransform;

		// Token: 0x040020BC RID: 8380
		[HideInInspector]
		public Transform predictionTransform;
	}
}
