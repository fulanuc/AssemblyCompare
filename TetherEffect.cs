using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000406 RID: 1030
	[RequireComponent(typeof(BezierCurveLine))]
	public class TetherEffect : MonoBehaviour
	{
		// Token: 0x06001718 RID: 5912 RVA: 0x0001164E File Offset: 0x0000F84E
		private void Start()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x00078BD4 File Offset: 0x00076DD4
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

		// Token: 0x04001A16 RID: 6678
		public float tetherMaxDistance;

		// Token: 0x04001A17 RID: 6679
		public Transform tetherEndTransform;

		// Token: 0x04001A18 RID: 6680
		public AnimateShaderAlpha fadeOut;

		// Token: 0x04001A19 RID: 6681
		private BezierCurveLine bezierCurveLine;
	}
}
