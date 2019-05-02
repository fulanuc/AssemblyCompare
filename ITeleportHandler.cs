using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000341 RID: 833
	public interface ITeleportHandler : IEventSystemHandler
	{
		// Token: 0x06001149 RID: 4425
		void OnTeleport(Vector3 oldPosition, Vector3 newPosition);
	}
}
