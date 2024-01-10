using ROZ.VContainerExtensions.Installers;
using UnityEngine;
using VContainer;

namespace ROZ.DB.Runtime.Installers
{
    [CreateAssetMenu(menuName = "Installers/Global/" + nameof(DB) + "/" + nameof(DataBaseInstaller))]
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