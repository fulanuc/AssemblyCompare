using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000200 RID: 512
	[Serializable]
	public struct ArtifactMask
	{
		// Token: 0x060009FF RID: 2559 RVA: 0x000080FF File Offset: 0x000062FF
		public bool HasArtifact(ArtifactIndex artifactIndex)
		{
			return artifactIndex >= ArtifactIndex.Command && artifactIndex < ArtifactIndex.Count && ((int)this.a & 1 << (int)artifactIndex) != 0;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0000811B File Offset: 0x0000631B
		public void AddArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a |= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0000813B File Offset: 0x0000633B
		public void ToggleArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a ^= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0000815B File Offset: 0x0000635B
		public void RemoveArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a &= (ushort)(~(ushort)(1 << (int)artifactIndex));
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00046540 File Offset: 0x00044740
		public static ArtifactMask operator &(ArtifactMask mask1, ArtifactMask mask2)
		{
			return new ArtifactMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x0004656C File Offset: 0x0004476C
		static ArtifactMask()
		{
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				ArtifactMask.all.AddArtifact(artifactIndex);
			}
		}

		// Token: 0x04000D40 RID: 3392
		[SerializeField]
		public ushort a;

		// Token: 0x04000D41 RID: 3393
		public static readonly ArtifactMask none;

		// Token: 0x04000D42 RID: 3394
		public static readonly ArtifactMask all = default(ArtifactMask);
	}
}
