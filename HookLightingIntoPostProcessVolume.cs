using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000441 RID: 1089
	[RequireComponent(typeof(PostProcessVolume))]
	public class HookLightingIntoPostProcessVolume : MonoBehaviour
	{
		// Token: 0x0600186C RID: 6252 RVA: 0x000124C3 File Offset: 0x000106C3
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

		// Token: 0x0600186D RID: 6253 RVA: 0x000124FC File Offset: 0x000106FC
		private void OnDisable()
		{
			SceneCamera.onSceneCameraPreRender -= this.OnPreRenderSceneCam;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0007E560 File Offset: 0x0007C760
		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			float interpFactor = this.GetInterpFactor(sceneCam.camera.transform.position);
			RenderSettings.ambientLight = Color.Lerp(this.defaultAmbientColor, this.overrideAmbientColor, interpFactor);
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x0007E59C File Offset: 0x0007C79C
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

		// Token: 0x04001B93 RID: 7059
		public PostProcessVolume volume;

		// Token: 0x04001B94 RID: 7060
		private Collider[] volumeColliders;

		// Token: 0x04001B95 RID: 7061
		[ColorUsage(true, true)]
		public Color overrideAmbientColor;

		// Token: 0x04001B96 RID: 7062
		private Color defaultAmbientColor;

		// Token: 0x04001B97 RID: 7063
		private bool hasCachedAmbientColor;
	}
}
