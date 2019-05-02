using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000638 RID: 1592
	public class RuleBookViewer : MonoBehaviour
	{
		// Token: 0x060023E9 RID: 9193 RVA: 0x0001A325 File Offset: 0x00018525
		private void Start()
		{
			this.AllocateCategories(RuleCatalog.categoryCount);
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x0001A332 File Offset: 0x00018532
		private void Update()
		{
			if (PreGameController.instance)
			{
				this.SetData(PreGameController.instance.resolvedRuleChoiceMask, PreGameController.instance.readOnlyRuleBook);
			}
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x000AAA3C File Offset: 0x000A8C3C
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

		// Token: 0x060023EC RID: 9196 RVA: 0x000AAABC File Offset: 0x000A8CBC
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

		// Token: 0x04002696 RID: 9878
		[Tooltip("The prefab to instantiate for a rule strip.")]
		public GameObject stripPrefab;

		// Token: 0x04002697 RID: 9879
		[Tooltip("The prefab to use for categories.")]
		public GameObject categoryPrefab;

		// Token: 0x04002698 RID: 9880
		public RectTransform categoryContainer;

		// Token: 0x04002699 RID: 9881
		private readonly List<RuleCategoryController> categoryControllers = new List<RuleCategoryController>();

		// Token: 0x0400269A RID: 9882
		private readonly RuleChoiceMask cachedRuleChoiceMask = new RuleChoiceMask();

		// Token: 0x0400269B RID: 9883
		private readonly RuleBook cachedRuleBook = new RuleBook();
	}
}
