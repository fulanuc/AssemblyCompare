using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024B RID: 587
	public class AddCurvedTorque : MonoBehaviour
	{
		// Token: 0x06000B08 RID: 2824 RVA: 0x0004AA34 File Offset: 0x00048C34
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

		// Token: 0x04000EF2 RID: 3826
		public AnimationCurve torqueCurve;

		// Token: 0x04000EF3 RID: 3827
		public Vector3 localTorqueVector;

		// Token: 0x04000EF4 RID: 3828
		public float lifetime;

		// Token: 0x04000EF5 RID: 3829
		public Rigidbody[] rigidbodies;

		// Token: 0x04000EF6 RID: 3830
		private float stopwatch;
	}
}
