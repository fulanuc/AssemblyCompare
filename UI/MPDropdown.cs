using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000612 RID: 1554
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPDropdown : TMP_Dropdown
	{
		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06002323 RID: 8995 RVA: 0x000199CC File Offset: 0x00017BCC
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x000199D9 File Offset: 0x00017BD9
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x000A8684 File Offset: 0x000A6884
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

		// Token: 0x06002326 RID: 8998 RVA: 0x000199ED File Offset: 0x00017BED
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x000199FB File Offset: 0x00017BFB
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x00019A09 File Offset: 0x00017C09
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x00019A17 File Offset: 0x00017C17
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600232A RID: 9002 RVA: 0x000A86C4 File Offset: 0x000A68C4
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x00019A25 File Offset: 0x00017C25
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x00019A5E File Offset: 0x00017C5E
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

		// Token: 0x0600232D RID: 9005 RVA: 0x000A86F8 File Offset: 0x000A68F8
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

		// Token: 0x0600232E RID: 9006 RVA: 0x00019A84 File Offset: 0x00017C84
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x00019A9C File Offset: 0x00017C9C
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

		// Token: 0x06002330 RID: 9008 RVA: 0x000A8758 File Offset: 0x000A6958
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

		// Token: 0x06002331 RID: 9009 RVA: 0x00019AC2 File Offset: 0x00017CC2
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x040025F5 RID: 9717
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025F6 RID: 9718
		protected bool isPointerInside;

		// Token: 0x040025F7 RID: 9719
		public bool allowAllEventSystems;

		// Token: 0x040025F8 RID: 9720
		private bool inPointerUp;
	}
}
