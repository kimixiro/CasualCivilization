using System.Diagnostics.CodeAnalysis;
using ROZ.MVP.Options;
using ROZ.UI.Presenters;
using VContainer;
using VContainer.Unity;
using Assert = UnityEngine.Assertions.Assert;

namespace ROZ.UI.Installers
{
    public static class PopupsInstallingExtensions
    {
        public static void RegisterPopup<TPresenter, TModel>
        (
            [NotNull] this IContainerBuilder builder, 
            [NotNull] TPresenter prefab
        )
            where TPresenter : BasePopupPresenter, IHasModel<TModel>
            where TModel : class
        {
            Assert.IsNotNull(builder);
            Assert.IsNotNull(prefab);

            builder
                .Register<TModel>(Lifetime.Transient)
                .AsSelf();
            
            builder
                .RegisterComponentInNewPrefab(prefab, Lifetime.Transient);
        }
        
        public static void RegisterPopup<TPresenter>
        (
            [NotNull] this IContainerBuilder builder, 
            [NotNull] TPresenter prefab
        )
            where TPresenter : BasePopupPresenter
        {
            Assert.IsNotNull(builder);
            Assert.IsNotNull(prefab);
            
            builder
                .RegisterComponentInNewPrefab(prefab, Lifetime.Transient);
        }
    }
}