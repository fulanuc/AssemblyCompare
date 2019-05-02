using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200051E RID: 1310
	public class GateStateSetter : MonoBehaviour
	{
		// Token: 0x06001D88 RID: 7560 RVA: 0x00015AA4 File Offset: 0x00013CA4
		private void OnEnable()
		{
			this.UpdateGates(true);
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x00015AAD File Offset: 0x00013CAD
		private void OnDisable()
		{
			this.UpdateGates(false);
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x00090B18 File Offset: 0x0008ED18
		private void UpdateGates(bool enabledState)
		{
			if (!SceneInfo.instance)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.gateToEnableWhenEnabled))
			{
				SceneInfo.instance.SetGateState(this.gateToEnableWhenEnabled, enabledState);
			}
			if (!string.IsNullOrEmpty(this.gateToDisableWhenEnabled))
			{
				SceneInfo.instance.SetGateState(this.gateToDisableWhenEnabled, !enabledState);
			}
		}

		// Token: 0x04001FC1 RID: 8129
		public string gateToEnableWhenEnabled;

		// Token: 0x04001FC2 RID: 8130
		public string gateToDisableWhenEnabled;
	}
}
