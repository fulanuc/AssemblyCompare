using System;
using System.Collections.ObjectModel;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000367 RID: 871
	public class MusicController : MonoBehaviour
	{
		// Token: 0x060011FD RID: 4605 RVA: 0x0000DAE6 File Offset: 0x0000BCE6
		private void RefreshStageInfo(Scene a, Scene b)
		{
			this.stageInfo = default(MusicController.StageInfo);
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x0000DAF4 File Offset: 0x0000BCF4
		private void Start()
		{
			this.enemyInfoBuffer = new NativeArray<MusicController.EnemyInfo>(64, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			SceneManager.activeSceneChanged += this.RefreshStageInfo;
			if (this.enableMusicSystem)
			{
				AkSoundEngine.PostEvent("Play_Music_System", base.gameObject);
			}
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00067C88 File Offset: 0x00065E88
		private void Update()
		{
			this.UpdateState();
			this.targetCamera = ((CameraRigController.readOnlyInstancesList.Count > 0) ? CameraRigController.readOnlyInstancesList[0] : null);
			this.target = (this.targetCamera ? this.targetCamera.target : null);
			this.ScheduleIntensityCalculation(this.target);
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00067CEC File Offset: 0x00065EEC
		private void RecalculateHealth(GameObject playerObject)
		{
			this.rtpcPlayerHealthValue = 1f;
			if (this.target)
			{
				HealthComponent component = this.target.GetComponent<HealthComponent>();
				if (component)
				{
					this.rtpcPlayerHealthValue = component.combinedHealthFraction;
				}
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00067D34 File Offset: 0x00065F34
		private void UpdateTeleporterParameters(TeleporterInteraction teleporter)
		{
			float num = 0.5f;
			this.rtpcTeleporterProximityValue = float.PositiveInfinity;
			this.rtpcTeleporterDirectionValue = 0f;
			this.rtpcTeleporterCharged = 100f;
			if (teleporter)
			{
				if (this.targetCamera)
				{
					Vector3 position = this.targetCamera.transform.position;
					Vector3 forward = this.targetCamera.transform.forward;
					Vector3 vector = teleporter.transform.position - position;
					float num2 = Vector2.SignedAngle(new Vector2(vector.x, vector.z), new Vector2(forward.x, forward.z));
					if (num2 < 0f)
					{
						num2 += 360f;
					}
					this.rtpcTeleporterProximityValue = vector.magnitude;
					this.rtpcTeleporterDirectionValue = num2;
				}
				this.rtpcTeleporterProximityValue /= num;
				this.rtpcTeleporterCharged = teleporter.chargeFraction * 90f / num;
			}
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00067E2C File Offset: 0x0006602C
		private void LateUpdate()
		{
			bool flag = Time.timeScale == 0f;
			if (this.wasPaused != flag)
			{
				AkSoundEngine.PostEvent(flag ? "Pause_Music" : "Unpause_Music", base.gameObject);
				this.wasPaused = flag;
			}
			this.RecalculateHealth(this.target);
			this.UpdateTeleporterParameters(TeleporterInteraction.instance);
			this.calculateIntensityJobHandle.Complete();
			float num;
			float num2;
			this.calculateIntensityJob.CalculateSum(out num, out num2);
			float num3 = 0.025f;
			Mathf.Clamp(1f - this.rtpcPlayerHealthValue, 0.25f, 0.75f);
			float num4 = (num * 0.75f + num2 * 0.25f) * num3;
			this.rtpcEnemyValue = num4;
			this.UpdateRTPCValues();
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x00067EE8 File Offset: 0x000660E8
		private void UpdateRTPCValues()
		{
			AkSoundEngine.SetRTPCValue("playerHealth", this.rtpcPlayerHealthValue * 100f);
			AkSoundEngine.SetRTPCValue("teleporterProximity", this.rtpcTeleporterProximityValue);
			AkSoundEngine.SetRTPCValue("teleporterDirection", this.rtpcTeleporterDirectionValue);
			AkSoundEngine.SetRTPCValue("teleporterPlayerStatus", this.rtpcTeleporterPlayerStatus);
			AkSoundEngine.SetRTPCValue("enemyValue", this.rtpcEnemyValue);
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x00067F50 File Offset: 0x00066150
		private void UpdateState()
		{
			string in_pszState = "None";
			string in_pszState2 = "None";
			string in_pszState3 = "None";
			string in_pszState4 = "None";
			this.rtpcTeleporterPlayerStatus = 1f;
			SceneDef mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
			if (mostRecentSceneDef)
			{
				string sceneName = mostRecentSceneDef.sceneName;
				if (!(sceneName == "title"))
				{
					if (!(sceneName == "lobby"))
					{
						if (!(sceneName == "logbook"))
						{
							if (!(sceneName == "crystalworld"))
							{
								if (!(sceneName == "bazaar"))
								{
									in_pszState2 = "Gameplay";
									if (mostRecentSceneDef)
									{
										in_pszState = mostRecentSceneDef.songName;
									}
									if (TeleporterInteraction.instance && !TeleporterInteraction.instance.isIdle)
									{
										in_pszState = mostRecentSceneDef.bossSongName;
										in_pszState4 = "alive";
										in_pszState2 = "Bossfight";
										if (TeleporterInteraction.instance.isIdleToCharging || TeleporterInteraction.instance.isCharging)
										{
											if (this.target)
											{
												this.rtpcTeleporterPlayerStatus = 0f;
												if (TeleporterInteraction.instance.IsInChargingRange(this.target))
												{
													this.rtpcTeleporterPlayerStatus = 1f;
												}
											}
										}
										else if (TeleporterInteraction.instance.isCharged)
										{
											in_pszState4 = "dead";
										}
									}
								}
								else
								{
									in_pszState2 = "SecretLevel";
								}
							}
							else
							{
								in_pszState2 = "Menu";
								in_pszState3 = "Logbook";
							}
						}
						else
						{
							in_pszState2 = "Menu";
							in_pszState3 = "Logbook";
						}
					}
					else
					{
						in_pszState2 = "Menu";
						in_pszState3 = "Main";
					}
				}
				else
				{
					in_pszState2 = "Menu";
					in_pszState3 = "Main";
				}
			}
			AkSoundEngine.SetState("Music_system", in_pszState2);
			AkSoundEngine.SetState("gameplaySongChoice", in_pszState);
			AkSoundEngine.SetState("Music_menu", in_pszState3);
			AkSoundEngine.SetState("bossStatus", in_pszState4);
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x0000DB2F File Offset: 0x0000BD2F
		private void EnsureEnemyBufferSize(int requiredSize)
		{
			if (this.enemyInfoBuffer.Length < requiredSize)
			{
				if (this.enemyInfoBuffer.Length != 0)
				{
					this.enemyInfoBuffer.Dispose();
				}
				this.enemyInfoBuffer = new NativeArray<MusicController.EnemyInfo>(requiredSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0000DB65 File Offset: 0x0000BD65
		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= this.RefreshStageInfo;
			this.enemyInfoBuffer.Dispose();
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0006810C File Offset: 0x0006630C
		private void ScheduleIntensityCalculation(GameObject targetBodyObject)
		{
			if (!targetBodyObject)
			{
				return;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
			int count = teamMembers.Count;
			this.EnsureEnemyBufferSize(count);
			int num = 0;
			int i = 0;
			int num2 = count;
			while (i < num2)
			{
				TeamComponent teamComponent = teamMembers[i];
				InputBankTest component = teamComponent.GetComponent<InputBankTest>();
				CharacterBody component2 = teamComponent.GetComponent<CharacterBody>();
				if (component)
				{
					this.enemyInfoBuffer[num++] = new MusicController.EnemyInfo
					{
						aimRay = new Ray(component.aimOrigin, component.aimDirection),
						threatScore = (component2.master ? component2.GetNormalizedThreatValue() : 0f)
					};
				}
				i++;
			}
			this.calculateIntensityJob = new MusicController.CalculateIntensityJob
			{
				enemyInfoBuffer = this.enemyInfoBuffer,
				elementCount = num,
				targetPosition = targetBodyObject.transform.position,
				nearDistance = 20f,
				farDistance = 75f
			};
			this.calculateIntensityJobHandle = this.calculateIntensityJob.Schedule(num, 32, default(JobHandle));
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x0000DB83 File Offset: 0x0000BD83
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(delegate()
			{
				UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MusicController"), RoR2Application.instance.transform);
			}));
		}

		// Token: 0x04001603 RID: 5635
		public GameObject target;

		// Token: 0x04001604 RID: 5636
		public bool enableMusicSystem = true;

		// Token: 0x04001605 RID: 5637
		private CameraRigController targetCamera;

		// Token: 0x04001606 RID: 5638
		private float rtpcPlayerHealthValue;

		// Token: 0x04001607 RID: 5639
		private float rtpcEnemyValue;

		// Token: 0x04001608 RID: 5640
		private float rtpcTeleporterProximityValue;

		// Token: 0x04001609 RID: 5641
		private float rtpcTeleporterDirectionValue;

		// Token: 0x0400160A RID: 5642
		private float rtpcTeleporterCharged;

		// Token: 0x0400160B RID: 5643
		private float rtpcTeleporterPlayerStatus;

		// Token: 0x0400160C RID: 5644
		private MusicController.StageInfo stageInfo;

		// Token: 0x0400160D RID: 5645
		private bool wasPaused;

		// Token: 0x0400160E RID: 5646
		private NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

		// Token: 0x0400160F RID: 5647
		private MusicController.CalculateIntensityJob calculateIntensityJob;

		// Token: 0x04001610 RID: 5648
		private JobHandle calculateIntensityJobHandle;

		// Token: 0x02000368 RID: 872
		private struct StageInfo
		{
			// Token: 0x04001611 RID: 5649
			public bool inAction;

			// Token: 0x04001612 RID: 5650
			public bool inIntro;
		}

		// Token: 0x02000369 RID: 873
		private struct EnemyInfo
		{
			// Token: 0x04001613 RID: 5651
			public Ray aimRay;

			// Token: 0x04001614 RID: 5652
			public float lookScore;

			// Token: 0x04001615 RID: 5653
			public float proximityScore;

			// Token: 0x04001616 RID: 5654
			public float threatScore;
		}

		// Token: 0x0200036A RID: 874
		private struct CalculateIntensityJob : IJobParallelFor
		{
			// Token: 0x0600120A RID: 4618 RVA: 0x0006822C File Offset: 0x0006642C
			public void Execute(int i)
			{
				MusicController.EnemyInfo enemyInfo = this.enemyInfoBuffer[i];
				Vector3 a = this.targetPosition - enemyInfo.aimRay.origin;
				float magnitude = a.magnitude;
				float num = Mathf.Clamp01(Vector3.Dot(a / magnitude, enemyInfo.aimRay.direction));
				float num2 = Mathf.Clamp01(Mathf.InverseLerp(this.farDistance, this.nearDistance, magnitude));
				enemyInfo.lookScore = num * enemyInfo.threatScore;
				enemyInfo.proximityScore = num2 * enemyInfo.threatScore;
				this.enemyInfoBuffer[i] = enemyInfo;
			}

			// Token: 0x0600120B RID: 4619 RVA: 0x000682CC File Offset: 0x000664CC
			public void CalculateSum(out float proximityScore, out float lookScore)
			{
				proximityScore = 0f;
				lookScore = 0f;
				for (int i = 0; i < this.elementCount; i++)
				{
					proximityScore += this.enemyInfoBuffer[i].proximityScore;
					lookScore += this.enemyInfoBuffer[i].lookScore;
				}
			}

			// Token: 0x04001617 RID: 5655
			[ReadOnly]
			public Vector3 targetPosition;

			// Token: 0x04001618 RID: 5656
			[ReadOnly]
			public int elementCount;

			// Token: 0x04001619 RID: 5657
			public NativeArray<MusicController.EnemyInfo> enemyInfoBuffer;

			// Token: 0x0400161A RID: 5658
			[ReadOnly]
			public float nearDistance;

			// Token: 0x0400161B RID: 5659
			[ReadOnly]
			public float farDistance;
		}
	}
}
