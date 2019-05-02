using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000276 RID: 630
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class CameraResolutionScaler : MonoBehaviour
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000BDF RID: 3039 RVA: 0x0000959C File Offset: 0x0000779C
		// (set) Token: 0x06000BE0 RID: 3040 RVA: 0x000095A4 File Offset: 0x000077A4
		public Camera camera { get; private set; }

		// Token: 0x06000BE1 RID: 3041 RVA: 0x000095AD File Offset: 0x000077AD
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x000095BB File Offset: 0x000077BB
		private void OnPreRender()
		{
			this.ApplyScalingRenderTexture();
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x000095C3 File Offset: 0x000077C3
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.scalingRenderTexture)
			{
				Graphics.Blit(source, destination);
				return;
			}
			this.camera.targetTexture = this.oldRenderTexture;
			Graphics.Blit(source, this.oldRenderTexture);
			this.oldRenderTexture = null;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x000095FE File Offset: 0x000077FE
		private static void SetResolutionScale(float newResolutionScale)
		{
			if (CameraResolutionScaler.resolutionScale == newResolutionScale)
			{
				return;
			}
			CameraResolutionScaler.resolutionScale = newResolutionScale;
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0004D5F8 File Offset: 0x0004B7F8
		private void ApplyScalingRenderTexture()
		{
			this.oldRenderTexture = this.camera.targetTexture;
			bool flag = CameraResolutionScaler.resolutionScale != 1f;
			this.camera.targetTexture = null;
			Rect pixelRect = this.camera.pixelRect;
			int num = Mathf.FloorToInt(pixelRect.width * CameraResolutionScaler.resolutionScale);
			int num2 = Mathf.FloorToInt(pixelRect.height * CameraResolutionScaler.resolutionScale);
			if (this.scalingRenderTexture && (this.scalingRenderTexture.width != num || this.scalingRenderTexture.height != num2))
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
			if (flag != this.scalingRenderTexture)
			{
				if (flag)
				{
					this.scalingRenderTexture = new RenderTexture(num, num2, 24);
					this.scalingRenderTexture.autoGenerateMips = false;
					this.scalingRenderTexture.filterMode = (((double)CameraResolutionScaler.resolutionScale > 1.0) ? FilterMode.Bilinear : FilterMode.Point);
				}
				else
				{
					UnityEngine.Object.Destroy(this.scalingRenderTexture);
					this.scalingRenderTexture = null;
				}
			}
			if (flag)
			{
				this.camera.targetTexture = this.scalingRenderTexture;
			}
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0000960F File Offset: 0x0000780F
		private void OnDestroy()
		{
			if (this.scalingRenderTexture)
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
		}

		// Token: 0x04000FD0 RID: 4048
		private RenderTexture oldRenderTexture;

		// Token: 0x04000FD1 RID: 4049
		private static float resolutionScale = 1f;

		// Token: 0x04000FD2 RID: 4050
		private RenderTexture scalingRenderTexture;

		// Token: 0x02000277 RID: 631
		private class ResolutionScaleConVar : BaseConVar
		{
			// Token: 0x06000BE9 RID: 3049 RVA: 0x000090CD File Offset: 0x000072CD
			private ResolutionScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000BEA RID: 3050 RVA: 0x0004D714 File Offset: 0x0004B914
			public override void SetString(string newValue)
			{
				float num;
				TextSerialization.TryParseInvariant(newValue, out num);
			}

			// Token: 0x06000BEB RID: 3051 RVA: 0x0000963C File Offset: 0x0000783C
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(CameraResolutionScaler.resolutionScale);
			}

			// Token: 0x04000FD3 RID: 4051
			private static CameraResolutionScaler.ResolutionScaleConVar instance = new CameraResolutionScaler.ResolutionScaleConVar("resolution_scale", ConVarFlags.Archive, null, "Resolution scale. Currently nonfunctional.");
		}
	}
}
