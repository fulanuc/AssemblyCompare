using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025E RID: 606
	public class ApplyTorqueOnStart : MonoBehaviour
	{
		// Token: 0x06000B47 RID: 2887 RVA: 0x0004B7CC File Offset: 0x000499CC
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				Vector3 vector = this.localTorque;
				if (this.randomize)
				{
					vector.x = UnityEngine.Random.Range(-vector.x / 2f, vector.x / 2f);
					vector.y = UnityEngine.Random.Range(-vector.y / 2f, vector.y / 2f);
					vector.z = UnityEngine.Random.Range(-vector.z / 2f, vector.z / 2f);
				}
				component.AddRelativeTorque(vector);
			}
		}

		// Token: 0x04000F52 RID: 3922
		public Vector3 localTorque;

		// Token: 0x04000F53 RID: 3923
		public bool randomize;
	}
}
