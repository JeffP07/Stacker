using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShowCredits : MonoBehaviour {

    public GameObject credits;
    public GameObject menu_ui;
    
    public void ShowCreditsUI() {
        credits.SetActive(true);
        menu_ui.SetActive(false);
    }

    public void ShowMenuUI() {
        credits.SetActive(false);
        menu_ui.SetActive(true);
    }
}
