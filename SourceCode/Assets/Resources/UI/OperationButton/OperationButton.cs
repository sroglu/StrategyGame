using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationButton : MonoBehaviour
{
    public Button mButton;
    public Text mButtonText;

    public RectTransform mRectTransform;

    private void Awake()
    {
        mRectTransform=GetComponent<RectTransform>();

        if (mButton == null)
        {
            mButton = GetComponent<Button>();
            if (mButton != null && mButtonText == null)
            {
                mButtonText = mButton.GetComponentInChildren<Text>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
