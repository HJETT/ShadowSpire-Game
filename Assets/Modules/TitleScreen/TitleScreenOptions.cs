using System.Collections;
using Battle.Options;
using CameraUtils;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenOptions : UIOptions<TitleScreenOption, TitleScreenOptionData>
{
    private void Start()
    {
        LoadOptions(new TitleScreenOptionData[] {
            new() {
                text = "Play",
                OnEnter = OnPlay
            },
            new() {
                text = "Quit",
                OnEnter = OnQuit
            }
        });

        ShowSelection();

        InputManager.Instance.OnMoveUI.AddListener(Move);
        InputManager.Instance.OnEnterUI.AddListener(Enter);
    }


    protected override void LoadOptions(TitleScreenOptionData[] options)
    {
        // Destroy all options
        foreach (Transform option in transform)
            Destroy(option.gameObject);

        loadedOptions = new TitleScreenOption[options.Length];

        float singleY = -rectTransform.rect.size.y / options.Length;
        float startY = (options.Length - 1) / 2f * -singleY;

        for (int i = 0; i < options.Length; i++)
        {
            var newOption = Instantiate(optionPrefab.gameObject, transform);

            if (newOption.TryGetComponent(out RectTransform rectTransform))
                rectTransform.anchoredPosition = new Vector2(0, startY + singleY * i);

            if (newOption.TryGetComponent(out TitleScreenOption titleOption))
            {
                titleOption.SetParent(this);
                titleOption.LoadOption(options[i]);
                loadedOptions[i] = titleOption;
            }
        }

        selectedIndex = 0;
    }

    protected override void OnMoveSelected(Vector2 dir) => base.OnMoveSelected(new(dir.y, dir.x));

    #region Play Sequence

    private bool isRunningPlaySequence;

    public void OnPlay()
    {
        if (isRunningPlaySequence)
            return;

        if (Camera2D.IsIn3D)
            Arcade.Arcade.Instance.StartCoroutine(PlaySequence());
        else
            StartCoroutine(PlaySequence());

        isRunningPlaySequence = true;
    }

    private IEnumerator PlaySequence()
    {
        if (Camera2D.IsIn3D)
        {
            Arcade.Arcade.QuitUnload = false;

            var title = SceneManager.UnloadSceneAsync("TitleScreen");

            while (!title.isDone)
                yield return null;
        }

        SceneManager.LoadScene("Game", Camera2D.IsIn3D ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    #endregion

    #region Quit Sequence

    public void OnQuit()
    {
        if (Camera2D.IsIn3D)
        {
            Arcade.Arcade.QuitUnload = true;
            SceneManager.UnloadSceneAsync("TitleScreen");
        }
        else
            Application.Quit();
    }

    #endregion
}
