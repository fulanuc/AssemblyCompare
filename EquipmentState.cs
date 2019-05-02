using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033F RID: 831
	public struct EquipmentState : IEquatable<EquipmentState>
	{
		// Token: 0x06001141 RID: 4417 RVA: 0x0000D2C6 File Offset: 0x0000B4C6
		public EquipmentState(EquipmentIndex equipmentIndex, Run.FixedTimeStamp chargeFinishTime, byte charges)
		{
			this.equipmentIndex = equipmentIndex;
			this.chargeFinishTime = chargeFinishTime;
			this.charges = charges;
			this.dirty = true;
			this.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x00065674 File Offset: 0x00063874
		public bool Equals(EquipmentState other)
		{
			return this.equipmentIndex == other.equipmentIndex && this.chargeFinishTime.Equals(other.chargeFinishTime) && this.charges == other.charges;
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0000D2F0 File Offset: 0x0000B4F0
		public override bool Equals(object obj)
		{
			return obj != null && obj is EquipmentState && this.Equals((EquipmentState)obj);
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x000656B8 File Offset: 0x000638B8
		public override int GetHashCode()
		{
			return (int)(this.equipmentIndex * (EquipmentIndex)397 ^ (EquipmentIndex)this.chargeFinishTime.GetHashCode());
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x000656E8 File Offset: 0x000638E8
		public static EquipmentState Deserialize(NetworkReader reader)
		{
			EquipmentIndex equipmentIndex = reader.ReadEquipmentIndex();
			Run.FixedTimeStamp fixedTimeStamp = reader.ReadFixedTimeStamp();
			byte b = reader.ReadByte();
			return new EquipmentState(equipmentIndex, fixedTimeStamp, b);
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x0000D30D File Offset: 0x0000B50D
		public static void Serialize(NetworkWriter writer, EquipmentState equipmentState)
		{
			writer.Write(equipmentState.equipmentIndex);
			writer.Write(equipmentState.chargeFinishTime);
			writer.Write(equipmentState.charges);
		}

		// Token: 0x04001542 RID: 5442
		public readonly EquipmentIndex equipmentIndex;

		// Token: 0x04001543 RID: 5443
		public readonly Run.FixedTimeStamp chargeFinishTime;

		// Token: 0x04001544 RID: 5444
		public readonly byte charges;

		// Token: 0x04001545 RID: 5445
		public bool dirty;

		// Token: 0x04001546 RID: 5446
		[CanBeNull]
		public readonly EquipmentDef equipmentDef;

		// Token: 0x04001547 RID: 5447
		public static readonly EquipmentState empty = new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.negativeInfinity, 0);
	}
}
