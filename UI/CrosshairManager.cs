using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D7 RID: 1495
	[RequireComponent(typeof(RectTransform))]
	public class CrosshairManager : MonoBehaviour
	{
		// Token: 0x060021C4 RID: 8644 RVA: 0x00018A1C File Offset: 0x00016C1C
		private void OnEnable()
		{
			CrosshairManager.instancesList.Add(this);
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x00018A29 File Offset: 0x00016C29
		private void OnDisable()
		{
			CrosshairManager.instancesList.Remove(this);
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x000A1CF0 File Offset: 0x0009FEF0
		private static void StaticLateUpdate()
		{
			for (int i = 0; i < CrosshairManager.instancesList.Count; i++)
			{
				CrosshairManager.instancesList[i].DoLateUpdate();
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x000A1D24 File Offset: 0x0009FF24
		private void DoLateUpdate()
		{
			if (this.cameraRigController)
			{
				this.UpdateCrosshair(this.cameraRigController.target ? this.cameraRigController.target.GetComponent<CharacterBody>() : null, this.cameraRigController.crosshairWorldPosition, this.cameraRigController.uiCam);
			}
			this.UpdateHitMarker();
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x000A1D88 File Offset: 0x0009FF88
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

		// Token: 0x060021C9 RID: 8649 RVA: 0x000A1E98 File Offset: 0x000A0098
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

		// Token: 0x060021CA RID: 8650 RVA: 0x000A1F00 File Offset: 0x000A0100
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

		// Token: 0x060021CB RID: 8651 RVA: 0x000A1F78 File Offset: 0x000A0178
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

		// Token: 0x060021CC RID: 8652 RVA: 0x00018A37 File Offset: 0x00016C37
		static CrosshairManager()
		{
			GlobalEventManager.onClientDamageNotified += CrosshairManager.HandleHitMarker;
			RoR2Application.onLateUpdate += CrosshairManager.StaticLateUpdate;
		}

		// Token: 0x04002408 RID: 9224
		[Tooltip("The transform which should act as the container for the crosshair.")]
		public RectTransform container;

		// Token: 0x04002409 RID: 9225
		public CameraRigController cameraRigController;

		// Token: 0x0400240A RID: 9226
		[Tooltip("The hitmarker image.")]
		public Image hitmarker;

		// Token: 0x0400240B RID: 9227
		private float hitmarkerAlpha;

		// Token: 0x0400240C RID: 9228
		private float hitmarkerTimer;

		// Token: 0x0400240D RID: 9229
		private const float hitmarkerDuration = 0.2f;

		// Token: 0x0400240E RID: 9230
		private GameObject currentCrosshairPrefab;

		// Token: 0x0400240F RID: 9231
		private CrosshairController crosshairController;

		// Token: 0x04002410 RID: 9232
		private HudElement crosshairHudElement;

		// Token: 0x04002411 RID: 9233
		private static readonly List<CrosshairManager> instancesList = new List<CrosshairManager>();
	}
}
