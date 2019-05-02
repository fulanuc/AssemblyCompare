using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000355 RID: 853
	[RequireComponent(typeof(CharacterMaster))]
	public class MasterDropDroplet : MonoBehaviour
	{
		// Token: 0x060011A0 RID: 4512 RVA: 0x0000D6D2 File Offset: 0x0000B8D2
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00066A54 File Offset: 0x00064C54
		public void DropItems()
		{
			CharacterBody body = this.characterMaster.GetBody();
			if (body)
			{
				SerializablePickupIndex[] array = this.pickupsToDrop;
				for (int i = 0; i < array.Length; i++)
				{
					PickupDropletController.CreatePickupDroplet(PickupIndex.Find(array[i].pickupName), body.coreTransform.position, new Vector3(UnityEngine.Random.Range(-4f, 4f), 20f, UnityEngine.Random.Range(-4f, 4f)));
				}
			}
		}

		// Token: 0x04001599 RID: 5529
		private CharacterMaster characterMaster;

		// Token: 0x0400159A RID: 5530
		public SerializablePickupIndex[] pickupsToDrop;
	}
}
