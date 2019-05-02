using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000477 RID: 1143
	[Serializable]
	public struct ProcChainMask : IEquatable<ProcChainMask>
	{
		// Token: 0x060019BC RID: 6588 RVA: 0x000132E9 File Offset: 0x000114E9
		public void AddProc(ProcType procType)
		{
			this.mask |= (ushort)(1 << (int)procType);
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x00013300 File Offset: 0x00011500
		public void RemoveProc(ProcType procType)
		{
			this.mask &= (ushort)(~(ushort)(1 << (int)procType));
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x00013318 File Offset: 0x00011518
		public bool HasProc(ProcType procType)
		{
			return ((int)this.mask & 1 << (int)procType) != 0;
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0001332A File Offset: 0x0001152A
		public bool Equals(ProcChainMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x0001333A File Offset: 0x0001153A
		public override bool Equals(object obj)
		{
			return obj != null && obj is ProcChainMask && this.Equals((ProcChainMask)obj);
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x00013357 File Offset: 0x00011557
		public override int GetHashCode()
		{
			return this.mask.GetHashCode();
		}

		// Token: 0x04001CFF RID: 7423
		[SerializeField]
		public ushort mask;
	}
}
