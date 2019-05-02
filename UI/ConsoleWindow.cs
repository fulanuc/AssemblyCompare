using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using RoR2.ConVar;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005C0 RID: 1472
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class ConsoleWindow : MonoBehaviour
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06002116 RID: 8470 RVA: 0x00018207 File Offset: 0x00016407
		// (set) Token: 0x06002117 RID: 8471 RVA: 0x0001820E File Offset: 0x0001640E
		public static ConsoleWindow instance { get; private set; }

		// Token: 0x06002118 RID: 8472 RVA: 0x0009FDFC File Offset: 0x0009DFFC
		public void Start()
		{
			base.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.kbmEventSystem;
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 1f;
			}
			this.outputField.textComponent.gameObject.AddComponent<RectTransformDimensionsChangeEvent>().onRectTransformDimensionsChange += this.OnOutputFieldRectTransformDimensionsChange;
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x00018216 File Offset: 0x00016416
		private void OnOutputFieldRectTransformDimensionsChange()
		{
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
			}
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x0009FE68 File Offset: 0x0009E068
		public void OnEnable()
		{
			Console.onLogReceived += this.OnLogReceived;
			Console.onClear += this.OnClear;
			this.RebuildOutput();
			this.inputField.onSubmit.AddListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			ConsoleWindow.instance = this;
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x00018254 File Offset: 0x00016454
		public void SubmitInputField()
		{
			this.inputField.onSubmit.Invoke(this.inputField.text);
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x0009FEDC File Offset: 0x0009E0DC
		public void Submit(string text)
		{
			if (this.inputField.text == "")
			{
				return;
			}
			if (this.autoCompleteDropdown)
			{
				this.autoCompleteDropdown.Hide();
			}
			this.inputField.text = "";
			ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
			NetworkUser sender = null;
			if (readOnlyLocalPlayersList.Count > 0)
			{
				sender = readOnlyLocalPlayersList[0];
			}
			Console.instance.SubmitCmd(sender, text, true);
			if (this.inputField && this.inputField.IsActive())
			{
				this.inputField.ActivateInputField();
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600211D RID: 8477 RVA: 0x00018271 File Offset: 0x00016471
		private bool autoCompleteInUse
		{
			get
			{
				return this.autoCompleteDropdown && this.autoCompleteDropdown.IsExpanded;
			}
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x0009FF74 File Offset: 0x0009E174
		private void OnInputFieldValueChanged(string text)
		{
			if (!this.preventHistoryReset)
			{
				this.historyIndex = -1;
			}
			if (!this.preventAutoCompleteUpdate)
			{
				if (text.Length > 0 != (this.autoComplete != null))
				{
					if (this.autoComplete != null)
					{
						UnityEngine.Object.Destroy(this.autoCompleteDropdown.gameObject);
						this.autoComplete = null;
					}
					else
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ConsoleAutoCompleteDropdown"), this.inputField.transform);
						this.autoCompleteDropdown = gameObject.GetComponent<TMP_Dropdown>();
						this.autoComplete = new Console.AutoComplete(Console.instance);
						this.autoCompleteDropdown.onValueChanged.AddListener(new UnityAction<int>(this.ApplyAutoComplete));
					}
				}
				if (this.autoComplete != null && this.autoComplete.SetSearchString(text))
				{
					List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
					List<string> resultsList = this.autoComplete.resultsList;
					for (int i = 0; i < resultsList.Count; i++)
					{
						list.Add(new TMP_Dropdown.OptionData(resultsList[i]));
					}
					this.autoCompleteDropdown.options = list;
				}
			}
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x000A007C File Offset: 0x0009E27C
		public void Update()
		{
			EventSystem eventSystem = MPEventSystemManager.FindEventSystem(ReInput.players.GetPlayer(0));
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				ConsoleWindow.InputFieldState inputFieldState = ConsoleWindow.InputFieldState.Neutral;
				if (this.autoCompleteDropdown && this.autoCompleteInUse)
				{
					inputFieldState = ConsoleWindow.InputFieldState.AutoComplete;
				}
				else if (this.historyIndex != -1)
				{
					inputFieldState = ConsoleWindow.InputFieldState.History;
				}
				bool keyDown = Input.GetKeyDown(KeyCode.UpArrow);
				bool keyDown2 = Input.GetKeyDown(KeyCode.DownArrow);
				switch (inputFieldState)
				{
				case ConsoleWindow.InputFieldState.Neutral:
					if (keyDown)
					{
						if (Console.userCmdHistory.Count > 0)
						{
							this.historyIndex = Console.userCmdHistory.Count - 1;
							this.preventHistoryReset = true;
							this.inputField.text = Console.userCmdHistory[this.historyIndex];
							this.inputField.MoveToEndOfLine(false, false);
							this.preventHistoryReset = false;
						}
					}
					else if (keyDown2 && this.autoCompleteDropdown)
					{
						this.autoCompleteDropdown.Show();
						this.autoCompleteDropdown.value = 0;
						this.autoCompleteDropdown.onValueChanged.Invoke(this.autoCompleteDropdown.value);
					}
					break;
				case ConsoleWindow.InputFieldState.History:
				{
					int num = 0;
					if (keyDown)
					{
						num--;
					}
					if (keyDown2)
					{
						num++;
					}
					if (num != 0)
					{
						this.historyIndex += num;
						if (this.historyIndex < 0)
						{
							this.historyIndex = 0;
						}
						if (this.historyIndex >= Console.userCmdHistory.Count)
						{
							this.historyIndex = -1;
						}
						else
						{
							this.preventHistoryReset = true;
							this.inputField.text = Console.userCmdHistory[this.historyIndex];
							this.inputField.MoveToEndOfLine(false, false);
							this.preventHistoryReset = false;
						}
					}
					break;
				}
				case ConsoleWindow.InputFieldState.AutoComplete:
					if (keyDown2)
					{
						TMP_Dropdown tmp_Dropdown = this.autoCompleteDropdown;
						int value = tmp_Dropdown.value + 1;
						tmp_Dropdown.value = value;
					}
					if (keyDown)
					{
						if (this.autoCompleteDropdown.value > 0)
						{
							TMP_Dropdown tmp_Dropdown2 = this.autoCompleteDropdown;
							int value = tmp_Dropdown2.value - 1;
							tmp_Dropdown2.value = value;
						}
						else
						{
							this.autoCompleteDropdown.Hide();
						}
					}
					break;
				}
				eventSystem.SetSelectedGameObject(this.inputField.gameObject);
			}
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x000A02B4 File Offset: 0x0009E4B4
		private void ApplyAutoComplete(int optionIndex)
		{
			if (this.autoCompleteDropdown && this.autoCompleteDropdown.options.Count > optionIndex)
			{
				this.preventAutoCompleteUpdate = true;
				this.inputField.text = this.autoCompleteDropdown.options[optionIndex].text;
				this.inputField.MoveToEndOfLine(false, false);
				this.preventAutoCompleteUpdate = false;
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x000A0320 File Offset: 0x0009E520
		public void OnDisable()
		{
			Console.onLogReceived -= this.OnLogReceived;
			Console.onClear -= this.OnClear;
			this.inputField.onSubmit.RemoveListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			if (ConsoleWindow.instance == this)
			{
				ConsoleWindow.instance = null;
			}
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x0001828D File Offset: 0x0001648D
		private void OnLogReceived(Console.Log log)
		{
			this.RebuildOutput();
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x0001828D File Offset: 0x0001648D
		private void OnClear()
		{
			this.RebuildOutput();
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x000A039C File Offset: 0x0009E59C
		private void RebuildOutput()
		{
			float value = 0f;
			if (this.outputField.verticalScrollbar)
			{
				value = this.outputField.verticalScrollbar.value;
			}
			string[] array = new string[Console.logs.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Console.logs[i].message;
			}
			this.outputField.text = string.Join("\n", array);
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
				this.outputField.verticalScrollbar.value = value;
			}
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000A0468 File Offset: 0x0009E668
		private static void CheckConsoleKey()
		{
			bool keyDown = Input.GetKeyDown(KeyCode.BackQuote);
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && keyDown)
			{
				ConsoleWindow.cvConsoleEnabled.SetBool(!ConsoleWindow.cvConsoleEnabled.value);
			}
			if (ConsoleWindow.cvConsoleEnabled.value && keyDown)
			{
				if (ConsoleWindow.instance)
				{
					UnityEngine.Object.Destroy(ConsoleWindow.instance.gameObject);
				}
				else
				{
					UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ConsoleWindow")).GetComponent<ConsoleWindow>().inputField.ActivateInputField();
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape) && ConsoleWindow.instance)
			{
				UnityEngine.Object.Destroy(ConsoleWindow.instance.gameObject);
			}
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x00018295 File Offset: 0x00016495
		[RuntimeInitializeOnLoadMethod]
		public static void Init()
		{
			RoR2Application.onUpdate += ConsoleWindow.CheckConsoleKey;
		}

		// Token: 0x04002397 RID: 9111
		public TMP_InputField inputField;

		// Token: 0x04002398 RID: 9112
		public TMP_InputField outputField;

		// Token: 0x04002399 RID: 9113
		private TMP_Dropdown autoCompleteDropdown;

		// Token: 0x0400239A RID: 9114
		private Console.AutoComplete autoComplete;

		// Token: 0x0400239B RID: 9115
		private bool preventAutoCompleteUpdate;

		// Token: 0x0400239C RID: 9116
		private bool preventHistoryReset;

		// Token: 0x0400239D RID: 9117
		private int historyIndex = -1;

		// Token: 0x0400239E RID: 9118
		private const string consoleEnabledDefaultValue = "0";

		// Token: 0x0400239F RID: 9119
		private static BoolConVar cvConsoleEnabled = new BoolConVar("console_enabled", ConVarFlags.None, "0", "Enables/Disables the console.");

		// Token: 0x020005C1 RID: 1473
		private enum InputFieldState
		{
			// Token: 0x040023A1 RID: 9121
			Neutral,
			// Token: 0x040023A2 RID: 9122
			History,
			// Token: 0x040023A3 RID: 9123
			AutoComplete
		}
	}
}
