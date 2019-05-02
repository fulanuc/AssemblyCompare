using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002FB RID: 763
	public class GameObjectUnlockableFilter : NetworkBehaviour
	{
		// Token: 0x06000F72 RID: 3954 RVA: 0x0000BD72 File Offset: 0x00009F72
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.Networkactive = this.GameObjectIsValid();
			}
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0000BD87 File Offset: 0x00009F87
		private void FixedUpdate()
		{
			base.gameObject.SetActive(this.active);
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0005D1D0 File Offset: 0x0005B3D0
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

		// Token: 0x06000F76 RID: 3958 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0005D234 File Offset: 0x0005B434
		// (set) Token: 0x06000F78 RID: 3960 RVA: 0x0000BD9A File Offset: 0x00009F9A
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

		// Token: 0x06000F79 RID: 3961 RVA: 0x0005D248 File Offset: 0x0005B448
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

		// Token: 0x06000F7A RID: 3962 RVA: 0x0005D2B4 File Offset: 0x0005B4B4
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

		// Token: 0x04001397 RID: 5015
		public string requiredUnlockable;

		// Token: 0x04001398 RID: 5016
		public string forbiddenUnlockable;

		// Token: 0x04001399 RID: 5017
		[SyncVar]
		private bool active;
	}
}
