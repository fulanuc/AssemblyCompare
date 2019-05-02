using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RoR2.UI
{
	// Token: 0x02000613 RID: 1555
	[RequireComponent(typeof(RewiredStandaloneInputModule))]
	public class MPEventSystem : EventSystem
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06002333 RID: 9011 RVA: 0x00019AD9 File Offset: 0x00017CD9
		// (set) Token: 0x06002334 RID: 9012 RVA: 0x00019AE0 File Offset: 0x00017CE0
		public static int activeCount { get; private set; }

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06002335 RID: 9013 RVA: 0x00019AE8 File Offset: 0x00017CE8
		public bool isHovering
		{
			get
			{
				return base.currentInputModule && ((MPInputModule)base.currentInputModule).isHovering;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06002336 RID: 9014 RVA: 0x00019B09 File Offset: 0x00017D09
		public bool isCursorVisible
		{
			get
			{
				return this.cursorIndicatorController.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x000A879C File Offset: 0x000A699C
		public static MPEventSystem FindByPlayer(Player player)
		{
			foreach (MPEventSystem mpeventSystem in MPEventSystem.instancesList)
			{
				if (mpeventSystem.player == player)
				{
					return mpeventSystem;
				}
			}
			return null;
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x000A87F8 File Offset: 0x000A69F8
		protected override void Update()
		{
			EventSystem current = EventSystem.current;
			EventSystem.current = this;
			base.Update();
			EventSystem.current = current;
			if (this.player.GetButtonDown(25) && (PauseScreenController.instancesList.Count == 0 || SimpleDialogBox.instancesList.Count == 0))
			{
				Console.instance.SubmitCmd(null, "pause", false);
			}
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x00019B1B File Offset: 0x00017D1B
		protected override void Awake()
		{
			base.Awake();
			MPEventSystem.instancesList.Add(this);
			this.cursorIndicatorController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/CursorIndicator"), base.transform).GetComponent<CursorIndicatorController>();
			this.inputMapperHelper = new InputMapperHelper(this);
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x00019B5A File Offset: 0x00017D5A
		private static void OnActiveSceneChanged(Scene scene1, Scene scene2)
		{
			MPEventSystem.RecenterCursors();
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x000A8854 File Offset: 0x000A6A54
		private static void RecenterCursors()
		{
			foreach (MPEventSystem mpeventSystem in MPEventSystem.instancesList)
			{
				if (mpeventSystem.currentInputSource == MPEventSystem.InputSource.Gamepad && mpeventSystem.currentInputModule)
				{
					((MPInput)mpeventSystem.currentInputModule.input).CenterCursor();
				}
			}
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x00019B61 File Offset: 0x00017D61
		protected override void OnDestroy()
		{
			this.player.controllers.RemoveLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			MPEventSystem.instancesList.Remove(this);
			this.inputMapperHelper.Dispose();
			base.OnDestroy();
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x000A88CC File Offset: 0x000A6ACC
		protected override void Start()
		{
			base.Start();
			this.SetCursorIndicatorEnabled(false);
			if (base.currentInputModule && base.currentInputModule.input)
			{
				((MPInput)base.currentInputModule.input).CenterCursor();
			}
			this.player.controllers.AddLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			this.OnLastActiveControllerChanged(this.player, this.player.controllers.GetLastActiveController());
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x00019B9C File Offset: 0x00017D9C
		protected override void OnEnable()
		{
			base.OnEnable();
			MPEventSystem.activeCount++;
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x00019BB0 File Offset: 0x00017DB0
		protected override void OnDisable()
		{
			this.SetCursorIndicatorEnabled(false);
			base.OnDisable();
			MPEventSystem.activeCount--;
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06002340 RID: 9024 RVA: 0x00019BCB File Offset: 0x00017DCB
		// (set) Token: 0x06002341 RID: 9025 RVA: 0x00019BD3 File Offset: 0x00017DD3
		public MPEventSystem.InputSource currentInputSource { get; private set; } = MPEventSystem.InputSource.Gamepad;

		// Token: 0x06002342 RID: 9026 RVA: 0x000A8954 File Offset: 0x000A6B54
		protected void LateUpdate()
		{
			bool flag = this.cursorOpenerCount > 0 && base.currentInputModule && base.currentInputModule.input;
			this.SetCursorIndicatorEnabled(flag);
			MPInputModule mpinputModule = base.currentInputModule as MPInputModule;
			if (flag)
			{
				CursorIndicatorController.CursorSet cursorSet = this.cursorIndicatorController.noneCursorSet;
				MPEventSystem.InputSource currentInputSource = this.currentInputSource;
				if (currentInputSource != MPEventSystem.InputSource.Keyboard)
				{
					if (currentInputSource == MPEventSystem.InputSource.Gamepad)
					{
						cursorSet = this.cursorIndicatorController.gamepadCursorSet;
					}
				}
				else
				{
					cursorSet = this.cursorIndicatorController.mouseCursorSet;
				}
				this.cursorIndicatorController.SetCursor(cursorSet, this.isHovering ? CursorIndicatorController.CursorImage.Hover : CursorIndicatorController.CursorImage.Pointer, this.GetColor());
				this.cursorIndicatorController.SetPosition(mpinputModule.input.mousePosition);
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000A8A0C File Offset: 0x000A6C0C
		private void OnLastActiveControllerChanged(Player player, Controller controller)
		{
			if (controller == null)
			{
				return;
			}
			ControllerType type = controller.type;
			if (type <= ControllerType.Mouse)
			{
				this.currentInputSource = MPEventSystem.InputSource.Keyboard;
				return;
			}
			if (type != ControllerType.Joystick)
			{
				return;
			}
			this.currentInputSource = MPEventSystem.InputSource.Gamepad;
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x000A8A3C File Offset: 0x000A6C3C
		private void SetCursorIndicatorEnabled(bool cursorIndicatorEnabled)
		{
			if (this.cursorIndicatorController.gameObject.activeSelf != cursorIndicatorEnabled)
			{
				this.cursorIndicatorController.gameObject.SetActive(cursorIndicatorEnabled);
				if (cursorIndicatorEnabled)
				{
					((MPInput)((MPInputModule)base.currentInputModule).input).CenterCursor();
				}
			}
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x00019BDC File Offset: 0x00017DDC
		public Color GetColor()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return Color.white;
			}
			return ColorCatalog.GetMultiplayerColor(this.playerSlot);
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x00019BF7 File Offset: 0x00017DF7
		public bool GetCursorPosition(out Vector2 position)
		{
			if (base.currentInputModule)
			{
				position = base.currentInputModule.input.mousePosition;
				return true;
			}
			position = Vector2.zero;
			return false;
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x000A8A8C File Offset: 0x000A6C8C
		public Rect GetScreenRect()
		{
			LocalUser localUser = this.localUser;
			CameraRigController cameraRigController = (localUser != null) ? localUser.cameraRigController : null;
			if (!cameraRigController)
			{
				return new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			}
			return cameraRigController.viewport;
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x000A8AD8 File Offset: 0x000A6CD8
		private static Vector2 RandomOnCircle()
		{
			float value = UnityEngine.Random.value;
			return new Vector2(Mathf.Cos(value * 3.14159274f * 2f), Mathf.Sin(value * 3.14159274f * 2f));
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000A8B14 File Offset: 0x000A6D14
		private static Vector2 CalculateCursorPushVector(Vector2 positionA, Vector2 positionB)
		{
			Vector2 vector = positionA - positionB;
			if (vector == Vector2.zero)
			{
				vector = MPEventSystem.RandomOnCircle();
			}
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude >= 576f)
			{
				return Vector2.zero;
			}
			float num = Mathf.Sqrt(sqrMagnitude);
			float num2 = num * 0.0416666679f;
			float d = 1f - num2;
			return vector / num * d * 10f * 0.5f;
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x000A8B8C File Offset: 0x000A6D8C
		private static void PushCursorsApart()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return;
			}
			int count = MPEventSystem.instancesList.Count;
			if (MPEventSystem.pushInfos.Length < MPEventSystem.activeCount)
			{
				MPEventSystem.pushInfos = new MPEventSystem.PushInfo[MPEventSystem.activeCount];
			}
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (MPEventSystem.instancesList[i].enabled)
				{
					Vector2 position;
					MPEventSystem.instancesList[i].GetCursorPosition(out position);
					MPEventSystem.pushInfos[num++] = new MPEventSystem.PushInfo
					{
						index = i,
						position = position
					};
				}
			}
			for (int j = 0; j < MPEventSystem.activeCount; j++)
			{
				MPEventSystem.PushInfo pushInfo = MPEventSystem.pushInfos[j];
				for (int k = j + 1; k < MPEventSystem.activeCount; k++)
				{
					MPEventSystem.PushInfo pushInfo2 = MPEventSystem.pushInfos[k];
					Vector2 b = MPEventSystem.CalculateCursorPushVector(pushInfo.position, pushInfo2.position);
					pushInfo.pushVector += b;
					pushInfo2.pushVector -= b;
					MPEventSystem.pushInfos[k] = pushInfo2;
				}
				MPEventSystem.pushInfos[j] = pushInfo;
			}
			for (int l = 0; l < MPEventSystem.activeCount; l++)
			{
				MPEventSystem.PushInfo pushInfo3 = MPEventSystem.pushInfos[l];
				MPEventSystem mpeventSystem = MPEventSystem.instancesList[pushInfo3.index];
				if (mpeventSystem.allowCursorPush && mpeventSystem.currentInputModule)
				{
					((MPInput)mpeventSystem.currentInputModule.input).internalMousePosition += pushInfo3.pushVector;
				}
			}
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x000A8D4C File Offset: 0x000A6F4C
		static MPEventSystem()
		{
			RoR2Application.onUpdate += MPEventSystem.PushCursorsApart;
			SceneManager.activeSceneChanged += MPEventSystem.OnActiveSceneChanged;
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x0600234C RID: 9036 RVA: 0x00019C2A File Offset: 0x00017E2A
		// (set) Token: 0x0600234D RID: 9037 RVA: 0x00019C32 File Offset: 0x00017E32
		public InputMapperHelper inputMapperHelper { get; private set; }

		// Token: 0x040025F9 RID: 9721
		private static readonly List<MPEventSystem> instancesList = new List<MPEventSystem>();

		// Token: 0x040025FA RID: 9722
		public static ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = new ReadOnlyCollection<MPEventSystem>(MPEventSystem.instancesList);

		// Token: 0x040025FC RID: 9724
		public int cursorOpenerCount;

		// Token: 0x040025FD RID: 9725
		public int playerSlot = -1;

		// Token: 0x040025FE RID: 9726
		[NonSerialized]
		public bool allowCursorPush = true;

		// Token: 0x040025FF RID: 9727
		[NonSerialized]
		public bool isCombinedEventSystem;

		// Token: 0x04002601 RID: 9729
		private CursorIndicatorController cursorIndicatorController;

		// Token: 0x04002602 RID: 9730
		[NotNull]
		public Player player;

		// Token: 0x04002603 RID: 9731
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x04002604 RID: 9732
		public TooltipProvider currentTooltipProvider;

		// Token: 0x04002605 RID: 9733
		public TooltipController currentTooltip;

		// Token: 0x04002606 RID: 9734
		private static MPEventSystem.PushInfo[] pushInfos = Array.Empty<MPEventSystem.PushInfo>();

		// Token: 0x04002607 RID: 9735
		private const float radius = 24f;

		// Token: 0x04002608 RID: 9736
		private const float invRadius = 0.0416666679f;

		// Token: 0x04002609 RID: 9737
		private const float radiusSqr = 576f;

		// Token: 0x0400260A RID: 9738
		private const float pushFactor = 10f;

		// Token: 0x02000614 RID: 1556
		public enum InputSource
		{
			// Token: 0x0400260D RID: 9741
			Keyboard,
			// Token: 0x0400260E RID: 9742
			Gamepad
		}

		// Token: 0x02000615 RID: 1557
		private struct PushInfo
		{
			// Token: 0x0400260F RID: 9743
			public int index;

			// Token: 0x04002610 RID: 9744
			public Vector2 position;

			// Token: 0x04002611 RID: 9745
			public Vector2 pushVector;
		}
	}
}
