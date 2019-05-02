using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025D RID: 605
	public class ApplyJiggleBoneMotion : MonoBehaviour
	{
		// Token: 0x06000B45 RID: 2885 RVA: 0x0004B76C File Offset: 0x0004996C
		private void FixedUpdate()
		{
			Vector3 position = this.rootTransform.position;
			Rigidbody[] array = this.rigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddForce((this.lastRootPosition - position) * this.forceScale * Time.fixedDeltaTime);
			}
			this.lastRootPosition = position;
		}

		// Token: 0x04000F4E RID: 3918
		public float forceScale = 100f;

		// Token: 0x04000F4F RID: 3919
		public Transform rootTransform;

		// Token: 0x04000F50 RID: 3920
		public Rigidbody[] rigidbodies;

		// Token: 0x04000F51 RID: 3921
		private Vector3 lastRootPosition;
	}
}
