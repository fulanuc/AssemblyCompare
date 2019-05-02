using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D3 RID: 979
	[ExecuteInEditMode]
	public class SceneWeatherController : MonoBehaviour
	{
		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06001549 RID: 5449 RVA: 0x0001014A File Offset: 0x0000E34A
		public static SceneWeatherController instance
		{
			get
			{
				return SceneWeatherController._instance;
			}
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00010151 File Offset: 0x0000E351
		private void OnEnable()
		{
			if (!SceneWeatherController._instance)
			{
				SceneWeatherController._instance = this;
			}
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00010165 File Offset: 0x0000E365
		private void OnDisable()
		{
			if (SceneWeatherController._instance == this)
			{
				SceneWeatherController._instance = null;
			}
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00072E1C File Offset: 0x0007101C
		private SceneWeatherController.WeatherParams GetWeatherParams(float t)
		{
			return new SceneWeatherController.WeatherParams
			{
				sunColor = Color.Lerp(this.initialWeatherParams.sunColor, this.finalWeatherParams.sunColor, t),
				sunIntensity = Mathf.Lerp(this.initialWeatherParams.sunIntensity, this.finalWeatherParams.sunIntensity, t),
				fogStart = Mathf.Lerp(this.initialWeatherParams.fogStart, this.finalWeatherParams.fogStart, t),
				fogScale = Mathf.Lerp(this.initialWeatherParams.fogScale, this.finalWeatherParams.fogScale, t),
				fogIntensity = Mathf.Lerp(this.initialWeatherParams.fogIntensity, this.finalWeatherParams.fogIntensity, t)
			};
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x00072EE4 File Offset: 0x000710E4
		private void Update()
		{
			SceneWeatherController.WeatherParams weatherParams = this.GetWeatherParams(this.weatherLerp);
			if (this.sun)
			{
				this.sun.color = weatherParams.sunColor;
				this.sun.intensity = weatherParams.sunIntensity;
			}
			if (this.fogMaterial)
			{
				this.fogMaterial.SetFloat("_FogPicker", this.weatherLerp);
				this.fogMaterial.SetFloat("_FogStart", weatherParams.fogStart);
				this.fogMaterial.SetFloat("_FogScale", weatherParams.fogScale);
				this.fogMaterial.SetFloat("_FogIntensity", weatherParams.fogIntensity);
			}
			if (true && this.rtpcWeather.Length != 0)
			{
				AkSoundEngine.SetRTPCValue(this.rtpcWeather, Mathf.Lerp(this.rtpcMin, this.rtpcMax, this.weatherLerp), base.gameObject);
			}
		}

		// Token: 0x04001881 RID: 6273
		private static SceneWeatherController _instance;

		// Token: 0x04001882 RID: 6274
		public SceneWeatherController.WeatherParams initialWeatherParams;

		// Token: 0x04001883 RID: 6275
		public SceneWeatherController.WeatherParams finalWeatherParams;

		// Token: 0x04001884 RID: 6276
		public Light sun;

		// Token: 0x04001885 RID: 6277
		public Material fogMaterial;

		// Token: 0x04001886 RID: 6278
		public string rtpcWeather;

		// Token: 0x04001887 RID: 6279
		public float rtpcMin;

		// Token: 0x04001888 RID: 6280
		public float rtpcMax = 100f;

		// Token: 0x04001889 RID: 6281
		public AnimationCurve weatherLerpOverChargeTime;

		// Token: 0x0400188A RID: 6282
		[Range(0f, 1f)]
		public float weatherLerp;

		// Token: 0x020003D4 RID: 980
		[Serializable]
		public struct WeatherParams
		{
			// Token: 0x0400188B RID: 6283
			[ColorUsage(true, true, 1f, 5f, 1f, 5f)]
			public Color sunColor;

			// Token: 0x0400188C RID: 6284
			public float sunIntensity;

			// Token: 0x0400188D RID: 6285
			public float fogStart;

			// Token: 0x0400188E RID: 6286
			public float fogScale;

			// Token: 0x0400188F RID: 6287
			public float fogIntensity;
		}
	}
}
