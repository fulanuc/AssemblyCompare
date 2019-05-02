using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E8 RID: 1512
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingControl : MonoBehaviour
	{
		// Token: 0x060021EC RID: 8684 RVA: 0x000A47C4 File Offset: 0x000A29C4
		public void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.bindingDisplay.actionName = this.actionName;
			this.bindingDisplay.useExplicitInputSource = true;
			this.bindingDisplay.explicitInputSource = this.inputSource;
			this.bindingDisplay.axisRange = this.axisRange;
			this.nameLabel.token = InputCatalog.GetActionNameToken(this.actionName, this.axisRange);
			this.action = ReInput.mapping.GetAction(this.actionName);
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060021ED RID: 8685 RVA: 0x00018B25 File Offset: 0x00016D25
		private bool isListening
		{
			get
			{
				return this.inputMapperHelper.isListening;
			}
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x00018B32 File Offset: 0x00016D32
		public void ToggleListening()
		{
			if (!this.isListening)
			{
				this.StartListening();
				return;
			}
			this.StopListening();
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x000A4850 File Offset: 0x000A2A50
		public void StartListening()
		{
			this.inputMapperHelper.Stop();
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			Player player;
			if (eventSystem == null)
			{
				player = null;
			}
			else
			{
				LocalUser localUser = eventSystem.localUser;
				player = ((localUser != null) ? localUser.inputPlayer : null);
			}
			this.currentPlayer = player;
			if (this.currentPlayer == null)
			{
				return;
			}
			IList<Controller> controllers = null;
			MPEventSystem.InputSource inputSource = this.inputSource;
			if (inputSource != MPEventSystem.InputSource.Keyboard)
			{
				if (inputSource == MPEventSystem.InputSource.Gamepad)
				{
					controllers = this.currentPlayer.controllers.Joysticks.ToArray<Joystick>();
				}
			}
			else
			{
				controllers = new Controller[]
				{
					this.currentPlayer.controllers.Keyboard,
					this.currentPlayer.controllers.Mouse
				};
			}
			this.inputMapperHelper.Start(this.currentPlayer, controllers, this.action, this.axisRange);
			if (this.button)
			{
				this.button.interactable = false;
			}
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x00018B49 File Offset: 0x00016D49
		private void StopListening()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.currentPlayer = null;
			this.inputMapperHelper.Stop();
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x00018B66 File Offset: 0x00016D66
		private void OnEnable()
		{
			if (!this.eventSystemLocator.eventSystem)
			{
				base.enabled = false;
				return;
			}
			this.inputMapperHelper = this.eventSystemLocator.eventSystem.inputMapperHelper;
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x00018B98 File Offset: 0x00016D98
		private void OnDisable()
		{
			this.StopListening();
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x000A4928 File Offset: 0x000A2B28
		private void Update()
		{
			if (this.button)
			{
				bool flag = !this.eventSystemLocator.eventSystem.inputMapperHelper.isListening;
				if (!flag)
				{
					this.buttonReactivationTime = Time.unscaledTime + 0.25f;
				}
				this.button.interactable = (flag && this.buttonReactivationTime <= Time.unscaledTime);
			}
		}

		// Token: 0x040024E6 RID: 9446
		public string actionName;

		// Token: 0x040024E7 RID: 9447
		public AxisRange axisRange;

		// Token: 0x040024E8 RID: 9448
		public LanguageTextMeshController nameLabel;

		// Token: 0x040024E9 RID: 9449
		public InputBindingDisplayController bindingDisplay;

		// Token: 0x040024EA RID: 9450
		public MPEventSystem.InputSource inputSource;

		// Token: 0x040024EB RID: 9451
		public MPButton button;

		// Token: 0x040024EC RID: 9452
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040024ED RID: 9453
		private InputAction action;

		// Token: 0x040024EE RID: 9454
		private InputMapperHelper inputMapperHelper;

		// Token: 0x040024EF RID: 9455
		private Player currentPlayer;

		// Token: 0x040024F0 RID: 9456
		private float buttonReactivationTime;
	}
}
