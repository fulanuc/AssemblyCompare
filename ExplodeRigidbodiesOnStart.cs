using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F3 RID: 755
	public class ExplodeRigidbodiesOnStart : MonoBehaviour
	{
		// Token: 0x06000F51 RID: 3921 RVA: 0x0005C670 File Offset: 0x0005A870
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.bodies.Length; i++)
			{
				this.bodies[i].AddExplosionForce(this.force, position, this.explosionRadius);
			}
		}

		// Token: 0x0400135E RID: 4958
		public Rigidbody[] bodies;

		// Token: 0x0400135F RID: 4959
		public float force;

		// Token: 0x04001360 RID: 4960
		public float explosionRadius;
	}
}
