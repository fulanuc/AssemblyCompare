using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004FC RID: 1276
	public struct StatField
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06001CE4 RID: 7396 RVA: 0x0001539B File Offset: 0x0001359B
		public string name
		{
			get
			{
				return this.statDef.name;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06001CE5 RID: 7397 RVA: 0x000153A8 File Offset: 0x000135A8
		public StatRecordType recordType
		{
			get
			{
				return this.statDef.recordType;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06001CE6 RID: 7398 RVA: 0x000153B5 File Offset: 0x000135B5
		public StatDataType dataType
		{
			get
			{
				return this.statDef.dataType;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06001CE7 RID: 7399 RVA: 0x000153C2 File Offset: 0x000135C2
		// (set) Token: 0x06001CE8 RID: 7400 RVA: 0x000153CF File Offset: 0x000135CF
		private ulong ulongValue
		{
			get
			{
				return this.value.ulongValue;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06001CE9 RID: 7401 RVA: 0x000153DD File Offset: 0x000135DD
		// (set) Token: 0x06001CEA RID: 7402 RVA: 0x000153EA File Offset: 0x000135EA
		private double doubleValue
		{
			get
			{
				return this.value.doubleValue;
			}
			set
			{
				this.value = value;
			}
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0008E2B8 File Offset: 0x0008C4B8
		public override string ToString()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return TextSerialization.ToStringInvariant(this.value.ulongValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return TextSerialization.ToStringInvariant(this.value.doubleValue);
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x00013CDA File Offset: 0x00011EDA
		public ulong CalculatePointValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0008E2FC File Offset: 0x0008C4FC
		[Pure]
		public static StatField GetDelta(ref StatField newerValue, ref StatField olderValue)
		{
			StatField result = new StatField
			{
				statDef = newerValue.statDef
			};
			StatDataType dataType = newerValue.dataType;
			if (dataType != StatDataType.ULong)
			{
				if (dataType == StatDataType.Double)
				{
					switch (newerValue.recordType)
					{
					case StatRecordType.Sum:
						result.doubleValue = newerValue.doubleValue - olderValue.doubleValue;
						break;
					case StatRecordType.Max:
						result.doubleValue = Math.Max(newerValue.doubleValue, olderValue.doubleValue);
						break;
					case StatRecordType.Newest:
						result.doubleValue = newerValue.doubleValue;
						break;
					}
				}
			}
			else
			{
				switch (newerValue.recordType)
				{
				case StatRecordType.Sum:
					result.ulongValue = newerValue.ulongValue - olderValue.ulongValue;
					break;
				case StatRecordType.Max:
					result.ulongValue = Math.Max(newerValue.ulongValue, olderValue.ulongValue);
					break;
				case StatRecordType.Newest:
					result.ulongValue = newerValue.ulongValue;
					break;
				}
			}
			return result;
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0008E3EC File Offset: 0x0008C5EC
		public void PushDelta(ref StatField deltaField)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.PushStatValue(deltaField.ulongValue);
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.PushStatValue(deltaField.doubleValue);
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x0008E428 File Offset: 0x0008C628
		public void Write(NetworkWriter writer)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				writer.WritePackedUInt64(this.ulongValue);
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			writer.Write(this.doubleValue);
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x0008E464 File Offset: 0x0008C664
		public void Read(NetworkReader reader)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.ulongValue = reader.ReadPackedUInt64();
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.doubleValue = reader.ReadDouble();
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x000153F8 File Offset: 0x000135F8
		private void EnforceDataType(StatDataType otherDataType)
		{
			if (this.dataType != otherDataType)
			{
				throw new InvalidOperationException(string.Format("Expected data type {0}, got data type {1}.", this.dataType, otherDataType));
			}
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x0008E4A0 File Offset: 0x0008C6A0
		public void PushStatValue(ulong incomingValue)
		{
			this.EnforceDataType(StatDataType.ULong);
			switch (this.recordType)
			{
			case StatRecordType.Sum:
				this.ulongValue += incomingValue;
				return;
			case StatRecordType.Max:
				this.ulongValue = Math.Max(incomingValue, this.ulongValue);
				return;
			case StatRecordType.Newest:
				this.ulongValue = incomingValue;
				return;
			default:
				return;
			}
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x0008E4F8 File Offset: 0x0008C6F8
		public void PushStatValue(double incomingValue)
		{
			this.EnforceDataType(StatDataType.Double);
			switch (this.recordType)
			{
			case StatRecordType.Sum:
				this.doubleValue += incomingValue;
				return;
			case StatRecordType.Max:
				this.doubleValue = Math.Max(incomingValue, this.doubleValue);
				return;
			case StatRecordType.Newest:
				this.doubleValue = incomingValue;
				return;
			default:
				return;
			}
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x0008E550 File Offset: 0x0008C750
		public void SetFromString(string valueString)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				ulong ulongValue;
				TextSerialization.TryParseInvariant(valueString, out ulongValue);
				this.value = ulongValue;
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			double doubleValue;
			TextSerialization.TryParseInvariant(valueString, out doubleValue);
			this.value = doubleValue;
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x00015424 File Offset: 0x00013624
		public ulong GetULongValue()
		{
			this.EnforceDataType(StatDataType.ULong);
			return this.ulongValue;
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x00015433 File Offset: 0x00013633
		public double GetDoubleValue()
		{
			this.EnforceDataType(StatDataType.Double);
			return this.doubleValue;
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0008E5A0 File Offset: 0x0008C7A0
		public bool IsDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType != StatDataType.ULong)
			{
				return dataType != StatDataType.Double || this.doubleValue == 0.0;
			}
			return this.ulongValue == 0UL;
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x0008E5DC File Offset: 0x0008C7DC
		public void SetDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				this.ulongValue = 0UL;
				return;
			}
			if (dataType != StatDataType.Double)
			{
				throw new NotImplementedException();
			}
			this.doubleValue = 0.0;
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x0008E618 File Offset: 0x0008C818
		public ulong GetPointValue(double pointValue)
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return (ulong)(this.ulongValue * pointValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (ulong)(this.doubleValue * pointValue);
		}

		// Token: 0x04001F1E RID: 7966
		public StatDef statDef;

		// Token: 0x04001F1F RID: 7967
		private StatField.ValueUnion value;

		// Token: 0x020004FD RID: 1277
		[StructLayout(LayoutKind.Explicit)]
		private struct ValueUnion
		{
			// Token: 0x06001CFA RID: 7418 RVA: 0x00015442 File Offset: 0x00013642
			public static explicit operator ulong(StatField.ValueUnion v)
			{
				return v.ulongValue;
			}

			// Token: 0x06001CFB RID: 7419 RVA: 0x0001544A File Offset: 0x0001364A
			public static explicit operator double(StatField.ValueUnion v)
			{
				return v.doubleValue;
			}

			// Token: 0x06001CFC RID: 7420 RVA: 0x00015452 File Offset: 0x00013652
			public static implicit operator StatField.ValueUnion(ulong ulongValue)
			{
				return new StatField.ValueUnion(ulongValue);
			}

			// Token: 0x06001CFD RID: 7421 RVA: 0x0001545A File Offset: 0x0001365A
			public static implicit operator StatField.ValueUnion(double doubleValue)
			{
				return new StatField.ValueUnion(doubleValue);
			}

			// Token: 0x06001CFE RID: 7422 RVA: 0x00015462 File Offset: 0x00013662
			private ValueUnion(ulong ulongValue)
			{
				this = default(StatField.ValueUnion);
				this.ulongValue = ulongValue;
			}

			// Token: 0x06001CFF RID: 7423 RVA: 0x00015472 File Offset: 0x00013672
			private ValueUnion(double doubleValue)
			{
				this = default(StatField.ValueUnion);
				this.doubleValue = doubleValue;
			}

			// Token: 0x04001F20 RID: 7968
			[FieldOffset(0)]
			public readonly ulong ulongValue;

			// Token: 0x04001F21 RID: 7969
			[FieldOffset(0)]
			public readonly double doubleValue;
		}
	}
}
