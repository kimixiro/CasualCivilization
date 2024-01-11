using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace SPG.VContainerExtensions.Installers
{
    public abstract class BaseMonoInstaller 
        : MonoBehaviour
        , IDependenciesInstaller
    {
        private void OnValidate()
        {
            name = $"[{GetType().Name}-Installer]";
        }

        public virtual void Install([NotNull] IContainerBuilder builder)
        {
            Assert.IsNotNull(builder);
        }
    }
}