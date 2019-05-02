using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000305 RID: 773
	[ExecuteAlways]
	public class GlassesSize : MonoBehaviour
	{
		// Token: 0x06000FE5 RID: 4069 RVA: 0x000025DA File Offset: 0x000007DA
		private void Start()
		{
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x0000C377 File Offset: 0x0000A577
		private void Update()
		{
			this.UpdateGlasses();
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x0005E610 File Offset: 0x0005C810
		private void UpdateGlasses()
		{
			Vector3 localScale = base.transform.localScale;
			float num = Mathf.Max(localScale.y, localScale.z);
			Vector3 localScale2 = new Vector3(1f / localScale.x * num, 1f / localScale.y * num, 1f / localScale.z * num);
			if (this.glassesModelBase)
			{
				this.glassesModelBase.transform.localScale = localScale2;
			}
			if (this.glassesBridgeLeft && this.glassesBridgeRight)
			{
				float num2 = (localScale.x / num - 1f) * this.bridgeOffsetScale;
				this.glassesBridgeLeft.transform.localPosition = this.offsetVector * -num2;
				this.glassesBridgeRight.transform.localPosition = this.offsetVector * num2;
			}
		}

		// Token: 0x040013E1 RID: 5089
		public Transform glassesModelBase;

		// Token: 0x040013E2 RID: 5090
		public Transform glassesBridgeLeft;

		// Token: 0x040013E3 RID: 5091
		public Transform glassesBridgeRight;

		// Token: 0x040013E4 RID: 5092
		public float bridgeOffsetScale;

		// Token: 0x040013E5 RID: 5093
		public Vector3 offsetVector = Vector3.right;
	}
}
