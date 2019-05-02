using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000600 RID: 1536
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPDropdown : TMP_Dropdown
	{
		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06002293 RID: 8851 RVA: 0x00019315 File Offset: 0x00017515
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002294 RID: 8852 RVA: 0x00019322 File Offset: 0x00017522
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x000A7008 File Offset: 0x000A5208
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

		// Token: 0x06002296 RID: 8854 RVA: 0x00019336 File Offset: 0x00017536
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x00019344 File Offset: 0x00017544
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x00019352 File Offset: 0x00017552
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x06002299 RID: 8857 RVA: 0x00019360 File Offset: 0x00017560
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000A7048 File Offset: 0x000A5248
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x0001936E File Offset: 0x0001756E
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x000193A7 File Offset: 0x000175A7
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

		// Token: 0x0600229D RID: 8861 RVA: 0x000A707C File Offset: 0x000A527C
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

		// Token: 0x0600229E RID: 8862 RVA: 0x000193CD File Offset: 0x000175CD
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x000193E5 File Offset: 0x000175E5
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

		// Token: 0x060022A0 RID: 8864 RVA: 0x000A70DC File Offset: 0x000A52DC
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

		// Token: 0x060022A1 RID: 8865 RVA: 0x0001940B File Offset: 0x0001760B
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x0400259A RID: 9626
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400259B RID: 9627
		protected bool isPointerInside;

		// Token: 0x0400259C RID: 9628
		public bool allowAllEventSystems;

		// Token: 0x0400259D RID: 9629
		private bool inPointerUp;
	}
}
