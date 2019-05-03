﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2
{
	// Token: 0x020004C9 RID: 1225
	public static class UnlockableCatalog
	{
		// Token: 0x06001B9B RID: 7067 RVA: 0x0001474E File Offset: 0x0001294E
		private static void RegisterUnlockable(string name, UnlockableDef unlockableDef)
		{
			unlockableDef.name = name;
			unlockableDef.index = new UnlockableIndex(UnlockableCatalog.nameToDefTable.Count);
			UnlockableCatalog.nameToDefTable.Add(name, unlockableDef);
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x00088DB4 File Offset: 0x00086FB4
		public static UnlockableDef GetUnlockableDef(string name)
		{
			UnlockableDef result;
			UnlockableCatalog.nameToDefTable.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x00014778 File Offset: 0x00012978
		public static UnlockableDef GetUnlockableDef(UnlockableIndex index)
		{
			return UnlockableCatalog.indexToDefTable[index.value];
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06001B9E RID: 7070 RVA: 0x00014787 File Offset: 0x00012987
		public static int unlockableCount
		{
			get
			{
				return UnlockableCatalog.indexToDefTable.Length;
			}
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x00088DD0 File Offset: 0x00086FD0
		[SystemInitializer(new Type[]
		{
			typeof(SurvivorCatalog)
		})]
		private static void Init()
		{
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleGuardBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLEGUARD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleQueenBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLEQUEEN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BisonBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BISON"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ClayBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_CLAY"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ClayBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_CLAYBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.GolemBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_GOLEM"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.TitanBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_TITAN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.TitanGoldBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_TITANGOLD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ImpBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_IMP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.JellyfishBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_JELLYFISH"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.VagrantBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_VAGRANT"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.LemurianBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_LEMURIAN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.MagmaWormBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_MAGMAWORM"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.WispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_WISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.GreaterWispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_GREATERWISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.AncientWispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ANCIENTWISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.HermitCrabBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_HERMITCRAB"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BellBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BELL"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.LemurianBruiserBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_LEMURIANBRUISER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ImpBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_IMPBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ShopkeeperBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_SHOPKEEPER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ElectricWormBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ELECTRICWORM"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Huntress", new UnlockableDef
			{
				nameToken = "HUNTRESS_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Bandit", new UnlockableDef
			{
				nameToken = "BANDIT_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Engineer", new UnlockableDef
			{
				nameToken = "ENGI_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Mercenary", new UnlockableDef
			{
				nameToken = "MERC_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Mage", new UnlockableDef
			{
				nameToken = "MAGE_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Toolbot", new UnlockableDef
			{
				nameToken = "TOOLBOT_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ExtraLife", new UnlockableDef
			{
				nameToken = "ITEM_EXTRALIFE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BFG", new UnlockableDef
			{
				nameToken = "EQUIPMENT_BFG_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ShockNearby", new UnlockableDef
			{
				nameToken = "ITEM_SHOCKNEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.AttackSpeedOnCrit", new UnlockableDef
			{
				nameToken = "ITEM_ATTACKSPEEDONCRIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Infusion", new UnlockableDef
			{
				nameToken = "ITEM_INFUSION_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Medkit", new UnlockableDef
			{
				nameToken = "ITEM_MEDKIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Bear", new UnlockableDef
			{
				nameToken = "ITEM_BEAR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Clover", new UnlockableDef
			{
				nameToken = "ITEM_CLOVER_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.GoldGat", new UnlockableDef
			{
				nameToken = "EQUIPMENT_GOLDGAT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.EquipmentMagazine", new UnlockableDef
			{
				nameToken = "ITEM_EQUIPMENTMAGAZINE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.DroneBackup", new UnlockableDef
			{
				nameToken = "EQUIPMENT_DRONEBACKUP_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Firework", new UnlockableDef
			{
				nameToken = "ITEM_FIREWORK_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.WarCryOnMultiKill", new UnlockableDef
			{
				nameToken = "ITEM_WARCRYONMULTIKILL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Lightning", new UnlockableDef
			{
				nameToken = "EQUIPMENT_LIGHTNING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.PassiveHealing", new UnlockableDef
			{
				nameToken = "EQUIPMENT_PASSIVEHEALING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Meteor", new UnlockableDef
			{
				nameToken = "EQUIPMENT_METEOR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BurnNearby", new UnlockableDef
			{
				nameToken = "EQUIPMENT_BURNNEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.AutoCastEquipment", new UnlockableDef
			{
				nameToken = "ITEM_AUTOCASTEQUIPMENT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.HealOnCrit", new UnlockableDef
			{
				nameToken = "EQUIPMENT_HEALONCRIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ElementalRings", new UnlockableDef
			{
				nameToken = "ITEM_ELEMENTALRINGS_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Scanner", new UnlockableDef
			{
				nameToken = "EQUIPMENT_SCANNER_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Talisman", new UnlockableDef
			{
				nameToken = "ITEM_TALISMAN_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BossDamageBonus", new UnlockableDef
			{
				nameToken = "ITEM_BOSSDAMAGEBONUS_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.KillEliteFrenzy", new UnlockableDef
			{
				nameToken = "ITEM_KILLELITEFRENZY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.SecondarySkillMagazine", new UnlockableDef
			{
				nameToken = "ITEM_SECONDARYSKILLMAGAZINE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.JumpBoost", new UnlockableDef
			{
				nameToken = "ITEM_JUMPBOOST_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Crowbar", new UnlockableDef
			{
				nameToken = "ITEM_CROWBAR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Hoof", new UnlockableDef
			{
				nameToken = "ITEM_HOOF_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BounceNearby", new UnlockableDef
			{
				nameToken = "ITEM_BOUNCENEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.TreasureCache", new UnlockableDef
			{
				nameToken = "ITEM_TREASURECACHE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.RepeatHeal", new UnlockableDef
			{
				nameToken = "ITEM_REPEATHEAL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.NovaOnHeal", new UnlockableDef
			{
				nameToken = "ITEM_NOVAONHEAL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.CrippleWardOnLevel", new UnlockableDef
			{
				nameToken = "ITEM_CRIPPLEWARDONLEVEL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.IncreaseHealing", new UnlockableDef
			{
				nameToken = "ITEM_INCREASEHEALING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ShieldOnly", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_BLUEPRINT_SHIELDONLY",
				displayModelPath = "Prefabs/PickupModels/PickupShieldBug"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_1"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_2"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.3", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_3"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.blackbeach", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_BLACKBEACH"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.goolake", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_GOOLAKE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.bazaar", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_BAZAAR"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.frozenwall", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_FROZENWALL"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.golemplains", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_GOLEMPLAINS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.foggyswamp", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_FOGGYSWAMP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.dampcavesimple", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_DAMPCAVE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.mysteryspace", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_MYSTERYSPACE"
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.indexToDefTable = new UnlockableDef[UnlockableCatalog.nameToDefTable.Count];
			foreach (KeyValuePair<string, UnlockableDef> keyValuePair in UnlockableCatalog.nameToDefTable)
			{
				UnlockableCatalog.indexToDefTable[keyValuePair.Value.index.value] = keyValuePair.Value;
			}
			for (int i = 0; i < UnlockableCatalog.indexToDefTable.Length; i++)
			{
				UnlockableCatalog.sortScores[UnlockableCatalog.indexToDefTable[i].name] = UnlockableCatalog.GuessUnlockableSortScore(UnlockableCatalog.indexToDefTable[i].name);
			}
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x000898E4 File Offset: 0x00087AE4
		public static int GetUnlockableSortScore(string unlockableName)
		{
			int result = 0;
			UnlockableCatalog.sortScores.TryGetValue(unlockableName, out result);
			return result;
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x00089904 File Offset: 0x00087B04
		private static int GuessUnlockableSortScore(string unlockableName)
		{
			int num = 0;
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return num;
			}
			num += 200;
			ItemDef itemDef = ItemCatalog.allItems.Select(new Func<ItemIndex, ItemDef>(ItemCatalog.GetItemDef)).FirstOrDefault((ItemDef v) => v.unlockableName == unlockableDef.name);
			if (itemDef != null)
			{
				return (int)(num + itemDef.tier);
			}
			num += 200;
			EquipmentDef equipmentDef = EquipmentCatalog.allEquipment.Select(new Func<EquipmentIndex, EquipmentDef>(EquipmentCatalog.GetEquipmentDef)).FirstOrDefault((EquipmentDef v) => v.unlockableName == unlockableDef.name);
			if (equipmentDef != null)
			{
				if (equipmentDef.isBoss)
				{
					return num + 1;
				}
				if (equipmentDef.isLunar)
				{
					return num - 1;
				}
				return num;
			}
			else
			{
				num += 200;
				if (SurvivorCatalog.allSurvivorDefs.FirstOrDefault((SurvivorDef v) => v.unlockableName == unlockableDef.name) != null)
				{
					return num;
				}
				return num + 200;
			}
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x000899F0 File Offset: 0x00087BF0
		[ConCommand(commandName = "unlockable_grant", flags = ConVarFlags.Cheat, helpText = "Grants the named unlockable, or \"all\" for all unlockables.")]
		private static void CCUnlockableGrant(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			LocalUser localUser = LocalUserManager.FindLocalUser(0);
			if (localUser == null)
			{
				throw new ConCommandException("No local user.");
			}
			string text = args[0];
			UnlockableDef[] array = Array.Empty<UnlockableDef>();
			if (string.CompareOrdinal(text, "all") == 0)
			{
				array = UnlockableCatalog.indexToDefTable;
			}
			else
			{
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(text);
				if (unlockableDef != null)
				{
					array = new UnlockableDef[]
					{
						unlockableDef
					};
				}
			}
			foreach (UnlockableDef unlockableDef2 in array)
			{
				localUser.userProfile.GrantUnlockable(unlockableDef2);
			}
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x00089A80 File Offset: 0x00087C80
		[ConCommand(commandName = "unlockable_revoke", flags = ConVarFlags.Cheat, helpText = "Revokes the named unlockable, or \"all\" for all unlockables.")]
		private static void CCUnlockableRevoke(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			LocalUser localUser = LocalUserManager.FindLocalUser(0);
			if (localUser == null)
			{
				throw new ConCommandException("No local user.");
			}
			string text = args[0];
			UnlockableDef[] array = Array.Empty<UnlockableDef>();
			if (string.CompareOrdinal(text, "all") == 0)
			{
				array = UnlockableCatalog.indexToDefTable;
			}
			else
			{
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(text);
				if (unlockableDef != null)
				{
					array = new UnlockableDef[]
					{
						unlockableDef
					};
				}
			}
			foreach (UnlockableDef unlockableDef2 in array)
			{
				localUser.userProfile.RevokeUnlockable(unlockableDef2);
			}
		}

		// Token: 0x04001E13 RID: 7699
		private static readonly Dictionary<string, UnlockableDef> nameToDefTable = new Dictionary<string, UnlockableDef>();

		// Token: 0x04001E14 RID: 7700
		private static UnlockableDef[] indexToDefTable;

		// Token: 0x04001E15 RID: 7701
		public static ResourceAvailability availability;

		// Token: 0x04001E16 RID: 7702
		private static readonly Dictionary<string, int> sortScores = new Dictionary<string, int>();
	}
}
