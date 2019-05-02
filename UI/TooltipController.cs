using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000657 RID: 1623
	public class TooltipController : MonoBehaviour
	{
		// Token: 0x06002480 RID: 9344 RVA: 0x000ACAC4 File Offset: 0x000AACC4
		private void SetTooltipProvider(TooltipProvider provider)
		{
			this.titleLabel.text = provider.titleText;
			this.titleLabel.richText = !provider.disableTitleRichText;
			this.bodyLabel.text = provider.bodyText;
			this.bodyLabel.richText = !provider.disableBodyRichText;
			this.colorHighlightImage.color = provider.titleColor;
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x000ACB2C File Offset: 0x000AAD2C
		private static UICamera FindUICamera(MPEventSystem mpEventSystem)
		{
			foreach (UICamera uicamera in UICamera.readOnlyInstancesList)
			{
				if (uicamera.GetAssociatedEventSystem() as MPEventSystem == mpEventSystem)
				{
					return uicamera;
				}
			}
			return null;
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x0001A93F File Offset: 0x00018B3F
		private void Awake()
		{
			TooltipController.instancesList.Add(this);
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x0001A94C File Offset: 0x00018B4C
		private void OnDestroy()
		{
			TooltipController.instancesList.Remove(this);
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x000ACB8C File Offset: 0x000AAD8C
		private void LateUpdate()
		{
			Vector2 v;
			if (this.owner && this.owner.GetCursorPosition(out v))
			{
				this.tooltipCenterTransform.position = v;
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x000ACBC8 File Offset: 0x000AADC8
		public static void RemoveTooltip(TooltipProvider tooltipProvider)
		{
			if (tooltipProvider.userCount > 0)
			{
				foreach (MPEventSystem eventSystem in MPEventSystem.readOnlyInstancesList)
				{
					TooltipController.RemoveTooltip(eventSystem, tooltipProvider);
				}
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x0001A95A File Offset: 0x00018B5A
		public static void RemoveTooltip(MPEventSystem eventSystem, TooltipProvider tooltipProvider)
		{
			if (eventSystem.currentTooltipProvider == tooltipProvider)
			{
				TooltipController.SetTooltip(eventSystem, null, Vector3.zero);
			}
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x000ACC1C File Offset: 0x000AAE1C
		public static void SetTooltip(MPEventSystem eventSystem, TooltipProvider newTooltipProvider, Vector2 tooltipPosition)
		{
			if (eventSystem.currentTooltipProvider != newTooltipProvider)
			{
				if (eventSystem.currentTooltip)
				{
					UnityEngine.Object.Destroy(eventSystem.currentTooltip.gameObject);
					eventSystem.currentTooltip = null;
				}
				if (eventSystem.currentTooltipProvider)
				{
					eventSystem.currentTooltipProvider.userCount--;
				}
				eventSystem.currentTooltipProvider = newTooltipProvider;
				if (newTooltipProvider)
				{
					newTooltipProvider.userCount++;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Tooltip"));
					eventSystem.currentTooltip = gameObject.GetComponent<TooltipController>();
					eventSystem.currentTooltip.owner = eventSystem;
					eventSystem.currentTooltip.uiCamera = TooltipController.FindUICamera(eventSystem);
					eventSystem.currentTooltip.SetTooltipProvider(eventSystem.currentTooltipProvider);
					Canvas component = gameObject.GetComponent<Canvas>();
					UICamera uicamera = eventSystem.currentTooltip.uiCamera;
					component.worldCamera = ((uicamera != null) ? uicamera.camera : null);
				}
			}
			if (eventSystem.currentTooltip)
			{
				Vector2 zero = Vector2.zero;
				UICamera uicamera2 = eventSystem.currentTooltip.uiCamera;
				Camera camera = Camera.main;
				if (uicamera2)
				{
					camera = uicamera2.camera;
				}
				if (camera)
				{
					Vector3 vector = camera.ScreenToViewportPoint(new Vector3(tooltipPosition.x, tooltipPosition.y, 0f));
					zero = new Vector2(vector.x, vector.y);
				}
				Vector2 vector2 = new Vector2(0f, 0f);
				vector2.x = ((zero.x > 0.5f) ? 1f : 0f);
				vector2.y = ((zero.y > 0.5f) ? 1f : 0f);
				eventSystem.currentTooltip.tooltipFlipTransform.anchorMin = vector2;
				eventSystem.currentTooltip.tooltipFlipTransform.anchorMax = vector2;
				eventSystem.currentTooltip.tooltipFlipTransform.pivot = vector2;
			}
		}

		// Token: 0x04002722 RID: 10018
		private static readonly List<TooltipController> instancesList = new List<TooltipController>();

		// Token: 0x04002723 RID: 10019
		[NonSerialized]
		public MPEventSystem owner;

		// Token: 0x04002724 RID: 10020
		public RectTransform tooltipCenterTransform;

		// Token: 0x04002725 RID: 10021
		public RectTransform tooltipFlipTransform;

		// Token: 0x04002726 RID: 10022
		public Image colorHighlightImage;

		// Token: 0x04002727 RID: 10023
		public TextMeshProUGUI titleLabel;

		// Token: 0x04002728 RID: 10024
		public TextMeshProUGUI bodyLabel;

		// Token: 0x04002729 RID: 10025
		private UICamera uiCamera;
	}
}
