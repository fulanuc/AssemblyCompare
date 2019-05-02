using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200033A RID: 826
	[RequireComponent(typeof(CharacterBody))]
	public class InputBankTest : MonoBehaviour
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060010FE RID: 4350 RVA: 0x0000CF2F File Offset: 0x0000B12F
		// (set) Token: 0x060010FF RID: 4351 RVA: 0x0000CF55 File Offset: 0x0000B155
		public Vector3 aimDirection
		{
			get
			{
				if (!(this._aimDirection != Vector3.zero))
				{
					return base.transform.forward;
				}
				return this._aimDirection;
			}
			set
			{
				this._aimDirection = value.normalized;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001100 RID: 4352 RVA: 0x0000CF64 File Offset: 0x0000B164
		public Vector3 aimOrigin
		{
			get
			{
				if (!this.characterBody.aimOriginTransform)
				{
					return base.transform.position;
				}
				return this.characterBody.aimOriginTransform.position;
			}
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0000CF94 File Offset: 0x0000B194
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x04001525 RID: 5413
		private CharacterBody characterBody;

		// Token: 0x04001526 RID: 5414
		public float lookPitch;

		// Token: 0x04001527 RID: 5415
		public float lookYaw;

		// Token: 0x04001528 RID: 5416
		private Vector3 _aimDirection;

		// Token: 0x04001529 RID: 5417
		public Vector3 moveVector;

		// Token: 0x0400152A RID: 5418
		public InputBankTest.ButtonState skill1;

		// Token: 0x0400152B RID: 5419
		public InputBankTest.ButtonState skill2;

		// Token: 0x0400152C RID: 5420
		public InputBankTest.ButtonState skill3;

		// Token: 0x0400152D RID: 5421
		public InputBankTest.ButtonState skill4;

		// Token: 0x0400152E RID: 5422
		public InputBankTest.ButtonState interact;

		// Token: 0x0400152F RID: 5423
		public InputBankTest.ButtonState jump;

		// Token: 0x04001530 RID: 5424
		public InputBankTest.ButtonState sprint;

		// Token: 0x04001531 RID: 5425
		public InputBankTest.ButtonState activateEquipment;

		// Token: 0x04001532 RID: 5426
		public InputBankTest.ButtonState ping;

		// Token: 0x04001533 RID: 5427
		[NonSerialized]
		public int emoteRequest = -1;

		// Token: 0x0200033B RID: 827
		public struct ButtonState
		{
			// Token: 0x17000179 RID: 377
			// (get) Token: 0x06001103 RID: 4355 RVA: 0x0000CFB1 File Offset: 0x0000B1B1
			public bool justReleased
			{
				get
				{
					return !this.down && this.wasDown;
				}
			}

			// Token: 0x1700017A RID: 378
			// (get) Token: 0x06001104 RID: 4356 RVA: 0x0000CFC3 File Offset: 0x0000B1C3
			public bool justPressed
			{
				get
				{
					return this.down && !this.wasDown;
				}
			}

			// Token: 0x06001105 RID: 4357 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
			public void PushState(bool newState)
			{
				this.wasDown = this.down;
				this.down = newState;
			}

			// Token: 0x04001534 RID: 5428
			public bool down;

			// Token: 0x04001535 RID: 5429
			public bool wasDown;
		}
	}
}
