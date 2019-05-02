using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200054C RID: 1356
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileCharacterController : MonoBehaviour
	{
		// Token: 0x06001E76 RID: 7798 RVA: 0x000163A1 File Offset: 0x000145A1
		private void Awake()
		{
			this.downVector = Vector3.down * 3f;
			this.characterController = base.GetComponent<CharacterController>();
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x000952DC File Offset: 0x000934DC
		private void FixedUpdate()
		{
			this.characterController.Move((base.transform.forward + this.downVector) * (this.velocity * Time.fixedDeltaTime));
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer > this.lifetime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x0400209D RID: 8349
		private Vector3 downVector;

		// Token: 0x0400209E RID: 8350
		public float velocity;

		// Token: 0x0400209F RID: 8351
		public float lifetime = 5f;

		// Token: 0x040020A0 RID: 8352
		private float timer;

		// Token: 0x040020A1 RID: 8353
		private CharacterController characterController;
	}
}
