using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D7 RID: 727
	public class DisableCollisionsBetweenColliders : MonoBehaviour
	{
		// Token: 0x06000E96 RID: 3734 RVA: 0x0005950C File Offset: 0x0005770C
		public void Awake()
		{
			foreach (Collider collider in this.collidersA)
			{
				foreach (Collider collider2 in this.collidersB)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
		}

		// Token: 0x0400129C RID: 4764
		public Collider[] collidersA;

		// Token: 0x0400129D RID: 4765
		public Collider[] collidersB;
	}
}
