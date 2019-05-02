﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000626 RID: 1574
	public class RuleBookViewer : MonoBehaviour
	{
		// Token: 0x06002359 RID: 9049 RVA: 0x00019C57 File Offset: 0x00017E57
		private void Start()
		{
			this.AllocateCategories(RuleCatalog.categoryCount);
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x00019C64 File Offset: 0x00017E64
		private void Update()
		{
			if (PreGameController.instance)
			{
				this.SetData(PreGameController.instance.resolvedRuleChoiceMask, PreGameController.instance.readOnlyRuleBook);
			}
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x000A93C0 File Offset: 0x000A75C0
		private void AllocateCategories(int desiredCount)
		{
			while (this.categoryControllers.Count > desiredCount)
			{
				int index = this.categoryControllers.Count - 1;
				UnityEngine.Object.Destroy(this.categoryControllers[index].gameObject);
				this.categoryControllers.RemoveAt(index);
			}
			while (this.categoryControllers.Count < desiredCount)
			{
				RuleCategoryController component = UnityEngine.Object.Instantiate<GameObject>(this.categoryPrefab, this.categoryContainer).GetComponent<RuleCategoryController>();
				this.categoryControllers.Add(component);
			}
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x000A9440 File Offset: 0x000A7640
		private void SetData(RuleChoiceMask choiceAvailability, RuleBook ruleBook)
		{
			if (choiceAvailability.Equals(this.cachedRuleChoiceMask) && ruleBook.Equals(this.cachedRuleBook))
			{
				return;
			}
			this.cachedRuleChoiceMask.Copy(choiceAvailability);
			this.cachedRuleBook.Copy(ruleBook);
			for (int i = 0; i < RuleCatalog.categoryCount; i++)
			{
				this.categoryControllers[i].SetData(RuleCatalog.GetCategoryDef(i), this.cachedRuleChoiceMask, this.cachedRuleBook);
				this.categoryControllers[i].gameObject.SetActive(!this.categoryControllers[i].shouldHide);
			}
		}

		// Token: 0x0400263B RID: 9787
		[Tooltip("The prefab to instantiate for a rule strip.")]
		public GameObject stripPrefab;

		// Token: 0x0400263C RID: 9788
		[Tooltip("The prefab to use for categories.")]
		public GameObject categoryPrefab;

		// Token: 0x0400263D RID: 9789
		public RectTransform categoryContainer;

		// Token: 0x0400263E RID: 9790
		private readonly List<RuleCategoryController> categoryControllers = new List<RuleCategoryController>();

		// Token: 0x0400263F RID: 9791
		private readonly RuleChoiceMask cachedRuleChoiceMask = new RuleChoiceMask();

		// Token: 0x04002640 RID: 9792
		private readonly RuleBook cachedRuleBook = new RuleBook();
	}
}
