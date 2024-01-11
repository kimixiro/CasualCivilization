using System;
using SPG.VContainerExtensions.Installers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = SPG.Logging.Logger;

namespace SPG.VContainerExtensions
{
    public abstract class BaseSceneScope
        : LifetimeScope
    {
        [SerializeField]
        private BaseMonoInstaller[] _monoInstallers;
        
        [SerializeField]
        private BaseScriptableObjectInstaller[] _soInstallers;

        private void OnValidate()
        {
            name = $"[{GetType().Name}-Scope]";
        }

        protected sealed override void Configure(IContainerBuilder builder)
        {
            Logger.LogInfo(GetType().Name, nameof(Configure), 
                $"Start installing dependencies of scope {name}", this);
            
            if (_monoInstallers != null)
            {
                Logger.LogInfo(GetType().Name, nameof(Configure), 
                    "Installing mono dependencies", this);
                
                for (var index = 0; index < _monoInstallers.Length; index++)
                {
                    var installer = _monoInstallers[index];
                    
                    if (installer == null)
                    {
                        Logger.LogWarning(GetType().Name, nameof(Configure), 
                            $"Installer at index {index} == null", this);
                        
                        continue;
                    }
                    try
                    {
                        Logger.LogInfo(GetType().Name, nameof(Configure), 
                            $"Start installing {installer.name} installer", this);
                        
                        installer.Install(builder);
                    }
                    catch (Exception e)
                    {
                        Logger.LogInfo(GetType().Name, nameof(Configure), 
                            $"Exception occured:", this);
                        
                        Logger.LogException(e);
                    }
                }
            }
            if (_soInstallers != null)
            {
                Logger.LogInfo(GetType().Name, nameof(Configure), 
                    "Start installing scriptable object installers", this);
                
                for (var index = 0; index < _soInstallers.Length; index++)
                {
                    var installer = _soInstallers[index];
                    
                    if (installer == null)
                    {
                        Logger.LogWarning(GetType().Name, nameof(Configure), 
                            $"Installer at index {index} == null", this);
                        
                        continue;
                    }
                    try
                    {
                        Logger.LogInfo(GetType().Name, nameof(Configure), 
                            $"Start installing {installer.name} installer", this);
                        
                        installer.Install(builder);
                    }
                    catch (Exception e)
                    {
                        Logger.LogInfo(GetType().Name, nameof(Configure), 
                            $"Exception occured:", this);
                        
                        Logger.LogException(e);
                    }
                }
            }
            Logger.LogInfo(GetType().Name, nameof(Configure), 
                "Start installing additional scope dependencies", this);
            
            Logger.LogInfo(GetType().Name, nameof(Configure), 
                "Installing completed", this);
        }
    }
}