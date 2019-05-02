using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000426 RID: 1062
	public class WeatherParticles : MonoBehaviour
	{
		// Token: 0x060017C7 RID: 6087 RVA: 0x00011D5D File Offset: 0x0000FF5D
		static WeatherParticles()
		{
			SceneCamera.onSceneCameraPreRender += WeatherParticles.OnSceneCameraPreRender;
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x0007AFDC File Offset: 0x000791DC
		private void UpdateForCamera(CameraRigController cameraRigController, bool lockPosition, bool lockRotation)
		{
			Transform transform = cameraRigController.transform;
			base.transform.SetPositionAndRotation(lockPosition ? transform.position : base.transform.position, lockRotation ? transform.rotation : base.transform.rotation);
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x0007B028 File Offset: 0x00079228
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

		// Token: 0x060017CA RID: 6090 RVA: 0x00011D7A File Offset: 0x0000FF7A
		private void OnEnable()
		{
			WeatherParticles.instancesList.Add(this);
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x00011D87 File Offset: 0x0000FF87
		private void OnDisable()
		{
			WeatherParticles.instancesList.Remove(this);
		}

		// Token: 0x04001AC5 RID: 6853
		public bool lockPosition = true;

		// Token: 0x04001AC6 RID: 6854
		public bool lockRotation = true;

		// Token: 0x04001AC7 RID: 6855
		private static List<WeatherParticles> instancesList = new List<WeatherParticles>();
	}
}
