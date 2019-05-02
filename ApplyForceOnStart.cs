using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025C RID: 604
	public class ApplyForceOnStart : MonoBehaviour
	{
		// Token: 0x06000B43 RID: 2883 RVA: 0x0004B744 File Offset: 0x00049944
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddRelativeForce(this.localForce);
			}
		}

		// Token: 0x04000F4D RID: 3917
		public Vector3 localForce;
	}
}
