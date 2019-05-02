using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000343 RID: 835
	public interface ITeleportHandler : IEventSystemHandler
	{
		// Token: 0x0600115D RID: 4445
		void OnTeleport(Vector3 oldPosition, Vector3 newPosition);
	}
}
