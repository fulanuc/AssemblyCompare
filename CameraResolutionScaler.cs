using System;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000276 RID: 630
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraResolutionScaler : MonoBehaviour
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00009544 File Offset: 0x00007744
		// (set) Token: 0x06000BD7 RID: 3031 RVA: 0x0000954C File Offset: 0x0000774C
		public Camera camera { get; private set; }

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00009555 File Offset: 0x00007755
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x00009563 File Offset: 0x00007763
		private void OnPreRender()
		{
			this.ApplyScalingRenderTexture();
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0000956B File Offset: 0x0000776B
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

		// Token: 0x06000BDB RID: 3035 RVA: 0x000095A6 File Offset: 0x000077A6
		private static void SetResolutionScale(float newResolutionScale)
		{
			if (CameraResolutionScaler.resolutionScale == newResolutionScale)
			{
				return;
			}
			CameraResolutionScaler.resolutionScale = newResolutionScale;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0004D3EC File Offset: 0x0004B5EC
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

		// Token: 0x06000BDD RID: 3037 RVA: 0x000095B7 File Offset: 0x000077B7
		private void OnDestroy()
		{
			if (this.scalingRenderTexture)
			{
				UnityEngine.Object.Destroy(this.scalingRenderTexture);
				this.scalingRenderTexture = null;
			}
		}

		// Token: 0x04000FCA RID: 4042
		private RenderTexture oldRenderTexture;

		// Token: 0x04000FCB RID: 4043
		private static float resolutionScale = 1f;

		// Token: 0x04000FCC RID: 4044
		private RenderTexture scalingRenderTexture;

		// Token: 0x02000277 RID: 631
		private class ResolutionScaleConVar : BaseConVar
		{
			// Token: 0x06000BE0 RID: 3040 RVA: 0x000090A8 File Offset: 0x000072A8
			private ResolutionScaleConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000BE1 RID: 3041 RVA: 0x0004D508 File Offset: 0x0004B708
			public override void SetString(string newValue)
			{
				float num;
				TextSerialization.TryParseInvariant(newValue, out num);
			}

			// Token: 0x06000BE2 RID: 3042 RVA: 0x000095E4 File Offset: 0x000077E4
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(CameraResolutionScaler.resolutionScale);
			}

			// Token: 0x04000FCD RID: 4045
			private static CameraResolutionScaler.ResolutionScaleConVar instance = new CameraResolutionScaler.ResolutionScaleConVar("resolution_scale", ConVarFlags.Archive, null, "Resolution scale. Currently nonfunctional.");
		}
	}
}
