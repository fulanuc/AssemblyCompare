using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x02000525 RID: 1317
	[RequireComponent(typeof(MapNode))]
	public class MapNodeLink : MonoBehaviour
	{
		// Token: 0x06001DAC RID: 7596 RVA: 0x00091B04 File Offset: 0x0008FD04
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

		// Token: 0x06001DAD RID: 7597 RVA: 0x00091B68 File Offset: 0x0008FD68
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

		// Token: 0x04001FDC RID: 8156
		public MapNode other;

		// Token: 0x04001FDD RID: 8157
		public float minJumpHeight;

		// Token: 0x04001FDE RID: 8158
		[Tooltip("The gate name associated with this link. If the named gate is closed, this link will not be used in pathfinding.")]
		public string gateName = "";

		// Token: 0x04001FDF RID: 8159
		public GameObject[] objectsToEnableDuringTest;

		// Token: 0x04001FE0 RID: 8160
		public GameObject[] objectsToDisableDuringTest;
	}
}
