using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D9 RID: 985
	[ExecuteInEditMode]
	public class SceneWeatherController : MonoBehaviour
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001577 RID: 5495 RVA: 0x000103D1 File Offset: 0x0000E5D1
		public static SceneWeatherController instance
		{
			get
			{
				return SceneWeatherController._instance;
			}
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x000103D8 File Offset: 0x0000E5D8
		private void OnEnable()
		{
			if (!SceneWeatherController._instance)
			{
				SceneWeatherController._instance = this;
			}
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x000103EC File Offset: 0x0000E5EC
		private void OnDisable()
		{
			if (SceneWeatherController._instance == this)
			{
				SceneWeatherController._instance = null;
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00073288 File Offset: 0x00071488
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

		// Token: 0x0600157B RID: 5499 RVA: 0x00073350 File Offset: 0x00071550
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

		// Token: 0x040018A3 RID: 6307
		private static SceneWeatherController _instance;

		// Token: 0x040018A4 RID: 6308
		public SceneWeatherController.WeatherParams initialWeatherParams;

		// Token: 0x040018A5 RID: 6309
		public SceneWeatherController.WeatherParams finalWeatherParams;

		// Token: 0x040018A6 RID: 6310
		public Light sun;

		// Token: 0x040018A7 RID: 6311
		public Material fogMaterial;

		// Token: 0x040018A8 RID: 6312
		public string rtpcWeather;

		// Token: 0x040018A9 RID: 6313
		public float rtpcMin;

		// Token: 0x040018AA RID: 6314
		public float rtpcMax = 100f;

		// Token: 0x040018AB RID: 6315
		public AnimationCurve weatherLerpOverChargeTime;

		// Token: 0x040018AC RID: 6316
		[Range(0f, 1f)]
		public float weatherLerp;

		// Token: 0x020003DA RID: 986
		[Serializable]
		public struct WeatherParams
		{
			// Token: 0x040018AD RID: 6317
			[ColorUsage(true, true, 1f, 5f, 1f, 5f)]
			public Color sunColor;

			// Token: 0x040018AE RID: 6318
			public float sunIntensity;

			// Token: 0x040018AF RID: 6319
			public float fogStart;

			// Token: 0x040018B0 RID: 6320
			public float fogScale;

			// Token: 0x040018B1 RID: 6321
			public float fogIntensity;
		}
	}
}
