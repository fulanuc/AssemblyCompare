using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200046C RID: 1132
	[Serializable]
	public struct ProcChainMask : IEquatable<ProcChainMask>
	{
		// Token: 0x0600195F RID: 6495 RVA: 0x00012DCF File Offset: 0x00010FCF
		public void AddProc(ProcType procType)
		{
			this.mask |= (ushort)(1 << (int)procType);
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x00012DE6 File Offset: 0x00010FE6
		public void RemoveProc(ProcType procType)
		{
			this.mask &= (ushort)(~(ushort)(1 << (int)procType));
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x00012DFE File Offset: 0x00010FFE
		public bool HasProc(ProcType procType)
		{
			return ((int)this.mask & 1 << (int)procType) != 0;
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x000038B4 File Offset: 0x00001AB4
		private static bool StaticCheck()
		{
			return true;
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x00012E10 File Offset: 0x00011010
		public bool Equals(ProcChainMask other)
		{
			return this.mask == other.mask;
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x00012E20 File Offset: 0x00011020
		public override bool Equals(object obj)
		{
			return obj != null && obj is ProcChainMask && this.Equals((ProcChainMask)obj);
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x00012E3D File Offset: 0x0001103D
		public override int GetHashCode()
		{
			return this.mask.GetHashCode();
		}

		// Token: 0x04001CCB RID: 7371
		[SerializeField]
		public ushort mask;
	}
}
