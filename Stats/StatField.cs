using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x0200050B RID: 1291
	public struct StatField
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x00015844 File Offset: 0x00013A44
		public string name
		{
			get
			{
				return this.statDef.name;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001D4C RID: 7500 RVA: 0x00015851 File Offset: 0x00013A51
		public StatRecordType recordType
		{
			get
			{
				return this.statDef.recordType;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x0001585E File Offset: 0x00013A5E
		public StatDataType dataType
		{
			get
			{
				return this.statDef.dataType;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001D4E RID: 7502 RVA: 0x0001586B File Offset: 0x00013A6B
		// (set) Token: 0x06001D4F RID: 7503 RVA: 0x00015878 File Offset: 0x00013A78
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

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06001D50 RID: 7504 RVA: 0x00015886 File Offset: 0x00013A86
		// (set) Token: 0x06001D51 RID: 7505 RVA: 0x00015893 File Offset: 0x00013A93
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

		// Token: 0x06001D52 RID: 7506 RVA: 0x0008F044 File Offset: 0x0008D244
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

		// Token: 0x06001D53 RID: 7507 RVA: 0x0008F088 File Offset: 0x0008D288
		public string ToLocalNumeric()
		{
			StatDataType dataType = this.dataType;
			if (dataType == StatDataType.ULong)
			{
				return TextSerialization.ToStringNumeric(this.value.ulongValue);
			}
			if (dataType != StatDataType.Double)
			{
				throw new ArgumentOutOfRangeException();
			}
			return TextSerialization.ToStringNumeric(this.value.doubleValue);
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x000141EC File Offset: 0x000123EC
		public ulong CalculatePointValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x0008F0CC File Offset: 0x0008D2CC
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

		// Token: 0x06001D56 RID: 7510 RVA: 0x0008F1BC File Offset: 0x0008D3BC
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

		// Token: 0x06001D57 RID: 7511 RVA: 0x0008F1F8 File Offset: 0x0008D3F8
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

		// Token: 0x06001D58 RID: 7512 RVA: 0x0008F234 File Offset: 0x0008D434
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

		// Token: 0x06001D59 RID: 7513 RVA: 0x000158A1 File Offset: 0x00013AA1
		private void EnforceDataType(StatDataType otherDataType)
		{
			if (this.dataType != otherDataType)
			{
				throw new InvalidOperationException(string.Format("Expected data type {0}, got data type {1}.", this.dataType, otherDataType));
			}
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x0008F270 File Offset: 0x0008D470
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

		// Token: 0x06001D5B RID: 7515 RVA: 0x0008F2C8 File Offset: 0x0008D4C8
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

		// Token: 0x06001D5C RID: 7516 RVA: 0x0008F320 File Offset: 0x0008D520
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

		// Token: 0x06001D5D RID: 7517 RVA: 0x000158CD File Offset: 0x00013ACD
		public ulong GetULongValue()
		{
			this.EnforceDataType(StatDataType.ULong);
			return this.ulongValue;
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x000158DC File Offset: 0x00013ADC
		public double GetDoubleValue()
		{
			this.EnforceDataType(StatDataType.Double);
			return this.doubleValue;
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0008F370 File Offset: 0x0008D570
		public bool IsDefault()
		{
			StatDataType dataType = this.dataType;
			if (dataType != StatDataType.ULong)
			{
				return dataType != StatDataType.Double || this.doubleValue == 0.0;
			}
			return this.ulongValue == 0UL;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0008F3AC File Offset: 0x0008D5AC
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

		// Token: 0x06001D61 RID: 7521 RVA: 0x0008F3E8 File Offset: 0x0008D5E8
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

		// Token: 0x04001F5C RID: 8028
		public StatDef statDef;

		// Token: 0x04001F5D RID: 8029
		private StatField.ValueUnion value;

		// Token: 0x0200050C RID: 1292
		[StructLayout(LayoutKind.Explicit)]
		private struct ValueUnion
		{
			// Token: 0x06001D62 RID: 7522 RVA: 0x000158EB File Offset: 0x00013AEB
			public static explicit operator ulong(StatField.ValueUnion v)
			{
				return v.ulongValue;
			}

			// Token: 0x06001D63 RID: 7523 RVA: 0x000158F3 File Offset: 0x00013AF3
			public static explicit operator double(StatField.ValueUnion v)
			{
				return v.doubleValue;
			}

			// Token: 0x06001D64 RID: 7524 RVA: 0x000158FB File Offset: 0x00013AFB
			public static implicit operator StatField.ValueUnion(ulong ulongValue)
			{
				return new StatField.ValueUnion(ulongValue);
			}

			// Token: 0x06001D65 RID: 7525 RVA: 0x00015903 File Offset: 0x00013B03
			public static implicit operator StatField.ValueUnion(double doubleValue)
			{
				return new StatField.ValueUnion(doubleValue);
			}

			// Token: 0x06001D66 RID: 7526 RVA: 0x0001590B File Offset: 0x00013B0B
			private ValueUnion(ulong ulongValue)
			{
				this = default(StatField.ValueUnion);
				this.ulongValue = ulongValue;
			}

			// Token: 0x06001D67 RID: 7527 RVA: 0x0001591B File Offset: 0x00013B1B
			private ValueUnion(double doubleValue)
			{
				this = default(StatField.ValueUnion);
				this.doubleValue = doubleValue;
			}

			// Token: 0x04001F5E RID: 8030
			[FieldOffset(0)]
			public readonly ulong ulongValue;

			// Token: 0x04001F5F RID: 8031
			[FieldOffset(0)]
			public readonly double doubleValue;
		}
	}
}
