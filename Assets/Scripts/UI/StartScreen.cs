using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [SerializeField] GameObject logoScreen;
    [SerializeField] RectTransform cloudWipe;
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject levelSelect;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroAnimation());
    }
    private IEnumerator IntroAnimation(){
        logoScreen.SetActive(true);
        startScreen.SetActive(false);
        levelSelect.SetActive(false);

        float timer = Time.time + 2;
        while(timer > Time.time){
            if(Input.GetMouseButtonDown(0)){
                break;
            }
            yield return null;
        }

        timer = Time.time + 1;
        while (timer > Time.time)
        {
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }
            cloudWipe.anchoredPosition = new Vector2(cloudWipe.anchoredPosition.x + Time.deltaTime * -800, 0);      
            yield return null;
        }

        logoScreen.SetActive(false);
        startScreen.SetActive(true);

        timer = Time.time + 1;
        while (timer > Time.time)
        {
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }
            cloudWipe.anchoredPosition = new Vector2(cloudWipe.anchoredPosition.x + Time.deltaTime * -800, 0);
            yield return null;
        }
        //
    }

    public void ToggleLevelSelect()
    {
        startScreen.SetActive(!startScreen.activeSelf);
        levelSelect.SetActive(!levelSelect.activeSelf);
    }
    public void ToggleMusic(){
        //ref to globalVar?
    }
    public void ToggleEffects(){

    }
    public void QuitGame(){
        Application.Quit();
    }
}
