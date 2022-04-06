using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;
//using Managers;

public class LoadingManager : MonoBehaviour {

    public GameObject UILoading;
    public GameObject UILogin;

    public Slider ProgressBar;
    public Text ProgressNumber;
    public Text ProgressText;
    

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);

        UserService.Instance.Init();
        
        // Fake Loading Simulate
        for (float i = 0; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            ProgressBar.value = i;
            ProgressNumber.text = i.ToString();
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }


    // Update is called once per frame
    void Update () {

    }
}
