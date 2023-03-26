using FEnum;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSceneManager : FSingleton<FSceneManager>
{
    private SceneType nextSceneType;
    private SceneType currentSceneType;
    private float progress;

    private SpriteRenderer fadeSpriteRenderer;
    private float fadeTime;

    public SceneType CurrentSceneType { get { return currentSceneType; } }
    public float Progress { get { return progress; } }


    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Instance.fadeSpriteRenderer = Instance.AddComponent<SpriteRenderer>();
        Instance.fadeSpriteRenderer.color = new Color(0, 0, 0, 0);
        Instance.fadeSpriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Loading/Square");
        Instance.fadeSpriteRenderer.sortingLayerID = SortingLayer.layers[SortingLayer.layers.Length - 1].id;
        Instance.fadeSpriteRenderer.enabled = false;

        float worldScreenHeight = (float)(Camera.main.orthographicSize * 2.0);
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        Instance.transform.localScale = new Vector3(worldScreenWidth, worldScreenHeight, 1);

        SceneManager.sceneLoaded += Instance.OnSceneLoaded;
    }

    public FSceneManager()
    {
    }

    public void ChangeSceneAfterLoading(SceneType InType, float InFadeTime = 0.0f)
    {
        progress = 0.0f;
        nextSceneType = InType;

        SceneManager.sceneLoaded += OnLoadingSceneLoaded;
        if (InFadeTime == 0)
        {
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            fadeTime = InFadeTime;
            StartCoroutine(FadeOutCoroutine());
            StartCoroutine(ChangeSceneCoroutine(SceneType.Loading));
        }
    }

    public void ChangeScene(SceneType InType, float InFadeTime = 0.0f)
    {
        if (InFadeTime == 0)
        {
            SceneManager.LoadScene(ConvertSceneTypeToString(InType));
        }
        else
        {
            fadeTime = InFadeTime;
            StartCoroutine(FadeOutCoroutine());
            StartCoroutine(ChangeSceneCoroutine(InType));
        }
    }

    private void OnSceneLoaded(Scene InScene, LoadSceneMode InMode)
    {
        currentSceneType = ConvertStringToSceneType(InScene.name);

        if (0 < fadeTime)
        {
            StartCoroutine(FadeInCoroutine());
        }
    }

    private void OnLoadingSceneLoaded(Scene InScene, LoadSceneMode InMode)
    {
        if (SceneType.Loading != ConvertStringToSceneType(InScene.name))
            return;

        StartCoroutine(ChangeSceneAfterLoadingCoroutine());
    }

    private IEnumerator ChangeSceneAfterLoadingCoroutine()
    {
        yield return new WaitForSecondsRealtime(fadeTime);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ConvertSceneTypeToString(nextSceneType));
        asyncOperation.allowSceneActivation = false;

        float timer = 0.0f;
        while (!asyncOperation.isDone)
        {
            yield return null;
            timer += Time.deltaTime;

            float maxProgress = asyncOperation.progress < 0.9f ? asyncOperation.progress : 1.0f;
            progress = Mathf.Lerp(progress, maxProgress, timer);
            if (maxProgress <= progress)
            {
                timer = 0f;
            }

            if (1.0f <= progress)
            {
                SceneManager.sceneLoaded -= OnLoadingSceneLoaded;
                currentSceneType = nextSceneType;
                nextSceneType = SceneType.None;

                if(0 < fadeTime)
                {
                    yield return StartCoroutine(FadeOutCoroutine());
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                }

                asyncOperation.allowSceneActivation = true;
                yield break;
            }
        }
    }
    
    private IEnumerator ChangeSceneCoroutine(SceneType InType)
    {
        yield return new WaitForSecondsRealtime(fadeTime);

        SceneManager.LoadScene(ConvertSceneTypeToString(InType));
    }

    private IEnumerator FadeOutCoroutine()
    {
        fadeSpriteRenderer.enabled = true;

        float deltaTime = 0;
        while (fadeSpriteRenderer.color.a < 1)
        {
            deltaTime += Time.unscaledDeltaTime;
            fadeSpriteRenderer.color = fadeSpriteRenderer.color.WithAlpha(Mathf.Lerp(0, 1, deltaTime / fadeTime));

            yield return null;
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        float deltaTime = 0;
        while (0 < fadeSpriteRenderer.color.a)
        {
            deltaTime += Time.unscaledDeltaTime;
            fadeSpriteRenderer.color = fadeSpriteRenderer.color.WithAlpha(Mathf.Lerp(0, 1, 1 - deltaTime / fadeTime));

            yield return null;
        }

        fadeSpriteRenderer.enabled = false;

        if(currentSceneType != SceneType.Loading)
        {
            fadeTime = 0;
        }
    }

    private string ConvertSceneTypeToString(SceneType InType)
    {
        switch (InType)
        {
            case SceneType.Login: return "LoginScene";
            case SceneType.Lobby: return "LobbyScene";
            case SceneType.Battle: return "BattleScene";
            case SceneType.Loading: return "LoadingScene";
        }

        return "";
    }

    private SceneType ConvertStringToSceneType(string InSceneName)
    {
        switch (InSceneName)
        {
            case "LoginScene": return SceneType.Login;
            case "LobbyScene": return SceneType.Lobby;
            case "BattleScene": return SceneType.Battle;
            case "LoadingScene": return SceneType.Loading;
        }

        return SceneType.None;
    }

}