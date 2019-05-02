using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024B RID: 587
	public class AddCurvedTorque : MonoBehaviour
	{
		// Token: 0x06000B05 RID: 2821 RVA: 0x0004A828 File Offset: 0x00048A28
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			float d = this.torqueCurve.Evaluate(this.stopwatch / this.lifetime);
			Rigidbody[] array = this.rigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddRelativeTorque(this.localTorqueVector * d);
			}
		}

		// Token: 0x04000EEC RID: 3820
		public AnimationCurve torqueCurve;

		// Token: 0x04000EED RID: 3821
		public Vector3 localTorqueVector;

		// Token: 0x04000EEE RID: 3822
		public float lifetime;

		// Token: 0x04000EEF RID: 3823
		public Rigidbody[] rigidbodies;

		// Token: 0x04000EF0 RID: 3824
		private float stopwatch;
	}
}
