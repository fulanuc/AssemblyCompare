using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F0 RID: 1520
	[RequireComponent(typeof(Canvas))]
	public class HighlightRect : MonoBehaviour
	{
		// Token: 0x0600223F RID: 8767 RVA: 0x00018F05 File Offset: 0x00017105
		static HighlightRect()
		{
			RoR2Application.onLateUpdate += HighlightRect.UpdateAll;
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x00018F2D File Offset: 0x0001712D
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x00018F3B File Offset: 0x0001713B
		private void OnEnable()
		{
			HighlightRect.instancesList.Add(this);
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x00018F48 File Offset: 0x00017148
		private void OnDisable()
		{
			HighlightRect.instancesList.Remove(this);
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x000A4A90 File Offset: 0x000A2C90
		private void Start()
		{
			this.highlightState = HighlightRect.HighlightState.Expanding;
			this.bottomLeftImage = this.bottomLeftRectTransform.GetComponent<Image>();
			this.bottomRightImage = this.bottomRightRectTransform.GetComponent<Image>();
			this.topLeftImage = this.topLeftRectTransform.GetComponent<Image>();
			this.topRightImage = this.topRightRectTransform.GetComponent<Image>();
			this.bottomLeftImage.sprite = this.cornerImage;
			this.bottomRightImage.sprite = this.cornerImage;
			this.topLeftImage.sprite = this.cornerImage;
			this.topRightImage.sprite = this.cornerImage;
			this.bottomLeftImage.color = this.highlightColor;
			this.bottomRightImage.color = this.highlightColor;
			this.topLeftImage.color = this.highlightColor;
			this.topRightImage.color = this.highlightColor;
			if (this.nametagRectTransform)
			{
				this.nametagText = this.nametagRectTransform.GetComponent<TextMeshProUGUI>();
				this.nametagText.color = this.highlightColor;
				this.nametagText.text = this.nametagString;
			}
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x000A4BB0 File Offset: 0x000A2DB0
		private static void UpdateAll()
		{
			for (int i = HighlightRect.instancesList.Count - 1; i >= 0; i--)
			{
				HighlightRect.instancesList[i].DoUpdate();
			}
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x000A4BE4 File Offset: 0x000A2DE4
		private void DoUpdate()
		{
			if (!this.targetRenderer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			switch (this.highlightState)
			{
			case HighlightRect.HighlightState.Expanding:
				this.time += Time.deltaTime;
				if (this.time >= this.expandTime)
				{
					this.time = this.expandTime;
					this.highlightState = HighlightRect.HighlightState.Holding;
				}
				break;
			case HighlightRect.HighlightState.Holding:
				if (this.destroyOnLifeEnd)
				{
					this.time += Time.deltaTime;
					if (this.time > this.maxLifeTime)
					{
						this.highlightState = HighlightRect.HighlightState.Contracting;
						this.time = this.expandTime;
					}
				}
				break;
			case HighlightRect.HighlightState.Contracting:
				this.time -= Time.deltaTime;
				if (this.time <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				break;
			}
			Rect rect = HighlightRect.GUIRectWithObject(this.sceneCam, this.targetRenderer);
			Vector2 a = new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, 0.5f), Mathf.Lerp(rect.yMin, rect.yMax, 0.5f));
			float t = this.curve.Evaluate(this.time / this.expandTime);
			this.bottomLeftRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMin), t);
			this.bottomRightRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMax, rect.yMin), t);
			this.topLeftRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMax), t);
			this.topRightRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMax, rect.yMax), t);
			if (this.nametagRectTransform)
			{
				this.nametagRectTransform.anchoredPosition = Vector2.LerpUnclamped(a, new Vector2(rect.xMin, rect.yMax), t);
			}
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x000A4DF0 File Offset: 0x000A2FF0
		public static Rect GUIRectWithObject(Camera cam, Renderer rend)
		{
			Vector3 center = rend.bounds.center;
			Vector3 extents = rend.bounds.extents;
			HighlightRect.extentPoints[0] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z));
			HighlightRect.extentPoints[1] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z));
			HighlightRect.extentPoints[2] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z));
			HighlightRect.extentPoints[3] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z));
			HighlightRect.extentPoints[4] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z));
			HighlightRect.extentPoints[5] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z));
			HighlightRect.extentPoints[6] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z));
			HighlightRect.extentPoints[7] = HighlightRect.WorldToGUIPoint(cam, new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z));
			Vector2 vector = HighlightRect.extentPoints[0];
			Vector2 vector2 = HighlightRect.extentPoints[0];
			foreach (Vector2 rhs in HighlightRect.extentPoints)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x00018F56 File Offset: 0x00017156
		public static Vector2 WorldToGUIPoint(Camera cam, Vector3 world)
		{
			return cam.WorldToScreenPoint(world);
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x000A5080 File Offset: 0x000A3280
		public static void CreateHighlight(GameObject viewerBodyObject, Renderer targetRenderer, GameObject highlightPrefab, float overrideDuration = -1f, bool visibleToAll = false)
		{
			ReadOnlyCollection<CameraRigController> readOnlyInstancesList = CameraRigController.readOnlyInstancesList;
			int i = 0;
			int count = readOnlyInstancesList.Count;
			while (i < count)
			{
				CameraRigController cameraRigController = readOnlyInstancesList[i];
				if (!(cameraRigController.target != viewerBodyObject) || visibleToAll)
				{
					HighlightRect component = UnityEngine.Object.Instantiate<GameObject>(highlightPrefab).GetComponent<HighlightRect>();
					component.targetRenderer = targetRenderer;
					component.canvas.worldCamera = cameraRigController.uiCam;
					component.uiCam = cameraRigController.uiCam;
					component.sceneCam = cameraRigController.sceneCam;
					if (overrideDuration > 0f)
					{
						component.maxLifeTime = overrideDuration;
					}
				}
				i++;
			}
		}

		// Token: 0x040024DA RID: 9434
		public AnimationCurve curve;

		// Token: 0x040024DB RID: 9435
		public Color highlightColor;

		// Token: 0x040024DC RID: 9436
		public Sprite cornerImage;

		// Token: 0x040024DD RID: 9437
		public string nametagString;

		// Token: 0x040024DE RID: 9438
		private Image bottomLeftImage;

		// Token: 0x040024DF RID: 9439
		private Image bottomRightImage;

		// Token: 0x040024E0 RID: 9440
		private Image topLeftImage;

		// Token: 0x040024E1 RID: 9441
		private Image topRightImage;

		// Token: 0x040024E2 RID: 9442
		private TextMeshProUGUI nametagText;

		// Token: 0x040024E3 RID: 9443
		public Renderer targetRenderer;

		// Token: 0x040024E4 RID: 9444
		public GameObject cameraTarget;

		// Token: 0x040024E5 RID: 9445
		public RectTransform nametagRectTransform;

		// Token: 0x040024E6 RID: 9446
		public RectTransform bottomLeftRectTransform;

		// Token: 0x040024E7 RID: 9447
		public RectTransform bottomRightRectTransform;

		// Token: 0x040024E8 RID: 9448
		public RectTransform topLeftRectTransform;

		// Token: 0x040024E9 RID: 9449
		public RectTransform topRightRectTransform;

		// Token: 0x040024EA RID: 9450
		public float expandTime = 1f;

		// Token: 0x040024EB RID: 9451
		public float maxLifeTime;

		// Token: 0x040024EC RID: 9452
		public bool destroyOnLifeEnd;

		// Token: 0x040024ED RID: 9453
		private float time;

		// Token: 0x040024EE RID: 9454
		public HighlightRect.HighlightState highlightState;

		// Token: 0x040024EF RID: 9455
		private static List<HighlightRect> instancesList = new List<HighlightRect>();

		// Token: 0x040024F0 RID: 9456
		private Canvas canvas;

		// Token: 0x040024F1 RID: 9457
		private Camera uiCam;

		// Token: 0x040024F2 RID: 9458
		private Camera sceneCam;

		// Token: 0x040024F3 RID: 9459
		private static readonly Vector2[] extentPoints = new Vector2[8];

		// Token: 0x020005F1 RID: 1521
		public enum HighlightState
		{
			// Token: 0x040024F5 RID: 9461
			Expanding,
			// Token: 0x040024F6 RID: 9462
			Holding,
			// Token: 0x040024F7 RID: 9463
			Contracting
		}
	}
}
