using System;
using UnityEngine;

namespace RoR2.PostProcessing
{
	// Token: 0x02000563 RID: 1379
	public class ScreenDamage : MonoBehaviour
	{
		// Token: 0x06001EDF RID: 7903 RVA: 0x000169D0 File Offset: 0x00014BD0
		private void Awake()
		{
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
			this.mat = UnityEngine.Object.Instantiate<Material>(this.mat);
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x00097C68 File Offset: 0x00095E68
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

		// Token: 0x04002174 RID: 8564
		private CameraRigController cameraRigController;

		// Token: 0x04002175 RID: 8565
		public Material mat;

		// Token: 0x04002176 RID: 8566
		public float DistortionScale = 1f;

		// Token: 0x04002177 RID: 8567
		public float DistortionPower = 1f;

		// Token: 0x04002178 RID: 8568
		public float DesaturationScale = 1f;

		// Token: 0x04002179 RID: 8569
		public float DesaturationPower = 1f;

		// Token: 0x0400217A RID: 8570
		public float TintScale = 1f;

		// Token: 0x0400217B RID: 8571
		public float TintPower = 1f;

		// Token: 0x0400217C RID: 8572
		private float healthPercentage = 1f;

		// Token: 0x0400217D RID: 8573
		private const float hitTintDecayTime = 0.6f;

		// Token: 0x0400217E RID: 8574
		private const float hitTintScale = 1.6f;

		// Token: 0x0400217F RID: 8575
		private const float deathWeight = 2f;
	}
}
