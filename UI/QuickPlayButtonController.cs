using System;
using RoR2.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000620 RID: 1568
	public class QuickPlayButtonController : UIBehaviour
	{
		// Token: 0x0600233F RID: 9023 RVA: 0x00019B24 File Offset: 0x00017D24
		protected new void Start()
		{
			base.Start();
			this.Update();
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x000A8F64 File Offset: 0x000A7164
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

		// Token: 0x06002341 RID: 9025 RVA: 0x00019B32 File Offset: 0x00017D32
		public void ToggleQuickplay()
		{
			Console.instance.SubmitCmd(null, SteamLobbyFinder.running ? "steam_quickplay_stop" : "steam_quickplay_start", false);
		}

		// Token: 0x0400262B RID: 9771
		public LanguageTextMeshController labelController;

		// Token: 0x0400262C RID: 9772
		public string startToken;

		// Token: 0x0400262D RID: 9773
		public string stopToken;

		// Token: 0x0400262E RID: 9774
		public RectTransform spinnerRectTransform;

		// Token: 0x0400262F RID: 9775
		public TextMeshProUGUI quickplayStateText;
	}
}
