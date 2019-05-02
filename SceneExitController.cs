using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D3 RID: 979
	public class SceneExitController : MonoBehaviour
	{
		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06001558 RID: 5464 RVA: 0x0001023B File Offset: 0x0000E43B
		// (set) Token: 0x06001559 RID: 5465 RVA: 0x00010242 File Offset: 0x0000E442
		public static bool isRunning { get; private set; }

		// Token: 0x0600155A RID: 5466 RVA: 0x0001024A File Offset: 0x0000E44A
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

		// Token: 0x0600155B RID: 5467 RVA: 0x00072CA4 File Offset: 0x00070EA4
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
				break;
			case SceneExitController.ExitState.ExtractExp:
				SceneExitController.isRunning = true;
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
				this.teleportOutTimer = 4f;
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
				break;
			default:
				return;
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00010277 File Offset: 0x0000E477
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
			}
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00072DE0 File Offset: 0x00070FE0
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

		// Token: 0x0600155E RID: 5470 RVA: 0x00010286 File Offset: 0x0000E486
		private void OnDestroy()
		{
			SceneExitController.isRunning = false;
		}

		// Token: 0x04001882 RID: 6274
		public bool useRunNextStageScene;

		// Token: 0x04001883 RID: 6275
		public SceneField destinationScene;

		// Token: 0x04001884 RID: 6276
		private const float teleportOutDuration = 4f;

		// Token: 0x04001885 RID: 6277
		private float teleportOutTimer;

		// Token: 0x04001886 RID: 6278
		private SceneExitController.ExitState exitState;

		// Token: 0x04001887 RID: 6279
		private ConvertPlayerMoneyToExperience experienceCollector;

		// Token: 0x020003D4 RID: 980
		public enum ExitState
		{
			// Token: 0x04001889 RID: 6281
			Idle,
			// Token: 0x0400188A RID: 6282
			ExtractExp,
			// Token: 0x0400188B RID: 6283
			TeleportOut,
			// Token: 0x0400188C RID: 6284
			Finished
		}
	}
}
