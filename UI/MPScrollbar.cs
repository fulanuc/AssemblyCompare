using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000609 RID: 1545
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPScrollbar : Scrollbar
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060022DF RID: 8927 RVA: 0x000196AB File Offset: 0x000178AB
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x000196B8 File Offset: 0x000178B8
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x000A7B18 File Offset: 0x000A5D18
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

		// Token: 0x060022E2 RID: 8930 RVA: 0x000A7B58 File Offset: 0x000A5D58
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x000196CC File Offset: 0x000178CC
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000196DA File Offset: 0x000178DA
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x000196E8 File Offset: 0x000178E8
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x000196F6 File Offset: 0x000178F6
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000A7B8C File Offset: 0x000A5D8C
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

		// Token: 0x060022E8 RID: 8936 RVA: 0x00019704 File Offset: 0x00017904
		public override void OnSelect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnSelect(eventData);
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x0001971C File Offset: 0x0001791C
		public override void OnDeselect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnDeselect(eventData);
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x00019734 File Offset: 0x00017934
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerEnter(eventData);
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x0001974C File Offset: 0x0001794C
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerExit(eventData);
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000A7BE4 File Offset: 0x000A5DE4
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

		// Token: 0x060022ED RID: 8941 RVA: 0x00019764 File Offset: 0x00017964
		public override void Select()
		{
			if (this.eventSystem.alreadySelecting)
			{
				return;
			}
			this.eventSystem.SetSelectedGameObject(base.gameObject);
		}

		// Token: 0x040025C5 RID: 9669
		public bool allowAllEventSystems;

		// Token: 0x040025C6 RID: 9670
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025C7 RID: 9671
		private RectTransform viewPortRectTransform;
	}
}
