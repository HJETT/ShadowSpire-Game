using Battle.Options;
using TMPro;
using UnityEngine;

public class TitleScreenOptionData : UIOptionData 
{
    public string text;
}

public class TitleScreenOption : UIOption<TitleScreenOptionData>
{
    [SerializeField] TextMeshProUGUI optionTitle;
    protected override void OnLoadOption(TitleScreenOptionData option)
    {
        optionTitle.text = loadedOption.text;
    }
    public override void Select()
    {
        optionTitle.text = "> " + loadedOption.text + " <";
    }

    public override void Deselect()
    {
        optionTitle.text = loadedOption.text;
    }
}
