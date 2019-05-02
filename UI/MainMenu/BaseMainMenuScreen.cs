using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000671 RID: 1649
	[RequireComponent(typeof(RectTransform))]
	public class BaseMainMenuScreen : MonoBehaviour
	{
		// Token: 0x060024FB RID: 9467 RVA: 0x000038B4 File Offset: 0x00001AB4
		public virtual bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x060024FC RID: 9468 RVA: 0x0001AF38 File Offset: 0x00019138
		public virtual void OnEnter(MainMenuController mainMenuController)
		{
			this.myMainMenuController = mainMenuController;
			this.onEnter.Invoke();
		}

		// Token: 0x060024FD RID: 9469 RVA: 0x0001AF4C File Offset: 0x0001914C
		public virtual void OnExit(MainMenuController mainMenuController)
		{
			if (this.myMainMenuController == mainMenuController)
			{
				this.myMainMenuController = null;
			}
			this.onExit.Invoke();
		}

		// Token: 0x040027B3 RID: 10163
		public Transform desiredCameraTransform;

		// Token: 0x040027B4 RID: 10164
		[HideInInspector]
		public bool shouldDisplay;

		// Token: 0x040027B5 RID: 10165
		protected MainMenuController myMainMenuController;

		// Token: 0x040027B6 RID: 10166
		public UnityEvent onEnter;

		// Token: 0x040027B7 RID: 10167
		public UnityEvent onExit;
	}
}
