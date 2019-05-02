using System;
using EntityStates;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002E7 RID: 743
	public class EntityStateMachine : MonoBehaviour
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0000B7CE File Offset: 0x000099CE
		// (set) Token: 0x06000EE3 RID: 3811 RVA: 0x0000B7D6 File Offset: 0x000099D6
		public EntityState state { get; private set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x0000B7DF File Offset: 0x000099DF
		// (set) Token: 0x06000EE5 RID: 3813 RVA: 0x0000B7E7 File Offset: 0x000099E7
		public NetworkStateMachine networker { get; private set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x0000B7F0 File Offset: 0x000099F0
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x0000B7F8 File Offset: 0x000099F8
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0000B801 File Offset: 0x00009A01
		public void SetNextState(EntityState newNextState)
		{
			this.nextState = newNextState;
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0000B80A File Offset: 0x00009A0A
		public void SetNextStateToMain()
		{
			this.nextState = EntityState.Instantiate(this.mainStateType);
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0000B81D File Offset: 0x00009A1D
		public bool SetInterruptState(EntityState newNextState, InterruptPriority interruptPriority)
		{
			if (((this.nextState != null) ? this.nextState : this.state).GetMinimumInterruptPriority() <= interruptPriority)
			{
				this.nextState = newNextState;
				return true;
			}
			return false;
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0000B847 File Offset: 0x00009A47
		public bool HasPendingState()
		{
			return this.nextState != null;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x0005A918 File Offset: 0x00058B18
		public void SetState([NotNull] EntityState newState)
		{
			this.nextState = null;
			newState.outer = this;
			if (this.state == null)
			{
				Debug.LogErrorFormat("State machine {0} on object {1} does not have a state!", new object[]
				{
					this.customName,
					base.gameObject
				});
			}
			this.state.OnExit();
			this.state = newState;
			this.state.OnEnter();
			if (this.networkIndex != -1)
			{
				if (!this.networker)
				{
					Debug.LogErrorFormat("State machine {0} on object {1} does not a networker assigned!", new object[]
					{
						this.customName,
						base.gameObject
					});
				}
				this.networker.SendSetEntityState(this.networkIndex);
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x0005A9C4 File Offset: 0x00058BC4
		private void Awake()
		{
			this.networker = base.GetComponent<NetworkStateMachine>();
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.commonComponents = new EntityStateMachine.CommonComponentCache(base.gameObject);
			this.state = new Uninitialized();
			this.state.outer = this;
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x0005AA14 File Offset: 0x00058C14
		private void Start()
		{
			if (this.nextState != null && this.networker && !this.networker.hasAuthority)
			{
				this.SetState(this.nextState);
				return;
			}
			Type stateType = this.initialStateType.stateType;
			if (this.state is Uninitialized && stateType != null && stateType.IsSubclassOf(typeof(EntityState)))
			{
				this.SetState(EntityState.Instantiate(stateType));
			}
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x0000B852 File Offset: 0x00009A52
		public void Update()
		{
			this.state.Update();
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0000B85F File Offset: 0x00009A5F
		public void FixedUpdate()
		{
			if (this.nextState != null)
			{
				this.SetState(this.nextState);
			}
			this.state.FixedUpdate();
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x0000B880 File Offset: 0x00009A80
		// (set) Token: 0x06000EF2 RID: 3826 RVA: 0x0000B888 File Offset: 0x00009A88
		public bool destroying { get; private set; }

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0000B891 File Offset: 0x00009A91
		private void OnDestroy()
		{
			this.destroying = true;
			if (this.state != null)
			{
				this.state.OnExit();
				this.state = null;
			}
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0005AA90 File Offset: 0x00058C90
		private void OnValidate()
		{
			if (this.mainStateType.stateType == null)
			{
				if (this.customName == "Body")
				{
					if (base.GetComponent<CharacterMotor>())
					{
						this.mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));
						return;
					}
					if (base.GetComponent<RigidbodyMotor>())
					{
						this.mainStateType = new SerializableEntityStateType(typeof(FlyState));
						return;
					}
				}
				else
				{
					if (this.customName == "Weapon")
					{
						this.mainStateType = new SerializableEntityStateType(typeof(Idle));
						return;
					}
					if (this.customName == "AI")
					{
						BaseAI component = base.GetComponent<BaseAI>();
						if (component)
						{
							this.mainStateType = component.scanState;
						}
					}
				}
			}
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0000B8D8 File Offset: 0x00009AD8
		public bool SetInterruptState1(EntityState newNextState, InterruptPriority interruptPriority)
		{
			this.nextState = newNextState;
			return true;
		}

		// Token: 0x040012F3 RID: 4851
		private EntityState nextState;

		// Token: 0x040012F4 RID: 4852
		[Tooltip("The name of this state machine.")]
		public string customName;

		// Token: 0x040012F5 RID: 4853
		[Tooltip("The type of the state to enter when this component is first activated.")]
		public SerializableEntityStateType initialStateType = new SerializableEntityStateType(typeof(TestState1));

		// Token: 0x040012F6 RID: 4854
		[Tooltip("The preferred main state of this state machine.")]
		public SerializableEntityStateType mainStateType;

		// Token: 0x040012F9 RID: 4857
		public EntityStateMachine.CommonComponentCache commonComponents;

		// Token: 0x040012FA RID: 4858
		[NonSerialized]
		public int networkIndex = -1;

		// Token: 0x020002E8 RID: 744
		public struct CommonComponentCache
		{
			// Token: 0x06000EF7 RID: 3831 RVA: 0x0005AB60 File Offset: 0x00058D60
			public CommonComponentCache(GameObject gameObject)
			{
				this.transform = gameObject.transform;
				this.characterBody = gameObject.GetComponent<CharacterBody>();
				this.characterMotor = gameObject.GetComponent<CharacterMotor>();
				this.characterDirection = gameObject.GetComponent<CharacterDirection>();
				this.rigidbody = gameObject.GetComponent<Rigidbody>();
				this.rigidbodyMotor = gameObject.GetComponent<RigidbodyMotor>();
				this.rigidbodyDirection = gameObject.GetComponent<RigidbodyDirection>();
				this.railMotor = gameObject.GetComponent<RailMotor>();
				this.modelLocator = gameObject.GetComponent<ModelLocator>();
				this.inputBank = gameObject.GetComponent<InputBankTest>();
				this.teamComponent = gameObject.GetComponent<TeamComponent>();
				this.healthComponent = gameObject.GetComponent<HealthComponent>();
				this.skillLocator = gameObject.GetComponent<SkillLocator>();
				this.characterEmoteDefinitions = gameObject.GetComponent<CharacterEmoteDefinitions>();
				this.cameraTargetParams = gameObject.GetComponent<CameraTargetParams>();
				this.sfxLocator = gameObject.GetComponent<SfxLocator>();
				this.bodyAnimatorSmoothingParameters = gameObject.GetComponent<BodyAnimatorSmoothingParameters>();
			}

			// Token: 0x040012FC RID: 4860
			public readonly Transform transform;

			// Token: 0x040012FD RID: 4861
			public readonly CharacterBody characterBody;

			// Token: 0x040012FE RID: 4862
			public readonly CharacterMotor characterMotor;

			// Token: 0x040012FF RID: 4863
			public readonly CharacterDirection characterDirection;

			// Token: 0x04001300 RID: 4864
			public readonly Rigidbody rigidbody;

			// Token: 0x04001301 RID: 4865
			public readonly RigidbodyMotor rigidbodyMotor;

			// Token: 0x04001302 RID: 4866
			public readonly RigidbodyDirection rigidbodyDirection;

			// Token: 0x04001303 RID: 4867
			public readonly RailMotor railMotor;

			// Token: 0x04001304 RID: 4868
			public readonly ModelLocator modelLocator;

			// Token: 0x04001305 RID: 4869
			public readonly InputBankTest inputBank;

			// Token: 0x04001306 RID: 4870
			public readonly TeamComponent teamComponent;

			// Token: 0x04001307 RID: 4871
			public readonly HealthComponent healthComponent;

			// Token: 0x04001308 RID: 4872
			public readonly SkillLocator skillLocator;

			// Token: 0x04001309 RID: 4873
			public readonly CharacterEmoteDefinitions characterEmoteDefinitions;

			// Token: 0x0400130A RID: 4874
			public readonly CameraTargetParams cameraTargetParams;

			// Token: 0x0400130B RID: 4875
			public readonly SfxLocator sfxLocator;

			// Token: 0x0400130C RID: 4876
			public readonly BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters;
		}
	}
}
