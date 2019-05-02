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
	// Token: 0x02000601 RID: 1537
	[RequireComponent(typeof(RewiredStandaloneInputModule))]
	public class MPEventSystem : EventSystem
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060022A3 RID: 8867 RVA: 0x00019422 File Offset: 0x00017622
		// (set) Token: 0x060022A4 RID: 8868 RVA: 0x00019429 File Offset: 0x00017629
		public static int activeCount { get; private set; }

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x060022A5 RID: 8869 RVA: 0x00019431 File Offset: 0x00017631
		public bool isHovering
		{
			get
			{
				return base.currentInputModule && ((MPInputModule)base.currentInputModule).isHovering;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x060022A6 RID: 8870 RVA: 0x00019452 File Offset: 0x00017652
		public bool isCursorVisible
		{
			get
			{
				return this.cursorIndicatorController.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x000A7120 File Offset: 0x000A5320
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

		// Token: 0x060022A8 RID: 8872 RVA: 0x000A717C File Offset: 0x000A537C
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

		// Token: 0x060022A9 RID: 8873 RVA: 0x00019464 File Offset: 0x00017664
		protected override void Awake()
		{
			base.Awake();
			MPEventSystem.instancesList.Add(this);
			this.cursorIndicatorController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/CursorIndicator"), base.transform).GetComponent<CursorIndicatorController>();
			this.inputMapperHelper = new InputMapperHelper(this);
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x000194A3 File Offset: 0x000176A3
		private static void OnActiveSceneChanged(Scene scene1, Scene scene2)
		{
			MPEventSystem.RecenterCursors();
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x000A71D8 File Offset: 0x000A53D8
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

		// Token: 0x060022AC RID: 8876 RVA: 0x000194AA File Offset: 0x000176AA
		protected override void OnDestroy()
		{
			this.player.controllers.RemoveLastActiveControllerChangedDelegate(new PlayerActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
			MPEventSystem.instancesList.Remove(this);
			this.inputMapperHelper.Dispose();
			base.OnDestroy();
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x000A7250 File Offset: 0x000A5450
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

		// Token: 0x060022AE RID: 8878 RVA: 0x000194E5 File Offset: 0x000176E5
		protected override void OnEnable()
		{
			base.OnEnable();
			MPEventSystem.activeCount++;
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x000194F9 File Offset: 0x000176F9
		protected override void OnDisable()
		{
			this.SetCursorIndicatorEnabled(false);
			base.OnDisable();
			MPEventSystem.activeCount--;
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060022B0 RID: 8880 RVA: 0x00019514 File Offset: 0x00017714
		// (set) Token: 0x060022B1 RID: 8881 RVA: 0x0001951C File Offset: 0x0001771C
		public MPEventSystem.InputSource currentInputSource { get; private set; } = MPEventSystem.InputSource.Gamepad;

		// Token: 0x060022B2 RID: 8882 RVA: 0x000A72D8 File Offset: 0x000A54D8
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

		// Token: 0x060022B3 RID: 8883 RVA: 0x000A7390 File Offset: 0x000A5590
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

		// Token: 0x060022B4 RID: 8884 RVA: 0x000A73C0 File Offset: 0x000A55C0
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

		// Token: 0x060022B5 RID: 8885 RVA: 0x00019525 File Offset: 0x00017725
		public Color GetColor()
		{
			if (MPEventSystem.activeCount <= 1)
			{
				return Color.white;
			}
			return ColorCatalog.GetMultiplayerColor(this.playerSlot);
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x00019540 File Offset: 0x00017740
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

		// Token: 0x060022B7 RID: 8887 RVA: 0x000A7410 File Offset: 0x000A5610
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

		// Token: 0x060022B8 RID: 8888 RVA: 0x000A745C File Offset: 0x000A565C
		private static Vector2 RandomOnCircle()
		{
			float value = UnityEngine.Random.value;
			return new Vector2(Mathf.Cos(value * 3.14159274f * 2f), Mathf.Sin(value * 3.14159274f * 2f));
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x000A7498 File Offset: 0x000A5698
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

		// Token: 0x060022BA RID: 8890 RVA: 0x000A7510 File Offset: 0x000A5710
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

		// Token: 0x060022BB RID: 8891 RVA: 0x000A76D0 File Offset: 0x000A58D0
		static MPEventSystem()
		{
			RoR2Application.onUpdate += MPEventSystem.PushCursorsApart;
			SceneManager.activeSceneChanged += MPEventSystem.OnActiveSceneChanged;
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060022BC RID: 8892 RVA: 0x00019573 File Offset: 0x00017773
		// (set) Token: 0x060022BD RID: 8893 RVA: 0x0001957B File Offset: 0x0001777B
		public InputMapperHelper inputMapperHelper { get; private set; }

		// Token: 0x0400259E RID: 9630
		private static readonly List<MPEventSystem> instancesList = new List<MPEventSystem>();

		// Token: 0x0400259F RID: 9631
		public static ReadOnlyCollection<MPEventSystem> readOnlyInstancesList = new ReadOnlyCollection<MPEventSystem>(MPEventSystem.instancesList);

		// Token: 0x040025A1 RID: 9633
		public int cursorOpenerCount;

		// Token: 0x040025A2 RID: 9634
		public int playerSlot = -1;

		// Token: 0x040025A3 RID: 9635
		[NonSerialized]
		public bool allowCursorPush = true;

		// Token: 0x040025A4 RID: 9636
		[NonSerialized]
		public bool isCombinedEventSystem;

		// Token: 0x040025A6 RID: 9638
		private CursorIndicatorController cursorIndicatorController;

		// Token: 0x040025A7 RID: 9639
		[NotNull]
		public Player player;

		// Token: 0x040025A8 RID: 9640
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x040025A9 RID: 9641
		public TooltipProvider currentTooltipProvider;

		// Token: 0x040025AA RID: 9642
		public TooltipController currentTooltip;

		// Token: 0x040025AB RID: 9643
		private static MPEventSystem.PushInfo[] pushInfos = Array.Empty<MPEventSystem.PushInfo>();

		// Token: 0x040025AC RID: 9644
		private const float radius = 24f;

		// Token: 0x040025AD RID: 9645
		private const float invRadius = 0.0416666679f;

		// Token: 0x040025AE RID: 9646
		private const float radiusSqr = 576f;

		// Token: 0x040025AF RID: 9647
		private const float pushFactor = 10f;

		// Token: 0x02000602 RID: 1538
		public enum InputSource
		{
			// Token: 0x040025B2 RID: 9650
			Keyboard,
			// Token: 0x040025B3 RID: 9651
			Gamepad
		}

		// Token: 0x02000603 RID: 1539
		private struct PushInfo
		{
			// Token: 0x040025B4 RID: 9652
			public int index;

			// Token: 0x040025B5 RID: 9653
			public Vector2 position;

			// Token: 0x040025B6 RID: 9654
			public Vector2 pushVector;
		}
	}
}
