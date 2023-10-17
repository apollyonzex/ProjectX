using Inits;
using UnityEngine;
using TMPro;

public class LanguageLoad : MonoBehaviour
{
    public string id;
    TextMeshProUGUI cpn;

    //==================================================================================================

    private void OnEnable()
    {
        cpn = transform.GetComponent<TextMeshProUGUI>();
        LanguageMgr.instance.load += load;

        load();
    }


    private void OnDisable()
    {
        LanguageMgr.instance.load -= load;
    }


    void load()
    {
        cpn.text = LanguageMgr.instance.get_text(id);
    }

}
