using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063C RID: 1596
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SteamBuildIdLabel : MonoBehaviour
	{
		// Token: 0x060023C5 RID: 9157 RVA: 0x000AAAD8 File Offset: 0x000A8CD8
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
