using System;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x0200047F RID: 1151
	public class RuleDef
	{
		// Token: 0x060019C1 RID: 6593 RVA: 0x000847B4 File Offset: 0x000829B4
		public RuleChoiceDef AddChoice(string choiceName, object extraData = null, bool excludeByDefault = false)
		{
			RuleChoiceDef ruleChoiceDef = new RuleChoiceDef();
			ruleChoiceDef.ruleDef = this;
			ruleChoiceDef.localName = choiceName;
			ruleChoiceDef.globalName = this.globalName + "." + choiceName;
			ruleChoiceDef.localIndex = this.choices.Count;
			ruleChoiceDef.extraData = extraData;
			ruleChoiceDef.excludeByDefault = excludeByDefault;
			this.choices.Add(ruleChoiceDef);
			return ruleChoiceDef;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x00084818 File Offset: 0x00082A18
		public RuleChoiceDef FindChoice(string choiceLocalName)
		{
			int i = 0;
			int count = this.choices.Count;
			while (i < count)
			{
				if (this.choices[i].localName == choiceLocalName)
				{
					return this.choices[i];
				}
				i++;
			}
			return null;
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x0001312F File Offset: 0x0001132F
		public void MakeNewestChoiceDefault()
		{
			this.defaultChoiceIndex = this.choices.Count - 1;
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x00013144 File Offset: 0x00011344
		public RuleDef(string globalName, string displayToken)
		{
			this.globalName = globalName;
			this.displayToken = displayToken;
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00084864 File Offset: 0x00082A64
		public static RuleDef FromDifficulty()
		{
			RuleDef ruleDef = new RuleDef("Difficulty", "RULE_NAME_DIFFICULTY");
			for (DifficultyIndex difficultyIndex = DifficultyIndex.Easy; difficultyIndex < DifficultyIndex.Count; difficultyIndex++)
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
				RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice(difficultyIndex.ToString(), null, false);
				ruleChoiceDef.spritePath = difficultyDef.iconPath;
				ruleChoiceDef.tooltipNameToken = difficultyDef.nameToken;
				ruleChoiceDef.tooltipNameColor = difficultyDef.color;
				ruleChoiceDef.tooltipBodyToken = difficultyDef.descriptionToken;
				ruleChoiceDef.difficultyIndex = difficultyIndex;
			}
			ruleDef.defaultChoiceIndex = 1;
			return ruleDef;
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x000848E8 File Offset: 0x00082AE8
		public static RuleDef FromArtifact(ArtifactIndex artifactIndex)
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(artifactIndex);
			RuleDef ruleDef = new RuleDef("Artifacts." + artifactIndex.ToString(), artifactDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = artifactDef.smallIconSelectedPath;
			ruleChoiceDef.tooltipBodyToken = artifactDef.descriptionToken;
			ruleChoiceDef.unlockableName = artifactDef.unlockableName;
			ruleChoiceDef.artifactIndex = artifactIndex;
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = artifactDef.smallIconDeselectedPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipBodyToken = null;
			ruleDef.MakeNewestChoiceDefault();
			return ruleDef;
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x00084988 File Offset: 0x00082B88
		public static RuleDef FromItem(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			RuleDef ruleDef = new RuleDef("Items." + itemIndex.ToString(), itemDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = itemDef.pickupIconPath;
			ruleChoiceDef.tooltipNameToken = itemDef.nameToken;
			ruleChoiceDef.unlockableName = itemDef.unlockableName;
			ruleChoiceDef.itemIndex = itemIndex;
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = itemDef.pickupIconPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipNameToken = null;
			return ruleDef;
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00084A28 File Offset: 0x00082C28
		public static RuleDef FromEquipment(EquipmentIndex equipmentIndex)
		{
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			RuleDef ruleDef = new RuleDef("Equipment." + equipmentIndex.ToString(), equipmentDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = equipmentDef.pickupIconPath;
			ruleChoiceDef.tooltipBodyToken = equipmentDef.nameToken;
			ruleChoiceDef.unlockableName = equipmentDef.unlockableName;
			ruleChoiceDef.equipmentIndex = equipmentIndex;
			ruleChoiceDef.availableInMultiPlayer = equipmentDef.appearsInMultiPlayer;
			ruleChoiceDef.availableInSinglePlayer = equipmentDef.appearsInSinglePlayer;
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = equipmentDef.pickupIconPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipBodyToken = null;
			return ruleDef;
		}

		// Token: 0x04001D14 RID: 7444
		public readonly string globalName;

		// Token: 0x04001D15 RID: 7445
		public int globalIndex;

		// Token: 0x04001D16 RID: 7446
		public readonly string displayToken;

		// Token: 0x04001D17 RID: 7447
		public readonly List<RuleChoiceDef> choices = new List<RuleChoiceDef>();

		// Token: 0x04001D18 RID: 7448
		public int defaultChoiceIndex;

		// Token: 0x04001D19 RID: 7449
		public RuleCategoryDef category;

		// Token: 0x04001D1A RID: 7450
		private const string pathToOffChoiceMaterial = "Materials/UI/matRuleChoiceOff";
	}
}
