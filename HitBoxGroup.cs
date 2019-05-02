using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000314 RID: 788
	public class HitBoxGroup : MonoBehaviour
	{
		// Token: 0x04001451 RID: 5201
		[Tooltip("The name of this hitbox group.")]
		public string groupName;

		// Token: 0x04001452 RID: 5202
		[Tooltip("The hitbox objects in this group.")]
		public HitBox[] hitBoxes;
	}
}
