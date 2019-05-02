using System;
using System.Collections.Generic;
using System.Reflection;
using Rewired.Integration.UnityUI;
using Rewired.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000607 RID: 1543
	[RequireComponent(typeof(MPInput))]
	public class MPInputModule : RewiredStandaloneInputModule
	{
		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060022D1 RID: 8913 RVA: 0x00019648 File Offset: 0x00017848
		private bool useCursor
		{
			get
			{
				return ((MPEventSystem)base.eventSystem).isCursorVisible;
			}
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x0001965A File Offset: 0x0001785A
		protected override void Awake()
		{
			this.m_InputOverride = base.GetComponent<MPInput>();
			this.m_MouseState = (RewiredPointerInputModule.MouseState)MPInputModule.m_MouseStateField.GetValue(this);
			base.Awake();
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x00019684 File Offset: 0x00017884
		protected override void Start()
		{
			base.Start();
			base.ClearMouseInputSources();
			base.AddMouseInputSource((MPInput)this.m_InputOverride);
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x000A7724 File Offset: 0x000A5924
		protected void UpdateHover(List<RaycastResult> raycastResults)
		{
			this.isHovering = false;
			if (!this.useCursor)
			{
				return;
			}
			foreach (RaycastResult raycastResult in raycastResults)
			{
				if (raycastResult.gameObject)
				{
					Selectable componentInParent = raycastResult.gameObject.GetComponentInParent<Selectable>();
					if (componentInParent != null && this.<UpdateHover>g__IsHoverable|9_0(componentInParent))
					{
						this.isHovering = true;
						break;
					}
				}
			}
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x000A77B4 File Offset: 0x000A59B4
		protected override RewiredPointerInputModule.MouseState GetMousePointerEventData(int playerId, int mouseIndex)
		{
			IMouseInputSource mouseInputSource = base.GetMouseInputSource(playerId, mouseIndex);
			if (mouseInputSource == null)
			{
				return null;
			}
			PlayerPointerEventData playerPointerEventData;
			bool pointerData = base.GetPointerData(playerId, mouseIndex, -1, out playerPointerEventData, true, PointerEventType.Mouse);
			playerPointerEventData.Reset();
			if (pointerData)
			{
				playerPointerEventData.position = base.input.mousePosition;
			}
			Vector2 mousePosition = base.input.mousePosition;
			if (mouseInputSource.locked)
			{
				playerPointerEventData.position = new Vector2(-1f, -1f);
				playerPointerEventData.delta = Vector2.zero;
			}
			else
			{
				playerPointerEventData.delta = mousePosition - playerPointerEventData.position;
				playerPointerEventData.position = mousePosition;
			}
			playerPointerEventData.scrollDelta = mouseInputSource.wheelDelta;
			playerPointerEventData.button = PointerEventData.InputButton.Left;
			base.eventSystem.RaycastAll(playerPointerEventData, this.m_RaycastResultCache);
			RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			playerPointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
			this.UpdateHover(this.m_RaycastResultCache);
			this.m_RaycastResultCache.Clear();
			PlayerPointerEventData playerPointerEventData2;
			base.GetPointerData(playerId, mouseIndex, -2, out playerPointerEventData2, true, PointerEventType.Mouse);
			base.CopyFromTo(playerPointerEventData, playerPointerEventData2);
			playerPointerEventData2.button = PointerEventData.InputButton.Right;
			PlayerPointerEventData playerPointerEventData3;
			base.GetPointerData(playerId, mouseIndex, -3, out playerPointerEventData3, true, PointerEventType.Mouse);
			base.CopyFromTo(playerPointerEventData, playerPointerEventData3);
			playerPointerEventData3.button = PointerEventData.InputButton.Middle;
			for (int i = 3; i < mouseInputSource.buttonCount; i++)
			{
				PlayerPointerEventData playerPointerEventData4;
				base.GetPointerData(playerId, mouseIndex, -2147483520 + i, out playerPointerEventData4, true, PointerEventType.Mouse);
				base.CopyFromTo(playerPointerEventData, playerPointerEventData4);
				playerPointerEventData4.button = (PointerEventData.InputButton)(-1);
			}
			this.m_MouseState.SetButtonState(0, base.StateForMouseButton(playerId, mouseIndex, 0), playerPointerEventData);
			this.m_MouseState.SetButtonState(1, base.StateForMouseButton(playerId, mouseIndex, 1), playerPointerEventData2);
			this.m_MouseState.SetButtonState(2, base.StateForMouseButton(playerId, mouseIndex, 2), playerPointerEventData3);
			for (int j = 3; j < mouseInputSource.buttonCount; j++)
			{
				PlayerPointerEventData data;
				base.GetPointerData(playerId, mouseIndex, -2147483520 + j, out data, false, PointerEventType.Mouse);
				this.m_MouseState.SetButtonState(j, base.StateForMouseButton(playerId, mouseIndex, j), data);
			}
			return this.m_MouseState;
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x000A799C File Offset: 0x000A5B9C
		protected override void ProcessMove(PlayerPointerEventData pointerEvent)
		{
			GameObject newEnterTarget = (!this.useCursor) ? null : pointerEvent.pointerCurrentRaycast.gameObject;
			base.HandlePointerExitAndEnter(pointerEvent, newEnterTarget);
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x000A79CC File Offset: 0x000A5BCC
		protected override void ProcessDrag(PlayerPointerEventData pointerEvent)
		{
			if (!pointerEvent.IsPointerMoving() || !this.useCursor || pointerEvent.pointerDrag == null)
			{
				return;
			}
			if (!pointerEvent.dragging && MPInputModule.ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float)base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
			{
				ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
				pointerEvent.dragging = true;
			}
			if (pointerEvent.dragging)
			{
				if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
					pointerEvent.eligibleForClick = false;
					pointerEvent.pointerPress = null;
					pointerEvent.rawPointerPress = null;
				}
				ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
			}
		}

		// Token: 0x040025C1 RID: 9665
		private RewiredPointerInputModule.MouseState m_MouseState;

		// Token: 0x040025C2 RID: 9666
		private static readonly FieldInfo m_MouseStateField = typeof(RewiredPointerInputModule).GetField("m_MouseState", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x040025C3 RID: 9667
		private static readonly MPInputModule.ShouldStartDragDelegate ShouldStartDrag = (MPInputModule.ShouldStartDragDelegate)typeof(RewiredPointerInputModule).GetMethod("ShouldStartDrag", BindingFlags.Static | BindingFlags.NonPublic).CreateDelegate(typeof(MPInputModule.ShouldStartDragDelegate));

		// Token: 0x040025C4 RID: 9668
		public bool isHovering;

		// Token: 0x02000608 RID: 1544
		// (Invoke) Token: 0x060022DC RID: 8924
		private delegate bool ShouldStartDragDelegate(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold);
	}
}
