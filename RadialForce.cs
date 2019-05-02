using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003A0 RID: 928
	public class RadialForce : MonoBehaviour
	{
		// Token: 0x060013B8 RID: 5048 RVA: 0x0000F0A4 File Offset: 0x0000D2A4
		private void Start()
		{
			this._transform = base.GetComponent<Transform>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x0006DEB4 File Offset: 0x0006C0B4
		private void AddToList(GameObject affectedObject)
		{
			if (this.tetherPrefab && !this.affectedObjects.Contains(affectedObject))
			{
				TetherEffect component = UnityEngine.Object.Instantiate<GameObject>(this.tetherPrefab, affectedObject.transform).GetComponent<TetherEffect>();
				component.tetherEndTransform = base.transform;
				component.tetherMaxDistance = this.radius + 1f;
				this.affectedObjects.Add(affectedObject);
			}
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0006DF1C File Offset: 0x0006C11C
		private void FixedUpdate()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, LayerIndex.defaultLayer.mask);
			for (int i = 0; i < array.Length; i++)
			{
				HealthComponent component = array[i].GetComponent<HealthComponent>();
				CharacterMotor component2 = array[i].GetComponent<CharacterMotor>();
				if (component)
				{
					TeamComponent component3 = component.GetComponent<TeamComponent>();
					bool flag = false;
					if (component3 && this.teamFilter)
					{
						flag = (component3.teamIndex == this.teamFilter.teamIndex);
					}
					if (!flag)
					{
						this.AddToList(component.gameObject);
						if (NetworkServer.active)
						{
							Vector3 a = array[i].transform.position - this._transform.position;
							float num = 1f - Mathf.Clamp(a.magnitude / this.radius, 0f, 1f - this.forceCoefficientAtEdge);
							a = a.normalized * this.forceMagnitude * (1f - num);
							Vector3 velocity;
							float mass;
							if (component2)
							{
								velocity = component2.velocity;
								mass = component2.mass;
							}
							else
							{
								Rigidbody component4 = component.GetComponent<Rigidbody>();
								velocity = component4.velocity;
								mass = component4.mass;
							}
							velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
							component.TakeDamageForce(a - velocity * (this.damping * mass * num), true);
						}
					}
				}
			}
		}

		// Token: 0x0400174A RID: 5962
		public GameObject tetherPrefab;

		// Token: 0x0400174B RID: 5963
		public float radius;

		// Token: 0x0400174C RID: 5964
		public float damping = 0.2f;

		// Token: 0x0400174D RID: 5965
		public float forceMagnitude;

		// Token: 0x0400174E RID: 5966
		public float forceCoefficientAtEdge = 0.5f;

		// Token: 0x0400174F RID: 5967
		private Transform _transform;

		// Token: 0x04001750 RID: 5968
		private TeamFilter teamFilter;

		// Token: 0x04001751 RID: 5969
		private List<GameObject> affectedObjects = new List<GameObject>();
	}
}
