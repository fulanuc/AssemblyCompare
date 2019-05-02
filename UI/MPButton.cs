using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000611 RID: 1553
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPButton : Button
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06002311 RID: 8977 RVA: 0x000198B0 File Offset: 0x00017AB0
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x000A8534 File Offset: 0x000A6734
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			Navigation navigation = base.navigation;
			navigation.mode = Navigation.Mode.None;
			base.navigation = navigation;
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000A856C File Offset: 0x000A676C
		private Selectable FilterSelectable(Selectable selectable)
		{
			if (selectable)
			{
				MPEventSystemLocator component = selectable.GetComponent<MPEventSystemLocator>();
				if (!component || component.eventSystem != this.eventSystemLocator.eventSystem)
				{
					selectable = null;
				}
			}
			return selectable;
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x000198BD File Offset: 0x00017ABD
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x000198CB File Offset: 0x00017ACB
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x000198D9 File Offset: 0x00017AD9
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x000198E7 File Offset: 0x00017AE7
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x000198F5 File Offset: 0x00017AF5
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x000A85AC File Offset: 0x000A67AC
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x000198FE File Offset: 0x00017AFE
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x0600231B RID: 8987 RVA: 0x00019937 File Offset: 0x00017B37
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			this.isPointerInside = true;
			base.OnPointerEnter(eventData);
			this.AttemptSelection(eventData);
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x000A85E0 File Offset: 0x000A67E0
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			if (this.eventSystem && base.gameObject == this.eventSystem.currentSelectedGameObject)
			{
				base.enabled = false;
				base.enabled = true;
			}
			this.isPointerInside = false;
			base.OnPointerExit(eventData);
		}

		// Token: 0x0600231D RID: 8989 RVA: 0x0001995D File Offset: 0x00017B5D
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x0600231E RID: 8990 RVA: 0x00019975 File Offset: 0x00017B75
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			this.inPointerUp = true;
			base.OnPointerUp(eventData);
			this.inPointerUp = false;
		}

		// Token: 0x0600231F RID: 8991 RVA: 0x0001999B File Offset: 0x00017B9B
		public override void OnSubmit(BaseEventData eventData)
		{
			if (this.pointerClickOnly && !this.inPointerUp)
			{
				return;
			}
			base.OnSubmit(eventData);
		}

		// Token: 0x06002320 RID: 8992 RVA: 0x000A8640 File Offset: 0x000A6840
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			if (this.IsInteractable() && base.navigation.mode != Navigation.Mode.None)
			{
				this.AttemptSelection(eventData);
			}
			base.OnPointerDown(eventData);
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x000199B5 File Offset: 0x00017BB5
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x040025F0 RID: 9712
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025F1 RID: 9713
		protected bool isPointerInside;

		// Token: 0x040025F2 RID: 9714
		public bool allowAllEventSystems;

		// Token: 0x040025F3 RID: 9715
		public bool pointerClickOnly;

		// Token: 0x040025F4 RID: 9716
		private bool inPointerUp;
	}
}
