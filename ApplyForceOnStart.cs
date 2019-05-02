using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025C RID: 604
	public class ApplyForceOnStart : MonoBehaviour
	{
		// Token: 0x06000B46 RID: 2886 RVA: 0x0004B950 File Offset: 0x00049B50
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddRelativeForce(this.localForce);
			}
		}

		// Token: 0x04000F53 RID: 3923
		public Vector3 localForce;
	}
}
