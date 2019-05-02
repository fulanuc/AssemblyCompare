using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2
{
	// Token: 0x02000413 RID: 1043
	[RequireComponent(typeof(Camera))]
	public class UICamera : MonoBehaviour
	{
		// Token: 0x14000033 RID: 51
		// (add) Token: 0x0600176C RID: 5996 RVA: 0x0007A0EC File Offset: 0x000782EC
		// (remove) Token: 0x0600176D RID: 5997 RVA: 0x0007A120 File Offset: 0x00078320
		public static event UICamera.UICameraDelegate onUICameraPreCull;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x0600176E RID: 5998 RVA: 0x0007A154 File Offset: 0x00078354
		// (remove) Token: 0x0600176F RID: 5999 RVA: 0x0007A188 File Offset: 0x00078388
		public static event UICamera.UICameraDelegate onUICameraPreRender;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06001770 RID: 6000 RVA: 0x0007A1BC File Offset: 0x000783BC
		// (remove) Token: 0x06001771 RID: 6001 RVA: 0x0007A1F0 File Offset: 0x000783F0
		public static event UICamera.UICameraDelegate onUICameraPostRender;

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06001772 RID: 6002 RVA: 0x00011984 File Offset: 0x0000FB84
		// (set) Token: 0x06001773 RID: 6003 RVA: 0x0001198C File Offset: 0x0000FB8C
		public Camera camera { get; private set; }

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x00011995 File Offset: 0x0000FB95
		// (set) Token: 0x06001775 RID: 6005 RVA: 0x0001199D File Offset: 0x0000FB9D
		public CameraRigController cameraRigController { get; private set; }

		// Token: 0x06001776 RID: 6006 RVA: 0x000119A6 File Offset: 0x0000FBA6
		private void Awake()
		{
			this.camera = base.GetComponent<Camera>();
			this.cameraRigController = base.GetComponentInParent<CameraRigController>();
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x000119C0 File Offset: 0x0000FBC0
		private void OnEnable()
		{
			UICamera.instancesList.Add(this);
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x000119CD File Offset: 0x0000FBCD
		private void OnDisable()
		{
			UICamera.instancesList.Remove(this);
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x000119DB File Offset: 0x0000FBDB
		private void OnPreCull()
		{
			if (UICamera.onUICameraPreCull != null)
			{
				UICamera.onUICameraPreCull(this);
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x000119EF File Offset: 0x0000FBEF
		private void OnPreRender()
		{
			if (UICamera.onUICameraPreRender != null)
			{
				UICamera.onUICameraPreRender(this);
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00011A03 File Offset: 0x0000FC03
		private void OnPostRender()
		{
			if (UICamera.onUICameraPostRender != null)
			{
				UICamera.onUICameraPostRender(this);
			}
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00011A17 File Offset: 0x0000FC17
		public EventSystem GetAssociatedEventSystem()
		{
			if (this.cameraRigController.viewer && this.cameraRigController.viewer.localUser != null)
			{
				return this.cameraRigController.viewer.localUser.eventSystem;
			}
			return null;
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x0007A224 File Offset: 0x00078424
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

		// Token: 0x04001A77 RID: 6775
		private static readonly List<UICamera> instancesList = new List<UICamera>();

		// Token: 0x04001A78 RID: 6776
		public static readonly ReadOnlyCollection<UICamera> readOnlyInstancesList = new ReadOnlyCollection<UICamera>(UICamera.instancesList);

		// Token: 0x02000414 RID: 1044
		// (Invoke) Token: 0x06001781 RID: 6017
		public delegate void UICameraDelegate(UICamera sceneCamera);
	}
}
