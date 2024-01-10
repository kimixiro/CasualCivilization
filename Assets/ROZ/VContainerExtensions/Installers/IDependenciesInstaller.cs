using JetBrains.Annotations;
using VContainer;

namespace ROZ.VContainerExtensions.Installers
{
    public interface IDependenciesInstaller
    {
        void Install([NotNull] IContainerBuilder builder);
    }
}