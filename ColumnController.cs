using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CymaticLabs.Unity3D.Amqp;
using Photon.Pun;

public class ColumnController : MonoBehaviour
    {
        public struct infoPos{
            public int nro;
            public Vector3 pos;
            public GameObject boton;
            public GameObject slot;
            public bool ocupado;
        }
        [SerializeField] GameObject PrefabBotonTarea;
        [SerializeField] GameObject PrefabSlotTarea;
        [SerializeField] GameObject DropdownOrdenar;
        public List<GameObject> slots;
        public List<GameObject> tareas;
        infoPos[] listaPosiciones;
        [SerializeField] GameObject Titulo;
        //comunicacion con el modelo
        public ArtefactoContenedor padre;
        public ArtefactoContenedor ArtefactoColumna;
        [SerializeField] GameObject MenuEliminar;
        [SerializeField] PhotonView pv;

        //MIRAR EL AWAKE
        public void Awake()
        {
            ArtefactoColumna = new ArtefactoContenedor();
            tareas = new List<GameObject>();
            slots = new List<GameObject>();
            listaPosiciones = new infoPos[8];
            var posInicial = -40;
            for(int i = 0; i < listaPosiciones.Length; i++)
            {
                GameObject slot = Instantiate(PrefabSlotTarea, PrefabSlotTarea.transform.position, PrefabSlotTarea.transform.rotation);
                infoPos nodo;
                nodo.ocupado = false;
                nodo.slot = slot;
                slot.transform.position = new Vector3(0, posInicial -30 * i,0);
                nodo.pos = slot.transform.position;
                slot.transform.SetParent(this.transform, false);
                nodo.nro = i;
                nodo.boton = null;
                listaPosiciones[i] = nodo;
            }
            pv=GetComponent<PhotonView>();
        }

        [PunRPC]
        public void CargarTareas()
        {
            //Debug.Log("tipo ="+ ArtefactoColumna.artefactos[0]+" atributos ="+ArtefactoColumna.artefactos[0].atributos);
            foreach (Tarea t in ArtefactoColumna.artefactos)
            {
                infoPos[] listaPos= listaPosiciones;
                var posy = listaPos[0].pos.y;
                for(int i=0;i<listaPos.Length;i++)
                {
                    if (!listaPos[i].ocupado)
                    {
                        GameObject BotonTarea = Instantiate(PrefabBotonTarea, listaPos[i].pos , PrefabBotonTarea.transform.rotation);
                        BotonTarea.transform.SetParent(transform, false);
                        listaPos[i].boton = BotonTarea;
                        listaPos[i].ocupado = true;
                        tareas.Add(BotonTarea);
                        TareaController auxtc = BotonTarea.GetComponent<TareaController>();
                        auxtc.setTarea((Tarea)(t)); 
                        auxtc.setPadre(ArtefactoColumna);
                        auxtc.setAtributos();
                    }
                }
            }
        }

        [PunRPC]
        public void editTitulo()
        {
            string nuevotitulo = transform.Find("InputFieldTitulo").GetComponent<InputField>().text;
            var exchangeName = "topic_logs";
            var routingKey = "Tablero";
            var message = "Se cambio el nombre de la columna \"" + Titulo.GetComponent<Text>().text +"\" a \""+nuevotitulo+"\""; 
            AmqpClient.Publish(exchangeName, routingKey, message);
            Titulo.GetComponent<Text>().text = nuevotitulo;
            ArtefactoColumna.setNombre(nuevotitulo);
        } 

        [PunRPC]
        public void setTitulo(string titulo)
        {
            Titulo.GetComponent<Text>().text = titulo;
            ArtefactoColumna.setNombre(Titulo.GetComponent<Text>().text);
        }

        [PunRPC]
        public void setPadre(ArtefactoContenedor ac)
        {
            padre = ac;
        }

        public ArtefactoContenedor getArtefactoColumna(){
            return ArtefactoColumna;
        }
        [PunRPC]
        public void setArtefactoContenedor(ArtefactoContenedor ac)
        {
            ArtefactoColumna = ac;
            ArtefactoColumna.setNombre(Titulo.GetComponent<Text>().text);
            CargarTareas();
        }

        [PunRPC]
        public void PVAniadir(){
            pv.RPC("AniadirTarea", RpcTarget.Others);
            AniadirTarea();
        }

        [PunRPC]
        public void AniadirTarea()
        {
            infoPos[] listaPos= listaPosiciones;
            var posy = listaPos[0].pos.y;
            for(int i=0;i<listaPos.Length;i++)
            {
                if (!listaPos[i].ocupado)
                {
                    GameObject BotonTarea = Instantiate(PrefabBotonTarea, listaPos[i].pos , PrefabBotonTarea.transform.rotation);
                    BotonTarea.transform.SetParent(transform, false);
                    listaPos[i].boton = BotonTarea;

                    //añadir Tarea al modelo
                    Tarea objetoTarea = new Tarea();
                    objetoTarea.setNombre("Nombre Tarea");
                    objetoTarea.setEstado(ArtefactoColumna);
                    ArtefactoColumna.addArtefacto(objetoTarea);

                    TareaController auxtc = BotonTarea.GetComponent<TareaController>(); 
                    auxtc.setPadre(ArtefactoColumna);
                    auxtc.setTarea(objetoTarea);
                    
                    listaPos[i].ocupado = true;
                    tareas.Add(BotonTarea);
                    break;
                }
            }
        }

    [PunRPC]
    public void eliminarTarea(GameObject tarea)
    {
        var exchangeName = "topic_logs";
        var routingKey = "Tablero";
        var message = "Se eliminó la tarea \""+ tarea.transform.Find("NombreTarea").GetComponent<Text>().text+"\" a la columna \"" + this.getNombre()+"\"";
        tareas.Remove(tarea);
    }

    [PunRPC]
    public void mostrarMenuEliminarBacklog()
    {
        MenuEliminar.SetActive(true);
        MenuEliminar.transform.SetAsLastSibling();
    }

    [PunRPC]
    public void esconderMenuEliminarBacklog()
    {
        MenuEliminar.SetActive(false);
    }

    [PunRPC]
    public void destroy()
    {
        foreach (GameObject tarea in tareas)
        {
            eliminarTarea(tarea);
        }
        var exchangeName = "topic_logs";
        var routingKey = "Tablero";
        var message = "Se eliminó la columna \""+ this.getNombre()+"\"";
        DestroyImmediate(this.transform.parent.gameObject,true);
    }
    
    [PunRPC]
    public List<GameObject> getTareas()
    {
        return tareas;
    }

    [PunRPC]
    public void Ordenar()
    {
        for (int i = 0; i<transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "SlotTarea(Clone)")
            {
                slots.Add(transform.GetChild(i).gameObject);
            }
        }
        switch(DropdownOrdenar.transform.Find("Label").GetComponent<Text>().text)
        {
            case "Prioridad": OrdenarPorPrioridad(); break;
            case "Vencimiento": OrdenarPorVencimiento(); break;
        }
    }

    [PunRPC]
    private void OrdenarPorVencimiento()
    {
        List<GameObject> auxtareas = new List<GameObject>();
        List<GameObject> tareassinfecha = new List<GameObject>();
        foreach (GameObject tarea in tareas)
        {
            TareaController tctarea = tarea.transform.GetComponent<TareaController>();
            int indice = 0;
            if (tctarea.extraerAtributoDeFecha("Año") != 0 && tctarea.extraerAtributoDeFecha("Mes") != 0 && tctarea.extraerAtributoDeFecha("Dia") != 0) 
            {
                for (int i = 0; i<auxtareas.Count; i++)
                {
                    TareaController tcaux = auxtareas.ElementAt(i).transform.GetComponent<TareaController>();
                    if (tctarea.extraerAtributoDeFecha("Año")  < tcaux.extraerAtributoDeFecha("Año") )
                    {
                        break;
                    }
                    else
                    {
                        if (tctarea.extraerAtributoDeFecha("Año")  == tcaux.extraerAtributoDeFecha("Año"))
                        {
                            if (tctarea.extraerAtributoDeFecha("Mes")  < tcaux.extraerAtributoDeFecha("Mes"))
                            {
                                break;
                            }
                            else
                            {
                                if (tctarea.extraerAtributoDeFecha("Mes")  < tcaux.extraerAtributoDeFecha("Mes"))
                                {
                                    if (tctarea.extraerAtributoDeFecha("Dia")  < tcaux.extraerAtributoDeFecha("Dia"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        indice++;
                                    }
                                }
                                else
                                {
                                    indice++;
                                }
                            }
                        }
                        else
                        {
                            indice++;
                        }
                    }
                }
                auxtareas.Insert(indice,tarea);
            }
            else
            {
                tareassinfecha.Add(tarea);
            }
        }
        for (int i=0; i<tareassinfecha.Count; i++)
        {
            auxtareas.Add(tareassinfecha.ElementAt(i));
        }

        for (int i = 0; i<auxtareas.Count; i++)
        {
            auxtareas.ElementAt(i).transform.position = slots.ElementAt(i).transform.position;
        }
        //CAMBIAR EL LABEL A ORDENAR
        //transform.Find("Label").GetComponent<Text>().text = "Ordenar";
    }

    [PunRPC]
    private void OrdenarPorPrioridad()
    {
        List<GameObject> auxtareas = new List<GameObject>();
        foreach (GameObject tarea in tareas)
        {
            int indice = 0;
            for (int i = 0; i<auxtareas.Count; i++)
            {
                if (tarea.transform.GetComponent<TareaController>().getPrioridad() > auxtareas.ElementAt(i).transform.GetComponent<TareaController>().getPrioridad())
                {
                    break;
                }
                else
                {
                    indice++;
                }
            }
            auxtareas.Insert(indice,tarea);
        }

        for (int i = 0; i<auxtareas.Count; i++)
        {
            auxtareas.ElementAt(i).transform.position = slots.ElementAt(i).transform.position;
        }
        //VOLVER TEXTO DEL LABEL A ORDENAR
        //transform.Find("Label").GetComponent<Text>().text = "Ordenar";
    }
    
    public infoPos[] getListaPosiciones()
    {
        return listaPosiciones;
    }

    public string getNombre()
    {
        return this.transform.Find("Titulo").GetComponent<Text>().text;
    }
}
