using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064F RID: 1615
	public class ViewableTag : MonoBehaviour
	{
		// Token: 0x06002435 RID: 9269 RVA: 0x000AC41C File Offset: 0x000AA61C
		private bool Check()
		{
			if (LocalUserManager.readOnlyLocalUsersList.Count == 0)
			{
				return false;
			}
			UserProfile userProfile = LocalUserManager.readOnlyLocalUsersList[0].userProfile;
			ViewablesCatalog.Node node = ViewablesCatalog.FindNode(this.viewableName ?? "");
			if (node == null)
			{
				Debug.LogErrorFormat("Viewable {0} is not defined.", new object[]
				{
					this.viewableName
				});
				return false;
			}
			return node.shouldShowUnviewed(userProfile);
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x0001A586 File Offset: 0x00018786
		private void OnEnable()
		{
			ViewableTag.instancesList.Add(this);
			RoR2Application.onNextUpdate += this.Refresh;
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x000AC488 File Offset: 0x000AA688
		public void Refresh()
		{
			bool flag = base.enabled && this.Check();
			if (this.tagInstance != flag)
			{
				if (this.tagInstance)
				{
					UnityEngine.Object.Destroy(this.tagInstance);
					this.tagInstance = null;
					return;
				}
				string childName = this.viewableVisualStyle.ToString();
				this.tagInstance = UnityEngine.Object.Instantiate<GameObject>(ViewableTag.tagPrefab, base.transform);
				this.tagInstance.GetComponent<ChildLocator>().FindChild(childName).gameObject.SetActive(true);
			}
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x0001A5A4 File Offset: 0x000187A4
		private void OnDisable()
		{
			ViewableTag.instancesList.Remove(this);
			this.Refresh();
			if (this.markAsViewedOnDisable)
			{
				ViewableTrigger.TriggerView(this.viewableName);
			}
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x0001A5CB File Offset: 0x000187CB
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			ViewableTag.tagPrefab = Resources.Load<GameObject>("Prefabs/UI/NewViewableTag");
			UserProfile.onUserProfileViewedViewablesChanged += delegate(UserProfile userProfile)
			{
				if (!ViewableTag.pendingRefreshAll)
				{
					ViewableTag.pendingRefreshAll = true;
					RoR2Application.onNextUpdate += delegate()
					{
						foreach (ViewableTag viewableTag in ViewableTag.instancesList)
						{
							viewableTag.Refresh();
						}
						ViewableTag.pendingRefreshAll = false;
					};
				}
			};
		}

		// Token: 0x0400271C RID: 10012
		private static readonly List<ViewableTag> instancesList = new List<ViewableTag>();

		// Token: 0x0400271D RID: 10013
		[Tooltip("The path of the viewable that determines whether or not the \"NEW\" tag is activated.")]
		public string viewableName;

		// Token: 0x0400271E RID: 10014
		[Tooltip("Marks the named viewable as viewed when this component is disabled.")]
		public bool markAsViewedOnDisable;

		// Token: 0x0400271F RID: 10015
		public ViewableTag.ViewableVisualStyle viewableVisualStyle;

		// Token: 0x04002720 RID: 10016
		private static GameObject tagPrefab;

		// Token: 0x04002721 RID: 10017
		private GameObject tagInstance;

		// Token: 0x04002722 RID: 10018
		private static bool pendingRefreshAll = false;

		// Token: 0x02000650 RID: 1616
		public enum ViewableVisualStyle
		{
			// Token: 0x04002724 RID: 10020
			Button,
			// Token: 0x04002725 RID: 10021
			Icon
		}
	}
}
