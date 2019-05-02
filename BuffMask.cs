using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200020A RID: 522
	[Serializable]
	public struct BuffMask : IEquatable<BuffMask>
	{
		// Token: 0x06000A27 RID: 2599 RVA: 0x000082FB File Offset: 0x000064FB
		private BuffMask(uint mask)
		{
			this.mask = mask;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00008304 File Offset: 0x00006504
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffAdded(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask | 1u << (int)buffIndex);
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00008318 File Offset: 0x00006518
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffRemoved(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask & ~(1u << (int)buffIndex));
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x0000832D File Offset: 0x0000652D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBuff(BuffIndex buffIndex)
		{
			return (this.mask & 1u << (int)buffIndex) > 0u;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x0000833F File Offset: 0x0000653F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(BuffMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x0000834F File Offset: 0x0000654F
		public override bool Equals(object obj)
		{
			return obj != null && obj is BuffMask && this.Equals((BuffMask)obj);
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0000836C File Offset: 0x0000656C
		public override int GetHashCode()
		{
			return (int)this.mask;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0000833F File Offset: 0x0000653F
		public static bool operator ==(BuffMask a, BuffMask b)
		{
			return a.mask == b.mask;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00008374 File Offset: 0x00006574
		public static bool operator !=(BuffMask a, BuffMask b)
		{
			return a.mask != b.mask;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x00008387 File Offset: 0x00006587
		public static void WriteBuffMask(NetworkWriter writer, BuffMask buffMask)
		{
			writer.WritePackedUInt32(buffMask.mask);
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00008395 File Offset: 0x00006595
		public static BuffMask ReadBuffMask(NetworkReader reader)
		{
			return new BuffMask(reader.ReadPackedUInt32());
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x000471E0 File Offset: 0x000453E0
		static BuffMask()
		{
			for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
			{
				if (BuffCatalog.GetBuffDef(buffIndex).isElite)
				{
					BuffMask.eliteMask |= 1u << (int)buffIndex;
				}
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x000083A2 File Offset: 0x000065A2
		public bool containsEliteBuff
		{
			get
			{
				return (this.mask & BuffMask.eliteMask) > 0u;
			}
		}

		// Token: 0x04000D94 RID: 3476
		[SerializeField]
		public readonly uint mask;

		// Token: 0x04000D95 RID: 3477
		private static readonly uint eliteMask;
	}
}
