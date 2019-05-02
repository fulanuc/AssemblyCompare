using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000439 RID: 1081
	[RequireComponent(typeof(PostProcessVolume))]
	public class HookLightingIntoPostProcessVolume : MonoBehaviour
	{
		// Token: 0x0600181F RID: 6175 RVA: 0x0001204F File Offset: 0x0001024F
		private void OnEnable()
		{
			this.volumeColliders = base.GetComponents<Collider>();
			if (!this.hasCachedAmbientColor)
			{
				this.defaultAmbientColor = RenderSettings.ambientLight;
				this.hasCachedAmbientColor = true;
			}
			SceneCamera.onSceneCameraPreRender += this.OnPreRenderSceneCam;
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x00012088 File Offset: 0x00010288
		private void OnDisable()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0007DDA4 File Offset: 0x0007BFA4
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			float interpFactor = this.GetInterpFactor(sceneCam.camera.transform.position);
			RenderSettings.ambientLight = Color.Lerp(this.defaultAmbientColor, this.overrideAmbientColor, interpFactor);
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x0007DDE0 File Offset: 0x0007BFE0
		private float GetInterpFactor(Vector3 triggerPos)
		{
			if (!this.volume.enabled || this.volume.weight <= 0f)
			{
				return 0f;
			}
			if (this.volume.isGlobal)
			{
				return 1f;
			}
			float num = 0f;
			foreach (Collider collider in this.volumeColliders)
			{
				float num2 = float.PositiveInfinity;
				if (collider.enabled)
				{
					float sqrMagnitude = ((collider.ClosestPoint(triggerPos) - triggerPos) / 2f).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
					}
					float num3 = this.volume.blendDistance * this.volume.blendDistance;
					if (num2 <= num3 && num3 > 0f)
					{
						num = Mathf.Max(num, 1f - num2 / num3);
					}
				}
			}
			return num;
		}

		// Token: 0x04001B63 RID: 7011
		public PostProcessVolume volume;

		// Token: 0x04001B64 RID: 7012
		private Collider[] volumeColliders;

		// Token: 0x04001B65 RID: 7013
		[ColorUsage(true, true)]
		public Color overrideAmbientColor;

		// Token: 0x04001B66 RID: 7014
		private Color defaultAmbientColor;

		// Token: 0x04001B67 RID: 7015
		private bool hasCachedAmbientColor;
	}
}
