using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003CD RID: 973
	public class SceneExitController : MonoBehaviour
	{
		// Token: 0x0600152D RID: 5421 RVA: 0x0000FFCB File Offset: 0x0000E1CB
		public void Begin()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (this.exitState == SceneExitController.ExitState.Idle)
			{
				this.SetState(Run.instance.ruleBook.keepMoneyBetweenStages ? SceneExitController.ExitState.TeleportOut : SceneExitController.ExitState.ExtractExp);
			}
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x0007283C File Offset: 0x00070A3C
		public void SetState(SceneExitController.ExitState newState)
		{
			if (newState == this.exitState)
			{
				return;
			}
			this.exitState = newState;
			switch (this.exitState)
			{
			case SceneExitController.ExitState.Idle:
				return;
			case SceneExitController.ExitState.ExtractExp:
				this.experienceCollector = base.gameObject.AddComponent<ConvertPlayerMoneyToExperience>();
				return;
			case SceneExitController.ExitState.TeleportOut:
			{
				ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
				for (int i = 0; i < readOnlyInstancesList.Count; i++)
				{
					CharacterMaster component = readOnlyInstancesList[i].GetComponent<CharacterMaster>();
					if (component.GetComponent<SetDontDestroyOnLoad>())
					{
						GameObject bodyObject = component.GetBodyObject();
						if (bodyObject)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/TeleportOutController"), bodyObject.transform.position, Quaternion.identity);
							gameObject.GetComponent<TeleportOutController>().Networktarget = bodyObject;
							NetworkServer.Spawn(gameObject);
						}
					}
				}
				this.teleportOutTimer = 0f;
				return;
			}
			case SceneExitController.ExitState.Finished:
				if (Run.instance && Run.instance.isGameOverServer)
				{
					return;
				}
				if (this.useRunNextStageScene)
				{
					Stage.instance.BeginAdvanceStage(Run.instance.nextStageScene);
					return;
				}
				if (this.destinationScene)
				{
					Stage.instance.BeginAdvanceStage(this.destinationScene);
					return;
				}
				Debug.Log("SceneExitController: destinationScene not set!");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x00072974 File Offset: 0x00070B74
		private void UpdateServer()
		{
			switch (this.exitState)
			{
			case SceneExitController.ExitState.Idle:
			case SceneExitController.ExitState.Finished:
				break;
			case SceneExitController.ExitState.ExtractExp:
				if (!this.experienceCollector)
				{
					this.SetState(SceneExitController.ExitState.TeleportOut);
					return;
				}
				break;
			case SceneExitController.ExitState.TeleportOut:
				this.teleportOutTimer -= Time.fixedDeltaTime;
				if (this.teleportOutTimer <= 0f)
				{
					this.SetState(SceneExitController.ExitState.Finished);
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x04001860 RID: 6240
		public bool useRunNextStageScene;

		// Token: 0x04001861 RID: 6241
		public SceneField destinationScene;

		// Token: 0x04001862 RID: 6242
		private const float teleportOutDuration = 4f;

		// Token: 0x04001863 RID: 6243
		private float teleportOutTimer;

		// Token: 0x04001864 RID: 6244
		private SceneExitController.ExitState exitState;

		// Token: 0x04001865 RID: 6245
		private ConvertPlayerMoneyToExperience experienceCollector;

		// Token: 0x020003CE RID: 974
		public enum ExitState
		{
			// Token: 0x04001867 RID: 6247
			Idle,
			// Token: 0x04001868 RID: 6248
			ExtractExp,
			// Token: 0x04001869 RID: 6249
			TeleportOut,
			// Token: 0x0400186A RID: 6250
			Finished
		}
	}
}
