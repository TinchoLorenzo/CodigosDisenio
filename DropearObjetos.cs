using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropearObjetos : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject Panel = eventData.pointerDrag.transform.parent.gameObject;
            ColumnController cc = Panel.GetComponent<ColumnController>();
            ColumnController.infoPos[] lista = cc.getListaPosiciones();
            for (int i=0; i < lista.Length; i++)
            {
                if (GameObject.ReferenceEquals(lista[i].boton, eventData.pointerDrag))
                {
                    lista[i].ocupado = false;
                    lista[i].boton = null;
                    break;
                }
            }

            if (transform.parent != eventData.pointerDrag.transform.parent)
            {
                transform.parent.GetComponent<ColumnController>().ArtefactoColumna.addArtefacto(eventData.pointerDrag.GetComponent<TareaController>().tarea);
                eventData.pointerDrag.transform.parent.GetComponent<ColumnController>().ArtefactoColumna.removeArtefacto(eventData.pointerDrag.GetComponent<TareaController>().tarea);
                eventData.pointerDrag.transform.SetParent(transform.parent);
                //EVENTO
                ColumnController nuevo = eventData.pointerDrag.transform.parent.gameObject.GetComponent<ColumnController>();
                TareaController tareaC= eventData.pointerDrag.GetComponent<TareaController>();
                tareaC.setPadre(nuevo.getArtefactoColumna());  
                tareaC.getTarea().setEstado(nuevo.getArtefactoColumna());
            }   
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Panel = eventData.pointerDrag.transform.parent.gameObject;
            lista = Panel.GetComponent<ColumnController>().getListaPosiciones();
            for (int i = 0; i < lista.Length; i++)
            {
                if (GameObject.ReferenceEquals(lista[i].slot, this))
                {
                    lista[i].ocupado = true;
                    lista[i].boton = eventData.pointerDrag.transform.gameObject;
                    break;
                }
            }
        }

        
    }

}