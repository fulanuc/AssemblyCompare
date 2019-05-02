using System;
using UnityEngine;

namespace RoR2.Navigation
{
	// Token: 0x0200052D RID: 1325
	public class GateStateSetter : MonoBehaviour
	{
		// Token: 0x06001DF0 RID: 7664 RVA: 0x00015F6D File Offset: 0x0001416D
		private void OnEnable()
		{
			this.UpdateGates(true);
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x00015F76 File Offset: 0x00014176
		private void OnDisable()
		{
			this.UpdateGates(false);
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x00091894 File Offset: 0x0008FA94
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

		// Token: 0x04001FFF RID: 8191
		public string gateToEnableWhenEnabled;

		// Token: 0x04002000 RID: 8192
		public string gateToDisableWhenEnabled;
	}
}
