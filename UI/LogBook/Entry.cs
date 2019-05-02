using System;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000668 RID: 1640
	public class Entry
	{
		// Token: 0x0600249A RID: 9370 RVA: 0x000AD6CC File Offset: 0x000AB8CC
		public static GameObject GetModelForPickup(Entry entry)
		{
			return ((PickupIndex)entry.extraData).GetPickupDisplayPrefab();
		}

		// Token: 0x04002793 RID: 10131
		public string nameToken;

		// Token: 0x04002794 RID: 10132
		public string categoryTypeToken;

		// Token: 0x04002795 RID: 10133
		public GameObject pagePrefab;

		// Token: 0x04002796 RID: 10134
		public Texture iconTexture;

		// Token: 0x04002797 RID: 10135
		public Texture bgTexture;

		// Token: 0x04002798 RID: 10136
		public Color color;

		// Token: 0x04002799 RID: 10137
		public GameObject modelPrefab;

		// Token: 0x0400279A RID: 10138
		public object extraData;

		// Token: 0x0400279B RID: 10139
		public bool isWIP;

		// Token: 0x0400279C RID: 10140
		public Func<UserProfile, Entry, EntryStatus> getStatus = (UserProfile userProfile, Entry entry) => EntryStatus.Unimplemented;

		// Token: 0x0400279D RID: 10141
		public Func<UserProfile, Entry, EntryStatus, TooltipContent> getTooltipContent = (UserProfile userProfile, Entry entry, EntryStatus status) => default(TooltipContent);

		// Token: 0x0400279E RID: 10142
		public UnlockableDef associatedUnlockable;

		// Token: 0x0400279F RID: 10143
		public Action<PageBuilder> addEntries;

		// Token: 0x040027A0 RID: 10144
		public ViewablesCatalog.Node viewableNode;
	}
}
