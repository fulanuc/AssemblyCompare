using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030B RID: 779
	public class DamageDealtMessage : MessageBase
	{
		// Token: 0x06001038 RID: 4152 RVA: 0x00061BA4 File Offset: 0x0005FDA4
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(this.victim);
			writer.Write(this.damage);
			writer.Write(this.attacker);
			writer.Write(this.position);
			writer.Write(this.crit);
			writer.Write(this.damageType);
			writer.Write(this.damageColorIndex);
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00061C10 File Offset: 0x0005FE10
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.victim = reader.ReadGameObject();
			this.damage = reader.ReadSingle();
			this.attacker = reader.ReadGameObject();
			this.position = reader.ReadVector3();
			this.crit = reader.ReadBoolean();
			this.damageType = reader.ReadDamageType();
			this.damageColorIndex = reader.ReadDamageColorIndex();
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600103A RID: 4154 RVA: 0x0000C6A3 File Offset: 0x0000A8A3
		public bool isSilent
		{
			get
			{
				return (this.damageType & DamageType.Silent) > DamageType.Generic;
			}
		}

		// Token: 0x04001416 RID: 5142
		public GameObject victim;

		// Token: 0x04001417 RID: 5143
		public float damage;

		// Token: 0x04001418 RID: 5144
		public GameObject attacker;

		// Token: 0x04001419 RID: 5145
		public Vector3 position;

		// Token: 0x0400141A RID: 5146
		public bool crit;

		// Token: 0x0400141B RID: 5147
		public DamageType damageType;

		// Token: 0x0400141C RID: 5148
		public DamageColorIndex damageColorIndex;
	}
}
