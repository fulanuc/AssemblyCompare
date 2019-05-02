using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200023B RID: 571
	public static class DevCommands
	{
		// Token: 0x06000ACD RID: 2765 RVA: 0x00008B41 File Offset: 0x00006D41
		private static void AddTokenIfDefault(List<string> lines, string token)
		{
			if (!string.IsNullOrEmpty(token) && Language.GetString(token) == token)
			{
				lines.Add(string.Format("\t\t\"{0}\": \"{0}\",", token));
			}
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x000493E4 File Offset: 0x000475E4
		[ConCommand(commandName = "language_generate_tokens", flags = ConVarFlags.None, helpText = "Generates default token definitions to be inserted into a JSON language file.")]
		private static void CCLanguageGenerateTokens(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			foreach (ItemDef itemDef in ItemCatalog.allItems.Select(new Func<ItemIndex, ItemDef>(ItemCatalog.GetItemDef)))
			{
				DevCommands.AddTokenIfDefault(list, itemDef.nameToken);
				DevCommands.AddTokenIfDefault(list, itemDef.pickupToken);
				DevCommands.AddTokenIfDefault(list, itemDef.descriptionToken);
			}
			list.Add("\r\n");
			foreach (EquipmentDef equipmentDef in EquipmentCatalog.allEquipment.Select(new Func<EquipmentIndex, EquipmentDef>(EquipmentCatalog.GetEquipmentDef)))
			{
				DevCommands.AddTokenIfDefault(list, equipmentDef.nameToken);
				DevCommands.AddTokenIfDefault(list, equipmentDef.pickupToken);
				DevCommands.AddTokenIfDefault(list, equipmentDef.descriptionToken);
			}
			Debug.Log(string.Join("\r\n", list));
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x000494F8 File Offset: 0x000476F8
		[ConCommand(commandName = "ignore_collision_with_this", flags = ConVarFlags.Cheat, helpText = "Disables collisions with the object you're looking at.")]
		private static void CCIgnoreCollisionWithThis(ConCommandArgs args)
		{
			if (args.senderMasterObject)
			{
				CharacterMaster component = args.senderMasterObject.GetComponent<CharacterMaster>();
				if (component)
				{
					CharacterBody body = component.GetBody();
					if (body)
					{
						Collider component2 = body.GetComponent<Collider>();
						if (component2)
						{
							InputBankTest component3 = body.GetComponent<InputBankTest>();
							RaycastHit raycastHit;
							if (Util.CharacterRaycast(body.gameObject, new Ray(component3.aimOrigin, component3.aimDirection), out raycastHit, 2000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
							{
								Physics.IgnoreCollision(component2, raycastHit.collider, args.TryGetArgBool(0) ?? true);
							}
						}
					}
				}
			}
		}
	}
}
