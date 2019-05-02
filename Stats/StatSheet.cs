using System;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x02000515 RID: 1301
	public class StatSheet
	{
		// Token: 0x06001D7F RID: 7551 RVA: 0x00015995 File Offset: 0x00013B95
		public void SetStatValueFromString([CanBeNull] StatDef statDef, string value)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].SetFromString(value);
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x000159B2 File Offset: 0x00013BB2
		public void PushStatValue([CanBeNull] StatDef statDef, ulong statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D81 RID: 7553 RVA: 0x000159CF File Offset: 0x00013BCF
		public void PushStatValue([CanBeNull] StatDef statDef, double statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D82 RID: 7554 RVA: 0x000159EC File Offset: 0x00013BEC
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, ulong statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x000159FC File Offset: 0x00013BFC
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, double statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x00015A0C File Offset: 0x00013C0C
		public ulong GetStatValueULong([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0UL;
			}
			return this.fields[statDef.index].GetULongValue();
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x00015A2A File Offset: 0x00013C2A
		public double GetStatValueDouble([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0.0;
			}
			return this.fields[statDef.index].GetDoubleValue();
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x00015A4F File Offset: 0x00013C4F
		[NotNull]
		public string GetStatValueString([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return this.fields[statDef.index].ToString();
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x00015A76 File Offset: 0x00013C76
		[NotNull]
		public string GetStatDisplayValue([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return statDef.displayValueFormatter(ref this.fields[statDef.index]);
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x00015A9D File Offset: 0x00013C9D
		public ulong GetStatPointValue([NotNull] StatDef statDef)
		{
			return this.fields[statDef.index].GetPointValue(statDef.pointValue);
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x00015ABB File Offset: 0x00013CBB
		public ulong GetStatValueULong([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueULong(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x00015ACA File Offset: 0x00013CCA
		public double GetStatValueDouble([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueDouble(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x00015AD9 File Offset: 0x00013CD9
		[NotNull]
		public string GetStatValueString([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueString(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x00015AE8 File Offset: 0x00013CE8
		[SystemInitializer(new Type[]
		{
			typeof(StatDef)
		})]
		private static void Init()
		{
			StatSheet.OnFieldsFinalized();
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x00015AEF File Offset: 0x00013CEF
		static StatSheet()
		{
			HGXml.Register<StatSheet>(new HGXml.Serializer<StatSheet>(StatSheet.ToXml), new HGXml.Deserializer<StatSheet>(StatSheet.FromXml));
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x0008FEFC File Offset: 0x0008E0FC
		public static void ToXml(XElement element, StatSheet statSheet)
		{
			element.RemoveAll();
			XElement xelement = new XElement("fields");
			element.Add(xelement);
			StatField[] array = statSheet.fields;
			for (int i = 0; i < array.Length; i++)
			{
				ref StatField ptr = ref array[i];
				if (!ptr.IsDefault())
				{
					xelement.Add(new XElement(ptr.name, ptr.ToString()));
				}
			}
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x0008FF70 File Offset: 0x0008E170
		public static bool FromXml(XElement element, ref StatSheet statSheet)
		{
			XElement xelement = element.Element("fields");
			if (xelement == null)
			{
				return false;
			}
			StatField[] array = statSheet.fields;
			for (int i = 0; i < array.Length; i++)
			{
				ref StatField ptr = ref array[i];
				XElement xelement2 = xelement.Element(ptr.name);
				if (xelement2 != null)
				{
					ptr.SetFromString(xelement2.Value);
				}
			}
			return true;
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x0008FFD8 File Offset: 0x0008E1D8
		private static void OnFieldsFinalized()
		{
			StatSheet.fieldsTemplate = (from v in StatDef.allStatDefs
			select new StatField
			{
				statDef = v
			}).ToArray<StatField>();
			StatSheet.nonDefaultFieldsBuffer = new bool[StatSheet.fieldsTemplate.Length];
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x00015B0E File Offset: 0x00013D0E
		private StatSheet([NotNull] StatField[] fields)
		{
			this.fields = fields;
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x0009002C File Offset: 0x0008E22C
		public static StatSheet New()
		{
			StatField[] array = new StatField[StatSheet.fieldsTemplate.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatSheet.fieldsTemplate[i];
			}
			return new StatSheet(array);
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x00015B28 File Offset: 0x00013D28
		public int GetUnlockableCount()
		{
			return this.unlockables.Length;
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x00015B32 File Offset: 0x00013D32
		public UnlockableIndex GetUnlockableIndex(int index)
		{
			return this.unlockables[index];
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x00015B40 File Offset: 0x00013D40
		public UnlockableDef GetUnlockable(int index)
		{
			return UnlockableCatalog.GetUnlockableDef(this.unlockables[index]);
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x0009006C File Offset: 0x0008E26C
		public bool HasUnlockable(UnlockableDef unlockableDef)
		{
			for (int i = 0; i < this.unlockables.Length; i++)
			{
				if (this.unlockables[i] == unlockableDef.index)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x00015B53 File Offset: 0x00013D53
		private void AllocateUnlockables(int desiredCount)
		{
			Array.Resize<UnlockableIndex>(ref this.unlockables, desiredCount);
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00015B61 File Offset: 0x00013D61
		public void AddUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			this.AddUnlockable(unlockableDef.index);
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x000900A8 File Offset: 0x0008E2A8
		public void AddUnlockable(UnlockableIndex unlockIndex)
		{
			for (int i = 0; i < this.unlockables.Length; i++)
			{
				if (this.unlockables[i] == unlockIndex)
				{
					return;
				}
			}
			Array.Resize<UnlockableIndex>(ref this.unlockables, this.unlockables.Length + 1);
			this.unlockables[this.unlockables.Length - 1] = unlockIndex;
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x00090108 File Offset: 0x0008E308
		public void RemoveUnlockable(UnlockableIndex unlockIndex)
		{
			int num = Array.IndexOf<UnlockableIndex>(this.unlockables, unlockIndex);
			if (num == -1)
			{
				return;
			}
			int newSize = this.unlockables.Length;
			HGArrayUtilities.ArrayRemoveAt<UnlockableIndex>(ref this.unlockables, ref newSize, num, 1);
			Array.Resize<UnlockableIndex>(ref this.unlockables, newSize);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0009014C File Offset: 0x0008E34C
		public void Write(NetworkWriter writer)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				StatSheet.nonDefaultFieldsBuffer[i] = !this.fields[i].IsDefault();
			}
			writer.WriteBitArray(StatSheet.nonDefaultFieldsBuffer);
			for (int j = 0; j < this.fields.Length; j++)
			{
				if (StatSheet.nonDefaultFieldsBuffer[j])
				{
					this.fields[j].Write(writer);
				}
			}
			writer.Write((byte)this.unlockables.Length);
			for (int k = 0; k < this.unlockables.Length; k++)
			{
				writer.Write(this.unlockables[k]);
			}
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x000901F4 File Offset: 0x0008E3F4
		public void Read(NetworkReader reader)
		{
			reader.ReadBitArray(StatSheet.nonDefaultFieldsBuffer);
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (StatSheet.nonDefaultFieldsBuffer[i])
				{
					this.fields[i].Read(reader);
				}
				else
				{
					this.fields[i].SetDefault();
				}
			}
			int num = (int)reader.ReadByte();
			this.AllocateUnlockables(num);
			for (int j = 0; j < num; j++)
			{
				this.unlockables[j] = reader.ReadUnlockableIndex();
			}
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0009027C File Offset: 0x0008E47C
		public static void GetDelta(StatSheet result, StatSheet newerStats, StatSheet olderStats)
		{
			StatField[] array = result.fields;
			StatField[] array2 = newerStats.fields;
			StatField[] array3 = olderStats.fields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatField.GetDelta(ref array2[i], ref array3[i]);
			}
			UnlockableIndex[] array4 = newerStats.unlockables;
			UnlockableIndex[] array5 = olderStats.unlockables;
			int num = 0;
			foreach (UnlockableIndex b in array4)
			{
				bool flag = false;
				for (int k = 0; k < array5.Length; k++)
				{
					if (array5[k] == b)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num++;
				}
			}
			result.AllocateUnlockables(num);
			UnlockableIndex[] array6 = result.unlockables;
			int num2 = 0;
			foreach (UnlockableIndex unlockableIndex in array4)
			{
				bool flag2 = false;
				for (int m = 0; m < array5.Length; m++)
				{
					if (array5[m] == unlockableIndex)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					array6[num2++] = unlockableIndex;
				}
			}
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x000903A0 File Offset: 0x0008E5A0
		public void ApplyDelta(StatSheet deltaSheet)
		{
			StatField[] array = deltaSheet.fields;
			for (int i = 0; i < this.fields.Length; i++)
			{
				this.fields[i].PushDelta(ref array[i]);
			}
			for (int j = 0; j < deltaSheet.unlockables.Length; j++)
			{
				this.AddUnlockable(deltaSheet.unlockables[j]);
			}
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x00090404 File Offset: 0x0008E604
		public static void Copy([NotNull] StatSheet src, [NotNull] StatSheet dest)
		{
			Array.Copy(src.fields, dest.fields, src.fields.Length);
			dest.AllocateUnlockables(src.unlockables.Length);
			Array.Copy(src.unlockables, dest.unlockables, src.unlockables.Length);
		}

		// Token: 0x04001F7E RID: 8062
		private static StatField[] fieldsTemplate;

		// Token: 0x04001F7F RID: 8063
		private static bool[] nonDefaultFieldsBuffer;

		// Token: 0x04001F80 RID: 8064
		public readonly StatField[] fields;

		// Token: 0x04001F81 RID: 8065
		private UnlockableIndex[] unlockables = Array.Empty<UnlockableIndex>();
	}
}
