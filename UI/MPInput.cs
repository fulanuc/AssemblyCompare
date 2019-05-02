using System;
using Rewired;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RoR2.UI
{
	// Token: 0x020005A2 RID: 1442
	public class MPInput : BaseInput, IMouseInputSource
	{
		// Token: 0x06002051 RID: 8273 RVA: 0x0001786E File Offset: 0x00015A6E
		protected override void Awake()
		{
			base.Awake();
			this.eventSystem = base.GetComponent<MPEventSystem>();
		}

		// Token: 0x06002052 RID: 8274 RVA: 0x00017882 File Offset: 0x00015A82
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

		// Token: 0x06002053 RID: 8275 RVA: 0x000178A2 File Offset: 0x00015AA2
		public override bool GetMouseButtonDown(int button)
		{
			return this.player.GetButtonDown(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x000178B5 File Offset: 0x00015AB5
		public override bool GetMouseButtonUp(int button)
		{
			return this.player.GetButtonUp(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000178C8 File Offset: 0x00015AC8
		public override bool GetMouseButton(int button)
		{
			return this.player.GetButton(MPInput.MouseButtonToAction(button));
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x000178DB File Offset: 0x00015ADB
		public void CenterCursor()
		{
			this.internalMousePosition = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x0009DA74 File Offset: 0x0009BC74
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

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06002058 RID: 8280 RVA: 0x00017900 File Offset: 0x00015B00
		public override Vector2 mousePosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06002059 RID: 8281 RVA: 0x00017908 File Offset: 0x00015B08
		public override Vector2 mouseScrollDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x00017910 File Offset: 0x00015B10
		public bool GetButtonDown(int button)
		{
			return this.GetMouseButtonDown(button);
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x00017919 File Offset: 0x00015B19
		public bool GetButtonUp(int button)
		{
			return this.GetMouseButtonUp(button);
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x00017922 File Offset: 0x00015B22
		public bool GetButton(int button)
		{
			return this.GetMouseButton(button);
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600205D RID: 8285 RVA: 0x0001792B File Offset: 0x00015B2B
		public int playerId
		{
			get
			{
				return this.player.id;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600205E RID: 8286 RVA: 0x00017938 File Offset: 0x00015B38
		public bool locked
		{
			get
			{
				return !this.eventSystem.isCursorVisible;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x00003BDD File Offset: 0x00001DDD
		public int buttonCount
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06002060 RID: 8288 RVA: 0x00017900 File Offset: 0x00015B00
		public Vector2 screenPosition
		{
			get
			{
				return this.internalMousePosition;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06002061 RID: 8289 RVA: 0x00017948 File Offset: 0x00015B48
		public Vector2 screenPositionDelta
		{
			get
			{
				return this.internalScreenPositionDelta;
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06002062 RID: 8290 RVA: 0x00017908 File Offset: 0x00015B08
		public Vector2 wheelDelta
		{
			get
			{
				return this._scrollDelta;
			}
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x00017980 File Offset: 0x00015B80
		bool IMouseInputSource.get_enabled()
		{
			return base.enabled;
		}

		// Token: 0x040022E0 RID: 8928
		public Player player;

		// Token: 0x040022E1 RID: 8929
		private MPEventSystem eventSystem;

		// Token: 0x040022E2 RID: 8930
		[FormerlySerializedAs("useAcceleration")]
		public bool useCursorAcceleration = true;

		// Token: 0x040022E3 RID: 8931
		[FormerlySerializedAs("acceleration")]
		public float cursorAcceleration = 8f;

		// Token: 0x040022E4 RID: 8932
		public float cursorStickyModifier = 0.333333343f;

		// Token: 0x040022E5 RID: 8933
		public float cursorScreenSpeed = 0.75f;

		// Token: 0x040022E6 RID: 8934
		private float stickMagnitude;

		// Token: 0x040022E7 RID: 8935
		private Vector2 _scrollDelta;

		// Token: 0x040022E8 RID: 8936
		private Vector2 internalScreenPositionDelta;

		// Token: 0x040022E9 RID: 8937
		public Vector2 internalMousePosition;
	}
}
