using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace ROZ.VContainerExtensions.Installers
{
    public abstract class BaseScriptableObjectInstaller
        : ScriptableObject
        , IDependenciesInstaller
    {
        public abstract void Install([NotNull] IContainerBuilder builder);
    }
}