using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000302 RID: 770
	[ExecuteAlways]
	public class GlassesSize : MonoBehaviour
	{
		// Token: 0x06000FCF RID: 4047 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Start()
		{
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0000C28D File Offset: 0x0000A48D
		private void Update()
		{
			this.UpdateGlasses();
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0005E38C File Offset: 0x0005C58C
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

		// Token: 0x040013C9 RID: 5065
		public Transform glassesModelBase;

		// Token: 0x040013CA RID: 5066
		public Transform glassesBridgeLeft;

		// Token: 0x040013CB RID: 5067
		public Transform glassesBridgeRight;

		// Token: 0x040013CC RID: 5068
		public float bridgeOffsetScale;

		// Token: 0x040013CD RID: 5069
		public Vector3 offsetVector = Vector3.right;
	}
}
