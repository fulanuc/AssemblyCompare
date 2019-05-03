using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000400 RID: 1024
	[RequireComponent(typeof(BezierCurveLine))]
	public class TetherEffect : MonoBehaviour
	{
		// Token: 0x060016D8 RID: 5848 RVA: 0x00011229 File Offset: 0x0000F429
		private void Start()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00078648 File Offset: 0x00076848
		private void Update()
		{
			bool flag = true;
			if (this.tetherEndTransform)
			{
				flag = false;
				this.bezierCurveLine.endTransform = this.tetherEndTransform;
				if ((this.tetherEndTransform.position - base.transform.position).sqrMagnitude > this.tetherMaxDistance * this.tetherMaxDistance)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (this.fadeOut)
				{
					this.fadeOut.enabled = true;
					return;
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x040019ED RID: 6637
		public float tetherMaxDistance;

		// Token: 0x040019EE RID: 6638
		public Transform tetherEndTransform;

		// Token: 0x040019EF RID: 6639
		public AnimateShaderAlpha fadeOut;

		// Token: 0x040019F0 RID: 6640
		private BezierCurveLine bezierCurveLine;
	}
}
