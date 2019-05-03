﻿using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FD RID: 1021
	public class TemporaryVisualEffect : MonoBehaviour
	{
		// Token: 0x060016D0 RID: 5840 RVA: 0x000111C5 File Offset: 0x0000F3C5
		private void Start()
		{
			this.RebuildVisualComponents();
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x000111CD File Offset: 0x0000F3CD
		private void FixedUpdate()
		{
			if (this.previousVisualState != this.visualState)
			{
				this.RebuildVisualComponents();
			}
			this.previousVisualState = this.visualState;
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000784F4 File Offset: 0x000766F4
		private void RebuildVisualComponents()
		{
			TemporaryVisualEffect.VisualState visualState = this.visualState;
			MonoBehaviour[] array;
			if (visualState == TemporaryVisualEffect.VisualState.Enter)
			{
				array = this.enterComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				array = this.exitComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				return;
			}
			if (visualState != TemporaryVisualEffect.VisualState.Exit)
			{
				return;
			}
			array = this.enterComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			array = this.exitComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x0007858C File Offset: 0x0007678C
		private void LateUpdate()
		{
			bool flag = this.healthComponent;
			if (this.parentTransform)
			{
				base.transform.position = this.parentTransform.position;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (!flag || (flag && !this.healthComponent.alive))
			{
				this.visualState = TemporaryVisualEffect.VisualState.Exit;
			}
			if (this.visualTransform)
			{
				this.visualTransform.localScale = new Vector3(this.radius, this.radius, this.radius);
			}
		}

		// Token: 0x040019E0 RID: 6624
		public float radius = 1f;

		// Token: 0x040019E1 RID: 6625
		public Transform parentTransform;

		// Token: 0x040019E2 RID: 6626
		public Transform visualTransform;

		// Token: 0x040019E3 RID: 6627
		public MonoBehaviour[] enterComponents;

		// Token: 0x040019E4 RID: 6628
		public MonoBehaviour[] exitComponents;

		// Token: 0x040019E5 RID: 6629
		public TemporaryVisualEffect.VisualState visualState;

		// Token: 0x040019E6 RID: 6630
		private TemporaryVisualEffect.VisualState previousVisualState;

		// Token: 0x040019E7 RID: 6631
		[HideInInspector]
		public HealthComponent healthComponent;

		// Token: 0x020003FE RID: 1022
		public enum VisualState
		{
			// Token: 0x040019E9 RID: 6633
			Enter,
			// Token: 0x040019EA RID: 6634
			Exit
		}
	}
}
