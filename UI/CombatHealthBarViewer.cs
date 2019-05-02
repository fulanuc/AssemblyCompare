using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BD RID: 1469
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(RectTransform))]
	public class CombatHealthBarViewer : MonoBehaviour, ILayoutGroup, ILayoutController
	{
		// Token: 0x060020F6 RID: 8438 RVA: 0x00018014 File Offset: 0x00016214
		static CombatHealthBarViewer()
		{
			GlobalEventManager.onClientDamageNotified += delegate(DamageDealtMessage msg)
			{
				if (!msg.victim || msg.isSilent)
				{
					return;
				}
				HealthComponent component = msg.victim.GetComponent<HealthComponent>();
				if (!component || component.dontShowHealthbar)
				{
					return;
				}
				TeamIndex objectTeam = TeamComponent.GetObjectTeam(component.gameObject);
				foreach (CombatHealthBarViewer combatHealthBarViewer in CombatHealthBarViewer.instancesList)
				{
					if (msg.attacker == combatHealthBarViewer.viewerBodyObject && combatHealthBarViewer.viewerBodyObject)
					{
						combatHealthBarViewer.HandleDamage(component, objectTeam);
					}
				}
			};
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060020F7 RID: 8439 RVA: 0x00018035 File Offset: 0x00016235
		// (set) Token: 0x060020F8 RID: 8440 RVA: 0x0001803D File Offset: 0x0001623D
		public HealthComponent crosshairTarget { get; set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x060020F9 RID: 8441 RVA: 0x00018046 File Offset: 0x00016246
		// (set) Token: 0x060020FA RID: 8442 RVA: 0x0001804E File Offset: 0x0001624E
		public GameObject viewerBodyObject { get; set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x060020FB RID: 8443 RVA: 0x00018057 File Offset: 0x00016257
		// (set) Token: 0x060020FC RID: 8444 RVA: 0x0001805F File Offset: 0x0001625F
		public CharacterBody viewerBody { get; set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x060020FD RID: 8445 RVA: 0x00018068 File Offset: 0x00016268
		// (set) Token: 0x060020FE RID: 8446 RVA: 0x00018070 File Offset: 0x00016270
		public TeamIndex viewerTeamIndex { get; set; }

		// Token: 0x060020FF RID: 8447 RVA: 0x00018079 File Offset: 0x00016279
		private void Update()
		{
			if (this.crosshairTarget)
			{
				CombatHealthBarViewer.HealthBarInfo healthBarInfo = this.GetHealthBarInfo(this.crosshairTarget);
				healthBarInfo.endTime = Mathf.Max(healthBarInfo.endTime, Time.time + 1f);
			}
			this.SetDirty();
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x000180B5 File Offset: 0x000162B5
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x000180D4 File Offset: 0x000162D4
		private void Start()
		{
			this.FindCamera();
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000180DC File Offset: 0x000162DC
		private void FindCamera()
		{
			this.uiCamera = this.canvas.rootCanvas.worldCamera.GetComponent<UICamera>();
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x000180F9 File Offset: 0x000162F9
		private void OnEnable()
		{
			CombatHealthBarViewer.instancesList.Add(this);
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x0009FA28 File Offset: 0x0009DC28
		private void OnDisable()
		{
			CombatHealthBarViewer.instancesList.Remove(this);
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				this.Remove(i);
			}
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x00018106 File Offset: 0x00016306
		private void Remove(int trackedVictimIndex)
		{
			this.Remove(trackedVictimIndex, this.victimToHealthBarInfo[this.trackedVictims[trackedVictimIndex]]);
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x00018126 File Offset: 0x00016326
		private void Remove(int trackedVictimIndex, CombatHealthBarViewer.HealthBarInfo healthBarInfo)
		{
			this.trackedVictims.RemoveAt(trackedVictimIndex);
			UnityEngine.Object.Destroy(healthBarInfo.healthBarRootObject);
			this.victimToHealthBarInfo.Remove(healthBarInfo.sourceHealthComponent);
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x00018151 File Offset: 0x00016351
		private bool VictimIsValid(HealthComponent victim)
		{
			return victim && victim.alive && (this.victimToHealthBarInfo[victim].endTime > Time.time || victim == this.crosshairTarget);
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x0001818B File Offset: 0x0001638B
		private void LateUpdate()
		{
			this.CleanUp();
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x0009FA60 File Offset: 0x0009DC60
		private void CleanUp()
		{
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				HealthComponent healthComponent = this.trackedVictims[i];
				if (!this.VictimIsValid(healthComponent))
				{
					this.Remove(i, this.victimToHealthBarInfo[healthComponent]);
				}
			}
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x0009FAB0 File Offset: 0x0009DCB0
		private void UpdateAllHealthbarPositions(Camera sceneCam, Camera uiCam)
		{
			foreach (CombatHealthBarViewer.HealthBarInfo healthBarInfo in this.victimToHealthBarInfo.Values)
			{
				Vector3 position = healthBarInfo.sourceTransform.position;
				position.y += healthBarInfo.verticalOffset;
				Vector3 vector = sceneCam.WorldToScreenPoint(position);
				vector.z = ((vector.z > 0f) ? 1f : -1f);
				Vector3 position2 = uiCam.ScreenToWorldPoint(vector);
				healthBarInfo.healthBarRootObjectTransform.position = position2;
			}
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x0009FB60 File Offset: 0x0009DD60
		private void HandleDamage(HealthComponent victimHealthComponent, TeamIndex victimTeam)
		{
			if (this.viewerTeamIndex == victimTeam || victimTeam == TeamIndex.None)
			{
				return;
			}
			CharacterBody body = victimHealthComponent.body;
			if (body && body.GetVisibilityLevel(this.viewerBody) < VisibilityLevel.Revealed)
			{
				return;
			}
			this.GetHealthBarInfo(victimHealthComponent).endTime = Time.time + this.healthBarDuration;
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x0009FBB4 File Offset: 0x0009DDB4
		private CombatHealthBarViewer.HealthBarInfo GetHealthBarInfo(HealthComponent victimHealthComponent)
		{
			CombatHealthBarViewer.HealthBarInfo healthBarInfo;
			if (!this.victimToHealthBarInfo.TryGetValue(victimHealthComponent, out healthBarInfo))
			{
				healthBarInfo = new CombatHealthBarViewer.HealthBarInfo();
				healthBarInfo.healthBarRootObject = UnityEngine.Object.Instantiate<GameObject>(this.healthBarPrefab, this.container);
				healthBarInfo.healthBarRootObjectTransform = healthBarInfo.healthBarRootObject.transform;
				healthBarInfo.healthBar = healthBarInfo.healthBarRootObject.GetComponent<HealthBar>();
				healthBarInfo.healthBar.source = victimHealthComponent;
				healthBarInfo.healthBarRootObject.GetComponentInChildren<BuffDisplay>().source = victimHealthComponent.body;
				healthBarInfo.sourceHealthComponent = victimHealthComponent;
				healthBarInfo.verticalOffset = 0f;
				Collider component = victimHealthComponent.GetComponent<Collider>();
				if (component)
				{
					healthBarInfo.verticalOffset = component.bounds.extents.y;
				}
				healthBarInfo.sourceTransform = (victimHealthComponent.body.coreTransform ?? victimHealthComponent.transform);
				ModelLocator component2 = victimHealthComponent.GetComponent<ModelLocator>();
				if (component2)
				{
					Transform modelTransform = component2.modelTransform;
					if (modelTransform)
					{
						ChildLocator component3 = modelTransform.GetComponent<ChildLocator>();
						if (component3)
						{
							Transform transform = component3.FindChild("HealthBarOrigin");
							if (transform)
							{
								healthBarInfo.sourceTransform = transform;
								healthBarInfo.verticalOffset = 0f;
							}
						}
					}
				}
				this.victimToHealthBarInfo.Add(victimHealthComponent, healthBarInfo);
				this.trackedVictims.Add(victimHealthComponent);
			}
			return healthBarInfo;
		}

		// Token: 0x0600210D RID: 8461 RVA: 0x00018193 File Offset: 0x00016393
		private void SetDirty()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!CanvasUpdateRegistry.IsRebuildingLayout())
			{
				LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			}
		}

		// Token: 0x0600210E RID: 8462 RVA: 0x0009FD00 File Offset: 0x0009DF00
		private static void LayoutForCamera(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			for (int i = 0; i < CombatHealthBarViewer.instancesList.Count; i++)
			{
				CombatHealthBarViewer.instancesList[i].UpdateAllHealthbarPositions(sceneCam, camera);
			}
		}

		// Token: 0x0600210F RID: 8463 RVA: 0x000181B0 File Offset: 0x000163B0
		public void SetLayoutHorizontal()
		{
			if (this.uiCamera)
			{
				CombatHealthBarViewer.LayoutForCamera(this.uiCamera);
			}
		}

		// Token: 0x06002110 RID: 8464 RVA: 0x000025F6 File Offset: 0x000007F6
		public void SetLayoutVertical()
		{
		}

		// Token: 0x0400237E RID: 9086
		private static readonly List<CombatHealthBarViewer> instancesList = new List<CombatHealthBarViewer>();

		// Token: 0x0400237F RID: 9087
		public RectTransform container;

		// Token: 0x04002380 RID: 9088
		public GameObject healthBarPrefab;

		// Token: 0x04002381 RID: 9089
		public float healthBarDuration;

		// Token: 0x04002386 RID: 9094
		private const float hoverHealthBarDuration = 1f;

		// Token: 0x04002387 RID: 9095
		private RectTransform rectTransform;

		// Token: 0x04002388 RID: 9096
		private Canvas canvas;

		// Token: 0x04002389 RID: 9097
		private UICamera uiCamera;

		// Token: 0x0400238A RID: 9098
		private List<HealthComponent> trackedVictims = new List<HealthComponent>();

		// Token: 0x0400238B RID: 9099
		private Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo> victimToHealthBarInfo = new Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo>();

		// Token: 0x0400238C RID: 9100
		public float zPosition;

		// Token: 0x0400238D RID: 9101
		private const float overheadOffset = 1f;

		// Token: 0x020005BE RID: 1470
		private class HealthBarInfo
		{
			// Token: 0x0400238E RID: 9102
			public HealthComponent sourceHealthComponent;

			// Token: 0x0400238F RID: 9103
			public Transform sourceTransform;

			// Token: 0x04002390 RID: 9104
			public GameObject healthBarRootObject;

			// Token: 0x04002391 RID: 9105
			public Transform healthBarRootObjectTransform;

			// Token: 0x04002392 RID: 9106
			public HealthBar healthBar;

			// Token: 0x04002393 RID: 9107
			public float verticalOffset;

			// Token: 0x04002394 RID: 9108
			public float endTime = float.NegativeInfinity;
		}
	}
}
