using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E6 RID: 742
	[DisallowMultipleComponent]
	public class EntityLocator : MonoBehaviour
	{
		// Token: 0x06000EE0 RID: 3808 RVA: 0x0005A8E8 File Offset: 0x00058AE8
		public static GameObject GetEntity(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return null;
			}
			EntityLocator component = gameObject.GetComponent<EntityLocator>();
			if (!component)
			{
				return null;
			}
			return component.entity;
		}

		// Token: 0x040012F1 RID: 4849
		[Tooltip("The root gameobject of the entity.")]
		public GameObject entity;
	}
}
