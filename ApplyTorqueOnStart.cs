using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025E RID: 606
	public class ApplyTorqueOnStart : MonoBehaviour
	{
		// Token: 0x06000B4A RID: 2890 RVA: 0x0004B9D8 File Offset: 0x00049BD8
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

		// Token: 0x04000F58 RID: 3928
		public Vector3 localTorque;

		// Token: 0x04000F59 RID: 3929
		public bool randomize;
	}
}
