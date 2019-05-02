using System;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BC RID: 1468
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ChatBox : MonoBehaviour
	{
		// Token: 0x060020E4 RID: 8420 RVA: 0x0009F6A0 File Offset: 0x0009D8A0
		private void UpdateFade(float deltaTime)
		{
			this.fadeTimer -= deltaTime;
			if (!this.fadeGroup)
			{
				return;
			}
			float alpha;
			if (this.showInput)
			{
				alpha = 1f;
				this.ResetFadeTimer();
			}
			else if (this.fadeTimer < 0f)
			{
				alpha = 0f;
			}
			else if (this.fadeTimer < 5f)
			{
				alpha = Mathf.Sqrt(Util.Remap(this.fadeTimer, 5f, 0f, 1f, 0f));
			}
			else
			{
				alpha = 1f;
			}
			this.fadeGroup.alpha = alpha;
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x00017F09 File Offset: 0x00016109
		private void ResetFadeTimer()
		{
			this.fadeTimer = 10f;
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x060020E6 RID: 8422 RVA: 0x00017F16 File Offset: 0x00016116
		private bool showKeybindTipOnStart
		{
			get
			{
				return Chat.readOnlyLog.Count == 0;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060020E7 RID: 8423 RVA: 0x00017F25 File Offset: 0x00016125
		// (set) Token: 0x060020E8 RID: 8424 RVA: 0x0009F73C File Offset: 0x0009D93C
		private bool showInput
		{
			get
			{
				return this._showInput;
			}
			set
			{
				if (this._showInput != value)
				{
					this._showInput = value;
					if (this.inputField)
					{
						this.inputField.gameObject.SetActive(this._showInput);
					}
					if (this.sendButton)
					{
						this.sendButton.gameObject.SetActive(this._showInput);
					}
					for (int i = 0; i < this.gameplayHiddenGraphics.Length; i++)
					{
						this.gameplayHiddenGraphics[i].enabled = this._showInput;
					}
					if (this._showInput)
					{
						this.FocusInputField();
						return;
					}
					this.UnfocusInputField();
				}
			}
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x0009F7E0 File Offset: 0x0009D9E0
		public void SubmitChat()
		{
			string text = this.inputField.text;
			if (text != "")
			{
				this.inputField.text = "";
				ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = NetworkUser.readOnlyLocalPlayersList;
				if (readOnlyLocalPlayersList.Count > 0)
				{
					string text2 = text;
					text2 = text2.Replace("\\", "\\\\");
					text2 = text2.Replace("\"", "\\\"");
					Console.instance.SubmitCmd(readOnlyLocalPlayersList[0], "say \"" + text2 + "\"", false);
					Debug.Log("Submitting say cmd.");
				}
			}
			Debug.LogFormat("SubmitChat() submittedText={0}", new object[]
			{
				text
			});
			this.showInput = false;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnInputFieldEndEdit()
		{
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00017F2D File Offset: 0x0001612D
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.showInput = true;
			this.showInput = false;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00017F49 File Offset: 0x00016149
		private void Start()
		{
			if (this.showKeybindTipOnStart && !RoR2Application.isInSinglePlayer)
			{
				Chat.AddMessage(Language.GetString("CHAT_KEYBIND_TIP"));
			}
			this.BuildChat();
			this.ScrollToBottom();
			this.inputField.resetOnDeActivation = true;
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00017F81 File Offset: 0x00016181
		private void OnEnable()
		{
			this.BuildChat();
			this.ScrollToBottom();
			base.Invoke("ScrollToBottom", 0f);
			Chat.onChatChanged += this.OnChatChangedHandler;
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x00017FB0 File Offset: 0x000161B0
		private void OnDisable()
		{
			Chat.onChatChanged -= this.OnChatChangedHandler;
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x00017FC3 File Offset: 0x000161C3
		private void OnChatChangedHandler()
		{
			float value = this.messagesText.verticalScrollbar.value;
			this.ResetFadeTimer();
			this.BuildChat();
			this.ScrollToBottom();
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00017FE8 File Offset: 0x000161E8
		public void ScrollToBottom()
		{
			this.messagesText.verticalScrollbar.value = 0f;
			this.messagesText.verticalScrollbar.value = 1f;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x0009F890 File Offset: 0x0009DA90
		private void BuildChat()
		{
			ReadOnlyCollection<string> readOnlyLog = Chat.readOnlyLog;
			string[] array = new string[readOnlyLog.Count];
			readOnlyLog.CopyTo(array, 0);
			this.messagesText.text = string.Join("\n", array);
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x0009F8CC File Offset: 0x0009DACC
		private void Update()
		{
			this.UpdateFade(Time.deltaTime);
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			GameObject gameObject = eventSystem ? eventSystem.currentSelectedGameObject : null;
			bool flag = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
			if (!this.showInput && flag && !(ConsoleWindow.instance != null))
			{
				this.showInput = true;
				return;
			}
			if (gameObject == this.inputField.gameObject)
			{
				if (flag)
				{
					if (this.showInput)
					{
						this.SubmitChat();
					}
					else if (!gameObject)
					{
						this.showInput = true;
					}
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					this.showInput = false;
					return;
				}
			}
			else
			{
				this.showInput = false;
			}
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x0009F98C File Offset: 0x0009DB8C
		private void FocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem)
			{
				eventSystem.SetSelectedGameObject(this.inputField.gameObject);
			}
			this.inputField.ActivateInputField();
			this.inputField.text = "";
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x0009F9DC File Offset: 0x0009DBDC
		private void UnfocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				eventSystem.SetSelectedGameObject(null);
			}
			this.inputField.DeactivateInputField();
		}

		// Token: 0x04002374 RID: 9076
		public TMP_InputField messagesText;

		// Token: 0x04002375 RID: 9077
		public TMP_InputField inputField;

		// Token: 0x04002376 RID: 9078
		public Button sendButton;

		// Token: 0x04002377 RID: 9079
		public Graphic[] gameplayHiddenGraphics;

		// Token: 0x04002378 RID: 9080
		[Tooltip("The canvas group to use to fade this chat box. Leave empty for no fading behavior.")]
		public CanvasGroup fadeGroup;

		// Token: 0x04002379 RID: 9081
		private const float fadeWait = 5f;

		// Token: 0x0400237A RID: 9082
		private const float fadeDuration = 5f;

		// Token: 0x0400237B RID: 9083
		private float fadeTimer;

		// Token: 0x0400237C RID: 9084
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x0400237D RID: 9085
		private bool _showInput;
	}
}
