using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200037C RID: 892
	[RequireComponent(typeof(Collider))]
	public class OnPlayerEnterEvent : MonoBehaviour
	{
		// Token: 0x060012B0 RID: 4784 RVA: 0x00069CB0 File Offset: 0x00067EB0
		private void OnTriggerEnter(Collider other)
		{
			if ((this.serverOnly && !NetworkServer.active) || this.calledAction)
			{
				return;
			}
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component)
			{
				CharacterMaster master = component.master;
				if (master && master.GetComponent<PlayerCharacterMasterController>())
				{
					this.calledAction = true;
					UnityEvent unityEvent = this.action;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
			}
		}

		// Token: 0x04001647 RID: 5703
		public bool serverOnly;

		// Token: 0x04001648 RID: 5704
		public UnityEvent action;

		// Token: 0x04001649 RID: 5705
		private bool calledAction;
	}
}
