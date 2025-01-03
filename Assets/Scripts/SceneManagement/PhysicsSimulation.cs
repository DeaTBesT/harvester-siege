using Mirror;
using UnityEngine;

namespace SceneManagement
{
    public class PhysicsSimulation : MonoBehaviour
    {
        private PhysicsScene _physicsScene;
        private PhysicsScene2D _physicsScene2D;

        private bool _simulatePhysicsScene;
        private bool _simulatePhysicsScene2D;

        private void Awake()
        {
            if (NetworkServer.active)
            {
                _physicsScene = gameObject.scene.GetPhysicsScene();
                _simulatePhysicsScene = (_physicsScene.IsValid()) && (_physicsScene != Physics.defaultPhysicsScene);

                _physicsScene2D = gameObject.scene.GetPhysicsScene2D();
                _simulatePhysicsScene2D =
                    (_physicsScene2D.IsValid()) && (_physicsScene2D != Physics2D.defaultPhysicsScene);
            }
            else
            {
                enabled = false;
            }
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (_simulatePhysicsScene)
            {
                _physicsScene.Simulate(Time.fixedDeltaTime);
            }

            if (_simulatePhysicsScene2D)
            {
                _physicsScene2D.Simulate(Time.fixedDeltaTime);
            }
        }
    }
}