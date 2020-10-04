using System;

namespace CGDK.Core
{
    [MonoSingletonPath("[CjFrame]/[MonoAgent]")]
    public class MonoAgent : MonoSingleton<MonoAgent>
    {
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        
#if UNITY_EDITOR
        public event Action OnGui;
        private void OnGUI()
        {
            OnGui?.Invoke();
        }
#endif

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}