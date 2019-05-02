using System;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x0200067A RID: 1658
	public class Entry
	{
		// Token: 0x06002531 RID: 9521 RVA: 0x000AEDBC File Offset: 0x000ACFBC
		public static GameObject GetModelForPickup(Entry entry)
		{
			return ((PickupIndex)entry.extraData).GetPickupDisplayPrefab();
		}

		// Token: 0x040027EF RID: 10223
		public string nameToken;

		// Token: 0x040027F0 RID: 10224
		public string categoryTypeToken;

		// Token: 0x040027F1 RID: 10225
		public GameObject pagePrefab;

		// Token: 0x040027F2 RID: 10226
		public Texture iconTexture;

		// Token: 0x040027F3 RID: 10227
		public Texture bgTexture;

		// Token: 0x040027F4 RID: 10228
		public Color color;

		// Token: 0x040027F5 RID: 10229
		public GameObject modelPrefab;

		// Token: 0x040027F6 RID: 10230
		public object extraData;

		// Token: 0x040027F7 RID: 10231
		public bool isWIP;

		// Token: 0x040027F8 RID: 10232
		public Func<UserProfile, Entry, EntryStatus> getStatus = (UserProfile userProfile, Entry entry) => EntryStatus.Unimplemented;

		// Token: 0x040027F9 RID: 10233
		public Func<UserProfile, Entry, EntryStatus, TooltipContent> getTooltipContent = (UserProfile userProfile, Entry entry, EntryStatus status) => default(TooltipContent);

		// Token: 0x040027FA RID: 10234
		public UnlockableDef associatedUnlockable;

		// Token: 0x040027FB RID: 10235
		public Action<PageBuilder> addEntries;

		// Token: 0x040027FC RID: 10236
		public ViewablesCatalog.Node viewableNode;
	}
}
