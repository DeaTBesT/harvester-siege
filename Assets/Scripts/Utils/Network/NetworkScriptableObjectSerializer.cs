using UnityEngine;

namespace Utils.Networking
{
    public static class NetworkScriptableObjectSerializer
    {
        public static string SerializeScriptableObject(ScriptableObject scriptableObject) =>
            scriptableObject.name; 
        
        public static ScriptableObject DeserializeScriptableObject(string scriptableObject) =>
            Resources.Load(scriptableObject) as ScriptableObject;
    }
}