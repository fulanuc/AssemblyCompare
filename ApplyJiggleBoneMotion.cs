using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025D RID: 605
	public class ApplyJiggleBoneMotion : MonoBehaviour
	{
		// Token: 0x06000B48 RID: 2888 RVA: 0x0004B978 File Offset: 0x00049B78
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

		// Token: 0x04000F54 RID: 3924
		public float forceScale = 100f;

		// Token: 0x04000F55 RID: 3925
		public Transform rootTransform;

		// Token: 0x04000F56 RID: 3926
		public Rigidbody[] rigidbodies;

		// Token: 0x04000F57 RID: 3927
		private Vector3 lastRootPosition;
	}
}
