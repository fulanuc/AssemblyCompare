using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000348 RID: 840
	[RequireComponent(typeof(LineRenderer))]
	public class LaserPointer : MonoBehaviour
	{
		// Token: 0x0600117B RID: 4475 RVA: 0x0000D5BE File Offset: 0x0000B7BE
		private void Start()
		{
			this.line = base.GetComponent<LineRenderer>();
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x000662D4 File Offset: 0x000644D4
		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.laserDistance, LayerIndex.world.mask))
			{
				this.line.SetPosition(0, base.transform.position);
				this.line.SetPosition(1, raycastHit.point);
			}
		}

		// Token: 0x04001572 RID: 5490
		public float laserDistance;

		// Token: 0x04001573 RID: 5491
		private LineRenderer line;
	}
}
