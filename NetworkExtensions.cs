using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001EF RID: 495
	public static class NetworkExtensions
	{
		// Token: 0x06000993 RID: 2451 RVA: 0x00007CAF File Offset: 0x00005EAF
		public static void WriteAchievementIndex(this NetworkWriter writer, AchievementIndex value)
		{
			writer.WritePackedUInt32((uint)value.intValue);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x0004562C File Offset: 0x0004382C
		public static AchievementIndex ReadAchievementIndex(this NetworkReader reader)
		{
			return new AchievementIndex
			{
				intValue = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00007CBD File Offset: 0x00005EBD
		public static void WriteBodyIndex(this NetworkWriter writer, int bodyIndex)
		{
			writer.WritePackedUInt32((uint)(bodyIndex + 1));
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00007CC8 File Offset: 0x00005EC8
		public static int ReadBodyIndex(this NetworkReader reader)
		{
			return (int)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00007CD2 File Offset: 0x00005ED2
		public static void WriteBuffMask(this NetworkWriter writer, BuffMask buffMask)
		{
			BuffMask.WriteBuffMask(writer, buffMask);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00007CDB File Offset: 0x00005EDB
		public static BuffMask ReadBuffMask(this NetworkReader reader)
		{
			return BuffMask.ReadBuffMask(reader);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00007CE3 File Offset: 0x00005EE3
		public static DamageType ReadDamageType(this NetworkReader reader)
		{
			return (DamageType)reader.ReadUInt16();
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00007CEB File Offset: 0x00005EEB
		public static void Write(this NetworkWriter writer, DamageType damageType)
		{
			writer.Write((ushort)damageType);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00007CF4 File Offset: 0x00005EF4
		public static DamageColorIndex ReadDamageColorIndex(this NetworkReader reader)
		{
			return (DamageColorIndex)reader.ReadByte();
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00007CFC File Offset: 0x00005EFC
		public static void Write(this NetworkWriter writer, DamageColorIndex damageColorIndex)
		{
			writer.Write((byte)damageColorIndex);
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00007D05 File Offset: 0x00005F05
		public static void Write(this NetworkWriter writer, EffectData effectData)
		{
			effectData.Serialize(writer);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00007D0E File Offset: 0x00005F0E
		public static EffectData ReadEffectData(this NetworkReader reader)
		{
			EffectData effectData = new EffectData();
			effectData.Deserialize(reader);
			return effectData;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00007D1C File Offset: 0x00005F1C
		public static void ReadEffectData(this NetworkReader reader, EffectData effectData)
		{
			effectData.Deserialize(reader);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00007CBD File Offset: 0x00005EBD
		public static void Write(this NetworkWriter writer, EquipmentIndex equipmentIndex)
		{
			writer.WritePackedUInt32((uint)(equipmentIndex + 1));
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00007CC8 File Offset: 0x00005EC8
		public static EquipmentIndex ReadEquipmentIndex(this NetworkReader reader)
		{
			return (EquipmentIndex)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00007D25 File Offset: 0x00005F25
		public static void Write(this NetworkWriter writer, HurtBoxReference hurtBoxReference)
		{
			hurtBoxReference.Write(writer);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00045650 File Offset: 0x00043850
		public static HurtBoxReference ReadHurtBoxReference(this NetworkReader reader)
		{
			HurtBoxReference result = default(HurtBoxReference);
			result.Read(reader);
			return result;
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00007D2F File Offset: 0x00005F2F
		public static void Write(this NetworkWriter writer, Run.TimeStamp timeStamp)
		{
			Run.TimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00007D38 File Offset: 0x00005F38
		public static Run.TimeStamp ReadTimeStamp(this NetworkReader reader)
		{
			return Run.TimeStamp.Deserialize(reader);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00007D40 File Offset: 0x00005F40
		public static void Write(this NetworkWriter writer, Run.FixedTimeStamp timeStamp)
		{
			Run.FixedTimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00007D49 File Offset: 0x00005F49
		public static Run.FixedTimeStamp ReadFixedTimeStamp(this NetworkReader reader)
		{
			return Run.FixedTimeStamp.Deserialize(reader);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00007CBD File Offset: 0x00005EBD
		public static void Write(this NetworkWriter writer, ItemIndex itemIndex)
		{
			writer.WritePackedUInt32((uint)(itemIndex + 1));
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00007CC8 File Offset: 0x00005EC8
		public static ItemIndex ReadItemIndex(this NetworkReader reader)
		{
			return (ItemIndex)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00045670 File Offset: 0x00043870
		public static void WriteItemStacks(this NetworkWriter writer, int[] srcItemStacks)
		{
			int num = 0;
			for (int i = 0; i < 10; i++)
			{
				byte b = 0;
				int num2 = 0;
				while (num2 < 8 && num < 78)
				{
					if (srcItemStacks[num] > 0)
					{
						b |= (byte)(1 << num2);
					}
					num2++;
					num++;
				}
				NetworkExtensions.itemMaskByteBuffer[i] = b;
			}
			for (int j = 0; j < 10; j++)
			{
				writer.Write(NetworkExtensions.itemMaskByteBuffer[j]);
			}
			for (int k = 0; k < 78; k++)
			{
				int num3 = srcItemStacks[k];
				if (num3 > 0)
				{
					writer.WritePackedUInt32((uint)num3);
				}
			}
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00045700 File Offset: 0x00043900
		public static void ReadItemStacks(this NetworkReader reader, int[] destItemStacks)
		{
			for (int i = 0; i < 10; i++)
			{
				NetworkExtensions.itemMaskByteBuffer[i] = reader.ReadByte();
			}
			int num = 0;
			for (int j = 0; j < 10; j++)
			{
				byte b = NetworkExtensions.itemMaskByteBuffer[j];
				int num2 = 0;
				while (num2 < 8 && num < 78)
				{
					destItemStacks[num] = (int)(((b & (byte)(1 << num2)) != 0) ? reader.ReadPackedUInt32() : 0u);
					num2++;
					num++;
				}
			}
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00007D51 File Offset: 0x00005F51
		public static void WriteBitArray(this NetworkWriter writer, [NotNull] bool[] values)
		{
			writer.WriteBitArray(values, values.Length);
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00045770 File Offset: 0x00043970
		public static void WriteBitArray(this NetworkWriter writer, [NotNull] bool[] values, int bufferLength)
		{
			int num = bufferLength + 7 >> 3;
			int num2 = num - 1;
			int num3 = bufferLength - (num2 << 3);
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				byte b = 0;
				int num5 = (i < num2) ? 8 : num3;
				int j = 0;
				while (j < num5)
				{
					if (values[num4])
					{
						b |= (byte)(1 << j);
					}
					j++;
					num4++;
				}
				writer.Write(b);
			}
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00007D5D File Offset: 0x00005F5D
		public static void ReadBitArray(this NetworkReader reader, [NotNull] bool[] values)
		{
			reader.ReadBitArray(values, values.Length);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x000457E0 File Offset: 0x000439E0
		public static void ReadBitArray(this NetworkReader reader, [NotNull] bool[] values, int bufferLength)
		{
			int num = bufferLength + 7 >> 3;
			int num2 = num - 1;
			int num3 = bufferLength - (num2 << 3);
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				int num5 = (i < num2) ? 8 : num3;
				byte b = reader.ReadByte();
				int j = 0;
				while (j < num5)
				{
					values[num4] = ((b & (byte)(1 << j)) > 0);
					j++;
					num4++;
				}
			}
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00007D69 File Offset: 0x00005F69
		public static void Write(this NetworkWriter writer, NetworkPlayerName networkPlayerName)
		{
			networkPlayerName.Serialize(writer);
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00045848 File Offset: 0x00043A48
		public static NetworkPlayerName ReadNetworkPlayerName(this NetworkReader reader)
		{
			NetworkPlayerName result = default(NetworkPlayerName);
			result.Deserialize(reader);
			return result;
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00007D73 File Offset: 0x00005F73
		public static void Write(this NetworkWriter writer, PitchYawPair pitchYawPair)
		{
			writer.Write(pitchYawPair.pitch);
			writer.Write(pitchYawPair.yaw);
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00045868 File Offset: 0x00043A68
		public static PitchYawPair ReadPitchYawPair(this NetworkReader reader)
		{
			float pitch = reader.ReadSingle();
			float yaw = reader.ReadSingle();
			return new PitchYawPair(pitch, yaw);
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00007D8F File Offset: 0x00005F8F
		public static void Write(this NetworkWriter writer, RuleBook src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00007D98 File Offset: 0x00005F98
		public static void ReadRuleBook(this NetworkReader reader, RuleBook dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00007DA1 File Offset: 0x00005FA1
		public static void Write(this NetworkWriter writer, RuleMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00007DAA File Offset: 0x00005FAA
		public static void ReadRuleMask(this NetworkReader reader, RuleMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00007DB3 File Offset: 0x00005FB3
		public static void Write(this NetworkWriter writer, RuleChoiceMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00007DBC File Offset: 0x00005FBC
		public static void ReadRuleChoiceMask(this NetworkReader reader, RuleChoiceMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00045888 File Offset: 0x00043A88
		public static void Write(this NetworkWriter writer, TeamIndex teamIndex)
		{
			byte value = (byte)(teamIndex + 1);
			writer.Write(value);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x00007DC5 File Offset: 0x00005FC5
		public static TeamIndex ReadTeamIndex(this NetworkReader reader)
		{
			return (TeamIndex)(reader.ReadByte() - 1);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00007DD0 File Offset: 0x00005FD0
		public static void Write(this NetworkWriter writer, UnlockableIndex index)
		{
			writer.Write((byte)index.internalValue);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x000458A4 File Offset: 0x00043AA4
		public static UnlockableIndex ReadUnlockableIndex(this NetworkReader reader)
		{
			return new UnlockableIndex
			{
				internalValue = (uint)reader.ReadByte()
			};
		}

		// Token: 0x04000CF4 RID: 3316
		private const int itemMaskBitCount = 78;

		// Token: 0x04000CF5 RID: 3317
		private const int itemMaskByteCount = 10;

		// Token: 0x04000CF6 RID: 3318
		private static readonly byte[] itemMaskByteBuffer = new byte[10];
	}
}
