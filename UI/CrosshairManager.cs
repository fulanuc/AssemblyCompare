using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005C5 RID: 1477
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairManager : MonoBehaviour
	{
		// Token: 0x06002133 RID: 8499 RVA: 0x00018322 File Offset: 0x00016522
		private void OnEnable()
		{
			CrosshairManager.instancesList.Add(this);
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x0001832F File Offset: 0x0001652F
		private void OnDisable()
		{
			CrosshairManager.instancesList.Remove(this);
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x000A071C File Offset: 0x0009E91C
		private static void StaticLateUpdate()
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager.instancesList[i].DoLateUpdate();
			}
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x000A0750 File Offset: 0x0009E950
		private void DoLateUpdate()
		{
			if (this.cameraRigController)
			{
				this.UpdateCrosshair(this.cameraRigController.target ? this.cameraRigController.target.GetComponent<CharacterBody>() : null, this.cameraRigController.crosshairWorldPosition, this.cameraRigController.uiCam);
			}
			this.UpdateHitMarker();
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x000A07B4 File Offset: 0x0009E9B4
		private void UpdateCrosshair(CharacterBody targetBody, Vector3 crosshairWorldPosition, Camera uiCamera)
		{
			GameObject gameObject = null;
			if (!this.cameraRigController.hasOverride && targetBody && targetBody.healthComponent.alive)
			{
				if (!targetBody.isSprinting)
				{
					gameObject = (targetBody.hideCrosshair ? null : targetBody.crosshairPrefab);
				}
				else
				{
					gameObject = Resources.Load<GameObject>("Prefabs/Crosshair/SprintingCrosshair");
				}
			}
			if (gameObject != this.currentCrosshairPrefab)
			{
				if (this.crosshairController)
				{
					UnityEngine.Object.Destroy(this.crosshairController.gameObject);
					this.crosshairController = null;
				}
				if (gameObject)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, this.container);
					this.crosshairController = gameObject2.GetComponent<CrosshairController>();
					this.crosshairHudElement = gameObject2.GetComponent<HudElement>();
				}
				this.currentCrosshairPrefab = gameObject;
			}
			if (this.crosshairController)
			{
				((RectTransform)this.crosshairController.gameObject.transform).anchoredPosition = new Vector2(0.5f, 0.5f);
			}
			if (this.crosshairHudElement)
			{
				this.crosshairHudElement.targetCharacterBody = targetBody;
			}
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x000A08C4 File Offset: 0x0009EAC4
		public void RefreshHitmarker(bool crit)
		{
			this.hitmarkerTimer = 0.2f;
			this.hitmarker.gameObject.SetActive(false);
			this.hitmarker.gameObject.SetActive(true);
			Util.PlaySound("Play_UI_hit", RoR2Application.instance.gameObject);
			if (crit)
			{
				Util.PlaySound("Play_UI_crit", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x000A092C File Offset: 0x0009EB2C
		private void UpdateHitMarker()
		{
			this.hitmarkerAlpha = Mathf.Pow(this.hitmarkerTimer / 0.2f, 0.75f);
			this.hitmarkerTimer = Mathf.Max(0f, this.hitmarkerTimer - Time.deltaTime);
			if (this.hitmarker)
			{
				Color color = this.hitmarker.color;
				color.a = this.hitmarkerAlpha;
				this.hitmarker.color = color;
			}
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x000A09A4 File Offset: 0x0009EBA4
		private static void HandleHitMarker(DamageDealtMessage damageDealtMessage)
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager crosshairManager = CrosshairManager.instancesList[i];
				if (crosshairManager.cameraRigController && damageDealtMessage.attacker == crosshairManager.cameraRigController.target)
				{
					crosshairManager.RefreshHitmarker(damageDealtMessage.crit);
				}
			}
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x0001833D File Offset: 0x0001653D
		static CrosshairManager()
		{
			GlobalEventManager.onClientDamageNotified += CrosshairManager.HandleHitMarker;
			RoR2Application.onLateUpdate += CrosshairManager.StaticLateUpdate;
		}

		// Token: 0x040023B4 RID: 9140
		[Tooltip("The transform which should act as the container for the crosshair.")]
		public RectTransform container;

		// Token: 0x040023B5 RID: 9141
		public CameraRigController cameraRigController;

		// Token: 0x040023B6 RID: 9142
		[Tooltip("The hitmarker image.")]
		public Image hitmarker;

		// Token: 0x040023B7 RID: 9143
		private float hitmarkerAlpha;

		// Token: 0x040023B8 RID: 9144
		private float hitmarkerTimer;

		// Token: 0x040023B9 RID: 9145
		private const float hitmarkerDuration = 0.2f;

		// Token: 0x040023BA RID: 9146
		private GameObject currentCrosshairPrefab;

		// Token: 0x040023BB RID: 9147
		private CrosshairController crosshairController;

		// Token: 0x040023BC RID: 9148
		private HudElement crosshairHudElement;

		// Token: 0x040023BD RID: 9149
		private static readonly List<CrosshairManager> instancesList = new List<CrosshairManager>();
	}
}
