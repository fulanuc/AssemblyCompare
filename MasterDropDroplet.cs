using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000358 RID: 856
	[RequireComponent(typeof(CharacterMaster))]
	public class MasterDropDroplet : MonoBehaviour
	{
		// Token: 0x060011B7 RID: 4535 RVA: 0x0000D7BB File Offset: 0x0000B9BB
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00066D8C File Offset: 0x00064F8C
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

		// Token: 0x040015B2 RID: 5554
		private CharacterMaster characterMaster;

		// Token: 0x040015B3 RID: 5555
		public SerializablePickupIndex[] pickupsToDrop;
	}
}
