using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000420 RID: 1056
	public class WeatherParticles : MonoBehaviour
	{
		// Token: 0x06001784 RID: 6020 RVA: 0x00011931 File Offset: 0x0000FB31
		static WeatherParticles()
		{
			SceneCamera.onSceneCameraPreRender += WeatherParticles.OnSceneCameraPreRender;
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x0007AA1C File Offset: 0x00078C1C
		private void UpdateForCamera(CameraRigController cameraRigController, bool lockPosition, bool lockRotation)
		{
			Transform transform = cameraRigController.transform;
			base.transform.SetPositionAndRotation(lockPosition ? transform.position : base.transform.position, lockRotation ? transform.rotation : base.transform.rotation);
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x0007AA68 File Offset: 0x00078C68
		private static void OnSceneCameraPreRender(SceneCamera sceneCamera)
		{
			if (sceneCamera.cameraRigController)
			{
				for (int i = 0; i < WeatherParticles.instancesList.Count; i++)
				{
					WeatherParticles weatherParticles = WeatherParticles.instancesList[i];
					weatherParticles.UpdateForCamera(sceneCamera.cameraRigController, weatherParticles.lockPosition, weatherParticles.lockRotation);
				}
			}
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0001194E File Offset: 0x0000FB4E
		private void OnEnable()
		{
			WeatherParticles.instancesList.Add(this);
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0001195B File Offset: 0x0000FB5B
		private void OnDisable()
		{
			WeatherParticles.instancesList.Remove(this);
		}

		// Token: 0x04001A9C RID: 6812
		public bool lockPosition = true;

		// Token: 0x04001A9D RID: 6813
		public bool lockRotation = true;

		// Token: 0x04001A9E RID: 6814
		private static List<WeatherParticles> instancesList = new List<WeatherParticles>();
	}
}
