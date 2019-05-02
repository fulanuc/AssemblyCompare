using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065F RID: 1631
	[RequireComponent(typeof(RectTransform))]
	public class BaseMainMenuScreen : MonoBehaviour
	{
		// Token: 0x06002464 RID: 9316 RVA: 0x000038B4 File Offset: 0x00001AB4
		public virtual bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x06002465 RID: 9317 RVA: 0x0001A805 File Offset: 0x00018A05
		public virtual void OnEnter(MainMenuController mainMenuController)
		{
			this.myMainMenuController = mainMenuController;
			this.onEnter.Invoke();
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x0001A819 File Offset: 0x00018A19
		public virtual void OnExit(MainMenuController mainMenuController)
		{
			if (this.myMainMenuController == mainMenuController)
			{
				this.myMainMenuController = null;
			}
			this.onExit.Invoke();
		}

		// Token: 0x04002757 RID: 10071
		public Transform desiredCameraTransform;

		// Token: 0x04002758 RID: 10072
		[HideInInspector]
		public bool shouldDisplay;

		// Token: 0x04002759 RID: 10073
		protected MainMenuController myMainMenuController;

		// Token: 0x0400275A RID: 10074
		public UnityEvent onEnter;

		// Token: 0x0400275B RID: 10075
		public UnityEvent onExit;
	}
}
