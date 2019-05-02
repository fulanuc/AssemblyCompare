using System;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x0200048C RID: 1164
	public class RuleDef
	{
		// Token: 0x06001A23 RID: 6691 RVA: 0x00085220 File Offset: 0x00083420
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

		// Token: 0x06001A24 RID: 6692 RVA: 0x00085284 File Offset: 0x00083484
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

		// Token: 0x06001A25 RID: 6693 RVA: 0x0001365D File Offset: 0x0001185D
		public void MakeNewestChoiceDefault()
		{
			this.defaultChoiceIndex = this.choices.Count - 1;
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x00013672 File Offset: 0x00011872
		public RuleDef(string globalName, string displayToken)
		{
			this.globalName = globalName;
			this.displayToken = displayToken;
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x000852D0 File Offset: 0x000834D0
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

		// Token: 0x06001A28 RID: 6696 RVA: 0x00085354 File Offset: 0x00083554
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

		// Token: 0x06001A29 RID: 6697 RVA: 0x000853F4 File Offset: 0x000835F4
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

		// Token: 0x06001A2A RID: 6698 RVA: 0x00085494 File Offset: 0x00083694
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

		// Token: 0x04001D4B RID: 7499
		public readonly string globalName;

		// Token: 0x04001D4C RID: 7500
		public int globalIndex;

		// Token: 0x04001D4D RID: 7501
		public readonly string displayToken;

		// Token: 0x04001D4E RID: 7502
		public readonly List<RuleChoiceDef> choices = new List<RuleChoiceDef>();

		// Token: 0x04001D4F RID: 7503
		public int defaultChoiceIndex;

		// Token: 0x04001D50 RID: 7504
		public RuleCategoryDef category;

		// Token: 0x04001D51 RID: 7505
		private const string pathToOffChoiceMaterial = "Materials/UI/matRuleChoiceOff";
	}
}
