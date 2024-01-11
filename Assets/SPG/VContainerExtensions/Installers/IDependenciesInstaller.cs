using JetBrains.Annotations;
using VContainer;

namespace SPG.VContainerExtensions.Installers
{
    public interface IDependenciesInstaller
    {
        void Install([NotNull] IContainerBuilder builder);
    }
}