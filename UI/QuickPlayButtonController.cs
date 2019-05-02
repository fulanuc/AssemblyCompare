using System;
using RoR2.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000632 RID: 1586
	public class QuickPlayButtonController : UIBehaviour
	{
		// Token: 0x060023CF RID: 9167 RVA: 0x0001A1F2 File Offset: 0x000183F2
		protected new void Start()
		{
			base.Start();
			this.Update();
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x000AA5E0 File Offset: 0x000A87E0
		protected void Update()
		{
			bool running = SteamLobbyFinder.running;
			this.quickplayStateText.text = SteamLobbyFinder.GetResolvedStateString();
			if (running)
			{
				this.spinnerRectTransform.gameObject.SetActive(true);
				this.quickplayStateText.gameObject.SetActive(true);
				Vector3 localEulerAngles = this.spinnerRectTransform.localEulerAngles;
				localEulerAngles.z += Time.deltaTime * 360f;
				this.spinnerRectTransform.localEulerAngles = localEulerAngles;
				this.labelController.token = this.stopToken;
				return;
			}
			this.spinnerRectTransform.gameObject.SetActive(false);
			this.quickplayStateText.gameObject.SetActive(false);
			this.labelController.token = this.startToken;
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x0001A200 File Offset: 0x00018400
		public void ToggleQuickplay()
		{
			Console.instance.SubmitCmd(null, SteamLobbyFinder.running ? "steam_quickplay_stop" : "steam_quickplay_start", false);
		}

		// Token: 0x04002686 RID: 9862
		public LanguageTextMeshController labelController;

		// Token: 0x04002687 RID: 9863
		public string startToken;

		// Token: 0x04002688 RID: 9864
		public string stopToken;

		// Token: 0x04002689 RID: 9865
		public RectTransform spinnerRectTransform;

		// Token: 0x0400268A RID: 9866
		public TextMeshProUGUI quickplayStateText;
	}
}
