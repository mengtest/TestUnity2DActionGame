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
using TestUnity2DActionGame.Presenter.Common;
using TestUnity2DActionGame.View.Title;

namespace TestUnity2DActionGame.Presenter.Title
{
    public class TitleViewController : SingletonMonoBehaviour<TitleViewController>
    {
        private TitleView titleView;

        // sound
        private SoundManager soundManager;

        // Start is called before the first frame update
        protected override void Awake()
        {
            titleView = GetComponent<TitleView>();

            foreach (var scene in _getPermanentScenesWithoutEntryPoint) {
                SceneManager.LoadSceneAsync($"{scene}", LoadSceneMode.Additive);
            }

        }

        public void Start()
        {
            // CommonからSoundManagerを取得する
            soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");

            soundManager.PlayBGM(BGM.Title);
            titleView.buttonEntry.OnClickAsObservable()
                .First()
                .Delay(TimeSpan.FromSeconds(0.6f))
                .Subscribe(_ => PushedEnter())
                .AddTo(this);
        }

        // Common Systemを除いたパーマネントシーンを読み込みたい順番に設定する
        readonly List<SceneName> _getPermanentScenesWithoutEntryPoint = new()
        {
            SceneName.Common
        };

        public void PushedEnter()
        { 
            // ステージセレクト画面へ遷移するプッシュアクションを生成
            // var pushToStageSelectAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.StageSelect);

            // テストのため、直接遷移する
            var pushToStageSelectAction = PageActionManager<SceneName>.ActionCreator.Push(SceneName.Stage1);
            
            // プッシュアクションのディスパッチ
            MiniUniduxService.Dispatch(pushToStageSelectAction);
            soundManager.StopBGM();    
        }
    }
}
