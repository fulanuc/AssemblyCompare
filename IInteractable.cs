using System;

namespace RoR2
{
	// Token: 0x02000331 RID: 817
	public interface IInteractable
	{
		// Token: 0x060010CF RID: 4303
		string GetContextString(Interactor activator);

		// Token: 0x060010D0 RID: 4304
		Interactability GetInteractability(Interactor activator);

		// Token: 0x060010D1 RID: 4305
		void OnInteractionBegin(Interactor activator);

		// Token: 0x060010D2 RID: 4306
		bool ShouldIgnoreSpherecastForInteractibility(Interactor activator);
	}
}
