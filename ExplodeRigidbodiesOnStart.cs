using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F0 RID: 752
	public class ExplodeRigidbodiesOnStart : MonoBehaviour
	{
		// Token: 0x06000F41 RID: 3905 RVA: 0x0005C450 File Offset: 0x0005A650
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.bodies.Length; i++)
			{
				this.bodies[i].AddExplosionForce(this.force, position, this.explosionRadius);
			}
		}

		// Token: 0x04001347 RID: 4935
		public Rigidbody[] bodies;

		// Token: 0x04001348 RID: 4936
		public float force;

		// Token: 0x04001349 RID: 4937
		public float explosionRadius;
	}
}
