using UnityEngine;

namespace Utils
{
    public static class NetworkSerializer
    {
        public static string SerializeScriptableObject(ScriptableObject scriptableObject) =>
            scriptableObject.name; 
        
        public static ScriptableObject DeserializeScriptableObject(string scriptableObject) =>
            Resources.Load(scriptableObject) as ScriptableObject;
    }
}