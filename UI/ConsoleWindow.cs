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
	// Token: 0x020005D2 RID: 1490
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class ConsoleWindow : MonoBehaviour
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060021A7 RID: 8615 RVA: 0x00018901 File Offset: 0x00016B01
		// (set) Token: 0x060021A8 RID: 8616 RVA: 0x00018908 File Offset: 0x00016B08
		public static ConsoleWindow instance { get; private set; }

		// Token: 0x060021A9 RID: 8617 RVA: 0x000A13D0 File Offset: 0x0009F5D0
		public void Start()
		{
			base.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.kbmEventSystem;
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 1f;
			}
			this.outputField.textComponent.gameObject.AddComponent<RectTransformDimensionsChangeEvent>().onRectTransformDimensionsChange += this.OnOutputFieldRectTransformDimensionsChange;
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x00018910 File Offset: 0x00016B10
		private void OnOutputFieldRectTransformDimensionsChange()
		{
			if (this.outputField.verticalScrollbar)
			{
				this.outputField.verticalScrollbar.value = 0f;
				this.outputField.verticalScrollbar.value = 1f;
			}
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x000A143C File Offset: 0x0009F63C
		public void OnEnable()
		{
			Console.onLogReceived += this.OnLogReceived;
			Console.onClear += this.OnClear;
			this.RebuildOutput();
			this.inputField.onSubmit.AddListener(new UnityAction<string>(this.Submit));
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldValueChanged));
			ConsoleWindow.instance = this;
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x0001894E File Offset: 0x00016B4E
		public void SubmitInputField()
		{
			this.inputField.onSubmit.Invoke(this.inputField.text);
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x000A14B0 File Offset: 0x0009F6B0
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

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060021AE RID: 8622 RVA: 0x0001896B File Offset: 0x00016B6B
		private bool autoCompleteInUse
		{
			get
			{
				return this.autoCompleteDropdown && this.autoCompleteDropdown.IsExpanded;
			}
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x000A1548 File Offset: 0x0009F748
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

		// Token: 0x060021B0 RID: 8624 RVA: 0x000A1650 File Offset: 0x0009F850
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

		// Token: 0x060021B1 RID: 8625 RVA: 0x000A1888 File Offset: 0x0009FA88
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

		// Token: 0x060021B2 RID: 8626 RVA: 0x000A18F4 File Offset: 0x0009FAF4
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

		// Token: 0x060021B3 RID: 8627 RVA: 0x00018987 File Offset: 0x00016B87
		private void OnLogReceived(Console.Log log)
		{
			this.RebuildOutput();
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x00018987 File Offset: 0x00016B87
		private void OnClear()
		{
			this.RebuildOutput();
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x000A1970 File Offset: 0x0009FB70
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

		// Token: 0x060021B6 RID: 8630 RVA: 0x000A1A3C File Offset: 0x0009FC3C
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

		// Token: 0x060021B7 RID: 8631 RVA: 0x0001898F File Offset: 0x00016B8F
		[RuntimeInitializeOnLoadMethod]
		public static void Init()
		{
			RoR2Application.onUpdate += ConsoleWindow.CheckConsoleKey;
		}

		// Token: 0x040023EB RID: 9195
		public TMP_InputField inputField;

		// Token: 0x040023EC RID: 9196
		public TMP_InputField outputField;

		// Token: 0x040023ED RID: 9197
		private TMP_Dropdown autoCompleteDropdown;

		// Token: 0x040023EE RID: 9198
		private Console.AutoComplete autoComplete;

		// Token: 0x040023EF RID: 9199
		private bool preventAutoCompleteUpdate;

		// Token: 0x040023F0 RID: 9200
		private bool preventHistoryReset;

		// Token: 0x040023F1 RID: 9201
		private int historyIndex = -1;

		// Token: 0x040023F2 RID: 9202
		private const string consoleEnabledDefaultValue = "0";

		// Token: 0x040023F3 RID: 9203
		private static BoolConVar cvConsoleEnabled = new BoolConVar("console_enabled", ConVarFlags.None, "0", "Enables/Disables the console.");

		// Token: 0x020005D3 RID: 1491
		private enum InputFieldState
		{
			// Token: 0x040023F5 RID: 9205
			Neutral,
			// Token: 0x040023F6 RID: 9206
			History,
			// Token: 0x040023F7 RID: 9207
			AutoComplete
		}
	}
}
