using UnityEngine;
using UnityEngine.UI;

///<summary>
///Put on the "Start Game" button. On click it reveals the mode panel (Survival/Endless)
///and hides the given object (usually the Start Game line itself), giving a two-step
///"Start Game -> choose mode" flow. Public fields so they can be wired from the editor.
/// </summary>
[RequireComponent(typeof(Button))]
public class ModeMenuToggle : MonoBehaviour
{
    //
    public GameObject panelToShow;
    public GameObject objectToHide;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Reveal);
    }

    public void Reveal()
    {
        if (panelToShow != null) panelToShow.SetActive(true);
        if (objectToHide != null) objectToHide.SetActive(false);
    }
}
