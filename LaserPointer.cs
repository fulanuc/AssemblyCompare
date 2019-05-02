using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034B RID: 843
	[RequireComponent(typeof(LineRenderer))]
	public class LaserPointer : MonoBehaviour
	{
		// Token: 0x06001192 RID: 4498 RVA: 0x0000D6A7 File Offset: 0x0000B8A7
		private void Start()
		{
			this.line = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x0006660C File Offset: 0x0006480C
		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.laserDistance, LayerIndex.world.mask))
			{
				this.line.SetPosition(0, base.transform.position);
				this.line.SetPosition(1, raycastHit.point);
			}
		}

		// Token: 0x0400158B RID: 5515
		public float laserDistance;

		// Token: 0x0400158C RID: 5516
		private LineRenderer line;
	}
}
