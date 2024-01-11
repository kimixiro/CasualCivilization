using UnityEditor;
using UnityEngine;
using Logger = SPG.Logging.Logger;

namespace SPG.DB.Runtime
{
    public abstract class BaseTableKey : ScriptableObject, ISerializationCallbackReceiver
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
                        nameof(BaseTableKey), 
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