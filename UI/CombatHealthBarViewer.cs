using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CF RID: 1487
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Canvas))]
	public class CombatHealthBarViewer : MonoBehaviour, ILayoutGroup, ILayoutController
	{
		// Token: 0x06002187 RID: 8583 RVA: 0x0001870E File Offset: 0x0001690E
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

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06002188 RID: 8584 RVA: 0x0001872F File Offset: 0x0001692F
		// (set) Token: 0x06002189 RID: 8585 RVA: 0x00018737 File Offset: 0x00016937
		public HealthComponent crosshairTarget { get; set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x0600218A RID: 8586 RVA: 0x00018740 File Offset: 0x00016940
		// (set) Token: 0x0600218B RID: 8587 RVA: 0x00018748 File Offset: 0x00016948
		public GameObject viewerBodyObject { get; set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600218C RID: 8588 RVA: 0x00018751 File Offset: 0x00016951
		// (set) Token: 0x0600218D RID: 8589 RVA: 0x00018759 File Offset: 0x00016959
		public CharacterBody viewerBody { get; set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600218E RID: 8590 RVA: 0x00018762 File Offset: 0x00016962
		// (set) Token: 0x0600218F RID: 8591 RVA: 0x0001876A File Offset: 0x0001696A
		public TeamIndex viewerTeamIndex { get; set; }

		// Token: 0x06002190 RID: 8592 RVA: 0x00018773 File Offset: 0x00016973
		private void Update()
		{
			if (this.crosshairTarget)
			{
				CombatHealthBarViewer.HealthBarInfo healthBarInfo = this.GetHealthBarInfo(this.crosshairTarget);
				healthBarInfo.endTime = Mathf.Max(healthBarInfo.endTime, Time.time + 1f);
			}
			this.SetDirty();
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x000187AF File Offset: 0x000169AF
		private void Awake()
		{
			this.rectTransform = (RectTransform)base.transform;
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x000187CE File Offset: 0x000169CE
		private void Start()
		{
			this.FindCamera();
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x000187D6 File Offset: 0x000169D6
		private void FindCamera()
		{
			this.uiCamera = this.canvas.rootCanvas.worldCamera.GetComponent<UICamera>();
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x000187F3 File Offset: 0x000169F3
		private void OnEnable()
		{
			CombatHealthBarViewer.instancesList.Add(this);
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x000A0FFC File Offset: 0x0009F1FC
		private void OnDisable()
		{
			CombatHealthBarViewer.instancesList.Remove(this);
			for (int i = this.trackedVictims.Count - 1; i >= 0; i--)
			{
				this.Remove(i);
			}
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x00018800 File Offset: 0x00016A00
		private void Remove(int trackedVictimIndex)
		{
			this.Remove(trackedVictimIndex, this.victimToHealthBarInfo[this.trackedVictims[trackedVictimIndex]]);
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x00018820 File Offset: 0x00016A20
		private void Remove(int trackedVictimIndex, CombatHealthBarViewer.HealthBarInfo healthBarInfo)
		{
			this.trackedVictims.RemoveAt(trackedVictimIndex);
			UnityEngine.Object.Destroy(healthBarInfo.healthBarRootObject);
			this.victimToHealthBarInfo.Remove(healthBarInfo.sourceHealthComponent);
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x0001884B File Offset: 0x00016A4B
		private bool VictimIsValid(HealthComponent victim)
		{
			return victim && victim.alive && (this.victimToHealthBarInfo[victim].endTime > Time.time || victim == this.crosshairTarget);
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00018885 File Offset: 0x00016A85
		private void LateUpdate()
		{
			this.CleanUp();
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x000A1034 File Offset: 0x0009F234
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

		// Token: 0x0600219B RID: 8603 RVA: 0x000A1084 File Offset: 0x0009F284
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

		// Token: 0x0600219C RID: 8604 RVA: 0x000A1134 File Offset: 0x0009F334
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

		// Token: 0x0600219D RID: 8605 RVA: 0x000A1188 File Offset: 0x0009F388
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

		// Token: 0x0600219E RID: 8606 RVA: 0x0001888D File Offset: 0x00016A8D
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

		// Token: 0x0600219F RID: 8607 RVA: 0x000A12D4 File Offset: 0x0009F4D4
		private static void LayoutForCamera(UICamera uiCamera)
		{
			Camera camera = uiCamera.camera;
			Camera sceneCam = uiCamera.cameraRigController.sceneCam;
			for (int i = 0; i < CombatHealthBarViewer.instancesList.Count; i++)
			{
				CombatHealthBarViewer.instancesList[i].UpdateAllHealthbarPositions(sceneCam, camera);
			}
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x000188AA File Offset: 0x00016AAA
		public void SetLayoutHorizontal()
		{
			if (this.uiCamera)
			{
				CombatHealthBarViewer.LayoutForCamera(this.uiCamera);
			}
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x000025DA File Offset: 0x000007DA
		public void SetLayoutVertical()
		{
		}

		// Token: 0x040023D2 RID: 9170
		private static readonly List<CombatHealthBarViewer> instancesList = new List<CombatHealthBarViewer>();

		// Token: 0x040023D3 RID: 9171
		public RectTransform container;

		// Token: 0x040023D4 RID: 9172
		public GameObject healthBarPrefab;

		// Token: 0x040023D5 RID: 9173
		public float healthBarDuration;

		// Token: 0x040023DA RID: 9178
		private const float hoverHealthBarDuration = 1f;

		// Token: 0x040023DB RID: 9179
		private RectTransform rectTransform;

		// Token: 0x040023DC RID: 9180
		private Canvas canvas;

		// Token: 0x040023DD RID: 9181
		private UICamera uiCamera;

		// Token: 0x040023DE RID: 9182
		private List<HealthComponent> trackedVictims = new List<HealthComponent>();

		// Token: 0x040023DF RID: 9183
		private Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo> victimToHealthBarInfo = new Dictionary<HealthComponent, CombatHealthBarViewer.HealthBarInfo>();

		// Token: 0x040023E0 RID: 9184
		public float zPosition;

		// Token: 0x040023E1 RID: 9185
		private const float overheadOffset = 1f;

		// Token: 0x020005D0 RID: 1488
		private class HealthBarInfo
		{
			// Token: 0x040023E2 RID: 9186
			public HealthComponent sourceHealthComponent;

			// Token: 0x040023E3 RID: 9187
			public Transform sourceTransform;

			// Token: 0x040023E4 RID: 9188
			public GameObject healthBarRootObject;

			// Token: 0x040023E5 RID: 9189
			public Transform healthBarRootObjectTransform;

			// Token: 0x040023E6 RID: 9190
			public HealthBar healthBar;

			// Token: 0x040023E7 RID: 9191
			public float verticalOffset;

			// Token: 0x040023E8 RID: 9192
			public float endTime = float.NegativeInfinity;
		}
	}
}
