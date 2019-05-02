using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004B9 RID: 1209
	public static class SurvivorCatalog
	{
		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06001B5B RID: 7003 RVA: 0x000143FC File Offset: 0x000125FC
		public static IEnumerable<SurvivorDef> allSurvivorDefs
		{
			get
			{
				return SurvivorCatalog._allSurvivorDefs;
			}
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x00014403 File Offset: 0x00012603
		private static void RegisterSurvivor(SurvivorIndex survivorIndex, SurvivorDef survivorDef)
		{
			survivorDef.survivorIndex = survivorIndex;
			SurvivorCatalog.survivorDefs[(int)survivorIndex] = survivorDef;
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x00014414 File Offset: 0x00012614
		public static SurvivorDef GetSurvivorDef(SurvivorIndex survivorIndex)
		{
			if (survivorIndex < SurvivorIndex.Commando || survivorIndex > SurvivorIndex.Count)
			{
				return null;
			}
			return SurvivorCatalog.survivorDefs[(int)survivorIndex];
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x00088404 File Offset: 0x00086604
		public static SurvivorDef FindSurvivorDefFromBody(GameObject characterBodyPrefab)
		{
			for (int i = 0; i < SurvivorCatalog.survivorDefs.Length; i++)
			{
				SurvivorDef survivorDef = SurvivorCatalog.survivorDefs[i];
				GameObject y = (survivorDef != null) ? survivorDef.bodyPrefab : null;
				if (characterBodyPrefab == y)
				{
					return survivorDef;
				}
			}
			return null;
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x00088444 File Offset: 0x00086644
		public static Texture GetSurvivorPortrait(SurvivorIndex survivorIndex)
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
			if (survivorDef.bodyPrefab != null)
			{
				CharacterBody component = survivorDef.bodyPrefab.GetComponent<CharacterBody>();
				if (component)
				{
					return component.portraitIcon;
				}
			}
			return null;
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x00088484 File Offset: 0x00086684
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog)
		})]
		private static void Init()
		{
			SurvivorCatalog.survivorDefs = new SurvivorDef[7];
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Commando, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("CommandoBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay"),
				descriptionToken = "COMMANDO_DESCRIPTION",
				primaryColor = new Color(0.929411769f, 0.5882353f, 0.07058824f)
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Huntress, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("HuntressBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/HuntressDisplay"),
				primaryColor = new Color(0.8352941f, 0.235294119f, 0.235294119f),
				descriptionToken = "HUNTRESS_DESCRIPTION",
				unlockableName = "Characters.Huntress"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Toolbot, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("ToolbotBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/ToolbotDisplay"),
				descriptionToken = "TOOLBOT_DESCRIPTION",
				primaryColor = new Color(0.827451f, 0.768627465f, 0.3137255f),
				unlockableName = "Characters.Toolbot"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Engineer, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("EngiBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/EngiDisplay"),
				descriptionToken = "ENGI_DESCRIPTION",
				primaryColor = new Color(0.372549027f, 0.8862745f, 0.5254902f),
				unlockableName = "Characters.Engineer"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Mage, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("MageBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MageDisplay"),
				descriptionToken = "MAGE_DESCRIPTION",
				primaryColor = new Color(0.968627453f, 0.75686276f, 0.992156863f),
				unlockableName = "Characters.Mage"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Merc, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("MercBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MercDisplay"),
				descriptionToken = "MERC_DESCRIPTION",
				primaryColor = new Color(0.423529416f, 0.819607854f, 0.917647064f),
				unlockableName = "Characters.Mercenary"
			});
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Bandit, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/BanditDisplay"),
				descriptionToken = "BANDIT_DESCRIPTION",
				primaryColor = new Color(0.423529416f, 0.819607854f, 0.917647064f),
				unlockableName = "Characters.Mercenary"
			});
			for (SurvivorIndex survivorIndex = SurvivorIndex.Commando; survivorIndex < SurvivorIndex.Count; survivorIndex++)
			{
				if (SurvivorCatalog.survivorDefs[(int)survivorIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered survivor {0}!", new object[]
					{
						Enum.GetName(typeof(SurvivorIndex), survivorIndex)
					});
				}
			}
			SurvivorCatalog._allSurvivorDefs = (from v in SurvivorCatalog.survivorDefs
			where v != null
			select v).ToArray<SurvivorDef>();
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Survivors", true, null);
			using (IEnumerator<SurvivorDef> enumerator = SurvivorCatalog.allSurvivorDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SurvivorDef survivor = new SurvivorDef();
					survivor = enumerator.Current;
					ViewablesCatalog.Node survivorEntryNode = new ViewablesCatalog.Node(survivor.survivorIndex.ToString(), false, node);
					survivorEntryNode.shouldShowUnviewed = ((UserProfile userProfile) => !userProfile.HasViewedViewable(survivorEntryNode.fullName) && userProfile.HasSurvivorUnlocked(survivor.survivorIndex) && !string.IsNullOrEmpty(survivor.unlockableName));
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x04001DE3 RID: 7651
		public static int survivorMaxCount = 10;

		// Token: 0x04001DE4 RID: 7652
		private static SurvivorDef[] survivorDefs;

		// Token: 0x04001DE5 RID: 7653
		private static SurvivorDef[] _allSurvivorDefs;

		// Token: 0x04001DE6 RID: 7654
		public static SurvivorIndex[] idealSurvivorOrder = new SurvivorIndex[]
		{
			SurvivorIndex.Commando,
			SurvivorIndex.Toolbot,
			SurvivorIndex.Huntress,
			SurvivorIndex.Engineer,
			SurvivorIndex.Mage,
			SurvivorIndex.Merc,
			SurvivorIndex.Bandit
		};
	}
}
