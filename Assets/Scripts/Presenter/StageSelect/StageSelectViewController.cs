using System.Net;
using System.Security.AccessControl;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnity2DActionGame.View.StageSelect;

namespace TestUnity2DActionGame.Presenter.StageSelect
{
    public class StageSelectViewController : SingletonMonoBehaviour<StageSelectViewController> 
    {

        StageSelectView stageSelectView;
        //[SerializeField] AudioManager audioManager; // Audio Manager

         // StageSelectTransform
        public Transform StageSelectTransform;

        // セレクト終了までの時間管理
        private int maxSeconds = 60;
        private int timeCount;

        private int existStageNum = 3;

        //private bool isReady = false;

        protected override void Awake()
        {
            stageSelectView = GetComponent<StageSelectView>();
        }

        public void Start()
        {
            //audioManager.PlayBGM(BGM.SelectStage);

            for(int i = 1; i <= existStageNum; i++) {
                //CreateStageInfo(i, stageSelectTransform, false);
            }

            // ランダムセレクトパネルを追加

            /*stageSelectView.battleStartButton.OnClickAsObservable()
                .First()
                .Delay(TimeSpan.FromSeconds(0.6f))
                .Subscribe(_ => OnSelectedStagees())
                .AddTo(this);
    
            StartCoroutine(SelectStageAndGameStart());*/
        }

        public void ClickedStageInfoCard(/*StageInfoController stageInfo*/)
        {
            /*if (stageInfo != null) {
                if (stageInfo.GetOwner() == Player.Player1) {
                    int stage1ID = stageInfo.GetStageInfoID(); // ランダムの場合は0
 
                    if (stage1ID == 0) {
                        stage1ID = UnityEngine.Random.Range(1, existStageNum);
                        stageInfo.UpdateModel(entitiesManager.GetStageEntity(stage1ID));
                    } 
                    // プレイヤーセレクトOKダイアログを表示する
                    stageSelectView.ShowStageInfoDialog(stage1ID, stageInfo.GetOwner(), stageInfo.model.GetIcon(),stageInfo.model.GetName(),stageInfo.model.GetInfo());
                } else if (stageInfo.GetOwner() == Player.Player2) {
                    int stage2ID = stageInfo.GetStageInfoID(); // ランダムの場合は0
                    if (stage2ID == 0) {
                        stage2ID = UnityEngine.Random.Range(1,  existStageNum);
                        stageInfo.UpdateModel(entitiesManager.GetStageEntity(stage2ID));
                    }
                    // プレイヤーセレクトOKダイアログを表示する
                stageSelectView.ShowStageInfoDialog(stage2ID, stageInfo.GetOwner(), stageInfo.model.GetIcon(),stageInfo.model.GetName(),stageInfo.model.GetInfo());
                }
            }*/
        }

        public IEnumerator SelectStageAndGameStart()
        {
            // カウントダウン開始
            timeCount = maxSeconds;

            while (timeCount > 0) {
                yield return new WaitForSeconds(1); // 1秒待機
                timeCount--;
                stageSelectView.SetUntilEndOfSelectText(timeCount.ToString());
            }

            // 一定時間過ぎてもボタンが押されなかった場合はプレイヤー1のStageを勝手に決めてプレイヤー2をAIにする
            yield return new WaitForSeconds(0.05f);

            OnSelectedStagees();
        }

        public void OnSelectedStagees()
        {
            // stageが決まっていない時は勝手に決める
            /* FixMe */
            //audioManager.StopBGM();
            StopAllCoroutines();             
            CleanUp();  // いらないオブジェクトの破棄
            
            /*var stageData = new BattleData(playerRank);

            // バトル画面へ遷移するプッシュアクションを生成
            var pushToBattleAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName[idx], battleData);

            // プッシュアクションのディスパッチ
            MiniUniduxService.Dispatch(pushToBattleAction);
            */
        }

        // いらないオブジェクトの破棄
        void CleanUp()
        {
            /*StageInfoController[] stageInfoList = GetStageInfoList();
            foreach (StageInfoController stageInfo in stage1InfoList) {
                Destroy(stageInfo.gameObject);
            }*/
        }
    }
}