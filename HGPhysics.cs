using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000438 RID: 1080
	public static class HGPhysics
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x0600181A RID: 6170 RVA: 0x00012003 File Offset: 0x00010203
		// (set) Token: 0x0600181B RID: 6171 RVA: 0x0007DD74 File Offset: 0x0007BF74
		public static int sharedCollidersBufferEntriesCount
		{
			get
			{
				return HGPhysics._sharedCollidersBufferEntriesCount;
			}
			private set
			{
				int num = HGPhysics.sharedCollidersBufferEntriesCount - value;
				if (num > 0)
				{
					Array.Clear(HGPhysics.sharedCollidersBuffer, HGPhysics.sharedCollidersBufferEntriesCount, num);
				}
				HGPhysics._sharedCollidersBufferEntriesCount = value;
			}
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x0001200A File Offset: 0x0001020A
		public static int OverlapBoxNonAllocShared(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapBoxNonAlloc(center, halfExtents, HGPhysics.sharedCollidersBuffer, orientation, layerMask, queryTriggerInteraction);
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x00012022 File Offset: 0x00010222
		public static int OverlapSphereNonAllocShared(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapSphereNonAlloc(position, radius, HGPhysics.sharedCollidersBuffer, layerMask, queryTriggerInteraction);
		}

		// Token: 0x04001B61 RID: 7009
		public static readonly Collider[] sharedCollidersBuffer = new Collider[65536];

		// Token: 0x04001B62 RID: 7010
		private static int _sharedCollidersBufferEntriesCount = 0;
	}
}
