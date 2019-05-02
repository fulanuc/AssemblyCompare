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
		// Token: 0x06000A23 RID: 2595 RVA: 0x000082D7 File Offset: 0x000064D7
		private BuffMask(uint mask)
		{
			this.mask = mask;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x000082E0 File Offset: 0x000064E0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffAdded(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask | 1u << (int)buffIndex);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000082F4 File Offset: 0x000064F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BuffMask GetBuffRemoved(BuffIndex buffIndex)
		{
			return new BuffMask(this.mask & ~(1u << (int)buffIndex));
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00008309 File Offset: 0x00006509
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBuff(BuffIndex buffIndex)
		{
			return (this.mask & 1u << (int)buffIndex) > 0u;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x0000831B File Offset: 0x0000651B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(BuffMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x0000832B File Offset: 0x0000652B
		public override bool Equals(object obj)
		{
			return obj != null && obj is BuffMask && this.Equals((BuffMask)obj);
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00008348 File Offset: 0x00006548
		public override int GetHashCode()
		{
			return (int)this.mask;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x0000831B File Offset: 0x0000651B
		public static bool operator ==(BuffMask a, BuffMask b)
		{
			return a.mask == b.mask;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00008350 File Offset: 0x00006550
		public static bool operator !=(BuffMask a, BuffMask b)
		{
			return a.mask != b.mask;
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00008363 File Offset: 0x00006563
		public static void WriteBuffMask(NetworkWriter writer, BuffMask buffMask)
		{
			writer.WritePackedUInt32(buffMask.mask);
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00008371 File Offset: 0x00006571
		public static BuffMask ReadBuffMask(NetworkReader reader)
		{
			return new BuffMask(reader.ReadPackedUInt32());
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00046F34 File Offset: 0x00045134
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

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x0000837E File Offset: 0x0000657E
		public bool containsEliteBuff
		{
			get
			{
				return (this.mask & BuffMask.eliteMask) > 0u;
			}
		}

		// Token: 0x04000D90 RID: 3472
		[SerializeField]
		public readonly uint mask;

		// Token: 0x04000D91 RID: 3473
		private static readonly uint eliteMask;
	}
}
