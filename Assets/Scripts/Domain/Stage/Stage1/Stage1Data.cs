using System;
using MiniUnidux.SceneTransition;
using TestUnity2DActionGame.Domain.Player;

namespace TestUnity2DActionGame.Domain.Stage.Stage1
{
    [Serializable]
    public class Stage1Data : ISceneData
    {
        public int score { get; set; }
        
        Stage1Data() {}

        public Stage1Data(PlayerState playerState, int score)
        {
            this.score = score;
        }
    }
}
