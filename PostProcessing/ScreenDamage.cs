using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000572 RID: 1394
	public class ScreenDamage : MonoBehaviour
	{
		// Token: 0x06001F49 RID: 8009 RVA: 0x00016EAF File Offset: 0x000150AF
		private void Awake()
		{
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
			this.mat = UnityEngine.Object.Instantiate<Material>(this.mat);
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x00098984 File Offset: 0x00096B84
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 0f;
			float num2 = 0f;
			if (this.cameraRigController)
			{
				if (this.cameraRigController.target)
				{
					num2 = 0.5f;
					HealthComponent component = this.cameraRigController.target.GetComponent<HealthComponent>();
					if (component)
					{
						this.healthPercentage = Mathf.Clamp(component.health / component.fullHealth, 0f, 1f);
						num = Mathf.Clamp01(1f - component.timeSinceLastHit / 0.6f) * 1.6f;
						if (component.health <= 0f)
						{
							num2 = 0f;
						}
					}
				}
				this.mat.SetFloat("_DistortionStrength", num2 * this.DistortionScale * Mathf.Pow(1f - this.healthPercentage, this.DistortionPower));
				this.mat.SetFloat("_DesaturationStrength", num2 * this.DesaturationScale * Mathf.Pow(1f - this.healthPercentage, this.DesaturationPower));
				this.mat.SetFloat("_TintStrength", num2 * this.TintScale * (Mathf.Pow(1f - this.healthPercentage, this.TintPower) + num));
			}
			Graphics.Blit(source, destination, this.mat);
		}

		// Token: 0x040021B2 RID: 8626
		private CameraRigController cameraRigController;

		// Token: 0x040021B3 RID: 8627
		public Material mat;

		// Token: 0x040021B4 RID: 8628
		public float DistortionScale = 1f;

		// Token: 0x040021B5 RID: 8629
		public float DistortionPower = 1f;

		// Token: 0x040021B6 RID: 8630
		public float DesaturationScale = 1f;

		// Token: 0x040021B7 RID: 8631
		public float DesaturationPower = 1f;

		// Token: 0x040021B8 RID: 8632
		public float TintScale = 1f;

		// Token: 0x040021B9 RID: 8633
		public float TintPower = 1f;

		// Token: 0x040021BA RID: 8634
		private float healthPercentage = 1f;

		// Token: 0x040021BB RID: 8635
		private const float hitTintDecayTime = 0.6f;

		// Token: 0x040021BC RID: 8636
		private const float hitTintScale = 1.6f;

		// Token: 0x040021BD RID: 8637
		private const float deathWeight = 2f;
	}
}
