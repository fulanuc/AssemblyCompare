using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000381 RID: 897
	[RequireComponent(typeof(Collider))]
	public class OnPlayerEnterEvent : MonoBehaviour
	{
		// Token: 0x060012D0 RID: 4816 RVA: 0x0006A054 File Offset: 0x00068254
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

		// Token: 0x04001663 RID: 5731
		public bool serverOnly;

		// Token: 0x04001664 RID: 5732
		public UnityEvent action;

		// Token: 0x04001665 RID: 5733
		private bool calledAction;
	}
}
