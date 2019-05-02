using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CE RID: 718
	public class DestroyOnTimer : MonoBehaviour
	{
		// Token: 0x06000E80 RID: 3712 RVA: 0x0000B2F8 File Offset: 0x000094F8
		private void FixedUpdate()
		{
			this.age += Time.fixedDeltaTime;
			if (this.age > this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x0000B325 File Offset: 0x00009525
		private void OnDisable()
		{
			if (this.resetAgeOnDisable)
			{
				this.age = 0f;
			}
		}

		// Token: 0x04001278 RID: 4728
		public float duration;

		// Token: 0x04001279 RID: 4729
		public bool resetAgeOnDisable;

		// Token: 0x0400127A RID: 4730
		private float age;
	}
}
