using System;
using MiniUnidux.SceneTransition;
using TestUnity2DActionGame.Domain.Player;

namespace TestUnity2DActionGame.Domain.Result
{
    [Serializable]
    public class ResultData : ISceneData
    {
        // Playerが勝ったか負けたか
        public PlayerState playerState { get; set; }
        public int score { get; set; }
        
        ResultData() {}

        public ResultData(PlayerState playerState, int score)
        {
            this.playerState = playerState;
            this.score = score;
        }
    }
}
