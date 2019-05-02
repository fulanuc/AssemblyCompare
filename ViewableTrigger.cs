using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000652 RID: 1618
	public class ViewableTrigger : MonoBehaviour
	{
		// Token: 0x06002440 RID: 9280 RVA: 0x0001A651 File Offset: 0x00018851
		private void OnEnable()
		{
			ViewableTrigger.TriggerView(this.viewableName);
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x0001A65E File Offset: 0x0001885E
		public static void TriggerView(string viewableName)
		{
			if (string.IsNullOrEmpty(viewableName))
			{
				return;
			}
			LocalUserManager.readOnlyLocalUsersList[0].userProfile.MarkViewableAsViewed(viewableName);
		}

		// Token: 0x04002729 RID: 10025
		[Tooltip("The name of the viewable to mark as viewed when this component becomes enabled.")]
		public string viewableName;
	}
}
