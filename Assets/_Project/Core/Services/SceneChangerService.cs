using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace Core.Services
{
    public interface ISceneChangerService
    {
        UniTask ChangeScene(string sceneName, bool skipFade = false);
    }

    public class SceneChangerService : SceneService, ISceneChangerService
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<ISceneChangerService>().AsSelf();
        }
        
        [SerializeField] private Image fade;
        [SerializeField] private float fadeSpeed = 1f;

        private Action _onSceneChange;
        public void OnChangeCheck(Action onChange) => _onSceneChange += onChange;

        private bool _isChanging;

        private void Start()
        {
            var c = fade.color;
            c.a = 1f;
            fade.color = c;

            FadeIn().Forget();
        }

        [Inject] private ILifetime _lifetime;
        public async UniTask ChangeScene(string sceneName, bool skipFade = false)
        {
            if (_isChanging) return;
            _isChanging = true;

            if (!skipFade)
                await FadeOut().AttachExternalCancellation(_lifetime.GetToken());

            await SceneManager.LoadSceneAsync(sceneName);

            _onSceneChange?.Invoke();
            _onSceneChange = null;

            if (!skipFade)
                await FadeIn();

            _isChanging = false;
        }

        private async UniTask FadeOut()
        {
            var color = fade.color;
            while (color.a < 1f)
            {
                if (!fade) return;
                color.a += fadeSpeed * Time.deltaTime;
                fade.color = color;
                await UniTask.Yield().ToUniTask().AttachExternalCancellation(_lifetime.GetToken());
            }
        }

        private async UniTask FadeIn()
        {
            var color = fade.color;
            while (color.a > 0f)
            {
                if (!fade) return;
                color.a -= fadeSpeed * Time.deltaTime;
                fade.color = color;
                await UniTask.Yield().ToUniTask().AttachExternalCancellation(_lifetime.GetToken());
            }
        }
    }
}
