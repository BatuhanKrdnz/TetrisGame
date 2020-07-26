using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{

    public Sprite m_iconTrue;
    public Sprite m_iconFalse;

    public bool m_defaultIconState = true;

    Image m_image;
    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();

        //alternative if else
        //m_image.sprite = (m_defaultIconState) ? m_iconTrue : m_iconFalse;

        if (m_defaultIconState)
        {
            m_image.sprite = m_iconTrue;
        }
        else
        {
            m_image.sprite = m_iconFalse;
        }
    }

    public void ToggleIcon(bool state)
    {
        if(!m_image || !m_iconTrue || !m_iconFalse)
        {

            Debug.LogWarning("WARNING! ICONTOGGLE is missing!!");
            return;
        }
        m_image.sprite = (state) ? m_iconTrue : m_iconFalse;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
