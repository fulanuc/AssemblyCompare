using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DE RID: 1502
	[RequireComponent(typeof(Canvas))]
	public class HighlightRect : MonoBehaviour
	{
		// Token: 0x060021AE RID: 8622 RVA: 0x0001880B File Offset: 0x00016A0B
		static HighlightRect()
		{
			RoR2Application.onLateUpdate += HighlightRect.UpdateAll;
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x00018833 File Offset: 0x00016A33
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x00018841 File Offset: 0x00016A41
		private void OnEnable()
		{
			HighlightRect.instancesList.Add(this);
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x0001884E File Offset: 0x00016A4E
		private void OnDisable()
		{
			HighlightRect.instancesList.Remove(this);
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x000A34BC File Offset: 0x000A16BC
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

		// Token: 0x060021B3 RID: 8627 RVA: 0x000A35DC File Offset: 0x000A17DC
		private static void UpdateAll()
		{
			for (int i = HighlightRect.instancesList.Count - 1; i >= 0; i--)
			{
				HighlightRect.instancesList[i].DoUpdate();
			}
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x000A3610 File Offset: 0x000A1810
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

		// Token: 0x060021B5 RID: 8629 RVA: 0x000A381C File Offset: 0x000A1A1C
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

		// Token: 0x060021B6 RID: 8630 RVA: 0x0001885C File Offset: 0x00016A5C
		public static Vector2 WorldToGUIPoint(Camera cam, Vector3 world)
		{
			return cam.WorldToScreenPoint(world);
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x000A3AAC File Offset: 0x000A1CAC
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

		// Token: 0x04002486 RID: 9350
		public AnimationCurve curve;

		// Token: 0x04002487 RID: 9351
		public Color highlightColor;

		// Token: 0x04002488 RID: 9352
		public Sprite cornerImage;

		// Token: 0x04002489 RID: 9353
		public string nametagString;

		// Token: 0x0400248A RID: 9354
		private Image bottomLeftImage;

		// Token: 0x0400248B RID: 9355
		private Image bottomRightImage;

		// Token: 0x0400248C RID: 9356
		private Image topLeftImage;

		// Token: 0x0400248D RID: 9357
		private Image topRightImage;

		// Token: 0x0400248E RID: 9358
		private TextMeshProUGUI nametagText;

		// Token: 0x0400248F RID: 9359
		public Renderer targetRenderer;

		// Token: 0x04002490 RID: 9360
		public GameObject cameraTarget;

		// Token: 0x04002491 RID: 9361
		public RectTransform nametagRectTransform;

		// Token: 0x04002492 RID: 9362
		public RectTransform bottomLeftRectTransform;

		// Token: 0x04002493 RID: 9363
		public RectTransform bottomRightRectTransform;

		// Token: 0x04002494 RID: 9364
		public RectTransform topLeftRectTransform;

		// Token: 0x04002495 RID: 9365
		public RectTransform topRightRectTransform;

		// Token: 0x04002496 RID: 9366
		public float expandTime = 1f;

		// Token: 0x04002497 RID: 9367
		public float maxLifeTime;

		// Token: 0x04002498 RID: 9368
		public bool destroyOnLifeEnd;

		// Token: 0x04002499 RID: 9369
		private float time;

		// Token: 0x0400249A RID: 9370
		public HighlightRect.HighlightState highlightState;

		// Token: 0x0400249B RID: 9371
		private static List<HighlightRect> instancesList = new List<HighlightRect>();

		// Token: 0x0400249C RID: 9372
		private Canvas canvas;

		// Token: 0x0400249D RID: 9373
		private Camera uiCam;

		// Token: 0x0400249E RID: 9374
		private Camera sceneCam;

		// Token: 0x0400249F RID: 9375
		private static readonly Vector2[] extentPoints = new Vector2[8];

		// Token: 0x020005DF RID: 1503
		public enum HighlightState
		{
			// Token: 0x040024A1 RID: 9377
			Expanding,
			// Token: 0x040024A2 RID: 9378
			Holding,
			// Token: 0x040024A3 RID: 9379
			Contracting
		}
	}
}
