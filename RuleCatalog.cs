using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047B RID: 1147
	public static class RuleCatalog
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060019A7 RID: 6567 RVA: 0x0001301A File Offset: 0x0001121A
		public static int ruleCount
		{
			get
			{
				return RuleCatalog.allRuleDefs.Count;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060019A8 RID: 6568 RVA: 0x00013026 File Offset: 0x00011226
		public static int choiceCount
		{
			get
			{
				return RuleCatalog.allChoicesDefs.Count;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060019A9 RID: 6569 RVA: 0x00013032 File Offset: 0x00011232
		public static int categoryCount
		{
			get
			{
				return RuleCatalog.allCategoryDefs.Count;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060019AA RID: 6570 RVA: 0x0001303E File Offset: 0x0001123E
		// (set) Token: 0x060019AB RID: 6571 RVA: 0x00013045 File Offset: 0x00011245
		public static int highestLocalChoiceCount { get; private set; }

		// Token: 0x060019AC RID: 6572 RVA: 0x0001304D File Offset: 0x0001124D
		public static RuleDef GetRuleDef(int ruleDefIndex)
		{
			return RuleCatalog.allRuleDefs[ruleDefIndex];
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x000840B0 File Offset: 0x000822B0
		public static RuleDef FindRuleDef(string ruleDefGlobalName)
		{
			RuleDef result;
			RuleCatalog.ruleDefsByGlobalName.TryGetValue(ruleDefGlobalName, out result);
			return result;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x000840CC File Offset: 0x000822CC
		public static RuleChoiceDef FindChoiceDef(string ruleChoiceDefGlobalName)
		{
			RuleChoiceDef result;
			RuleCatalog.ruleChoiceDefsByGlobalName.TryGetValue(ruleChoiceDefGlobalName, out result);
			return result;
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0001305A File Offset: 0x0001125A
		public static RuleChoiceDef GetChoiceDef(int ruleChoiceDefIndex)
		{
			return RuleCatalog.allChoicesDefs[ruleChoiceDefIndex];
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x00013067 File Offset: 0x00011267
		public static RuleCategoryDef GetCategoryDef(int ruleCategoryDefIndex)
		{
			return RuleCatalog.allCategoryDefs[ruleCategoryDefIndex];
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool HiddenTestTrue()
		{
			return true;
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x00003696 File Offset: 0x00001896
		private static bool HiddenTestFalse()
		{
			return false;
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x00013074 File Offset: 0x00011274
		private static bool HiddenTestItemsConvar()
		{
			return !RuleCatalog.ruleShowItems.value;
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x00013083 File Offset: 0x00011283
		private static void AddCategory(string displayToken, Color color)
		{
			RuleCatalog.AddCategory(displayToken, color, null, new Func<bool>(RuleCatalog.HiddenTestFalse));
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x00013099 File Offset: 0x00011299
		private static void AddCategory(string displayToken, Color color, string emptyTipToken, Func<bool> hiddenTest)
		{
			RuleCatalog.allCategoryDefs.Add(new RuleCategoryDef
			{
				position = RuleCatalog.allRuleDefs.Count,
				displayToken = displayToken,
				color = color,
				emptyTipToken = emptyTipToken,
				hiddenTest = hiddenTest
			});
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x000840E8 File Offset: 0x000822E8
		private static void AddRule(RuleDef ruleDef)
		{
			if (RuleCatalog.allCategoryDefs.Count > 0)
			{
				ruleDef.category = RuleCatalog.allCategoryDefs[RuleCatalog.allCategoryDefs.Count - 1];
				RuleCatalog.allCategoryDefs[RuleCatalog.allCategoryDefs.Count - 1].children.Add(ruleDef);
			}
			RuleCatalog.allRuleDefs.Add(ruleDef);
			if (RuleCatalog.highestLocalChoiceCount < ruleDef.choices.Count)
			{
				RuleCatalog.highestLocalChoiceCount = ruleDef.choices.Count;
			}
			RuleCatalog.ruleDefsByGlobalName[ruleDef.globalName] = ruleDef;
			foreach (RuleChoiceDef ruleChoiceDef in ruleDef.choices)
			{
				RuleCatalog.ruleChoiceDefsByGlobalName[ruleChoiceDef.globalName] = ruleChoiceDef;
			}
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x000841D0 File Offset: 0x000823D0
		static RuleCatalog()
		{
			RuleCatalog.AddCategory("RULE_HEADER_DIFFICULTY", new Color32(28, 99, 150, byte.MaxValue));
			RuleCatalog.AddRule(RuleDef.FromDifficulty());
			RuleCatalog.AddCategory("RULE_HEADER_ARTIFACTS", new Color32(74, 50, 149, byte.MaxValue), "RULE_ARTIFACTS_EMPTY_TIP", new Func<bool>(RuleCatalog.HiddenTestFalse));
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				RuleCatalog.AddRule(RuleDef.FromArtifact(artifactIndex));
			}
			RuleCatalog.AddCategory("RULE_HEADER_ITEMS", new Color32(147, 225, 128, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestItemsConvar));
			List<ItemIndex> list = new List<ItemIndex>();
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				list.Add(itemIndex);
			}
			foreach (ItemIndex itemIndex2 in from i in list
			where ItemCatalog.GetItemDef(i).inDroppableTier
			orderby ItemCatalog.GetItemDef(i).tier
			select i)
			{
				RuleCatalog.AddRule(RuleDef.FromItem(itemIndex2));
			}
			RuleCatalog.AddCategory("RULE_HEADER_EQUIPMENT", new Color32(byte.MaxValue, 128, 0, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestItemsConvar));
			List<EquipmentIndex> list2 = new List<EquipmentIndex>();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				list2.Add(equipmentIndex);
			}
			foreach (EquipmentIndex equipmentIndex2 in from i in list2
			where EquipmentCatalog.GetEquipmentDef(i).canDrop
			select i)
			{
				RuleCatalog.AddRule(RuleDef.FromEquipment(equipmentIndex2));
			}
			RuleCatalog.AddCategory("RULE_HEADER_MISC", new Color32(192, 192, 192, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestFalse));
			RuleDef ruleDef = new RuleDef("Misc.StartingMoney", "RULE_MISC_STARTING_MONEY");
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("0", 0u, true);
			ruleChoiceDef.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_0_NAME";
			ruleChoiceDef.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_0_DESC";
			ruleChoiceDef.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("15", 15u, true);
			ruleChoiceDef2.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_15_NAME";
			ruleChoiceDef2.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_15_DESC";
			ruleChoiceDef2.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef3 = ruleDef.AddChoice("50", 50u, true);
			ruleChoiceDef3.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_50_NAME";
			ruleChoiceDef3.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_50_DESC";
			ruleChoiceDef3.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
			ruleChoiceDef3.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef);
			RuleDef ruleDef2 = new RuleDef("Misc.StageOrder", "RULE_MISC_STAGE_ORDER");
			RuleChoiceDef ruleChoiceDef4 = ruleDef2.AddChoice("Normal", StageOrder.Normal, true);
			ruleChoiceDef4.tooltipNameToken = "RULE_STAGEORDER_CHOICE_NORMAL_NAME";
			ruleChoiceDef4.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_NORMAL_DESC";
			ruleChoiceDef4.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef2.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef5 = ruleDef2.AddChoice("Random", StageOrder.Random, true);
			ruleChoiceDef5.tooltipNameToken = "RULE_STAGEORDER_CHOICE_RANDOM_NAME";
			ruleChoiceDef5.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_RANDOM_DESC";
			ruleChoiceDef5.spritePath = "Textures/MiscIcons/texRuleMapIsRandom";
			ruleChoiceDef5.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef2);
			RuleDef ruleDef3 = new RuleDef("Misc.KeepMoneyBetweenStages", "RULE_MISC_KEEP_MONEY_BETWEEN_STAGES");
			ruleDef3.AddChoice("On", true, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_ON_DESC";
			ruleDef3.AddChoice("Off", false, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_OFF_DESC";
			ruleDef3.MakeNewestChoiceDefault();
			RuleCatalog.AddRule(ruleDef3);
			for (int k = 0; k < RuleCatalog.allRuleDefs.Count; k++)
			{
				RuleDef ruleDef4 = RuleCatalog.allRuleDefs[k];
				ruleDef4.globalIndex = k;
				for (int j = 0; j < ruleDef4.choices.Count; j++)
				{
					RuleChoiceDef ruleChoiceDef6 = ruleDef4.choices[j];
					ruleChoiceDef6.localIndex = j;
					ruleChoiceDef6.globalIndex = RuleCatalog.allChoicesDefs.Count;
					RuleCatalog.allChoicesDefs.Add(ruleChoiceDef6);
				}
			}
			RuleCatalog.availability.MakeAvailable();
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00084678 File Offset: 0x00082878
		[ConCommand(commandName = "rules_dump", flags = ConVarFlags.None, helpText = "Dump information about the rules system.")]
		private static void CCRulesDump(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < RuleCatalog.ruleCount; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				for (int j = 0; j < ruleDef.choices.Count; j++)
				{
					RuleChoiceDef ruleChoiceDef = ruleDef.choices[j];
					string item = string.Format("  {{localChoiceIndex={0} globalChoiceIndex={1} localName={2}}}", ruleChoiceDef.localIndex, ruleChoiceDef.globalIndex, ruleChoiceDef.localName);
					list2.Add(item);
				}
				string str = string.Join("\n", list2);
				list2.Clear();
				string str2 = string.Format("[{0}] {1} defaultChoiceIndex={2}\n", i, ruleDef.globalName, ruleDef.defaultChoiceIndex);
				list.Add(str2 + str);
			}
			Debug.Log(string.Join("\n", list));
		}

		// Token: 0x04001CF1 RID: 7409
		private static readonly List<RuleDef> allRuleDefs = new List<RuleDef>();

		// Token: 0x04001CF2 RID: 7410
		private static readonly List<RuleChoiceDef> allChoicesDefs = new List<RuleChoiceDef>();

		// Token: 0x04001CF3 RID: 7411
		public static readonly List<RuleCategoryDef> allCategoryDefs = new List<RuleCategoryDef>();

		// Token: 0x04001CF4 RID: 7412
		private static readonly Dictionary<string, RuleDef> ruleDefsByGlobalName = new Dictionary<string, RuleDef>();

		// Token: 0x04001CF5 RID: 7413
		private static readonly Dictionary<string, RuleChoiceDef> ruleChoiceDefsByGlobalName = new Dictionary<string, RuleChoiceDef>();

		// Token: 0x04001CF6 RID: 7414
		public static ResourceAvailability availability;

		// Token: 0x04001CF8 RID: 7416
		private static readonly BoolConVar ruleShowItems = new BoolConVar("rule_show_items", ConVarFlags.Cheat, "0", "Whether or not to allow voting on items in the pregame rules.");
	}
}
