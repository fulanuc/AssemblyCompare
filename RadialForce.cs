using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003A5 RID: 933
	public class RadialForce : MonoBehaviour
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0000F248 File Offset: 0x0000D448
		private void Start()
		{
			this._transform = base.GetComponent<Transform>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x0006E0BC File Offset: 0x0006C2BC
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

		// Token: 0x060013D7 RID: 5079 RVA: 0x0006E124 File Offset: 0x0006C324
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

		// Token: 0x04001764 RID: 5988
		public GameObject tetherPrefab;

		// Token: 0x04001765 RID: 5989
		public float radius;

		// Token: 0x04001766 RID: 5990
		public float damping = 0.2f;

		// Token: 0x04001767 RID: 5991
		public float forceMagnitude;

		// Token: 0x04001768 RID: 5992
		public float forceCoefficientAtEdge = 0.5f;

		// Token: 0x04001769 RID: 5993
		private Transform _transform;

		// Token: 0x0400176A RID: 5994
		private TeamFilter teamFilter;

		// Token: 0x0400176B RID: 5995
		private List<GameObject> affectedObjects = new List<GameObject>();
	}
}
