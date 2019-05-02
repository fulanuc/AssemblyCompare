using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000289 RID: 649
	public class CharacterDeathBehavior : MonoBehaviour
	{
		// Token: 0x06000CBE RID: 3262 RVA: 0x00051FC0 File Offset: 0x000501C0
		private void OnDeath()
		{
			if (Util.HasEffectiveAuthority(base.gameObject))
			{
				if (this.deathStateMachine)
				{
					this.deathStateMachine.SetNextState(EntityState.Instantiate(this.deathState));
				}
				EntityStateMachine[] array = this.idleStateMachine;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetNextState(new Idle());
				}
			}
			base.gameObject.layer = LayerIndex.debris.intVal;
			CharacterMotor component = base.GetComponent<CharacterMotor>();
			if (component)
			{
				component.Motor.RebuildCollidableLayers();
			}
			ILifeBehavior[] components = base.GetComponents<ILifeBehavior>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnDeathStart();
			}
			ModelLocator component2 = base.GetComponent<ModelLocator>();
			if (component2)
			{
				Transform modelTransform = component2.modelTransform;
				if (modelTransform)
				{
					components = modelTransform.GetComponents<ILifeBehavior>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].OnDeathStart();
					}
				}
			}
		}

		// Token: 0x040010D9 RID: 4313
		[Tooltip("The state machine to set the state of when this character is killed.")]
		public EntityStateMachine deathStateMachine;

		// Token: 0x040010DA RID: 4314
		[Tooltip("The state to enter when this character is killed.")]
		public SerializableEntityStateType deathState;

		// Token: 0x040010DB RID: 4315
		[Tooltip("The state machine(s) to set to idle when this character is killed.")]
		public EntityStateMachine[] idleStateMachine;
	}
}
