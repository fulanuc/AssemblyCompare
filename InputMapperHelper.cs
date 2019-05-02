using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using RoR2.UI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000444 RID: 1092
	public class InputMapperHelper : IDisposable
	{
		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06001857 RID: 6231 RVA: 0x000123BD File Offset: 0x000105BD
		// (set) Token: 0x06001858 RID: 6232 RVA: 0x000123C5 File Offset: 0x000105C5
		public bool isListening { get; private set; }

		// Token: 0x06001859 RID: 6233 RVA: 0x000123CE File Offset: 0x000105CE
		public InputMapperHelper(MPEventSystem eventSystem)
		{
			this.eventSystem = eventSystem;
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0007E5B8 File Offset: 0x0007C7B8
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

		// Token: 0x0600185B RID: 6235 RVA: 0x0007E6D0 File Offset: 0x0007C8D0
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

		// Token: 0x0600185C RID: 6236 RVA: 0x0007E768 File Offset: 0x0007C968
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

		// Token: 0x0600185D RID: 6237 RVA: 0x0007E884 File Offset: 0x0007CA84
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

		// Token: 0x0600185E RID: 6238 RVA: 0x0007E924 File Offset: 0x0007CB24
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

		// Token: 0x0600185F RID: 6239 RVA: 0x0007E9C8 File Offset: 0x0007CBC8
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

		// Token: 0x06001860 RID: 6240 RVA: 0x000123FE File Offset: 0x000105FE
		private void InputMapperOnTimedOutEvent(InputMapper.TimedOutEventData timedOutEventData)
		{
			Debug.Log("InputMapperOnTimedOutEvent");
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0001240A File Offset: 0x0001060A
		private void InputMapperOnStoppedEvent(InputMapper.StoppedEventData stoppedEventData)
		{
			Debug.Log("InputMapperOnStoppedEvent");
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x00012416 File Offset: 0x00010616
		private void InputMapperOnStartedEvent(InputMapper.StartedEventData startedEventData)
		{
			Debug.Log("InputMapperOnStartedEvent");
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0007EA78 File Offset: 0x0007CC78
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

		// Token: 0x06001864 RID: 6244 RVA: 0x00012422 File Offset: 0x00010622
		private void InputMapperOnErrorEvent(InputMapper.ErrorEventData errorEventData)
		{
			Debug.Log("InputMapperOnErrorEvent");
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0001242E File Offset: 0x0001062E
		private void InputMapperOnCanceledEvent(InputMapper.CanceledEventData canceledEventData)
		{
			Debug.Log("InputMapperOnCanceledEvent");
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0007EB50 File Offset: 0x0007CD50
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

		// Token: 0x06001867 RID: 6247 RVA: 0x0001243A File Offset: 0x0001063A
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x04001B86 RID: 7046
		private readonly MPEventSystem eventSystem;

		// Token: 0x04001B87 RID: 7047
		private readonly List<InputMapper> inputMappers = new List<InputMapper>();

		// Token: 0x04001B88 RID: 7048
		private ControllerMap[] maps = Array.Empty<ControllerMap>();

		// Token: 0x04001B89 RID: 7049
		private SimpleDialogBox dialogBox;

		// Token: 0x04001B8A RID: 7050
		public float timeout = 5f;

		// Token: 0x04001B8B RID: 7051
		private float timer;

		// Token: 0x04001B8C RID: 7052
		private Player currentPlayer;

		// Token: 0x04001B8D RID: 7053
		private InputAction currentAction;

		// Token: 0x04001B8E RID: 7054
		private AxisRange currentAxisRange;

		// Token: 0x04001B90 RID: 7056
		private Action<InputMapper.ConflictResponse> conflictResponseCallback;

		// Token: 0x04001B91 RID: 7057
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
