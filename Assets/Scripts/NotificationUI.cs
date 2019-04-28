using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [HideInInspector]
    public int dayOfExpiration;
    public TMP_Text textField; 
    [HideInInspector]
    public string baseText;
    public RectTransform rect;
}
