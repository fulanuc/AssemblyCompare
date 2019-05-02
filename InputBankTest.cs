using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000338 RID: 824
	[RequireComponent(typeof(CharacterBody))]
	public class InputBankTest : MonoBehaviour
	{
		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x0000CE46 File Offset: 0x0000B046
		// (set) Token: 0x060010EA RID: 4330 RVA: 0x0000CE6C File Offset: 0x0000B06C
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

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x0000CE7B File Offset: 0x0000B07B
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

		// Token: 0x060010EC RID: 4332 RVA: 0x0000CEAB File Offset: 0x0000B0AB
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x00064428 File Offset: 0x00062628
		[ConCommand(commandName = "debug_draw_aim_rays", flags = ConVarFlags.Cheat, helpText = "Draws all aim rays for 20 seconds")]
		private static void CCDebugDrawAimRays(ConCommandArgs args)
		{
			foreach (InputBankTest inputBankTest in UnityEngine.Object.FindObjectsOfType<InputBankTest>())
			{
				Debug.DrawRay(inputBankTest.aimOrigin, inputBankTest.aimDirection * 100f, Color.cyan, 20f);
			}
		}

		// Token: 0x04001511 RID: 5393
		private CharacterBody characterBody;

		// Token: 0x04001512 RID: 5394
		public float lookPitch;

		// Token: 0x04001513 RID: 5395
		public float lookYaw;

		// Token: 0x04001514 RID: 5396
		private Vector3 _aimDirection;

		// Token: 0x04001515 RID: 5397
		public Vector3 moveVector;

		// Token: 0x04001516 RID: 5398
		public InputBankTest.ButtonState skill1;

		// Token: 0x04001517 RID: 5399
		public InputBankTest.ButtonState skill2;

		// Token: 0x04001518 RID: 5400
		public InputBankTest.ButtonState skill3;

		// Token: 0x04001519 RID: 5401
		public InputBankTest.ButtonState skill4;

		// Token: 0x0400151A RID: 5402
		public InputBankTest.ButtonState interact;

		// Token: 0x0400151B RID: 5403
		public InputBankTest.ButtonState jump;

		// Token: 0x0400151C RID: 5404
		public InputBankTest.ButtonState sprint;

		// Token: 0x0400151D RID: 5405
		public InputBankTest.ButtonState activateEquipment;

		// Token: 0x0400151E RID: 5406
		public InputBankTest.ButtonState ping;

		// Token: 0x0400151F RID: 5407
		[NonSerialized]
		public int emoteRequest = -1;

		// Token: 0x02000339 RID: 825
		public struct ButtonState
		{
			// Token: 0x17000174 RID: 372
			// (get) Token: 0x060010EF RID: 4335 RVA: 0x0000CEC8 File Offset: 0x0000B0C8
			public bool justReleased
			{
				get
				{
					return !this.down && this.wasDown;
				}
			}

			// Token: 0x17000175 RID: 373
			// (get) Token: 0x060010F0 RID: 4336 RVA: 0x0000CEDA File Offset: 0x0000B0DA
			public bool justPressed
			{
				get
				{
					return this.down && !this.wasDown;
				}
			}

			// Token: 0x060010F1 RID: 4337 RVA: 0x0000CEEF File Offset: 0x0000B0EF
			public void PushState(bool newState)
			{
				this.wasDown = this.down;
				this.down = newState;
			}

			// Token: 0x04001520 RID: 5408
			public bool down;

			// Token: 0x04001521 RID: 5409
			public bool wasDown;
		}
	}
}
