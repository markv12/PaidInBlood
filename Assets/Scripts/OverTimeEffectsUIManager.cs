using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OverTimeEffectsUIManager : MonoBehaviour
{
    public GameObject title;
    public RectTransform container;
    public GameObject notificationPrefab;
    
    List<NotificationUI> notifications = new List<NotificationUI>();
    List<NotificationUI> freeNotifications = new List<NotificationUI>();

    public void AddNotification(string text, int day){
        NotificationUI ui = GetFreeNotification();
        ui.baseText = text;
        ui.dayOfExpiration = day;
        notifications.Add(ui);
    }

    public void RefreshNotificaitons(int currentDay){
        int showing = 0;
        for (int i = 0; i < notifications.Count; i++)
        {
            NotificationUI ui = notifications[i];
            if(ui.dayOfExpiration <= currentDay){
                DisposeNotification(ui);
                i--;
            }else{
                ui.rect.anchoredPosition = new Vector2(0, -showing * ui.rect.sizeDelta.y);
                showing++;
                string notificationText = String.Format(ui.baseText, ui.dayOfExpiration - currentDay); 
                ui.textField.text = notificationText;
            }
        }
    }

    NotificationUI GetFreeNotification(){
        NotificationUI ui = null;
        if(freeNotifications.Count == 0){
            ui = Instantiate(notificationPrefab).GetComponent<NotificationUI>();
            ui.transform.SetParent(container);
            ui.transform.localScale = Vector3.one;
        }else{
            ui = freeNotifications[freeNotifications.Count - 1];
            freeNotifications.RemoveAt(freeNotifications.Count - 1);
        }
        ui.gameObject.SetActive(true);
        return ui;
    }

    void DisposeNotification(NotificationUI ui){
        ui.gameObject.SetActive(false);
        freeNotifications.Add(ui);
        if(notifications.Contains(ui))
            notifications.Remove(ui);
    }
}
