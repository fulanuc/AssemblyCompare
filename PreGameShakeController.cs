using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000398 RID: 920
	public class PreGameShakeController : MonoBehaviour
	{
		// Token: 0x06001377 RID: 4983 RVA: 0x0000EDF3 File Offset: 0x0000CFF3
		private void ResetTimer()
		{
			this.timer = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x0006CE48 File Offset: 0x0006B048
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

		// Token: 0x06001379 RID: 4985 RVA: 0x0000EE0C File Offset: 0x0000D00C
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x0000EE14 File Offset: 0x0000D014
		private void Update()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.ResetTimer();
				this.DoShake();
			}
		}

		// Token: 0x040016FF RID: 5887
		public ShakeEmitter shakeEmitter;

		// Token: 0x04001700 RID: 5888
		public float minInterval = 0.5f;

		// Token: 0x04001701 RID: 5889
		public float maxInterval = 7f;

		// Token: 0x04001702 RID: 5890
		public Rigidbody[] physicsBodies;

		// Token: 0x04001703 RID: 5891
		public float physicsForce;

		// Token: 0x04001704 RID: 5892
		private float timer;
	}
}
