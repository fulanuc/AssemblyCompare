using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000326 RID: 806
	[Serializable]
	public struct HurtBoxReference
	{
		// Token: 0x0600109F RID: 4255 RVA: 0x00062F00 File Offset: 0x00061100
		public static HurtBoxReference FromHurtBox(HurtBox hurtBox)
		{
			HurtBoxReference result;
			if (!hurtBox)
			{
				result = default(HurtBoxReference);
				return result;
			}
			result = new HurtBoxReference
			{
				rootObject = (hurtBox.healthComponent ? hurtBox.healthComponent.gameObject : null),
				hurtBoxIndexPlusOne = (byte)(hurtBox.indexInGroup + 1)
			};
			return result;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x00062F5C File Offset: 0x0006115C
		public static HurtBoxReference FromRootObject(GameObject rootObject)
		{
			return new HurtBoxReference
			{
				rootObject = rootObject,
				hurtBoxIndexPlusOne = 0
			};
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x00062F84 File Offset: 0x00061184
		public GameObject ResolveGameObject()
		{
			if (this.hurtBoxIndexPlusOne == 0)
			{
				return this.rootObject;
			}
			GameObject gameObject = this.rootObject;
			HurtBox[] array;
			if (gameObject == null)
			{
				array = null;
			}
			else
			{
				ModelLocator component = gameObject.GetComponent<ModelLocator>();
				if (component == null)
				{
					array = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform == null)
					{
						array = null;
					}
					else
					{
						HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
						array = ((component2 != null) ? component2.hurtBoxes : null);
					}
				}
			}
			HurtBox[] array2 = array;
			if (array2 != null)
			{
				int num = (int)(this.hurtBoxIndexPlusOne - 1);
				if (num < array2.Length)
				{
					return array2[num].gameObject;
				}
			}
			return null;
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00062FF4 File Offset: 0x000611F4
		public HurtBox ResolveHurtBox()
		{
			GameObject gameObject = this.ResolveGameObject();
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponent<HurtBox>();
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x0000CBD8 File Offset: 0x0000ADD8
		public void Write(NetworkWriter writer)
		{
			writer.Write(this.rootObject);
			writer.Write(this.hurtBoxIndexPlusOne);
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x0000CBF2 File Offset: 0x0000ADF2
		public void Read(NetworkReader reader)
		{
			this.rootObject = reader.ReadGameObject();
			this.hurtBoxIndexPlusOne = reader.ReadByte();
		}

		// Token: 0x040014AF RID: 5295
		public GameObject rootObject;

		// Token: 0x040014B0 RID: 5296
		public byte hurtBoxIndexPlusOne;
	}
}
