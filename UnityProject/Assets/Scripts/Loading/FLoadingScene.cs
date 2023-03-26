using TMPro;
using UnityEngine;

public class FLoadingScene : MonoBehaviour
{
    [SerializeField]
    Transform progressBar;
    [SerializeField]
    TextMeshPro progressText;

    private float progressBarMaxScale;

    private void Awake()
    {
        progressBarMaxScale = progressBar.localScale.x;
        SetProgress(0);
    }

    private void Update()
    {
        SetProgress(FSceneManager.Instance.Progress);
    }

    private void SetProgress(float InProgress)
    {
        int progress = (int)(InProgress * 100);
        progressBar.localScale = new Vector2(progressBarMaxScale * FSceneManager.Instance.Progress, progressBar.localScale.y);
        progressText.text = progress + "%";
    }
}
