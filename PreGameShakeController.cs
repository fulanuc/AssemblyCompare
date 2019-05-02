using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039D RID: 925
	public class PreGameShakeController : MonoBehaviour
	{
		// Token: 0x06001394 RID: 5012 RVA: 0x0000EFB0 File Offset: 0x0000D1B0
		private void ResetTimer()
		{
			this.timer = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0006D060 File Offset: 0x0006B260
		private void DoShake()
		{
			this.shakeEmitter.StartShake();
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			foreach (Rigidbody rigidbody in this.physicsBodies)
			{
				if (rigidbody)
				{
					Vector3 force = onUnitSphere * ((0.75f + UnityEngine.Random.value * 0.25f) * this.physicsForce);
					float y = rigidbody.GetComponent<Collider>().bounds.min.y;
					Vector3 centerOfMass = rigidbody.centerOfMass;
					centerOfMass.y = y;
					rigidbody.AddForceAtPosition(force, centerOfMass);
				}
			}
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x0000EFC9 File Offset: 0x0000D1C9
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0000EFD1 File Offset: 0x0000D1D1
		private void Update()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.ResetTimer();
				this.DoShake();
			}
		}

		// Token: 0x0400171B RID: 5915
		public ShakeEmitter shakeEmitter;

		// Token: 0x0400171C RID: 5916
		public float minInterval = 0.5f;

		// Token: 0x0400171D RID: 5917
		public float maxInterval = 7f;

		// Token: 0x0400171E RID: 5918
		public Rigidbody[] physicsBodies;

		// Token: 0x0400171F RID: 5919
		public float physicsForce;

		// Token: 0x04001720 RID: 5920
		private float timer;
	}
}
