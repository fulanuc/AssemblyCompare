using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000645 RID: 1605
	public class TooltipController : MonoBehaviour
	{
		// Token: 0x060023F0 RID: 9200 RVA: 0x000AB444 File Offset: 0x000A9644
		private void SetTooltipProvider(TooltipProvider provider)
		{
			this.titleLabel.text = provider.titleText;
			this.titleLabel.richText = !provider.disableTitleRichText;
			this.bodyLabel.text = provider.bodyText;
			this.bodyLabel.richText = !provider.disableBodyRichText;
			this.colorHighlightImage.color = provider.titleColor;
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x000AB4AC File Offset: 0x000A96AC
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

		// Token: 0x060023F2 RID: 9202 RVA: 0x0001A271 File Offset: 0x00018471
		private void Awake()
		{
			TooltipController.instancesList.Add(this);
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x0001A27E File Offset: 0x0001847E
		private void OnDestroy()
		{
			TooltipController.instancesList.Remove(this);
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x000AB50C File Offset: 0x000A970C
		private void LateUpdate()
		{
			Vector2 v;
			if (this.owner && this.owner.GetCursorPosition(out v))
			{
				this.tooltipCenterTransform.position = v;
			}
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x000AB548 File Offset: 0x000A9748
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

		// Token: 0x060023F6 RID: 9206 RVA: 0x0001A28C File Offset: 0x0001848C
		public static void RemoveTooltip(MPEventSystem eventSystem, TooltipProvider tooltipProvider)
		{
			if (eventSystem.currentTooltipProvider == tooltipProvider)
			{
				TooltipController.SetTooltip(eventSystem, null, Vector3.zero);
			}
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x000AB59C File Offset: 0x000A979C
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

		// Token: 0x040026C7 RID: 9927
		private static readonly List<TooltipController> instancesList = new List<TooltipController>();

		// Token: 0x040026C8 RID: 9928
		[NonSerialized]
		public MPEventSystem owner;

		// Token: 0x040026C9 RID: 9929
		public RectTransform tooltipCenterTransform;

		// Token: 0x040026CA RID: 9930
		public RectTransform tooltipFlipTransform;

		// Token: 0x040026CB RID: 9931
		public Image colorHighlightImage;

		// Token: 0x040026CC RID: 9932
		public TextMeshProUGUI titleLabel;

		// Token: 0x040026CD RID: 9933
		public TextMeshProUGUI bodyLabel;

		// Token: 0x040026CE RID: 9934
		private UICamera uiCamera;
	}
}
