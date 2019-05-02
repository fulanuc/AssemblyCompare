using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000534 RID: 1332
	[RequireComponent(typeof(MapNode))]
	public class MapNodeLink : MonoBehaviour
	{
		// Token: 0x06001E14 RID: 7700 RVA: 0x00092880 File Offset: 0x00090A80
		private void OnValidate()
		{
			if (this.other == this)
			{
				Debug.LogWarning("Map node link cannot link a node to itself.");
				this.other = null;
			}
			if (this.other && this.other.GetComponentInParent<MapNodeGroup>() != base.GetComponentInParent<MapNodeGroup>())
			{
				Debug.LogWarning("Map node link cannot link to a node in a separate node group.");
				this.other = null;
			}
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x000928E4 File Offset: 0x00090AE4
		private void OnDrawGizmos()
		{
			if (this.other)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = this.other.transform.position;
				Vector3 vector = (position + position2) * 0.5f;
				Color yellow = Color.yellow;
				yellow.a = 0.5f;
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(position, vector);
				Gizmos.color = yellow;
				Gizmos.DrawLine(vector, position2);
			}
		}

		// Token: 0x0400201A RID: 8218
		public MapNode other;

		// Token: 0x0400201B RID: 8219
		public float minJumpHeight;

		// Token: 0x0400201C RID: 8220
		[Tooltip("The gate name associated with this link. If the named gate is closed, this link will not be used in pathfinding.")]
		public string gateName = "";

		// Token: 0x0400201D RID: 8221
		public GameObject[] objectsToEnableDuringTest;

		// Token: 0x0400201E RID: 8222
		public GameObject[] objectsToDisableDuringTest;
	}
}
