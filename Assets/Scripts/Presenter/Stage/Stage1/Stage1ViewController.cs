using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnity2DActionGame.Domain.Service;
using TestUnity2DActionGame.Domain.Common;
using TestUnity2DActionGame.Domain.Player;
using TestUnity2DActionGame.Domain.Stage;
using TestUnity2DActionGame.Domain.Stage.Stage1;
using TestUnity2DActionGame.Domain.Result;
using TestUnity2DActionGame.Presenter.Common;
using TestUnity2DActionGame.Presenter.Player;
using TestUnity2DActionGame.View.Stage.Stage1;

namespace TestUnity2DActionGame.Presenter.Stage.Stage1
{
    public class Stage1ViewController : SingletonMonoBehaviour<Stage1ViewController>
    {
        private Stage1View stage1View;
        [SerializeField] Stage1Data stage1Data;
        [SerializeField] PlayerController player;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] List<GameObject> enemyPrefabs;
        [SerializeField] GameObject carrotPrefab;

        // Sound
        private SoundManager soundManager;

        private int enemyNum;
        private int carrotNum;

        // Start is called before the first frame update
        protected override void Awake()
        {
            stage1View = GetComponent<Stage1View>();

            stage1Data.score = 0;
            enemyNum = 1;
            carrotNum = 10;

            // FixMe: 初期データを受け取る
            // stage1Data = MiniUniduxService.State.Scene.GetData<Stage1Data>();

        }

        public void Start()
        {
            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");

            soundManager.PlayBGM(BGM.Stage1);

            CreateCharacters();
            CreateCarrots();
        }

        public void CreateCharacters()
        {
            var initPos = new Vector3(0.5f, 0f, 1.0f);
            GameObject playerObject = Instantiate(playerPrefab, initPos, Quaternion.identity);
            player = playerObject.GetComponent<PlayerController>();

            SetObservers();

            foreach (GameObject enemyPrefab in enemyPrefabs) {

                // 敵の生成数
                for (var i = 0; i < enemyNum; i++) {
                    // FixMe: 敵の位置は適当にランダムに生成するべき
                    initPos = new Vector3(1f + i * 0.1f, 5f + i * 0.01f, 1.0f);
                    Instantiate(enemyPrefab, initPos, Quaternion.identity);
                }
            }
        }

        public void CreateCarrots()
        {
            for (var i = 0; i < carrotNum; i++) {
                // FixMe: 敵の位置は適当にランダムに生成するべき
                var initPos = new Vector3(0f + i * 0.1f ,0f + i * 5f, 1f);
                GameObject tresureObject = Instantiate(carrotPrefab, initPos, Quaternion.identity);
                //TresureController tresureController = tresureObject.GetComponent<PlayerController>();
                //tresureController.initPos = initPos;
            }
        }

        public void SetObservers()
        {
            if (player != null) {
                player.tresureCount
                    .Subscribe(_ => CalcScore())
                    .AddTo(this);

                // プレイヤーが死亡したらゲームオーバー
                player.playerState
                    .Where(x => (x == PlayerState.StageClear) || (x == PlayerState.Dead))
                    .First()
                    .Delay(TimeSpan.FromSeconds(5.0f))
                    .Subscribe(_ => GameOver())
                    .AddTo(this);
            }
        }

        public void CalcScore()
        {
            // 入手したニンジンの数1個につき100点
            stage1Data.score = player.tresureCount.Value * 100;
        }

        public void GameOver()
        {
            //音楽を停止
            soundManager.StopBGM();

            UnityEngine.Debug.Log("gameover");

            StopAllCoroutines();

            var resultData = new ResultData(player.playerState.Value, stage1Data.score);

            // リザルト画面へ遷移するプッシュアクションを生成
            var pushToResultAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Result, resultData);

            // プッシュアクションのディスパッチ
            MiniUniduxService.Dispatch(pushToResultAction);
        }
    }
}
