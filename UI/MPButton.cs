using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FF RID: 1535
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPButton : Button
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06002281 RID: 8833 RVA: 0x000191F9 File Offset: 0x000173F9
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x000A6EB8 File Offset: 0x000A50B8
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			Navigation navigation = base.navigation;
			navigation.mode = Navigation.Mode.None;
			base.navigation = navigation;
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x000A6EF0 File Offset: 0x000A50F0
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

		// Token: 0x06002284 RID: 8836 RVA: 0x00019206 File Offset: 0x00017406
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x00019214 File Offset: 0x00017414
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x00019222 File Offset: 0x00017422
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x00019230 File Offset: 0x00017430
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x0001923E File Offset: 0x0001743E
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x000A6F30 File Offset: 0x000A5130
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x00019247 File Offset: 0x00017447
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x00019280 File Offset: 0x00017480
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

		// Token: 0x0600228C RID: 8844 RVA: 0x000A6F64 File Offset: 0x000A5164
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

		// Token: 0x0600228D RID: 8845 RVA: 0x000192A6 File Offset: 0x000174A6
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x000192BE File Offset: 0x000174BE
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

		// Token: 0x0600228F RID: 8847 RVA: 0x000192E4 File Offset: 0x000174E4
		public override void OnSubmit(BaseEventData eventData)
		{
			if (this.pointerClickOnly && !this.inPointerUp)
			{
				return;
			}
			base.OnSubmit(eventData);
		}

		// Token: 0x06002290 RID: 8848 RVA: 0x000A6FC4 File Offset: 0x000A51C4
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

		// Token: 0x06002291 RID: 8849 RVA: 0x000192FE File Offset: 0x000174FE
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x04002595 RID: 9621
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002596 RID: 9622
		protected bool isPointerInside;

		// Token: 0x04002597 RID: 9623
		public bool allowAllEventSystems;

		// Token: 0x04002598 RID: 9624
		public bool pointerClickOnly;

		// Token: 0x04002599 RID: 9625
		private bool inPointerUp;
	}
}
