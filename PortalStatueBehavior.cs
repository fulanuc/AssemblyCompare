using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038C RID: 908
	public class PortalStatueBehavior : MonoBehaviour
	{
		// Token: 0x0600131F RID: 4895 RVA: 0x0006BC18 File Offset: 0x00069E18
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

		// Token: 0x040016CF RID: 5839
		public PortalStatueBehavior.PortalType portalType;

		// Token: 0x0200038D RID: 909
		public enum PortalType
		{
			// Token: 0x040016D1 RID: 5841
			Shop,
			// Token: 0x040016D2 RID: 5842
			Goldshores,
			// Token: 0x040016D3 RID: 5843
			Count
		}
	}
}
