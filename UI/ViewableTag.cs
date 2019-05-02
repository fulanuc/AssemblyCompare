using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000661 RID: 1633
	public class ViewableTag : MonoBehaviour
	{
		// Token: 0x060024C5 RID: 9413 RVA: 0x000ADA9C File Offset: 0x000ABC9C
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

		// Token: 0x060024C6 RID: 9414 RVA: 0x0001AC5E File Offset: 0x00018E5E
		private void OnEnable()
		{
			ViewableTag.instancesList.Add(this);
			RoR2Application.onNextUpdate += this.Refresh;
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x000ADB08 File Offset: 0x000ABD08
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

		// Token: 0x060024C8 RID: 9416 RVA: 0x0001AC7C File Offset: 0x00018E7C
		private void OnDisable()
		{
			ViewableTag.instancesList.Remove(this);
			this.Refresh();
			if (this.markAsViewedOnDisable)
			{
				ViewableTrigger.TriggerView(this.viewableName);
			}
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x0001ACA3 File Offset: 0x00018EA3
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

		// Token: 0x04002777 RID: 10103
		private static readonly List<ViewableTag> instancesList = new List<ViewableTag>();

		// Token: 0x04002778 RID: 10104
		[Tooltip("The path of the viewable that determines whether or not the \"NEW\" tag is activated.")]
		public string viewableName;

		// Token: 0x04002779 RID: 10105
		[Tooltip("Marks the named viewable as viewed when this component is disabled.")]
		public bool markAsViewedOnDisable;

		// Token: 0x0400277A RID: 10106
		public ViewableTag.ViewableVisualStyle viewableVisualStyle;

		// Token: 0x0400277B RID: 10107
		private static GameObject tagPrefab;

		// Token: 0x0400277C RID: 10108
		private GameObject tagInstance;

		// Token: 0x0400277D RID: 10109
		private static bool pendingRefreshAll = false;

		// Token: 0x02000662 RID: 1634
		public enum ViewableVisualStyle
		{
			// Token: 0x0400277F RID: 10111
			Button,
			// Token: 0x04002780 RID: 10112
			Icon
		}
	}
}
