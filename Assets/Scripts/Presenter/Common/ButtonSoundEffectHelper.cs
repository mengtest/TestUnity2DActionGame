using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MiniUnidux.Util;
using TestUnity2DActionGame.Domain.Common;
using TestUnity2DActionGame.Presenter.Common;

namespace TestUnity2DActionGame.Presenter.Common {
	public class ButtonSoundEffectHelper : MonoBehaviour
	{
		// sound
		[SerializeField] SE soundEffect;
		private SoundManager soundManager;

		void Start()
		{
			// CommonからSoundManagerを取得する
			soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");
		}
		public void PlaySE(){
			soundManager.PlaySE(soundEffect);
		}

		void OnDestroy()
		{}
	}
}