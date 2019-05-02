using System;

namespace RoR2
{
	// Token: 0x02000333 RID: 819
	public interface IInteractable
	{
		// Token: 0x060010E3 RID: 4323
		string GetContextString(Interactor activator);

		// Token: 0x060010E4 RID: 4324
		Interactability GetInteractability(Interactor activator);

		// Token: 0x060010E5 RID: 4325
		void OnInteractionBegin(Interactor activator);

		// Token: 0x060010E6 RID: 4326
		bool ShouldIgnoreSpherecastForInteractibility(Interactor activator);

		// Token: 0x060010E7 RID: 4327
		bool ShouldShowOnScanner();
	}
}
