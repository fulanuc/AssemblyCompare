using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FA RID: 1530
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class InputBindingControl : MonoBehaviour
	{
		// Token: 0x0600227D RID: 8829 RVA: 0x000A5D78 File Offset: 0x000A3F78
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

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600227E RID: 8830 RVA: 0x0001921F File Offset: 0x0001741F
		private bool isListening
		{
			get
			{
				return this.inputMapperHelper.isListening;
			}
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x0001922C File Offset: 0x0001742C
		public void ToggleListening()
		{
			if (!this.isListening)
			{
				this.StartListening();
				return;
			}
			this.StopListening();
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x000A5E04 File Offset: 0x000A4004
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

		// Token: 0x06002281 RID: 8833 RVA: 0x00019243 File Offset: 0x00017443
		private void StopListening()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.currentPlayer = null;
			this.inputMapperHelper.Stop();
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x00019260 File Offset: 0x00017460
		private void OnEnable()
		{
			if (!this.eventSystemLocator.eventSystem)
			{
				base.enabled = false;
				return;
			}
			this.inputMapperHelper = this.eventSystemLocator.eventSystem.inputMapperHelper;
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x00019292 File Offset: 0x00017492
		private void OnDisable()
		{
			this.StopListening();
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x000A5EDC File Offset: 0x000A40DC
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

		// Token: 0x0400253B RID: 9531
		public string actionName;

		// Token: 0x0400253C RID: 9532
		public AxisRange axisRange;

		// Token: 0x0400253D RID: 9533
		public LanguageTextMeshController nameLabel;

		// Token: 0x0400253E RID: 9534
		public InputBindingDisplayController bindingDisplay;

		// Token: 0x0400253F RID: 9535
		public MPEventSystem.InputSource inputSource;

		// Token: 0x04002540 RID: 9536
		public MPButton button;

		// Token: 0x04002541 RID: 9537
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002542 RID: 9538
		private InputAction action;

		// Token: 0x04002543 RID: 9539
		private InputMapperHelper inputMapperHelper;

		// Token: 0x04002544 RID: 9540
		private Player currentPlayer;

		// Token: 0x04002545 RID: 9541
		private float buttonReactivationTime;
	}
}
