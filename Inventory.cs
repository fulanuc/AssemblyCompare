using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033D RID: 829
	public class Inventory : NetworkBehaviour
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x0600110B RID: 4363 RVA: 0x00064A58 File Offset: 0x00062C58
		// (remove) Token: 0x0600110C RID: 4364 RVA: 0x00064A90 File Offset: 0x00062C90
		public event Action onInventoryChanged;

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x0000D00A File Offset: 0x0000B20A
		public EquipmentIndex currentEquipmentIndex
		{
			get
			{
				return this.currentEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600110E RID: 4366 RVA: 0x0000D017 File Offset: 0x0000B217
		public EquipmentState currentEquipmentState
		{
			get
			{
				return this.GetEquipment((uint)this.activeEquipmentSlot);
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x0600110F RID: 4367 RVA: 0x0000D025 File Offset: 0x0000B225
		public EquipmentIndex alternateEquipmentIndex
		{
			get
			{
				return this.alternateEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x00064AC8 File Offset: 0x00062CC8
		public EquipmentState alternateEquipmentState
		{
			get
			{
				uint num = 0u;
				while ((ulong)num < (ulong)((long)this.GetEquipmentSlotCount()))
				{
					if (num != (uint)this.activeEquipmentSlot)
					{
						return this.GetEquipment(num);
					}
					num += 1u;
				}
				return EquipmentState.empty;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x0000D032 File Offset: 0x0000B232
		// (set) Token: 0x06001112 RID: 4370 RVA: 0x0000D03A File Offset: 0x0000B23A
		public byte activeEquipmentSlot { get; private set; }

		// Token: 0x06001113 RID: 4371 RVA: 0x00064B00 File Offset: 0x00062D00
		private bool SetEquipmentInternal(EquipmentState equipmentState, uint slot)
		{
			if ((long)this.equipmentStateSlots.Length <= (long)((ulong)slot))
			{
				int num = this.equipmentStateSlots.Length;
				Array.Resize<EquipmentState>(ref this.equipmentStateSlots, (int)(slot + 1u));
				for (int i = num; i < this.equipmentStateSlots.Length; i++)
				{
					this.equipmentStateSlots[i] = EquipmentState.empty;
				}
			}
			if (this.equipmentStateSlots[(int)slot].Equals(equipmentState))
			{
				return false;
			}
			this.equipmentStateSlots[(int)slot] = equipmentState;
			return true;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x0000D043 File Offset: 0x0000B243
		[Server]
		public void SetEquipment(EquipmentState equipmentState, uint slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetEquipment(RoR2.EquipmentState,System.UInt32)' called on client");
				return;
			}
			if (this.SetEquipmentInternal(equipmentState, slot))
			{
				if (NetworkServer.active)
				{
					base.SetDirtyBit(16u);
				}
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0000D083 File Offset: 0x0000B283
		public EquipmentState GetEquipment(uint slot)
		{
			if ((ulong)slot >= (ulong)((long)this.equipmentStateSlots.Length))
			{
				return EquipmentState.empty;
			}
			return this.equipmentStateSlots[(int)slot];
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0000D0A4 File Offset: 0x0000B2A4
		[Server]
		public void SetActiveEquipmentSlot(byte slotIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetActiveEquipmentSlot(System.Byte)' called on client");
				return;
			}
			this.activeEquipmentSlot = slotIndex;
			base.SetDirtyBit(16u);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0000D0DA File Offset: 0x0000B2DA
		public int GetEquipmentSlotCount()
		{
			return this.equipmentStateSlots.Length;
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00064B78 File Offset: 0x00062D78
		[Server]
		public void SetEquipmentIndex(EquipmentIndex newEquipmentIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetEquipmentIndex(RoR2.EquipmentIndex)' called on client");
				return;
			}
			if (this.currentEquipmentIndex != newEquipmentIndex)
			{
				EquipmentState equipment = this.GetEquipment(0u);
				byte charges = equipment.charges;
				if (equipment.equipmentIndex == EquipmentIndex.None)
				{
					charges = 1;
				}
				EquipmentState equipmentState = new EquipmentState(newEquipmentIndex, equipment.chargeFinishTime, charges);
				this.SetEquipment(equipmentState, (uint)this.activeEquipmentSlot);
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0000D0E4 File Offset: 0x0000B2E4
		public EquipmentIndex GetEquipmentIndex()
		{
			return this.currentEquipmentIndex;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00064BDC File Offset: 0x00062DDC
		[Server]
		public void DeductActiveEquipmentCharges(int deduction)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::DeductActiveEquipmentCharges(System.Int32)' called on client");
				return;
			}
			EquipmentState equipment = this.GetEquipment((uint)this.activeEquipmentSlot);
			byte b = equipment.charges;
			if ((int)b < deduction)
			{
				b = 0;
			}
			else
			{
				b -= (byte)deduction;
			}
			this.SetEquipment(new EquipmentState(equipment.equipmentIndex, equipment.chargeFinishTime, b), (uint)this.activeEquipmentSlot);
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00064C40 File Offset: 0x00062E40
		[Server]
		public void DeductActiveEquipmentCooldown(float seconds)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::DeductActiveEquipmentCooldown(System.Single)' called on client");
				return;
			}
			EquipmentState equipment = this.GetEquipment((uint)this.activeEquipmentSlot);
			this.SetEquipment(new EquipmentState(equipment.equipmentIndex, equipment.chargeFinishTime - seconds, equipment.charges), (uint)this.activeEquipmentSlot);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0000D0EC File Offset: 0x0000B2EC
		public int GetActiveEquipmentMaxCharges()
		{
			return 1 + this.GetItemCount(ItemIndex.EquipmentMagazine);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00064C98 File Offset: 0x00062E98
		[Server]
		private void UpdateEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::UpdateEquipment()' called on client");
				return;
			}
			Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
			int itemCount = this.GetItemCount(ItemIndex.EquipmentMagazine);
			byte b = (byte)Mathf.Min(1 + this.GetItemCount(ItemIndex.EquipmentMagazine), 255);
			uint num = 0u;
			while ((ulong)num < (ulong)((long)this.equipmentStateSlots.Length))
			{
				EquipmentState equipmentState = this.equipmentStateSlots[(int)num];
				if (equipmentState.equipmentIndex != EquipmentIndex.None)
				{
					if (equipmentState.charges < b)
					{
						Run.FixedTimeStamp a = equipmentState.chargeFinishTime;
						byte b2 = equipmentState.charges;
						if (a.isPositiveInfinity)
						{
							a = now;
						}
						else
						{
							b2 += 1;
						}
						if (a <= now)
						{
							float num2 = Mathf.Pow(0.85f, (float)itemCount);
							num2 *= Mathf.Pow(0.5f, (float)this.GetItemCount(ItemIndex.AutoCastEquipment));
							float b3 = equipmentState.equipmentDef.cooldown * num2;
							this.SetEquipment(new EquipmentState(equipmentState.equipmentIndex, a + b3, b2), num);
						}
					}
					else if (equipmentState.chargeFinishTime != Run.FixedTimeStamp.positiveInfinity)
					{
						this.SetEquipment(new EquipmentState(equipmentState.equipmentIndex, Run.FixedTimeStamp.positiveInfinity, b), num);
					}
				}
				num += 1u;
			}
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x0000D0F8 File Offset: 0x0000B2F8
		private void Start()
		{
			if (NetworkServer.active && Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Enigma))
			{
				this.SetEquipmentIndex(EquipmentIndex.Enigma);
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x0000D11B File Offset: 0x0000B31B
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateEquipment();
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001120 RID: 4384 RVA: 0x0000D12A File Offset: 0x0000B32A
		// (set) Token: 0x06001121 RID: 4385 RVA: 0x0000D132 File Offset: 0x0000B332
		public uint infusionBonus { get; private set; }

		// Token: 0x06001122 RID: 4386 RVA: 0x0000D13B File Offset: 0x0000B33B
		[Server]
		public void AddInfusionBonus(uint value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::AddInfusionBonus(System.UInt32)' called on client");
				return;
			}
			if (value != 0u)
			{
				this.infusionBonus += value;
				base.SetDirtyBit(4u);
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x0000D17A File Offset: 0x0000B37A
		[Server]
		public void GiveItemString(string itemString)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveItemString(System.String)' called on client");
				return;
			}
			this.GiveItem((ItemIndex)Enum.Parse(typeof(ItemIndex), itemString), 1);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0000D1AD File Offset: 0x0000B3AD
		[Server]
		public void GiveEquipmentString(string equipmentString)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveEquipmentString(System.String)' called on client");
				return;
			}
			this.SetEquipmentIndex((EquipmentIndex)Enum.Parse(typeof(EquipmentIndex), equipmentString));
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00064DDC File Offset: 0x00062FDC
		[Server]
		public void GiveItem(ItemIndex itemIndex, int count = 1)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveItem(RoR2.ItemIndex,System.Int32)' called on client");
				return;
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					this.RemoveItem(itemIndex, -count);
				}
				return;
			}
			base.SetDirtyBit(1u);
			if ((this.itemStacks[(int)itemIndex] += count) == count)
			{
				this.itemAcquisitionOrder.Add(itemIndex);
				base.SetDirtyBit(8u);
			}
			Action action = this.onInventoryChanged;
			if (action != null)
			{
				action();
			}
			Action<Inventory, ItemIndex, int> action2 = Inventory.onServerItemGiven;
			if (action2 != null)
			{
				action2(this, itemIndex, count);
			}
			this.CallRpcItemAdded(itemIndex);
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06001126 RID: 4390 RVA: 0x00064E6C File Offset: 0x0006306C
		// (remove) Token: 0x06001127 RID: 4391 RVA: 0x00064EA0 File Offset: 0x000630A0
		public static event Action<Inventory, ItemIndex, int> onServerItemGiven;

		// Token: 0x06001128 RID: 4392 RVA: 0x0000D1DF File Offset: 0x0000B3DF
		private IEnumerator HighlightNewItem(ItemIndex itemIndex)
		{
			yield return new WaitForSeconds(0.05f);
			CharacterMaster component = base.GetComponent<CharacterMaster>();
			if (component)
			{
				GameObject bodyObject = component.GetBodyObject();
				if (bodyObject)
				{
					ModelLocator component2 = bodyObject.GetComponent<ModelLocator>();
					if (component2)
					{
						Transform modelTransform = component2.modelTransform;
						if (modelTransform)
						{
							CharacterModel component3 = modelTransform.GetComponent<CharacterModel>();
							if (component3)
							{
								component3.HighlightItemDisplay(itemIndex);
							}
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x0000D1F5 File Offset: 0x0000B3F5
		[ClientRpc]
		private void RpcItemAdded(ItemIndex itemIndex)
		{
			base.StartCoroutine(this.HighlightNewItem(itemIndex));
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00064ED4 File Offset: 0x000630D4
		[Server]
		public void RemoveItem(ItemIndex itemIndex, int count = 0)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::RemoveItem(RoR2.ItemIndex,System.Int32)' called on client");
				return;
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					this.GiveItem(itemIndex, -count);
				}
				return;
			}
			int num = this.itemStacks[(int)itemIndex];
			count = Math.Min(count, num);
			if (count == 0)
			{
				return;
			}
			if ((this.itemStacks[(int)itemIndex] = num - count) == 0)
			{
				this.itemAcquisitionOrder.Remove(itemIndex);
				base.SetDirtyBit(8u);
			}
			base.SetDirtyBit(1u);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00064F5C File Offset: 0x0006315C
		[Server]
		public void ResetItem(ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ResetItem(RoR2.ItemIndex)' called on client");
				return;
			}
			if (this.itemStacks[(int)itemIndex] <= 0)
			{
				return;
			}
			this.itemStacks[(int)itemIndex] = 0;
			base.SetDirtyBit(1u);
			base.SetDirtyBit(8u);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00064FB4 File Offset: 0x000631B4
		[Server]
		public void CopyEquipmentFrom(Inventory other)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::CopyEquipmentFrom(RoR2.Inventory)' called on client");
				return;
			}
			for (int i = 0; i < other.equipmentStateSlots.Length; i++)
			{
				this.SetEquipment(new EquipmentState(other.equipmentStateSlots[i].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 1), (uint)i);
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x0006500C File Offset: 0x0006320C
		[Server]
		public void CopyItemsFrom(Inventory other)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::CopyItemsFrom(RoR2.Inventory)' called on client");
				return;
			}
			other.itemStacks.CopyTo(this.itemStacks, 0);
			this.itemAcquisitionOrder.Clear();
			this.itemAcquisitionOrder.AddRange(other.itemAcquisitionOrder);
			base.SetDirtyBit(1u);
			base.SetDirtyBit(8u);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0006507C File Offset: 0x0006327C
		[Server]
		public void ShrineRestackInventory([NotNull] Xoroshiro128Plus rng)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ShrineRestackInventory(Xoroshiro128Plus)' called on client");
				return;
			}
			List<ItemIndex> list = new List<ItemIndex>();
			List<ItemIndex> list2 = new List<ItemIndex>();
			List<ItemIndex> list3 = new List<ItemIndex>();
			List<ItemIndex> list4 = new List<ItemIndex>();
			List<ItemIndex> list5 = new List<ItemIndex>();
			new List<ItemIndex>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < this.itemStacks.Length; i++)
			{
				ItemIndex itemIndex = (ItemIndex)i;
				if (this.itemStacks[i] > 0)
				{
					switch (ItemCatalog.GetItemDef(itemIndex).tier)
					{
					case ItemTier.Tier1:
						num += this.itemStacks[i];
						list.Add(itemIndex);
						break;
					case ItemTier.Tier2:
						num2 += this.itemStacks[i];
						list2.Add(itemIndex);
						break;
					case ItemTier.Tier3:
						num3 += this.itemStacks[i];
						list3.Add(itemIndex);
						break;
					case ItemTier.Lunar:
						num4 += this.itemStacks[i];
						list4.Add(itemIndex);
						break;
					case ItemTier.Boss:
						num5 += this.itemStacks[i];
						list5.Add(itemIndex);
						break;
					}
				}
				this.ResetItem(itemIndex);
			}
			ItemIndex itemIndex2 = (list.Count == 0) ? ItemIndex.None : list[rng.RangeInt(0, list.Count)];
			ItemIndex itemIndex3 = (list2.Count == 0) ? ItemIndex.None : list2[rng.RangeInt(0, list2.Count)];
			ItemIndex itemIndex4 = (list3.Count == 0) ? ItemIndex.None : list3[rng.RangeInt(0, list3.Count)];
			ItemIndex itemIndex5 = (list4.Count == 0) ? ItemIndex.None : list4[rng.RangeInt(0, list4.Count)];
			ItemIndex itemIndex6 = (list5.Count == 0) ? ItemIndex.None : list5[rng.RangeInt(0, list5.Count)];
			this.itemAcquisitionOrder.Clear();
			base.SetDirtyBit(8u);
			this.GiveItem(itemIndex2, num);
			this.GiveItem(itemIndex3, num2);
			this.GiveItem(itemIndex4, num3);
			this.GiveItem(itemIndex5, num4);
			this.GiveItem(itemIndex6, num5);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x0000D205 File Offset: 0x0000B405
		public int GetItemCount(ItemIndex itemIndex)
		{
			return this.itemStacks[(int)itemIndex];
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000652AC File Offset: 0x000634AC
		public bool HasAtLeastXTotalItemsOfTier(ItemTier itemTier, int x)
		{
			int num = 0;
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == itemTier)
				{
					num += this.GetItemCount(itemIndex);
					if (num >= x)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x000652E8 File Offset: 0x000634E8
		public int GetTotalItemCountOfTier(ItemTier itemTier)
		{
			int num = 0;
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == itemTier)
				{
					num += this.GetItemCount(itemIndex);
				}
			}
			return num;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x0000D20F File Offset: 0x0000B40F
		public void WriteItemStacks(int[] output)
		{
			Array.Copy(this.itemStacks, output, output.Length);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00065320 File Offset: 0x00063520
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 4) > 0;
			bool flag3 = (b & 8) > 0;
			bool flag4 = (b & 16) > 0;
			if (flag)
			{
				reader.ReadItemStacks(this.itemStacks);
			}
			if (flag2)
			{
				this.infusionBonus = reader.ReadPackedUInt32();
			}
			if (flag3)
			{
				byte b2 = reader.ReadByte();
				this.itemAcquisitionOrder.Clear();
				this.itemAcquisitionOrder.Capacity = (int)b2;
				for (byte b3 = 0; b3 < b2; b3 += 1)
				{
					ItemIndex item = (ItemIndex)reader.ReadByte();
					this.itemAcquisitionOrder.Add(item);
				}
			}
			if (flag4)
			{
				uint num = (uint)reader.ReadByte();
				for (uint num2 = 0u; num2 < num; num2 += 1u)
				{
					this.SetEquipmentInternal(EquipmentState.Deserialize(reader), num2);
				}
				this.activeEquipmentSlot = reader.ReadByte();
			}
			if (flag || flag4 || flag2)
			{
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00065408 File Offset: 0x00063608
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 17u;
			}
			for (int i = 0; i < this.equipmentStateSlots.Length; i++)
			{
				if (this.equipmentStateSlots[i].dirty)
				{
					num |= 16u;
					break;
				}
			}
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 4u) > 0u;
			bool flag3 = (num & 8u) > 0u;
			bool flag4 = (num & 16u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.WriteItemStacks(this.itemStacks);
			}
			if (flag2)
			{
				writer.WritePackedUInt32(this.infusionBonus);
			}
			if (flag3)
			{
				byte b = (byte)this.itemAcquisitionOrder.Count;
				writer.Write(b);
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					writer.Write((byte)this.itemAcquisitionOrder[(int)b2]);
				}
			}
			if (flag4)
			{
				writer.Write((byte)this.equipmentStateSlots.Length);
				for (int j = 0; j < this.equipmentStateSlots.Length; j++)
				{
					EquipmentState.Serialize(writer, this.equipmentStateSlots[j]);
				}
				writer.Write(this.activeEquipmentSlot);
			}
			if (!initialState)
			{
				for (int k = 0; k < this.equipmentStateSlots.Length; k++)
				{
					this.equipmentStateSlots[k].dirty = false;
				}
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x0000D24B File Offset: 0x0000B44B
		protected static void InvokeRpcRpcItemAdded(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcItemAdded called on server.");
				return;
			}
			((Inventory)obj).RpcItemAdded((ItemIndex)reader.ReadInt32());
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00065554 File Offset: 0x00063754
		public void CallRpcItemAdded(ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcItemAdded called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)Inventory.kRpcRpcItemAdded);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write((int)itemIndex);
			this.SendRPCInternal(networkWriter, 0, "RpcItemAdded");
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0000D274 File Offset: 0x0000B474
		static Inventory()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(Inventory), Inventory.kRpcRpcItemAdded, new NetworkBehaviour.CmdDelegate(Inventory.InvokeRpcRpcItemAdded));
			NetworkCRC.RegisterBehaviour("Inventory", 0);
		}

		// Token: 0x04001532 RID: 5426
		private readonly int[] itemStacks = new int[78];

		// Token: 0x04001533 RID: 5427
		public readonly List<ItemIndex> itemAcquisitionOrder = new List<ItemIndex>();

		// Token: 0x04001534 RID: 5428
		private const uint itemListDirtyBit = 1u;

		// Token: 0x04001535 RID: 5429
		private const uint infusionBonusDirtyBit = 4u;

		// Token: 0x04001536 RID: 5430
		private const uint itemAcquisitionOrderDirtyBit = 8u;

		// Token: 0x04001537 RID: 5431
		private const uint equipmentDirtyBit = 16u;

		// Token: 0x0400153A RID: 5434
		private EquipmentState[] equipmentStateSlots = Array.Empty<EquipmentState>();

		// Token: 0x0400153D RID: 5437
		private static int kRpcRpcItemAdded = 1978705787;
	}
}
