using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053D RID: 1341
	[RequireComponent(typeof(CharacterController))]
	public class ProjectileCharacterController : MonoBehaviour
	{
		// Token: 0x06001E0C RID: 7692 RVA: 0x00015EC2 File Offset: 0x000140C2
		private void Awake()
		{
			this.downVector = Vector3.down * 3f;
			this.characterController = base.GetComponent<CharacterController>();
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x000945C0 File Offset: 0x000927C0
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

		// Token: 0x0400205F RID: 8287
		private Vector3 downVector;

		// Token: 0x04002060 RID: 8288
		public float velocity;

		// Token: 0x04002061 RID: 8289
		public float lifetime = 5f;

		// Token: 0x04002062 RID: 8290
		private float timer;

		// Token: 0x04002063 RID: 8291
		private CharacterController characterController;
	}
}
