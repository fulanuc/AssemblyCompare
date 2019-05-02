using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000488 RID: 1160
	public static class RuleCatalog
	{
		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06001A09 RID: 6665 RVA: 0x00013548 File Offset: 0x00011748
		public static int ruleCount
		{
			get
			{
				return RuleCatalog.allRuleDefs.Count;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06001A0A RID: 6666 RVA: 0x00013554 File Offset: 0x00011754
		public static int choiceCount
		{
			get
			{
				return RuleCatalog.allChoicesDefs.Count;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06001A0B RID: 6667 RVA: 0x00013560 File Offset: 0x00011760
		public static int categoryCount
		{
			get
			{
				return RuleCatalog.allCategoryDefs.Count;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06001A0C RID: 6668 RVA: 0x0001356C File Offset: 0x0001176C
		// (set) Token: 0x06001A0D RID: 6669 RVA: 0x00013573 File Offset: 0x00011773
		public static int highestLocalChoiceCount { get; private set; }

		// Token: 0x06001A0E RID: 6670 RVA: 0x0001357B File Offset: 0x0001177B
		public static RuleDef GetRuleDef(int ruleDefIndex)
		{
			return RuleCatalog.allRuleDefs[ruleDefIndex];
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x00084B1C File Offset: 0x00082D1C
		public static RuleDef FindRuleDef(string ruleDefGlobalName)
		{
			RuleDef result;
			RuleCatalog.ruleDefsByGlobalName.TryGetValue(ruleDefGlobalName, out result);
			return result;
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x00084B38 File Offset: 0x00082D38
		public static RuleChoiceDef FindChoiceDef(string ruleChoiceDefGlobalName)
		{
			RuleChoiceDef result;
			RuleCatalog.ruleChoiceDefsByGlobalName.TryGetValue(ruleChoiceDefGlobalName, out result);
			return result;
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x00013588 File Offset: 0x00011788
		public static RuleChoiceDef GetChoiceDef(int ruleChoiceDefIndex)
		{
			return RuleCatalog.allChoicesDefs[ruleChoiceDefIndex];
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x00013595 File Offset: 0x00011795
		public static RuleCategoryDef GetCategoryDef(int ruleCategoryDefIndex)
		{
			return RuleCatalog.allCategoryDefs[ruleCategoryDefIndex];
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool HiddenTestTrue()
		{
			return true;
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x00003696 File Offset: 0x00001896
		private static bool HiddenTestFalse()
		{
			return false;
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x000135A2 File Offset: 0x000117A2
		private static bool HiddenTestItemsConvar()
		{
			return !RuleCatalog.ruleShowItems.value;
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x000135B1 File Offset: 0x000117B1
		private static void AddCategory(string displayToken, Color color)
		{
			RuleCatalog.AddCategory(displayToken, color, null, new Func<bool>(RuleCatalog.HiddenTestFalse));
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x000135C7 File Offset: 0x000117C7
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

		// Token: 0x06001A18 RID: 6680 RVA: 0x00084B54 File Offset: 0x00082D54
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

		// Token: 0x06001A19 RID: 6681 RVA: 0x00084C3C File Offset: 0x00082E3C
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

		// Token: 0x06001A1A RID: 6682 RVA: 0x000850E4 File Offset: 0x000832E4
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

		// Token: 0x04001D28 RID: 7464
		private static readonly List<RuleDef> allRuleDefs = new List<RuleDef>();

		// Token: 0x04001D29 RID: 7465
		private static readonly List<RuleChoiceDef> allChoicesDefs = new List<RuleChoiceDef>();

		// Token: 0x04001D2A RID: 7466
		public static readonly List<RuleCategoryDef> allCategoryDefs = new List<RuleCategoryDef>();

		// Token: 0x04001D2B RID: 7467
		private static readonly Dictionary<string, RuleDef> ruleDefsByGlobalName = new Dictionary<string, RuleDef>();

		// Token: 0x04001D2C RID: 7468
		private static readonly Dictionary<string, RuleChoiceDef> ruleChoiceDefsByGlobalName = new Dictionary<string, RuleChoiceDef>();

		// Token: 0x04001D2D RID: 7469
		public static ResourceAvailability availability;

		// Token: 0x04001D2F RID: 7471
		private static readonly BoolConVar ruleShowItems = new BoolConVar("rule_show_items", ConVarFlags.Cheat, "0", "Whether or not to allow voting on items in the pregame rules.");
	}
}
