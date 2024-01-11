using SPG.VContainerExtensions.Installers;
using UnityEngine;
using VContainer;

namespace SPG.DB.Runtime.Installers
{
    [CreateAssetMenu(menuName = "Installers/Global/" + nameof(SPG.DB) + "/" + nameof(DataBaseInstaller))]
    public class DataBaseInstaller
        : BaseScriptableObjectInstaller
    {
        [SerializeField] 
        private Lifetime _dataBaseLifeTime;
        
        public override void Install(IContainerBuilder builder)
        {
            builder
                .Register<DataBase>(_dataBaseLifeTime)
                .AsSelf();
        }
    }
}