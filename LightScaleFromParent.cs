using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034A RID: 842
	public class LightScaleFromParent : MonoBehaviour
	{
		// Token: 0x06001180 RID: 4480 RVA: 0x00066434 File Offset: 0x00064634
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
