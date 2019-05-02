using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI
{
	// Token: 0x02000618 RID: 1560
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(MPEventSystemProvider))]
	public class PauseScreenController : MonoBehaviour
	{
		// Token: 0x06002324 RID: 8996 RVA: 0x00019A4A File Offset: 0x00017C4A
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x000A873C File Offset: 0x000A693C
		private void OnEnable()
		{
			if (PauseScreenController.instancesList.Count == 0)
			{
				PauseScreenController.paused = NetworkServer.dontListen;
				if (PauseScreenController.paused)
				{
					if (RoR2Application.onPauseStartGlobal != null)
					{
						RoR2Application.onPauseStartGlobal();
					}
					PauseScreenController.oldTimeScale = Time.timeScale;
					Time.timeScale = 0f;
				}
			}
			PauseScreenController.instancesList.Add(this);
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x000A8798 File Offset: 0x000A6998
		private void OnDisable()
		{
			PauseScreenController.instancesList.Remove(this);
			if (PauseScreenController.instancesList.Count == 0 && PauseScreenController.paused)
			{
				Time.timeScale = PauseScreenController.oldTimeScale;
				PauseScreenController.paused = false;
				if (RoR2Application.onPauseEndGlobal != null)
				{
					RoR2Application.onPauseEndGlobal();
				}
			}
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x00019A58 File Offset: 0x00017C58
		public void OpenSettingsMenu()
		{
			UnityEngine.Object.Destroy(this.submenuObject);
			this.submenuObject = UnityEngine.Object.Instantiate<GameObject>(this.settingsPanelPrefab, this.rootPanel);
			this.mainPanel.gameObject.SetActive(false);
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x00019A8D File Offset: 0x00017C8D
		public void Update()
		{
			if (!this.submenuObject)
			{
				this.mainPanel.gameObject.SetActive(true);
			}
		}

		// Token: 0x040025F7 RID: 9719
		public static readonly List<PauseScreenController> instancesList = new List<PauseScreenController>();

		// Token: 0x040025F8 RID: 9720
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x040025F9 RID: 9721
		[Tooltip("The main panel to which any submenus will be parented.")]
		public RectTransform rootPanel;

		// Token: 0x040025FA RID: 9722
		[Tooltip("The panel which contains the main controls. This will be disabled while in a submenu.")]
		public RectTransform mainPanel;

		// Token: 0x040025FB RID: 9723
		public GameObject settingsPanelPrefab;

		// Token: 0x040025FC RID: 9724
		private GameObject submenuObject;

		// Token: 0x040025FD RID: 9725
		private static float oldTimeScale;

		// Token: 0x040025FE RID: 9726
		private static bool paused = false;
	}
}
