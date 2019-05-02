using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033F RID: 831
	public class Inventory : NetworkBehaviour
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x0600111F RID: 4383 RVA: 0x00064C98 File Offset: 0x00062E98
		// (remove) Token: 0x06001120 RID: 4384 RVA: 0x00064CD0 File Offset: 0x00062ED0
		public event Action onInventoryChanged;

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x0000D0F3 File Offset: 0x0000B2F3
		public EquipmentIndex currentEquipmentIndex
		{
			get
			{
				return this.currentEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001122 RID: 4386 RVA: 0x0000D100 File Offset: 0x0000B300
		public EquipmentState currentEquipmentState
		{
			get
			{
				return this.GetEquipment((uint)this.activeEquipmentSlot);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06001123 RID: 4387 RVA: 0x0000D10E File Offset: 0x0000B30E
		public EquipmentIndex alternateEquipmentIndex
		{
			get
			{
				return this.alternateEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001124 RID: 4388 RVA: 0x00064D08 File Offset: 0x00062F08
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

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001125 RID: 4389 RVA: 0x0000D11B File Offset: 0x0000B31B
		// (set) Token: 0x06001126 RID: 4390 RVA: 0x0000D123 File Offset: 0x0000B323
		public byte activeEquipmentSlot { get; private set; }

		// Token: 0x06001127 RID: 4391 RVA: 0x00064D40 File Offset: 0x00062F40
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

		// Token: 0x06001128 RID: 4392 RVA: 0x0000D12C File Offset: 0x0000B32C
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

		// Token: 0x06001129 RID: 4393 RVA: 0x0000D16C File Offset: 0x0000B36C
		public EquipmentState GetEquipment(uint slot)
		{
			if ((ulong)slot >= (ulong)((long)this.equipmentStateSlots.Length))
			{
				return EquipmentState.empty;
			}
			return this.equipmentStateSlots[(int)slot];
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x0000D18D File Offset: 0x0000B38D
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

		// Token: 0x0600112B RID: 4395 RVA: 0x0000D1C3 File Offset: 0x0000B3C3
		public int GetEquipmentSlotCount()
		{
			return this.equipmentStateSlots.Length;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00064DB8 File Offset: 0x00062FB8
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

		// Token: 0x0600112D RID: 4397 RVA: 0x0000D1CD File Offset: 0x0000B3CD
		public EquipmentIndex GetEquipmentIndex()
		{
			return this.currentEquipmentIndex;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x00064E1C File Offset: 0x0006301C
		[Server]
		public void DeductEquipmentCharges(byte slot, int deduction)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::DeductEquipmentCharges(System.Byte,System.Int32)' called on client");
				return;
			}
			EquipmentState equipment = this.GetEquipment((uint)slot);
			byte b = equipment.charges;
			if ((int)b < deduction)
			{
				b = 0;
			}
			else
			{
				b -= (byte)deduction;
			}
			this.SetEquipment(new EquipmentState(equipment.equipmentIndex, equipment.chargeFinishTime, b), (uint)slot);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00064E74 File Offset: 0x00063074
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

		// Token: 0x06001130 RID: 4400 RVA: 0x0000D1D5 File Offset: 0x0000B3D5
		public int GetActiveEquipmentMaxCharges()
		{
			return 1 + this.GetItemCount(ItemIndex.EquipmentMagazine);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00064ECC File Offset: 0x000630CC
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

		// Token: 0x06001132 RID: 4402 RVA: 0x0000D1E1 File Offset: 0x0000B3E1
		private void Start()
		{
			if (NetworkServer.active && Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Enigma))
			{
				this.SetEquipmentIndex(EquipmentIndex.Enigma);
			}
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0000D204 File Offset: 0x0000B404
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateEquipment();
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x0000D213 File Offset: 0x0000B413
		// (set) Token: 0x06001135 RID: 4405 RVA: 0x0000D21B File Offset: 0x0000B41B
		public uint infusionBonus { get; private set; }

		// Token: 0x06001136 RID: 4406 RVA: 0x0000D224 File Offset: 0x0000B424
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

		// Token: 0x06001137 RID: 4407 RVA: 0x0000D263 File Offset: 0x0000B463
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

		// Token: 0x06001138 RID: 4408 RVA: 0x0000D296 File Offset: 0x0000B496
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

		// Token: 0x06001139 RID: 4409 RVA: 0x00065010 File Offset: 0x00063210
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
		// (add) Token: 0x0600113A RID: 4410 RVA: 0x000650A0 File Offset: 0x000632A0
		// (remove) Token: 0x0600113B RID: 4411 RVA: 0x000650D4 File Offset: 0x000632D4
		public static event Action<Inventory, ItemIndex, int> onServerItemGiven;

		// Token: 0x0600113C RID: 4412 RVA: 0x0000D2C8 File Offset: 0x0000B4C8
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

		// Token: 0x0600113D RID: 4413 RVA: 0x0000D2DE File Offset: 0x0000B4DE
		[ClientRpc]
		private void RpcItemAdded(ItemIndex itemIndex)
		{
			base.StartCoroutine(this.HighlightNewItem(itemIndex));
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00065108 File Offset: 0x00063308
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

		// Token: 0x0600113F RID: 4415 RVA: 0x00065190 File Offset: 0x00063390
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

		// Token: 0x06001140 RID: 4416 RVA: 0x000651E8 File Offset: 0x000633E8
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

		// Token: 0x06001141 RID: 4417 RVA: 0x00065240 File Offset: 0x00063440
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

		// Token: 0x06001142 RID: 4418 RVA: 0x000652B0 File Offset: 0x000634B0
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

		// Token: 0x06001143 RID: 4419 RVA: 0x0000D2EE File Offset: 0x0000B4EE
		public int GetItemCount(ItemIndex itemIndex)
		{
			return this.itemStacks[(int)itemIndex];
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x000654E0 File Offset: 0x000636E0
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

		// Token: 0x06001145 RID: 4421 RVA: 0x0006551C File Offset: 0x0006371C
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

		// Token: 0x06001146 RID: 4422 RVA: 0x0000D2F8 File Offset: 0x0000B4F8
		public void WriteItemStacks(int[] output)
		{
			Array.Copy(this.itemStacks, output, output.Length);
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00065554 File Offset: 0x00063754
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

		// Token: 0x06001149 RID: 4425 RVA: 0x0006563C File Offset: 0x0006383C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 29u;
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

		// Token: 0x0600114B RID: 4427 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0000D334 File Offset: 0x0000B534
		protected static void InvokeRpcRpcItemAdded(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcItemAdded called on server.");
				return;
			}
			((Inventory)obj).RpcItemAdded((ItemIndex)reader.ReadInt32());
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x00065788 File Offset: 0x00063988
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

		// Token: 0x0600114E RID: 4430 RVA: 0x0000D35D File Offset: 0x0000B55D
		static Inventory()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(Inventory), Inventory.kRpcRpcItemAdded, new NetworkBehaviour.CmdDelegate(Inventory.InvokeRpcRpcItemAdded));
			NetworkCRC.RegisterBehaviour("Inventory", 0);
		}

		// Token: 0x04001546 RID: 5446
		private readonly int[] itemStacks = new int[78];

		// Token: 0x04001547 RID: 5447
		public readonly List<ItemIndex> itemAcquisitionOrder = new List<ItemIndex>();

		// Token: 0x04001548 RID: 5448
		private const uint itemListDirtyBit = 1u;

		// Token: 0x04001549 RID: 5449
		private const uint infusionBonusDirtyBit = 4u;

		// Token: 0x0400154A RID: 5450
		private const uint itemAcquisitionOrderDirtyBit = 8u;

		// Token: 0x0400154B RID: 5451
		private const uint equipmentDirtyBit = 16u;

		// Token: 0x0400154C RID: 5452
		private const uint allDirtyBits = 29u;

		// Token: 0x0400154F RID: 5455
		private EquipmentState[] equipmentStateSlots = Array.Empty<EquipmentState>();

		// Token: 0x04001552 RID: 5458
		private static int kRpcRpcItemAdded = 1978705787;
	}
}
