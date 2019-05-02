﻿using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C8 RID: 968
	public class RunCameraManager : MonoBehaviour
	{
		// Token: 0x0600151F RID: 5407 RVA: 0x00071CCC File Offset: 0x0006FECC
		private static GameObject GetNetworkUserBodyObject(NetworkUser networkUser)
		{
			if (networkUser.masterObject)
			{
				CharacterMaster component = networkUser.masterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					return component.GetBodyObject();
				}
			}
			return null;
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00071D04 File Offset: 0x0006FF04
		private static TeamIndex GetNetworkUserTeamIndex(NetworkUser networkUser)
		{
			if (networkUser.masterObject)
			{
				CharacterMaster component = networkUser.masterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					return component.teamIndex;
				}
			}
			return TeamIndex.Neutral;
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00071D3C File Offset: 0x0006FF3C
		private void Update()
		{
			if (Stage.instance)
			{
				int num = 0;
				ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
				for (int i = 0; i < readOnlyLocalPlayersList.Count; i++)
				{
					NetworkUser networkUser = readOnlyLocalPlayersList[i];
					CameraRigController cameraRigController = this.cameras[num];
					if (!cameraRigController)
					{
						cameraRigController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Main Camera")).GetComponent<CameraRigController>();
						this.cameras[num] = cameraRigController;
					}
					cameraRigController.viewer = networkUser;
					networkUser.cameraRigController = cameraRigController;
					GameObject networkUserBodyObject = RunCameraManager.GetNetworkUserBodyObject(networkUser);
					if (networkUserBodyObject)
					{
						cameraRigController.target = networkUserBodyObject;
						cameraRigController.cameraMode = CameraRigController.CameraMode.PlayerBasic;
					}
					else if (!cameraRigController.disableSpectating)
					{
						cameraRigController.cameraMode = CameraRigController.CameraMode.SpectateUser;
						if (!cameraRigController.target)
						{
							cameraRigController.target = CameraRigController.GetNextSpectateGameObject(networkUser, null);
						}
					}
					else
					{
						cameraRigController.cameraMode = CameraRigController.CameraMode.None;
					}
					num++;
				}
				int num2 = num;
				for (int j = num; j < this.cameras.Length; j++)
				{
					ref CameraRigController ptr = ref this.cameras[num];
					if (ptr != null)
					{
						if (ptr)
						{
							UnityEngine.Object.Destroy(this.cameras[num].gameObject);
						}
						ptr = null;
					}
				}
				Rect[] array = RunCameraManager.screenLayouts[num2];
				for (int k = 0; k < num2; k++)
				{
					this.cameras[k].viewport = array[k];
				}
				return;
			}
			for (int l = 0; l < this.cameras.Length; l++)
			{
				if (this.cameras[l])
				{
					UnityEngine.Object.Destroy(this.cameras[l].gameObject);
				}
			}
		}

		// Token: 0x0400185D RID: 6237
		private readonly CameraRigController[] cameras = new CameraRigController[RoR2Application.maxLocalPlayers];

		// Token: 0x0400185E RID: 6238
		private static readonly Rect[][] screenLayouts = new Rect[][]
		{
			new Rect[0],
			new Rect[]
			{
				new Rect(0f, 0f, 1f, 1f)
			},
			new Rect[]
			{
				new Rect(0f, 0.5f, 1f, 0.5f),
				new Rect(0f, 0f, 1f, 0.5f)
			},
			new Rect[]
			{
				new Rect(0f, 0.5f, 1f, 0.5f),
				new Rect(0f, 0f, 0.5f, 0.5f),
				new Rect(0.5f, 0f, 0.5f, 0.5f)
			},
			new Rect[]
			{
				new Rect(0f, 0.5f, 0.5f, 0.5f),
				new Rect(0.5f, 0.5f, 0.5f, 0.5f),
				new Rect(0f, 0f, 0.5f, 0.5f),
				new Rect(0.5f, 0f, 0.5f, 0.5f)
			}
		};
	}
}
