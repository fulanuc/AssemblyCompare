using System;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000476 RID: 1142
	public class RuleBook
	{
		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600197F RID: 6527 RVA: 0x00012EB8 File Offset: 0x000110B8
		public uint startingMoney
		{
			get
			{
				return (uint)this.GetRuleChoice(RuleBook.startingMoneyRule).extraData;
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x00012ECF File Offset: 0x000110CF
		public StageOrder stageOrder
		{
			get
			{
				return (StageOrder)this.GetRuleChoice(RuleBook.stageOrderRule).extraData;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06001981 RID: 6529 RVA: 0x00012EE6 File Offset: 0x000110E6
		public bool keepMoneyBetweenStages
		{
			get
			{
				return (bool)this.GetRuleChoice(RuleBook.keepMoneyBetweenStagesRule).extraData;
			}
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x00083BB0 File Offset: 0x00081DB0
		static RuleBook()
		{
			RuleCatalog.availability.CallWhenAvailable(delegate
			{
				RuleBook.defaultValues = new byte[RuleCatalog.ruleCount];
				for (int i = 0; i < RuleCatalog.ruleCount; i++)
				{
					RuleBook.defaultValues[i] = (byte)RuleCatalog.GetRuleDef(i).defaultChoiceIndex;
				}
			});
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x00012EFD File Offset: 0x000110FD
		public RuleBook()
		{
			HGXml.Register<RuleBook>(new HGXml.Serializer<RuleBook>(RuleBook.ToXml), new HGXml.Deserializer<RuleBook>(RuleBook.FromXml));
			this.SetToDefaults();
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x00012F38 File Offset: 0x00011138
		public void SetToDefaults()
		{
			Array.Copy(RuleBook.defaultValues, 0, this.ruleValues, 0, this.ruleValues.Length);
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x00012F54 File Offset: 0x00011154
		public void ApplyChoice(RuleChoiceDef choiceDef)
		{
			this.ruleValues[choiceDef.ruleDef.globalIndex] = (byte)choiceDef.localIndex;
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x00012F6F File Offset: 0x0001116F
		public int GetRuleChoiceIndex(int ruleIndex)
		{
			return (int)this.ruleValues[ruleIndex];
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x00012F79 File Offset: 0x00011179
		public int GetRuleChoiceIndex(RuleDef ruleDef)
		{
			return (int)this.ruleValues[ruleDef.globalIndex];
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x00012F88 File Offset: 0x00011188
		public RuleChoiceDef GetRuleChoice(int ruleIndex)
		{
			return RuleCatalog.GetRuleDef(ruleIndex).choices[(int)this.ruleValues[ruleIndex]];
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x00012FA2 File Offset: 0x000111A2
		public RuleChoiceDef GetRuleChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[(int)this.ruleValues[ruleDef.globalIndex]];
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x00083C04 File Offset: 0x00081E04
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				writer.Write(this.ruleValues[i]);
			}
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x00083C34 File Offset: 0x00081E34
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				this.ruleValues[i] = reader.ReadByte();
			}
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x00083C64 File Offset: 0x00081E64
		public override bool Equals(object obj)
		{
			RuleBook ruleBook = obj as RuleBook;
			if (ruleBook != null)
			{
				for (int i = 0; i < this.ruleValues.Length; i++)
				{
					if (this.ruleValues[i] != ruleBook.ruleValues[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x00083CA4 File Offset: 0x00081EA4
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				num += (int)this.ruleValues[i];
			}
			return num;
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x00012FBC File Offset: 0x000111BC
		public void Copy([NotNull] RuleBook src)
		{
			Array.Copy(src.ruleValues, this.ruleValues, this.ruleValues.Length);
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x00083CD4 File Offset: 0x00081ED4
		public DifficultyIndex FindDifficulty()
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.difficultyIndex != DifficultyIndex.Invalid)
				{
					return ruleChoiceDef.difficultyIndex;
				}
			}
			return DifficultyIndex.Invalid;
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x00083D20 File Offset: 0x00081F20
		public ArtifactMask GenerateArtifactMask()
		{
			ArtifactMask result = default(ArtifactMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.artifactIndex != ArtifactIndex.None)
				{
					result.AddArtifact(ruleChoiceDef.artifactIndex);
				}
			}
			return result;
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x00083D78 File Offset: 0x00081F78
		public ItemMask GenerateItemMask()
		{
			ItemMask result = default(ItemMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.itemIndex != ItemIndex.None)
				{
					result.AddItem(ruleChoiceDef.itemIndex);
				}
			}
			return result;
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x00083DD0 File Offset: 0x00081FD0
		public EquipmentMask GenerateEquipmentMask()
		{
			EquipmentMask result = default(EquipmentMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.equipmentIndex != EquipmentIndex.None)
				{
					result.AddEquipment(ruleChoiceDef.equipmentIndex);
				}
			}
			return result;
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x00083E28 File Offset: 0x00082028
		public static void ToXml(XElement element, RuleBook src)
		{
			byte[] array = src.ruleValues;
			RuleBook.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.choiceNamesBuffer = new string[array.Length];
			CS$<>8__locals1.choiceNamesCount = 0;
			for (int i = 0; i < array.Length; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				byte b = array[i];
				if ((ulong)b < (ulong)((long)ruleDef.choices.Count))
				{
					RuleBook.<ToXml>g__AddChoice|28_0(ruleDef.choices[(int)b].globalName, ref CS$<>8__locals1);
				}
			}
			element.Value = string.Join(" ", CS$<>8__locals1.choiceNamesBuffer, 0, CS$<>8__locals1.choiceNamesCount);
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x00083EB4 File Offset: 0x000820B4
		public static bool FromXml(XElement element, ref RuleBook dest)
		{
			dest.SetToDefaults();
			string[] array = element.Value.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.FindChoiceDef(array[i]);
				if (ruleChoiceDef != null)
				{
					dest.ApplyChoice(ruleChoiceDef);
				}
			}
			return true;
		}

		// Token: 0x04001CE9 RID: 7401
		private readonly byte[] ruleValues = new byte[RuleCatalog.ruleCount];

		// Token: 0x04001CEA RID: 7402
		protected static readonly RuleDef startingMoneyRule = RuleCatalog.FindRuleDef("Misc.StartingMoney");

		// Token: 0x04001CEB RID: 7403
		protected static readonly RuleDef stageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");

		// Token: 0x04001CEC RID: 7404
		protected static readonly RuleDef keepMoneyBetweenStagesRule = RuleCatalog.FindRuleDef("Misc.KeepMoneyBetweenStages");

		// Token: 0x04001CED RID: 7405
		private static byte[] defaultValues;
	}
}
