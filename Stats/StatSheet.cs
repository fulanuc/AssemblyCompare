using System;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x02000506 RID: 1286
	public class StatSheet
	{
		// Token: 0x06001D17 RID: 7447 RVA: 0x000154EC File Offset: 0x000136EC
		public void SetStatValueFromString([CanBeNull] StatDef statDef, string value)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].SetFromString(value);
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00015509 File Offset: 0x00013709
		public void PushStatValue([CanBeNull] StatDef statDef, ulong statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x00015526 File Offset: 0x00013726
		public void PushStatValue([CanBeNull] StatDef statDef, double statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x00015543 File Offset: 0x00013743
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, ulong statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x00015553 File Offset: 0x00013753
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, double statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x00015563 File Offset: 0x00013763
		public ulong GetStatValueULong([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0UL;
			}
			return this.fields[statDef.index].GetULongValue();
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x00015581 File Offset: 0x00013781
		public double GetStatValueDouble([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0.0;
			}
			return this.fields[statDef.index].GetDoubleValue();
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x000155A6 File Offset: 0x000137A6
		[NotNull]
		public string GetStatValueString([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return this.fields[statDef.index].ToString();
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x000155CD File Offset: 0x000137CD
		[NotNull]
		public string GetStatDisplayValue([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return statDef.displayValueFormatter(ref this.fields[statDef.index]);
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x000155F4 File Offset: 0x000137F4
		public ulong GetStatPointValue([NotNull] StatDef statDef)
		{
			return this.fields[statDef.index].GetPointValue(statDef.pointValue);
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x00015612 File Offset: 0x00013812
		public ulong GetStatValueULong([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueULong(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x00015621 File Offset: 0x00013821
		public double GetStatValueDouble([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueDouble(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x00015630 File Offset: 0x00013830
		[NotNull]
		public string GetStatValueString([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueString(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x0001563F File Offset: 0x0001383F
		[SystemInitializer(new Type[]
		{
			typeof(StatDef)
		})]
		private static void Init()
		{
			StatSheet.OnFieldsFinalized();
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x00015646 File Offset: 0x00013846
		static StatSheet()
		{
			HGXml.Register<StatSheet>(new HGXml.Serializer<StatSheet>(StatSheet.ToXml), new HGXml.Deserializer<StatSheet>(StatSheet.FromXml));
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x0008F12C File Offset: 0x0008D32C
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

		// Token: 0x06001D27 RID: 7463 RVA: 0x0008F1A0 File Offset: 0x0008D3A0
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

		// Token: 0x06001D28 RID: 7464 RVA: 0x0008F208 File Offset: 0x0008D408
		private static void OnFieldsFinalized()
		{
			StatSheet.fieldsTemplate = (from v in StatDef.allStatDefs
			select new StatField
			{
				statDef = v
			}).ToArray<StatField>();
			StatSheet.nonDefaultFieldsBuffer = new bool[StatSheet.fieldsTemplate.Length];
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x00015665 File Offset: 0x00013865
		private StatSheet([NotNull] StatField[] fields)
		{
			this.fields = fields;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x0008F25C File Offset: 0x0008D45C
		public static StatSheet New()
		{
			StatField[] array = new StatField[StatSheet.fieldsTemplate.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatSheet.fieldsTemplate[i];
			}
			return new StatSheet(array);
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x0001567F File Offset: 0x0001387F
		public int GetUnlockableCount()
		{
			return this.unlockables.Length;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x00015689 File Offset: 0x00013889
		public UnlockableIndex GetUnlockableIndex(int index)
		{
			return this.unlockables[index];
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x00015697 File Offset: 0x00013897
		public UnlockableDef GetUnlockable(int index)
		{
			return UnlockableCatalog.GetUnlockableDef(this.unlockables[index]);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0008F29C File Offset: 0x0008D49C
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

		// Token: 0x06001D2F RID: 7471 RVA: 0x000156AA File Offset: 0x000138AA
		private void AllocateUnlockables(int desiredCount)
		{
			Array.Resize<UnlockableIndex>(ref this.unlockables, desiredCount);
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x000156B8 File Offset: 0x000138B8
		public void AddUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			this.AddUnlockable(unlockableDef.index);
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0008F2D8 File Offset: 0x0008D4D8
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

		// Token: 0x06001D32 RID: 7474 RVA: 0x0008F338 File Offset: 0x0008D538
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

		// Token: 0x06001D33 RID: 7475 RVA: 0x0008F37C File Offset: 0x0008D57C
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

		// Token: 0x06001D34 RID: 7476 RVA: 0x0008F424 File Offset: 0x0008D624
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

		// Token: 0x06001D35 RID: 7477 RVA: 0x0008F4AC File Offset: 0x0008D6AC
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

		// Token: 0x06001D36 RID: 7478 RVA: 0x0008F5D0 File Offset: 0x0008D7D0
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

		// Token: 0x06001D37 RID: 7479 RVA: 0x0008F634 File Offset: 0x0008D834
		public static void Copy([NotNull] StatSheet src, [NotNull] StatSheet dest)
		{
			Array.Copy(src.fields, dest.fields, src.fields.Length);
			dest.AllocateUnlockables(src.unlockables.Length);
			Array.Copy(src.unlockables, dest.unlockables, src.unlockables.Length);
		}

		// Token: 0x04001F40 RID: 8000
		private static StatField[] fieldsTemplate;

		// Token: 0x04001F41 RID: 8001
		private static bool[] nonDefaultFieldsBuffer;

		// Token: 0x04001F42 RID: 8002
		public readonly StatField[] fields;

		// Token: 0x04001F43 RID: 8003
		private UnlockableIndex[] unlockables = Array.Empty<UnlockableIndex>();
	}
}
