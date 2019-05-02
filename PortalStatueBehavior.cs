using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000391 RID: 913
	public class PortalStatueBehavior : MonoBehaviour
	{
		// Token: 0x0600133D RID: 4925 RVA: 0x0006BE84 File Offset: 0x0006A084
		public void GrantPortalEntry()
		{
			PortalStatueBehavior.PortalType portalType = this.portalType;
			if (portalType != PortalStatueBehavior.PortalType.Shop)
			{
				if (portalType == PortalStatueBehavior.PortalType.Goldshores)
				{
					if (TeleporterInteraction.instance)
					{
						TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
					}
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
					{
						origin = base.transform.position,
						rotation = Quaternion.identity,
						scale = 1f,
						color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Money)
					}, true);
				}
			}
			else
			{
				if (TeleporterInteraction.instance)
				{
					TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
				}
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
				{
					origin = base.transform.position,
					rotation = Quaternion.identity,
					scale = 1f,
					color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem)
				}, true);
			}
			foreach (PortalStatueBehavior portalStatueBehavior in UnityEngine.Object.FindObjectsOfType<PortalStatueBehavior>())
			{
				if (portalStatueBehavior.portalType == this.portalType)
				{
					PurchaseInteraction component = portalStatueBehavior.GetComponent<PurchaseInteraction>();
					if (component)
					{
						component.Networkavailable = false;
					}
				}
			}
		}

		// Token: 0x040016EB RID: 5867
		public PortalStatueBehavior.PortalType portalType;

		// Token: 0x02000392 RID: 914
		public enum PortalType
		{
			// Token: 0x040016ED RID: 5869
			Shop,
			// Token: 0x040016EE RID: 5870
			Goldshores,
			// Token: 0x040016EF RID: 5871
			Count
		}
	}
}
