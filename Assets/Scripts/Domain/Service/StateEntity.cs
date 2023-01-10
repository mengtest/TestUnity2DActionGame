using System;
using MiniUnidux;
using MiniUnidux.SceneTransition;

namespace TestUnity2DActionGame.Domain.Service
{
    // State Entity
    [Serializable]
    public class StateEntity : State
    {
            public SceneState<SceneName> Scene { get; set; } = new();
    }
}
