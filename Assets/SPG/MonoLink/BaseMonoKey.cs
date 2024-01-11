#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Logger = SPG.Logging.Logger;

namespace SPG.MonoLink
{
    public abstract class BaseMonoKey
        : ScriptableObject
        , ISerializationCallbackReceiver
    {
        [SerializeField] 
        private string _guid;

        public string Guid => _guid;
        
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out var currentGuid, out long _))
            {
                if (currentGuid != _guid)
                {
                    Logger.LogWarning
                    (
                        nameof(BaseMonoKey), 
                        nameof(OnBeforeSerialize), 
                        $"Guid changed from {_guid} to {currentGuid}", 
                        this
                    );
                    _guid = currentGuid;

                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }
        public void OnAfterDeserialize() { }
    }
}