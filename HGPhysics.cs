using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000440 RID: 1088
	public static class HGPhysics
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06001867 RID: 6247 RVA: 0x00012477 File Offset: 0x00010677
		// (set) Token: 0x06001868 RID: 6248 RVA: 0x0007E530 File Offset: 0x0007C730
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

		// Token: 0x06001869 RID: 6249 RVA: 0x0001247E File Offset: 0x0001067E
		public static int OverlapBoxNonAllocShared(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapBoxNonAlloc(center, halfExtents, HGPhysics.sharedCollidersBuffer, orientation, layerMask, queryTriggerInteraction);
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x00012496 File Offset: 0x00010696
		public static int OverlapSphereNonAllocShared(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return HGPhysics.sharedCollidersBufferEntriesCount = Physics.OverlapSphereNonAlloc(position, radius, HGPhysics.sharedCollidersBuffer, layerMask, queryTriggerInteraction);
		}

		// Token: 0x04001B91 RID: 7057
		public static readonly Collider[] sharedCollidersBuffer = new Collider[65536];

		// Token: 0x04001B92 RID: 7058
		private static int _sharedCollidersBufferEntriesCount = 0;
	}
}
