using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063A RID: 1594
	public class RuleCategoryController : MonoBehaviour
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060023F6 RID: 9206 RVA: 0x0001A3E3 File Offset: 0x000185E3
		public bool shouldHide
		{
			get
			{
				return (this.strips.Count == 0 && !this.tipObject) || this.currentCategory == null || this.currentCategory.isHidden;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060023F7 RID: 9207 RVA: 0x0001A414 File Offset: 0x00018614
		public bool isEmpty
		{
			get
			{
				return this.strips.Count == 0;
			}
		}

		// Token: 0x060023F8 RID: 9208 RVA: 0x0001A424 File Offset: 0x00018624
		private void Awake()
		{
			this.SetCollapsed(this.collapsed);
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x000AAD44 File Offset: 0x000A8F44
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

		// Token: 0x060023FA RID: 9210 RVA: 0x000AADC0 File Offset: 0x000A8FC0
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

		// Token: 0x060023FB RID: 9211 RVA: 0x000AAE48 File Offset: 0x000A9048
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

		// Token: 0x060023FC RID: 9212 RVA: 0x0001A432 File Offset: 0x00018632
		public void ToggleCollapsed()
		{
			this.SetCollapsed(!this.collapsed);
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x000AB040 File Offset: 0x000A9240
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

		// Token: 0x040026A5 RID: 9893
		public GameObject headerObject;

		// Token: 0x040026A6 RID: 9894
		public GameObject collapsedIndicator;

		// Token: 0x040026A7 RID: 9895
		public GameObject uncollapsedIndicator;

		// Token: 0x040026A8 RID: 9896
		public GameObject stripPrefab;

		// Token: 0x040026A9 RID: 9897
		public RectTransform stripContainer;

		// Token: 0x040026AA RID: 9898
		public RectTransform framePanel;

		// Token: 0x040026AB RID: 9899
		public GameObject tipPrefab;

		// Token: 0x040026AC RID: 9900
		public RectTransform tipContainer;

		// Token: 0x040026AD RID: 9901
		private readonly List<RuleDef> rulesToDisplay = new List<RuleDef>(RuleCatalog.ruleCount);

		// Token: 0x040026AE RID: 9902
		private readonly List<GameObject> strips = new List<GameObject>();

		// Token: 0x040026AF RID: 9903
		private GameObject tipObject;

		// Token: 0x040026B0 RID: 9904
		private RuleCategoryDef currentCategory;

		// Token: 0x040026B1 RID: 9905
		[SerializeField]
		private bool collapsed;
	}
}
