using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace SPG.VContainerExtensions.Installers
{
    public abstract class BaseScriptableObjectInstaller
        : ScriptableObject
        , IDependenciesInstaller
    {
        public abstract void Install([NotNull] IContainerBuilder builder);
    }
}