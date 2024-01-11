using System.Diagnostics.CodeAnalysis;
using SPG.DB.Runtime.Providers;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

namespace SPG.DB.Runtime.Installers
{
    public static class TablesInstallingUtils
    {
        public static void RegisterTable<TTable, TKey, TData, TDataProvider>
        (
            [NotNull] this IContainerBuilder builder,
            [NotNull] TTable table,
            Lifetime lifeTime = Lifetime.Scoped
        )
            where TTable : BaseDataTable<TKey, TData>
            where TKey : BaseTableKey
            where TData : class, new()
            where TDataProvider : BaseDataProvider<TTable, TKey, TData>
        {
            Assert.IsNotNull(builder);
            Assert.IsNotNull(table);
            
            builder
                .RegisterComponent(table)
                .AsImplementedInterfaces()
                .AsSelf();
            
            builder
                .Register<TDataProvider>(lifeTime)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}