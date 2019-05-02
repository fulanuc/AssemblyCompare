using System;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CE RID: 1486
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ChatBox : MonoBehaviour
	{
		// Token: 0x06002175 RID: 8565 RVA: 0x000A0C74 File Offset: 0x0009EE74
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

		// Token: 0x06002176 RID: 8566 RVA: 0x00018603 File Offset: 0x00016803
		private void ResetFadeTimer()
		{
			this.fadeTimer = 10f;
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06002177 RID: 8567 RVA: 0x00018610 File Offset: 0x00016810
		private bool showKeybindTipOnStart
		{
			get
			{
				return Chat.readOnlyLog.Count == 0;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06002178 RID: 8568 RVA: 0x0001861F File Offset: 0x0001681F
		// (set) Token: 0x06002179 RID: 8569 RVA: 0x000A0D10 File Offset: 0x0009EF10
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

		// Token: 0x0600217A RID: 8570 RVA: 0x000A0DB4 File Offset: 0x0009EFB4
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

		// Token: 0x0600217B RID: 8571 RVA: 0x000025DA File Offset: 0x000007DA
		public void OnInputFieldEndEdit()
		{
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x00018627 File Offset: 0x00016827
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.showInput = true;
			this.showInput = false;
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x00018643 File Offset: 0x00016843
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

		// Token: 0x0600217E RID: 8574 RVA: 0x0001867B File Offset: 0x0001687B
		private void OnEnable()
		{
			this.BuildChat();
			this.ScrollToBottom();
			base.Invoke("ScrollToBottom", 0f);
			Chat.onChatChanged += this.OnChatChangedHandler;
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x000186AA File Offset: 0x000168AA
		private void OnDisable()
		{
			Chat.onChatChanged -= this.OnChatChangedHandler;
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x000186BD File Offset: 0x000168BD
		private void OnChatChangedHandler()
		{
			float value = this.messagesText.verticalScrollbar.value;
			this.ResetFadeTimer();
			this.BuildChat();
			this.ScrollToBottom();
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x000186E2 File Offset: 0x000168E2
		public void ScrollToBottom()
		{
			this.messagesText.verticalScrollbar.value = 0f;
			this.messagesText.verticalScrollbar.value = 1f;
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x000A0E64 File Offset: 0x0009F064
		private void BuildChat()
		{
			ReadOnlyCollection<string> readOnlyLog = Chat.readOnlyLog;
			string[] array = new string[readOnlyLog.Count];
			readOnlyLog.CopyTo(array, 0);
			this.messagesText.text = string.Join("\n", array);
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x000A0EA0 File Offset: 0x0009F0A0
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

		// Token: 0x06002184 RID: 8580 RVA: 0x000A0F60 File Offset: 0x0009F160
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

		// Token: 0x06002185 RID: 8581 RVA: 0x000A0FB0 File Offset: 0x0009F1B0
		private void UnfocusInputField()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			if (eventSystem && eventSystem.currentSelectedGameObject == this.inputField.gameObject)
			{
				eventSystem.SetSelectedGameObject(null);
			}
			this.inputField.DeactivateInputField();
		}

		// Token: 0x040023C8 RID: 9160
		public TMP_InputField messagesText;

		// Token: 0x040023C9 RID: 9161
		public TMP_InputField inputField;

		// Token: 0x040023CA RID: 9162
		public Button sendButton;

		// Token: 0x040023CB RID: 9163
		public Graphic[] gameplayHiddenGraphics;

		// Token: 0x040023CC RID: 9164
		[Tooltip("The canvas group to use to fade this chat box. Leave empty for no fading behavior.")]
		public CanvasGroup fadeGroup;

		// Token: 0x040023CD RID: 9165
		private const float fadeWait = 5f;

		// Token: 0x040023CE RID: 9166
		private const float fadeDuration = 5f;

		// Token: 0x040023CF RID: 9167
		private float fadeTimer;

		// Token: 0x040023D0 RID: 9168
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040023D1 RID: 9169
		private bool _showInput;
	}
}
