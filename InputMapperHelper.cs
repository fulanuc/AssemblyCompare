using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044C RID: 1100
	public class InputMapperHelper : IDisposable
	{
		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060018A4 RID: 6308 RVA: 0x00012831 File Offset: 0x00010A31
		// (set) Token: 0x060018A5 RID: 6309 RVA: 0x00012839 File Offset: 0x00010A39
		public bool isListening { get; private set; }

		// Token: 0x060018A6 RID: 6310 RVA: 0x00012842 File Offset: 0x00010A42
		public InputMapperHelper(MPEventSystem eventSystem)
		{
			this.eventSystem = eventSystem;
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0007ED74 File Offset: 0x0007CF74
		private InputMapper AddInputMapper()
		{
			InputMapper inputMapper = new InputMapper();
			inputMapper.ConflictFoundEvent += this.InputMapperOnConflictFoundEvent;
			inputMapper.CanceledEvent += this.InputMapperOnCanceledEvent;
			inputMapper.ErrorEvent += this.InputMapperOnErrorEvent;
			inputMapper.InputMappedEvent += this.InputMapperOnInputMappedEvent;
			inputMapper.StartedEvent += this.InputMapperOnStartedEvent;
			inputMapper.StoppedEvent += this.InputMapperOnStoppedEvent;
			inputMapper.TimedOutEvent += this.InputMapperOnTimedOutEvent;
			inputMapper.options = new InputMapper.Options
			{
				allowAxes = true,
				allowButtons = true,
				allowKeyboardKeysWithModifiers = false,
				allowKeyboardModifierKeyAsPrimary = true,
				checkForConflicts = true,
				checkForConflictsWithAllPlayers = false,
				checkForConflictsWithPlayerIds = Array.Empty<int>(),
				checkForConflictsWithSelf = true,
				checkForConflictsWithSystemPlayer = false,
				defaultActionWhenConflictFound = InputMapper.ConflictResponse.Add,
				holdDurationToMapKeyboardModifierKeyAsPrimary = 0f,
				ignoreMouseXAxis = true,
				ignoreMouseYAxis = true,
				timeout = float.PositiveInfinity
			};
			this.inputMappers.Add(inputMapper);
			return inputMapper;
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0007EE8C File Offset: 0x0007D08C
		private void RemoveInputMapper(InputMapper inputMapper)
		{
			inputMapper.ConflictFoundEvent -= this.InputMapperOnConflictFoundEvent;
			inputMapper.CanceledEvent -= this.InputMapperOnCanceledEvent;
			inputMapper.ErrorEvent -= this.InputMapperOnErrorEvent;
			inputMapper.InputMappedEvent -= this.InputMapperOnInputMappedEvent;
			inputMapper.StartedEvent -= this.InputMapperOnStartedEvent;
			inputMapper.StoppedEvent -= this.InputMapperOnStoppedEvent;
			inputMapper.TimedOutEvent -= this.InputMapperOnTimedOutEvent;
			this.inputMappers.Remove(inputMapper);
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0007EF24 File Offset: 0x0007D124
		public void Start(Player player, IList<Controller> controllers, InputAction action, AxisRange axisRange)
		{
			this.Stop();
			this.isListening = true;
			this.currentPlayer = player;
			this.currentAction = action;
			this.currentAxisRange = axisRange;
			this.maps = (from controller in controllers
			select player.controllers.maps.GetFirstMapInCategory(controller, 0) into map
			where map != null
			select map).Distinct<ControllerMap>().ToArray<ControllerMap>();
			Debug.Log(this.maps.Length);
			foreach (ControllerMap controllerMap in this.maps)
			{
				InputMapper.Context mappingContext = new InputMapper.Context
				{
					actionId = action.id,
					controllerMap = controllerMap,
					actionRange = this.currentAxisRange
				};
				this.AddInputMapper().Start(mappingContext);
			}
			this.dialogBox = SimpleDialogBox.Create(this.eventSystem);
			this.timer = this.timeout;
			this.UpdateDialogBoxString();
			RoR2Application.onUpdate += this.Update;
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0007F040 File Offset: 0x0007D240
		public void Stop()
		{
			if (!this.isListening)
			{
				return;
			}
			this.maps = Array.Empty<ControllerMap>();
			this.currentPlayer = null;
			this.currentAction = null;
			for (int i = this.inputMappers.Count - 1; i >= 0; i--)
			{
				InputMapper inputMapper = this.inputMappers[i];
				inputMapper.Stop();
				this.RemoveInputMapper(inputMapper);
			}
			if (this.dialogBox)
			{
				UnityEngine.Object.Destroy(this.dialogBox.rootObject);
				this.dialogBox = null;
			}
			this.isListening = false;
			RoR2Application.onUpdate -= this.Update;
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0007F0E0 File Offset: 0x0007D2E0
		private void Update()
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (this.isListening)
			{
				this.timer -= unscaledDeltaTime;
				if (this.timer < 0f)
				{
					this.Stop();
					return;
				}
				if (this.currentPlayer.GetButtonDown(25))
				{
					this.Stop();
					SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(this.eventSystem);
					simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair("OPTION_REBIND_DIALOG_TITLE", Array.Empty<object>());
					simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair("OPTION_REBIND_CANCELLED_DIALOG_DESCRIPTION", Array.Empty<object>());
					simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
					return;
				}
				this.UpdateDialogBoxString();
			}
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0007F184 File Offset: 0x0007D384
		private void UpdateDialogBoxString()
		{
			if (this.dialogBox && this.timer >= 0f)
			{
				string @string = Language.GetString(InputCatalog.GetActionNameToken(this.currentAction.name, AxisRange.Full));
				this.dialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
				{
					token = CommonLanguageTokens.optionRebindDialogTitle,
					formatParams = Array.Empty<object>()
				};
				this.dialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair
				{
					token = CommonLanguageTokens.optionRebindDialogDescription,
					formatParams = new object[]
					{
						@string,
						this.timer
					}
				};
			}
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x00012872 File Offset: 0x00010A72
		private void InputMapperOnTimedOutEvent(InputMapper.TimedOutEventData timedOutEventData)
		{
			Debug.Log("InputMapperOnTimedOutEvent");
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0001287E File Offset: 0x00010A7E
		private void InputMapperOnStoppedEvent(InputMapper.StoppedEventData stoppedEventData)
		{
			Debug.Log("InputMapperOnStoppedEvent");
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0001288A File Offset: 0x00010A8A
		private void InputMapperOnStartedEvent(InputMapper.StartedEventData startedEventData)
		{
			Debug.Log("InputMapperOnStartedEvent");
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0007F234 File Offset: 0x0007D434
		private void InputMapperOnInputMappedEvent(InputMapper.InputMappedEventData inputMappedEventData)
		{
			Debug.Log("InputMapperOnInputMappedEvent");
			InputMapperHelper.<>c__DisplayClass23_0 CS$<>8__locals1;
			CS$<>8__locals1.incomingActionElementMap = inputMappedEventData.actionElementMap;
			CS$<>8__locals1.incomingActionId = inputMappedEventData.actionElementMap.actionId;
			CS$<>8__locals1.incomingElementIndex = inputMappedEventData.actionElementMap.elementIndex;
			CS$<>8__locals1.incomingElementType = inputMappedEventData.actionElementMap.elementType;
			CS$<>8__locals1.map = inputMappedEventData.actionElementMap.controllerMap;
			foreach (ControllerMap controllerMap in this.maps)
			{
				if (controllerMap != CS$<>8__locals1.map)
				{
					controllerMap.DeleteElementMapsWithAction(CS$<>8__locals1.incomingActionId);
				}
			}
			while (InputMapperHelper.<InputMapperOnInputMappedEvent>g__DeleteFirstConflictingElementMap|23_1(ref CS$<>8__locals1))
			{
			}
			MPEventSystem mpeventSystem = this.eventSystem;
			if (mpeventSystem != null)
			{
				LocalUser localUser = mpeventSystem.localUser;
				if (localUser != null)
				{
					localUser.userProfile.RequestSave(false);
				}
			}
			Debug.Log("Mapping accepted.");
			this.Stop();
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x00012896 File Offset: 0x00010A96
		private void InputMapperOnErrorEvent(InputMapper.ErrorEventData errorEventData)
		{
			Debug.Log("InputMapperOnErrorEvent");
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x000128A2 File Offset: 0x00010AA2
		private void InputMapperOnCanceledEvent(InputMapper.CanceledEventData canceledEventData)
		{
			Debug.Log("InputMapperOnCanceledEvent");
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0007F30C File Offset: 0x0007D50C
		private void InputMapperOnConflictFoundEvent(InputMapper.ConflictFoundEventData conflictFoundEventData)
		{
			Debug.Log("InputMapperOnConflictFoundEvent");
			InputMapper.ConflictResponse obj;
			if (conflictFoundEventData.conflicts.Any((ElementAssignmentConflictInfo elementAssignmentConflictInfo) => InputMapperHelper.forbiddenElements.Contains(elementAssignmentConflictInfo.elementIdentifier.name)))
			{
				obj = InputMapper.ConflictResponse.Ignore;
			}
			else
			{
				obj = InputMapper.ConflictResponse.Add;
			}
			conflictFoundEventData.responseCallback(obj);
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x000128AE File Offset: 0x00010AAE
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x04001BB6 RID: 7094
		private readonly MPEventSystem eventSystem;

		// Token: 0x04001BB7 RID: 7095
		private readonly List<InputMapper> inputMappers = new List<InputMapper>();

		// Token: 0x04001BB8 RID: 7096
		private ControllerMap[] maps = Array.Empty<ControllerMap>();

		// Token: 0x04001BB9 RID: 7097
		private SimpleDialogBox dialogBox;

		// Token: 0x04001BBA RID: 7098
		public float timeout = 5f;

		// Token: 0x04001BBB RID: 7099
		private float timer;

		// Token: 0x04001BBC RID: 7100
		private Player currentPlayer;

		// Token: 0x04001BBD RID: 7101
		private InputAction currentAction;

		// Token: 0x04001BBE RID: 7102
		private AxisRange currentAxisRange;

		// Token: 0x04001BC0 RID: 7104
		private Action<InputMapper.ConflictResponse> conflictResponseCallback;

		// Token: 0x04001BC1 RID: 7105
		private static readonly HashSet<string> forbiddenElements = new HashSet<string>
		{
			"Left Stick X",
			"Left Stick Y",
			"Right Stick X",
			"Right Stick Y",
			"Mouse Horizontal",
			"Mouse Vertical",
			Keyboard.GetKeyName(KeyCode.Escape),
			Keyboard.GetKeyName(KeyCode.KeypadEnter),
			Keyboard.GetKeyName(KeyCode.Return)
		};
	}
}
