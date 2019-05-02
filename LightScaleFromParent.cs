using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034D RID: 845
	public class LightScaleFromParent : MonoBehaviour
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x0006676C File Offset: 0x0006496C
		private void Start()
		{
			Light component = base.GetComponent<Light>();
			if (component)
			{
				float range = component.range;
				Vector3 lossyScale = base.transform.lossyScale;
				component.range = range * Mathf.Max(new float[]
				{
					lossyScale.x,
					lossyScale.y,
					lossyScale.z
				});
			}
		}
	}
}
