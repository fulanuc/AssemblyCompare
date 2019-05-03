using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x0200040D RID: 1037
	[RequireComponent(typeof(Camera))]
	public class UICamera : MonoBehaviour
	{
		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06001729 RID: 5929 RVA: 0x00079B2C File Offset: 0x00077D2C
		// (remove) Token: 0x0600172A RID: 5930 RVA: 0x00079B60 File Offset: 0x00077D60
		public static event UICamera.UICameraDelegate onUICameraPreCull;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x0600172B RID: 5931 RVA: 0x00079B94 File Offset: 0x00077D94
		// (remove) Token: 0x0600172C RID: 5932 RVA: 0x00079BC8 File Offset: 0x00077DC8
		public static event UICamera.UICameraDelegate onUICameraPreRender;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x0600172D RID: 5933 RVA: 0x00079BFC File Offset: 0x00077DFC
		// (remove) Token: 0x0600172E RID: 5934 RVA: 0x00079C30 File Offset: 0x00077E30
		public static event UICamera.UICameraDelegate onUICameraPostRender;

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x00011558 File Offset: 0x0000F758
		// (set) Token: 0x06001730 RID: 5936 RVA: 0x00011560 File Offset: 0x0000F760
		public Camera camera { get; private set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x00011569 File Offset: 0x0000F769
		// (set) Token: 0x06001732 RID: 5938 RVA: 0x00011571 File Offset: 0x0000F771
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001733 RID: 5939 RVA: 0x0001157A File Offset: 0x0000F77A
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x00011594 File Offset: 0x0000F794
		private void OnEnable()
		{
			UICamera.instancesList.Add(this);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x000115A1 File Offset: 0x0000F7A1
		private void OnDisable()
		{
			UICamera.instancesList.Remove(this);
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000115AF File Offset: 0x0000F7AF
		private void OnPreCull()
		{
			if (UICamera.onUICameraPreCull != null)
			{
				UICamera.onUICameraPreCull(this);
			}
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x000115C3 File Offset: 0x0000F7C3
		private void OnPreRender()
		{
			if (UICamera.onUICameraPreRender != null)
			{
				UICamera.onUICameraPreRender(this);
			}
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x000115D7 File Offset: 0x0000F7D7
		private void OnPostRender()
		{
			if (UICamera.onUICameraPostRender != null)
			{
				UICamera.onUICameraPostRender(this);
			}
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x000115EB File Offset: 0x0000F7EB
		public EventSystem GetAssociatedEventSystem()
		{
			if (this.cameraRigController.viewer && this.cameraRigController.viewer.localUser != null)
			{
				return this.cameraRigController.viewer.localUser.eventSystem;
			}
			return null;
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x00079C64 File Offset: 0x00077E64
		public static UICamera FindViewerUICamera(LocalUser localUserViewer)
		{
			if (localUserViewer != null)
			{
				for (int i = 0; i < UICamera.instancesList.Count; i++)
				{
					if (UICamera.instancesList[i].cameraRigController.viewer.localUser == localUserViewer)
					{
						return UICamera.instancesList[i];
					}
				}
			}
			return null;
		}

		// Token: 0x04001A4E RID: 6734
		private static readonly List<UICamera> instancesList = new List<UICamera>();

		// Token: 0x04001A4F RID: 6735
		public static readonly ReadOnlyCollection<UICamera> readOnlyInstancesList = new ReadOnlyCollection<UICamera>(UICamera.instancesList);

		// Token: 0x0200040E RID: 1038
		// (Invoke) Token: 0x0600173E RID: 5950
		public delegate void UICameraDelegate(UICamera sceneCamera);
	}
}
