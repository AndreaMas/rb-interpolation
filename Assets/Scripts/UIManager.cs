using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager mInstance;
    public static UIManager Instance => mInstance;

    [SerializeField] private Canvas masterInfoCanvas;
    [SerializeField] private Text bytesSentText;

    private void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // check for missing UI components
        if (!masterInfoCanvas)
            Debug.Log("[MY WARNING] UIManager misses reference to Canvas!");
        if (!bytesSentText)
            Debug.Log("[MY WARNING] UIManager misses reference to Text displaying bytes sent!");
    }

    public void DisableMasterInfoCanvas()
    {
        masterInfoCanvas.enabled = false;
    }

    public void SetBitesSentText(uint number)
    {
        bytesSentText.text = number.ToString();
    }
}
