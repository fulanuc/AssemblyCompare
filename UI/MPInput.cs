using System;
using Rewired;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005B4 RID: 1460
	public class MPInput : BaseInput, IMouseInputSource
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x00017F68 File Offset: 0x00016168
		protected override void Awake()
		{
			base.Awake();
			this.eventSystem = base.GetComponent<MPEventSystem>();
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x00017F7C File Offset: 0x0001617C
		private static int MouseButtonToAction(int button)
		{
			switch (button)
			{
			case 0:
				return 20;
			case 1:
				return 21;
			case 2:
				return 22;
			default:
				return -1;
			}
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x00017F9C File Offset: 0x0001619C
		public override bool GetMouseButtonDown(int button)
		{
			return this.player.GetButtonDown(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x00017FAF File Offset: 0x000161AF
		public override bool GetMouseButtonUp(int button)
		{
			return this.player.GetButtonUp(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x00017FC2 File Offset: 0x000161C2
		public override bool GetMouseButton(int button)
		{
			return this.player.GetButton(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x00017FD5 File Offset: 0x000161D5
		public void CenterCursor()
		{
			this.internalMousePosition = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x0009F048 File Offset: 0x0009D248
		public void Update()
		{
			if (!this.eventSystem.isCursorVisible)
			{
				return;
			}
			float num = (float)Screen.width;
			float num2 = (float)Screen.height;
			float num3 = Mathf.Min(num / 1920f, num2 / 1080f);
			this.internalScreenPositionDelta = Vector2.zero;
			if (this.eventSystem.currentInputSource == MPEventSystem.InputSource.Keyboard)
			{
				if (Application.isFocused)
				{
					this.internalMousePosition = Input.mousePosition;
				}
			}
			else
			{
				Vector2 a = new Vector2(this.player.GetAxis(23), this.player.GetAxis(24));
				float magnitude = a.magnitude;
				this.stickMagnitude = Mathf.Min(Mathf.MoveTowards(this.stickMagnitude, magnitude, this.cursorAcceleration * Time.unscaledDeltaTime), magnitude);
				float num4 = this.stickMagnitude;
				if (this.eventSystem.isHovering)
				{
					num4 *= this.cursorStickyModifier;
				}
				Vector2 a2 = (magnitude == 0f) ? Vector2.zero : (a * (num4 / magnitude));
				float d = 1920f * this.cursorScreenSpeed * num3;
				this.internalScreenPositionDelta = a2 * Time.unscaledDeltaTime * d;
				this.internalMousePosition += this.internalScreenPositionDelta;
			}
			this.internalMousePosition.x = Mathf.Clamp(this.internalMousePosition.x, 0f, num);
			this.internalMousePosition.y = Mathf.Clamp(this.internalMousePosition.y, 0f, num2);
			this._scrollDelta = new Vector2(0f, this.player.GetAxis(26));
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060020E9 RID: 8425 RVA: 0x00017FFA File Offset: 0x000161FA
		public override Vector2 mousePosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060020EA RID: 8426 RVA: 0x00018002 File Offset: 0x00016202
		public override Vector2 mouseScrollDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x0001800A File Offset: 0x0001620A
		public bool GetButtonDown(int button)
		{
			return this.GetMouseButtonDown(button);
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00018013 File Offset: 0x00016213
		public bool GetButtonUp(int button)
		{
			return this.GetMouseButtonUp(button);
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x0001801C File Offset: 0x0001621C
		public bool GetButton(int button)
		{
			return this.GetMouseButton(button);
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060020EE RID: 8430 RVA: 0x00018025 File Offset: 0x00016225
		public int playerId
		{
			get
			{
				return this.player.id;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060020EF RID: 8431 RVA: 0x00018032 File Offset: 0x00016232
		public bool locked
		{
			get
			{
				return !this.eventSystem.isCursorVisible;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060020F0 RID: 8432 RVA: 0x00003BDD File Offset: 0x00001DDD
		public int buttonCount
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x060020F1 RID: 8433 RVA: 0x00017FFA File Offset: 0x000161FA
		public Vector2 screenPosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x060020F2 RID: 8434 RVA: 0x00018042 File Offset: 0x00016242
		public Vector2 screenPositionDelta
		{
			get
			{
				return this.internalScreenPositionDelta;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x060020F3 RID: 8435 RVA: 0x00018002 File Offset: 0x00016202
		public Vector2 wheelDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x0001807A File Offset: 0x0001627A
		bool IMouseInputSource.get_enabled()
		{
			return base.enabled;
		}

		// Token: 0x04002334 RID: 9012
		public Player player;

		// Token: 0x04002335 RID: 9013
		private MPEventSystem eventSystem;

		// Token: 0x04002336 RID: 9014
		[FormerlySerializedAs("useAcceleration")]
		public bool useCursorAcceleration = true;

		// Token: 0x04002337 RID: 9015
		[FormerlySerializedAs("acceleration")]
		public float cursorAcceleration = 8f;

		// Token: 0x04002338 RID: 9016
		public float cursorStickyModifier = 0.333333343f;

		// Token: 0x04002339 RID: 9017
		public float cursorScreenSpeed = 0.75f;

		// Token: 0x0400233A RID: 9018
		private float stickMagnitude;

		// Token: 0x0400233B RID: 9019
		private Vector2 _scrollDelta;

		// Token: 0x0400233C RID: 9020
		private Vector2 internalScreenPositionDelta;

		// Token: 0x0400233D RID: 9021
		public Vector2 internalMousePosition;
	}
}
