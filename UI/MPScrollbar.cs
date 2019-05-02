using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061B RID: 1563
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPScrollbar : Scrollbar
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x0600236F RID: 9071 RVA: 0x00019D62 File Offset: 0x00017F62
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x00019D6F File Offset: 0x00017F6F
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x000A9194 File Offset: 0x000A7394
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

		// Token: 0x06002372 RID: 9074 RVA: 0x000A91D4 File Offset: 0x000A73D4
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x00019D83 File Offset: 0x00017F83
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x00019D91 File Offset: 0x00017F91
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x00019D9F File Offset: 0x00017F9F
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x00019DAD File Offset: 0x00017FAD
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x000A9208 File Offset: 0x000A7408
		public override void OnPointerUp(PointerEventData eventData)
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
			base.OnPointerUp(eventData);
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x00019DBB File Offset: 0x00017FBB
		public override void OnSelect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnSelect(eventData);
		}

		// Token: 0x06002379 RID: 9081 RVA: 0x00019DD3 File Offset: 0x00017FD3
		public override void OnDeselect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnDeselect(eventData);
		}

		// Token: 0x0600237A RID: 9082 RVA: 0x00019DEB File Offset: 0x00017FEB
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerEnter(eventData);
		}

		// Token: 0x0600237B RID: 9083 RVA: 0x00019E03 File Offset: 0x00018003
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerExit(eventData);
		}

		// Token: 0x0600237C RID: 9084 RVA: 0x000A9260 File Offset: 0x000A7460
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (this.IsInteractable() && base.navigation.mode != Navigation.Mode.None)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
			base.OnPointerDown(eventData);
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x00019E1B File Offset: 0x0001801B
		public override void Select()
		{
			if (this.eventSystem.alreadySelecting)
			{
				return;
			}
			this.eventSystem.SetSelectedGameObject(base.gameObject);
		}

		// Token: 0x04002620 RID: 9760
		public bool allowAllEventSystems;

		// Token: 0x04002621 RID: 9761
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002622 RID: 9762
		private RectTransform viewPortRectTransform;
	}
}
