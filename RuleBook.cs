using System;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000483 RID: 1155
	public class RuleBook
	{
		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060019E1 RID: 6625 RVA: 0x000133E6 File Offset: 0x000115E6
		public uint startingMoney
		{
			get
			{
				return (uint)this.GetRuleChoice(RuleBook.startingMoneyRule).extraData;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060019E2 RID: 6626 RVA: 0x000133FD File Offset: 0x000115FD
		public StageOrder stageOrder
		{
			get
			{
				return (StageOrder)this.GetRuleChoice(RuleBook.stageOrderRule).extraData;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060019E3 RID: 6627 RVA: 0x00013414 File Offset: 0x00011614
		public bool keepMoneyBetweenStages
		{
			get
			{
				return (bool)this.GetRuleChoice(RuleBook.keepMoneyBetweenStagesRule).extraData;
			}
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0008461C File Offset: 0x0008281C
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

		// Token: 0x060019E5 RID: 6629 RVA: 0x0001342B File Offset: 0x0001162B
		public RuleBook()
		{
			HGXml.Register<RuleBook>(new HGXml.Serializer<RuleBook>(RuleBook.ToXml), new HGXml.Deserializer<RuleBook>(RuleBook.FromXml));
			this.SetToDefaults();
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00013466 File Offset: 0x00011666
		public void SetToDefaults()
		{
			Array.Copy(RuleBook.defaultValues, 0, this.ruleValues, 0, this.ruleValues.Length);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00013482 File Offset: 0x00011682
		public void ApplyChoice(RuleChoiceDef choiceDef)
		{
			this.ruleValues[choiceDef.ruleDef.globalIndex] = (byte)choiceDef.localIndex;
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x0001349D File Offset: 0x0001169D
		public int GetRuleChoiceIndex(int ruleIndex)
		{
			return (int)this.ruleValues[ruleIndex];
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x000134A7 File Offset: 0x000116A7
		public int GetRuleChoiceIndex(RuleDef ruleDef)
		{
			return (int)this.ruleValues[ruleDef.globalIndex];
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x000134B6 File Offset: 0x000116B6
		public RuleChoiceDef GetRuleChoice(int ruleIndex)
		{
			return RuleCatalog.GetRuleDef(ruleIndex).choices[(int)this.ruleValues[ruleIndex]];
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x000134D0 File Offset: 0x000116D0
		public RuleChoiceDef GetRuleChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[(int)this.ruleValues[ruleDef.globalIndex]];
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00084670 File Offset: 0x00082870
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				writer.Write(this.ruleValues[i]);
			}
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x000846A0 File Offset: 0x000828A0
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				this.ruleValues[i] = reader.ReadByte();
			}
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x000846D0 File Offset: 0x000828D0
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

		// Token: 0x060019EF RID: 6639 RVA: 0x00084710 File Offset: 0x00082910
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				num += (int)this.ruleValues[i];
			}
			return num;
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x000134EA File Offset: 0x000116EA
		public void Copy([NotNull] RuleBook src)
		{
			Array.Copy(src.ruleValues, this.ruleValues, this.ruleValues.Length);
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x00084740 File Offset: 0x00082940
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

		// Token: 0x060019F2 RID: 6642 RVA: 0x0008478C File Offset: 0x0008298C
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

		// Token: 0x060019F3 RID: 6643 RVA: 0x000847E4 File Offset: 0x000829E4
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

		// Token: 0x060019F4 RID: 6644 RVA: 0x0008483C File Offset: 0x00082A3C
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

		// Token: 0x060019F5 RID: 6645 RVA: 0x00084894 File Offset: 0x00082A94
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

		// Token: 0x060019F6 RID: 6646 RVA: 0x00084920 File Offset: 0x00082B20
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

		// Token: 0x04001D20 RID: 7456
		private readonly byte[] ruleValues = new byte[RuleCatalog.ruleCount];

		// Token: 0x04001D21 RID: 7457
		protected static readonly RuleDef startingMoneyRule = RuleCatalog.FindRuleDef("Misc.StartingMoney");

		// Token: 0x04001D22 RID: 7458
		protected static readonly RuleDef stageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");

		// Token: 0x04001D23 RID: 7459
		protected static readonly RuleDef keepMoneyBetweenStagesRule = RuleCatalog.FindRuleDef("Misc.KeepMoneyBetweenStages");

		// Token: 0x04001D24 RID: 7460
		private static byte[] defaultValues;
	}
}
