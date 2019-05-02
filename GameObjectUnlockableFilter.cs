using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F8 RID: 760
	public class GameObjectUnlockableFilter : NetworkBehaviour
	{
		// Token: 0x06000F62 RID: 3938 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.Networkactive = this.GameObjectIsValid();
			}
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0000BCD9 File Offset: 0x00009ED9
		private void FixedUpdate()
		{
			base.gameObject.SetActive(this.active);
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0005CFB0 File Offset: 0x0005B1B0
		private bool GameObjectIsValid()
		{
			if (Run.instance)
			{
				bool flag = string.IsNullOrEmpty(this.requiredUnlockable) || Run.instance.IsUnlockableUnlocked(this.requiredUnlockable);
				bool flag2 = !string.IsNullOrEmpty(this.forbiddenUnlockable) && Run.instance.DoesEveryoneHaveThisUnlockableUnlocked(this.forbiddenUnlockable);
				return flag && !flag2;
			}
			return true;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000F67 RID: 3943 RVA: 0x0005D014 File Offset: 0x0005B214
		// (set) Token: 0x06000F68 RID: 3944 RVA: 0x0000BCEC File Offset: 0x00009EEC
		public bool Networkactive
		{
			get
			{
				return this.active;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.active, 1u);
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0005D028 File Offset: 0x0005B228
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.active);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.active);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0005D094 File Offset: 0x0005B294
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.active = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.active = reader.ReadBoolean();
			}
		}

		// Token: 0x04001380 RID: 4992
		public string requiredUnlockable;

		// Token: 0x04001381 RID: 4993
		public string forbiddenUnlockable;

		// Token: 0x04001382 RID: 4994
		[SyncVar]
		private bool active;
	}
}
