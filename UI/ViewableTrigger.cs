using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000664 RID: 1636
	public class ViewableTrigger : MonoBehaviour
	{
		// Token: 0x060024D0 RID: 9424 RVA: 0x0001AD29 File Offset: 0x00018F29
		private void OnEnable()
		{
			ViewableTrigger.TriggerView(this.viewableName);
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x0001AD36 File Offset: 0x00018F36
		public static void TriggerView(string viewableName)
		{
			if (string.IsNullOrEmpty(viewableName))
			{
				return;
			}
			LocalUserManager.readOnlyLocalUsersList[0].userProfile.MarkViewableAsViewed(viewableName);
		}

		// Token: 0x04002784 RID: 10116
		[Tooltip("The name of the viewable to mark as viewed when this component becomes enabled.")]
		public string viewableName;
	}
}
