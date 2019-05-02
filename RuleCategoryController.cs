using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000628 RID: 1576
	public class RuleCategoryController : MonoBehaviour
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06002366 RID: 9062 RVA: 0x00019D15 File Offset: 0x00017F15
		public bool shouldHide
		{
			get
			{
				return (this.strips.Count == 0 && !this.tipObject) || this.currentCategory == null || this.currentCategory.isHidden;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06002367 RID: 9063 RVA: 0x00019D46 File Offset: 0x00017F46
		public bool isEmpty
		{
			get
			{
				return this.strips.Count == 0;
			}
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x00019D56 File Offset: 0x00017F56
		private void Awake()
		{
			this.SetCollapsed(this.collapsed);
		}

		// Token: 0x06002369 RID: 9065 RVA: 0x000A96C8 File Offset: 0x000A78C8
		private void SetTip(string tipToken)
		{
			if (tipToken == null)
			{
				UnityEngine.Object.Destroy(this.tipObject);
				this.tipObject = null;
				this.SetCollapsed(this.collapsed);
				return;
			}
			if (!this.tipObject)
			{
				this.tipObject = UnityEngine.Object.Instantiate<GameObject>(this.tipPrefab, this.tipContainer);
				this.tipObject.SetActive(true);
				this.SetCollapsed(this.collapsed);
			}
			this.tipObject.GetComponentInChildren<LanguageTextMeshController>().token = tipToken;
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x000A9744 File Offset: 0x000A7944
		private void AllocateStrips(int desiredCount)
		{
			while (this.strips.Count > desiredCount)
			{
				int index = this.strips.Count - 1;
				UnityEngine.Object.Destroy(this.strips[index]);
				this.strips.RemoveAt(index);
			}
			while (this.strips.Count < desiredCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.stripPrefab, this.stripContainer);
				gameObject.SetActive(true);
				this.strips.Add(gameObject);
			}
			this.framePanel.SetAsLastSibling();
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x000A97CC File Offset: 0x000A79CC
		public void SetData(RuleCategoryDef categoryDef, RuleChoiceMask availability, RuleBook ruleBook)
		{
			this.currentCategory = categoryDef;
			this.rulesToDisplay.Clear();
			List<RuleDef> children = categoryDef.children;
			for (int i = 0; i < children.Count; i++)
			{
				RuleDef ruleDef = children[i];
				int num = 0;
				foreach (RuleChoiceDef ruleChoiceDef in ruleDef.choices)
				{
					if (availability[ruleChoiceDef.globalIndex])
					{
						num++;
					}
				}
				bool flag = (!availability[ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex] && num != 0) || num > 1;
				if (flag)
				{
					this.rulesToDisplay.Add(children[i]);
				}
			}
			this.AllocateStrips(this.rulesToDisplay.Count);
			List<RuleChoiceDef> list = new List<RuleChoiceDef>();
			for (int j = 0; j < this.rulesToDisplay.Count; j++)
			{
				RuleDef ruleDef2 = this.rulesToDisplay[j];
				list.Clear();
				foreach (RuleChoiceDef ruleChoiceDef2 in ruleDef2.choices)
				{
					if (availability[ruleChoiceDef2.globalIndex])
					{
						list.Add(ruleChoiceDef2);
					}
				}
				this.strips[j].GetComponent<RuleBookViewerStrip>().SetData(list, ruleBook.GetRuleChoiceIndex(ruleDef2));
			}
			if (this.headerObject)
			{
				this.headerObject.GetComponent<Image>().color = categoryDef.color;
				this.headerObject.GetComponentInChildren<LanguageTextMeshController>().token = categoryDef.displayToken;
			}
			this.SetTip(this.isEmpty ? categoryDef.emptyTipToken : null);
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x00019D64 File Offset: 0x00017F64
		public void ToggleCollapsed()
		{
			this.SetCollapsed(!this.collapsed);
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x000A99C4 File Offset: 0x000A7BC4
		public void SetCollapsed(bool newCollapsed)
		{
			this.collapsed = newCollapsed;
			this.stripContainer.gameObject.SetActive(!this.collapsed);
			if (this.tipObject)
			{
				this.tipObject.SetActive(!this.collapsed);
			}
			if (this.collapsedIndicator)
			{
				this.collapsedIndicator.SetActive(this.collapsed);
			}
			if (this.uncollapsedIndicator)
			{
				this.uncollapsedIndicator.SetActive(!this.collapsed);
			}
		}

		// Token: 0x0400264A RID: 9802
		public GameObject headerObject;

		// Token: 0x0400264B RID: 9803
		public GameObject collapsedIndicator;

		// Token: 0x0400264C RID: 9804
		public GameObject uncollapsedIndicator;

		// Token: 0x0400264D RID: 9805
		public GameObject stripPrefab;

		// Token: 0x0400264E RID: 9806
		public RectTransform stripContainer;

		// Token: 0x0400264F RID: 9807
		public RectTransform framePanel;

		// Token: 0x04002650 RID: 9808
		public GameObject tipPrefab;

		// Token: 0x04002651 RID: 9809
		public RectTransform tipContainer;

		// Token: 0x04002652 RID: 9810
		private readonly List<RuleDef> rulesToDisplay = new List<RuleDef>(RuleCatalog.ruleCount);

		// Token: 0x04002653 RID: 9811
		private readonly List<GameObject> strips = new List<GameObject>();

		// Token: 0x04002654 RID: 9812
		private GameObject tipObject;

		// Token: 0x04002655 RID: 9813
		private RuleCategoryDef currentCategory;

		// Token: 0x04002656 RID: 9814
		[SerializeField]
		private bool collapsed;
	}
}
