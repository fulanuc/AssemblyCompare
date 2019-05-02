using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034C RID: 844
	internal class LaserPointerController : MonoBehaviour
	{
		// Token: 0x06001195 RID: 4501 RVA: 0x0006667C File Offset: 0x0006487C
		private void LateUpdate()
		{
			bool enabled = false;
			bool active = false;
			if (this.source)
			{
				Ray ray = new Ray(this.source.aimOrigin, this.source.aimDirection);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					base.transform.position = raycastHit.point;
					base.transform.forward = -ray.direction;
					float num = raycastHit.distance - this.minDistanceFromStart;
					if (num >= 0.1f)
					{
						this.beam.SetPosition(1, new Vector3(0f, 0f, num));
						enabled = true;
					}
					active = true;
				}
			}
			this.dotObject.SetActive(active);
			this.beam.enabled = enabled;
		}

		// Token: 0x0400158D RID: 5517
		public InputBankTest source;

		// Token: 0x0400158E RID: 5518
		public GameObject dotObject;

		// Token: 0x0400158F RID: 5519
		public LineRenderer beam;

		// Token: 0x04001590 RID: 5520
		public float minDistanceFromStart = 4f;
	}
}
