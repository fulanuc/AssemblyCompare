using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000438 RID: 1080
	public static class GamepadVibrationManager
	{
		// Token: 0x0600183B RID: 6203 RVA: 0x00012236 File Offset: 0x00010436
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onUpdate += GamepadVibrationManager.Update;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x0007D8F0 File Offset: 0x0007BAF0
		private static void Update()
		{
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			int count = joysticks.Count;
			if (GamepadVibrationManager.motorValuesBuffer.Length < count)
			{
				Array.Resize<GamepadVibrationManager.MotorValues>(ref GamepadVibrationManager.motorValuesBuffer, count);
			}
			ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
			int count2 = readOnlyInstancesList.Count;
			if (Time.deltaTime != 0f)
			{
				for (int i = 0; i < count2; i++)
				{
					CameraRigController cameraRigController = readOnlyInstancesList[i];
					if (cameraRigController.localUserViewer != null && cameraRigController.localUserViewer.eventSystem && cameraRigController.localUserViewer.eventSystem.currentInputSource == MPEventSystem.InputSource.Gamepad)
					{
						int num = -1;
						Player inputPlayer = cameraRigController.localUserViewer.inputPlayer;
						Controller controller = (inputPlayer != null) ? inputPlayer.controllers.GetLastActiveController<Joystick>() : null;
						if (controller != null)
						{
							for (int j = 0; j < count; j++)
							{
								if (joysticks[j] == controller)
								{
									num = j;
									break;
								}
							}
						}
						if (num != -1)
						{
							UserProfile userProfile = cameraRigController.localUserViewer.userProfile;
							GamepadVibrationManager.MotorValues motorValues = GamepadVibrationManager.CalculateMotorValuesForCameraDisplacement((userProfile != null) ? userProfile.gamepadVibrationScale : 0f, cameraRigController.rawScreenShakeDisplacement);
							GamepadVibrationManager.motorValuesBuffer[num] = motorValues;
						}
					}
				}
			}
			for (int k = 0; k < count; k++)
			{
				Joystick joystick = joysticks[k];
				GamepadVibrationManager.MotorValues motorValues2 = GamepadVibrationManager.motorValuesBuffer[k];
				joystick.SetVibration(0, motorValues2.motor0);
				joystick.SetVibration(1, motorValues2.motor1);
				GamepadVibrationManager.motorValuesBuffer[k] = default(GamepadVibrationManager.MotorValues);
			}
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0007DA74 File Offset: 0x0007BC74
		private static GamepadVibrationManager.MotorValues CalculateMotorValuesForCameraDisplacement(float userScale, Vector3 cameraDisplacement)
		{
			float magnitude = cameraDisplacement.magnitude;
			return new GamepadVibrationManager.MotorValues
			{
				deepMotor = magnitude * userScale / 5f,
				quickMotor = magnitude * userScale
			};
		}

		// Token: 0x04001B66 RID: 7014
		private static GamepadVibrationManager.MotorValues[] motorValuesBuffer = new GamepadVibrationManager.MotorValues[4];

		// Token: 0x04001B67 RID: 7015
		private const float deepRumbleFactor = 5f;

		// Token: 0x02000439 RID: 1081
		private struct MotorValues
		{
			// Token: 0x17000231 RID: 561
			// (get) Token: 0x0600183F RID: 6207 RVA: 0x00012256 File Offset: 0x00010456
			// (set) Token: 0x06001840 RID: 6208 RVA: 0x0001225E File Offset: 0x0001045E
			public float deepMotor
			{
				get
				{
					return this.motor0;
				}
				set
				{
					this.motor0 = value;
				}
			}

			// Token: 0x17000232 RID: 562
			// (get) Token: 0x06001841 RID: 6209 RVA: 0x00012267 File Offset: 0x00010467
			// (set) Token: 0x06001842 RID: 6210 RVA: 0x0001226F File Offset: 0x0001046F
			public float quickMotor
			{
				get
				{
					return this.motor1;
				}
				set
				{
					this.motor1 = value;
				}
			}

			// Token: 0x04001B68 RID: 7016
			public float motor0;

			// Token: 0x04001B69 RID: 7017
			public float motor1;
		}
	}
}
