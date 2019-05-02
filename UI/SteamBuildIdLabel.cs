using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064E RID: 1614
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SteamBuildIdLabel : MonoBehaviour
	{
		// Token: 0x06002455 RID: 9301 RVA: 0x000AC154 File Offset: 0x000AA354
		private void Start()
		{
			if (Client.Instance != null)
			{
				string text = "Steam Build ID " + RoR2Application.GetBuildId();
				string betaName = Client.Instance.BetaName;
				if (!string.IsNullOrEmpty(betaName))
				{
					text = text + "[" + betaName + "]";
				}
				base.GetComponent<TextMeshProUGUI>().text = text;
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
