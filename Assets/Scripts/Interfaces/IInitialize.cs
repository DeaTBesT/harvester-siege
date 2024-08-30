namespace Interfaces
{
    public interface IInitialize
    {
        public bool IsEnable { get; set; }
        
        void Initialize(params object[] objects);
    }
}