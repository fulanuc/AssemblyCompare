using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.UI
{
	// Token: 0x0200062A RID: 1578
	[RequireComponent(typeof(MPEventSystemProvider))]
	[RequireComponent(typeof(RectTransform))]
	public class PauseScreenController : MonoBehaviour
	{
		// Token: 0x060023B4 RID: 9140 RVA: 0x0001A101 File Offset: 0x00018301
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponent<MPEventSystemProvider>();
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x000A9DB8 File Offset: 0x000A7FB8
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

		// Token: 0x060023B6 RID: 9142 RVA: 0x000A9E14 File Offset: 0x000A8014
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

		// Token: 0x060023B7 RID: 9143 RVA: 0x0001A10F File Offset: 0x0001830F
		public void OpenSettingsMenu()
		{
			UnityEngine.Object.Destroy(this.submenuObject);
			this.submenuObject = UnityEngine.Object.Instantiate<GameObject>(this.settingsPanelPrefab, this.rootPanel);
			this.mainPanel.gameObject.SetActive(false);
		}

		// Token: 0x060023B8 RID: 9144 RVA: 0x0001A144 File Offset: 0x00018344
		public void Update()
		{
			if (!this.submenuObject)
			{
				this.mainPanel.gameObject.SetActive(true);
			}
			if (!NetworkManager.singleton.isNetworkActive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002652 RID: 9810
		public static readonly List<PauseScreenController> instancesList = new List<PauseScreenController>();

		// Token: 0x04002653 RID: 9811
		private MPEventSystemProvider eventSystemProvider;

		// Token: 0x04002654 RID: 9812
		[Tooltip("The main panel to which any submenus will be parented.")]
		public RectTransform rootPanel;

		// Token: 0x04002655 RID: 9813
		[Tooltip("The panel which contains the main controls. This will be disabled while in a submenu.")]
		public RectTransform mainPanel;

		// Token: 0x04002656 RID: 9814
		public GameObject settingsPanelPrefab;

		// Token: 0x04002657 RID: 9815
		private GameObject submenuObject;

		// Token: 0x04002658 RID: 9816
		private static float oldTimeScale;

		// Token: 0x04002659 RID: 9817
		private static bool paused = false;
	}
}
