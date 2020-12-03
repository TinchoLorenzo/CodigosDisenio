using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;

public class TareaController : MonoBehaviour
{
    [SerializeField] GameObject Subtareas;
    [SerializeField] GameObject Usuarios;
    [SerializeField] GameObject PanelTarea;
    [SerializeField] Text prioridad;
    [SerializeField] GameObject PrefabToggleUsuario;
    [SerializeField] GameObject PrefabToggleSubtarea;
    [SerializeField] Text Nombre;
    [SerializeField] Text Descripcion;
    [SerializeField] GameObject Fecha;
    [SerializeField] GameObject BotonCerrar;
    List<GameObject> subtareas;
    List<GameObject> usuarios;
    List<GameObject> toggles;
    ArtefactoContenedor padre;
    public Tarea tarea;
    bool isOpen;
    DateTime timeOpen;
    //interaccion con el modelo
    public void Awake()
    {
        isOpen=false;
        toggles = new List<GameObject>();
        subtareas = new List<GameObject>();
        usuarios = new List<GameObject>();
    }
    public void PanelTareaOpener()
    {
        isOpen=true;
        timeOpen=DateTime.Now;
        transform.SetAsLastSibling();
        transform.parent.parent.SetAsLastSibling();
        PanelTarea.SetActive(true);
        Subtareas.transform.GetComponent<Dropdown>().ClearOptions();
        GameObject[] paneles = GameObject.FindGameObjectsWithTag("PanelBacklog");
        List<string> nombrestareas = new List<string>(){"Subtareas"};
        List<GameObject> tareas = new List<GameObject>();
        foreach (GameObject panel in paneles)
        {
            tareas = panel.transform.GetComponent<ColumnController>().getTareas();
            foreach (GameObject tarea in tareas)
            {
                if (!GameObject.Equals(this.gameObject,tarea.gameObject))
                {
                    nombrestareas.Add(tarea.transform.Find("NombreTarea").GetComponent<Text>().text);
                }
            }
        }
        Subtareas.transform.GetComponent<Dropdown>().AddOptions(nombrestareas);
        BotonCerrar.SetActive(true);

        //OBTENER LOS USUARIOS PARA AÑADIR LAS OPCIONES AL DROPDOWN
        
        List<string> lista = new List<string>();
        foreach(Actor a in Proyecto.ProyectoActual.getActores()){
            lista.Add(a.getNombre());
        }
        Usuarios.GetComponent<Dropdown>().ClearOptions();
        Usuarios.GetComponent<Dropdown>().AddOptions(lista);
    }

    public void setTarea(Tarea t)
    {
        tarea = t;
    }

    public Tarea getTarea(){
        return tarea;
    }
    public void setPadre(ArtefactoContenedor ac)
    {
        padre = ac;
    }

    public void PanelTareaClose()
    {
        //cuando cierro el panel actualizo el modelo
        tarea.addAtributo("prioridad", prioridad.transform.GetComponent<Text>().text);
        if ((this.extraerAtributoDeFecha("Año") != 0) && (extraerAtributoDeFecha("Mes") != 0) && (extraerAtributoDeFecha("Dia") != 0))
        {
            tarea.addAtributo("fecha", new DateTime(Convert.ToInt32(this.extraerAtributoDeFecha("Año")), Convert.ToInt32(this.extraerAtributoDeFecha("Mes")), Convert.ToInt32(this.extraerAtributoDeFecha("Dia"))));
        }

        Debug.Log("salio");
        PanelTarea.SetActive(false);
        isOpen=false;
        String message= String.Format("{{\"user_id\": \"{0}\", \"value\": {1}}}", Actor.actual.getId(), (DateTime.Now - timeOpen).Seconds );
        AmqpClient.Publish("topic_logs", "TiempoLecturaUserStory", message);

    }

    public void setAtributos()
    {
        setNombre(tarea.getNombre());
        setDescripcion(tarea.getDescripcion());
        setPrioridad((String)(tarea.getValorAtributo("prioridad")));
        setVencimiento((DateTime)(tarea.getValorAtributo("fecha")));
        setUsuarios((String)(tarea.getValorAtributo("usuario")));
    }

    public void CambiarColorTarea()
    {
        ColorBlock cb = this.transform.GetComponent<Button>().colors;
        if (string.Equals(prioridad.GetComponent<Text>().text,"Alta"))
        {
            /*cb.normalColor = AltaPrioridad;
            cb.highlightedColor = AltaPrioridad;
            cb.pressedColor = AltaPrioridad;
            Debug.Log("cambiar color de tarea a rojo");
            this.transform.GetComponent<Button>().colors = cb;*/
        }
        else
        {
            if (string.Equals(prioridad.GetComponent<Text>().text,"Media"))
            {
                /*cb.normalColor = MediaPrioridad;
                cb.highlightedColor = MediaPrioridad;
                cb.pressedColor = MediaPrioridad;
                Debug.Log("cambiar color de tarea a amarillo");
                this.transform.GetComponent<Button>().colors = cb;*/
            }
            else
            {
                if (string.Equals(prioridad.GetComponent<Text>().text,"Baja"))
                {
                    /*cb.normalColor = BajaPrioridad;
                    cb.highlightedColor = BajaPrioridad;
                    cb.pressedColor = BajaPrioridad;
                    Debug.Log("cambiar color de tarea a celeste o verde");
                    this.transform.GetComponent<Button>().colors = cb;*/
                }
                else
                {
                    /*cb.normalColor = SinPrioridad;
                    cb.highlightedColor = SinPrioridad;
                    cb.pressedColor = SinPrioridad;
                    Debug.Log("Cambiar color de tarea a como si no tuviera color");
                    this.transform.GetComponent<Button>().colors = cb;*/
                }
            }
        }
    }

    public void AñadirUsuario()
    {
        GameObject PanelCampoUsuario = Usuarios.transform.parent.parent.gameObject;
        for (int i = 0; i<PanelCampoUsuario.transform.childCount; i++)
        {
            if (PanelCampoUsuario.transform.GetChild(i).name == "ToggleUsuario(Clone)")
            {
                if (usuarios.Contains(PanelCampoUsuario.transform.GetChild(i).gameObject))
                {
                    return;
                }
            }
        }
        GameObject ToggleUsuario = Instantiate(PrefabToggleUsuario, PrefabToggleUsuario.transform.localPosition, PrefabToggleUsuario.transform.rotation);
        ToggleUsuario.transform.SetParent(PanelCampoUsuario.transform,false);
        ToggleUsuario.transform.Find("Label").GetComponent<Text>().text = Usuarios.transform.Find("Label").GetComponent<Text>().text;
        usuarios.Add(ToggleUsuario);
        var exchangeName = "topic_logs";
        var routingKey = "Tablero";
        tarea.addAtributo("usuario", ToggleUsuario.transform.Find("Label").GetComponent<Text>().text);
        var message = "Se añadio el usuario \""+ ToggleUsuario.transform.Find("Label").GetComponent<Text>().text+"\" a la tarea \""+ this.getNombre()+"\"";
    }

    public void EliminarUsuario()
    {
        DestroyImmediate (Usuarios.transform.parent,true);
    }

    public void AñadirSubtarea()
    {
        GameObject PanelCampoSubtarea = Subtareas.transform.parent.parent.gameObject;
        for (int i = 0; i<PanelCampoSubtarea.transform.childCount; i++)
        {
            if (PanelCampoSubtarea.transform.GetChild(i).name == "ToggleSubtarea(Clone)")
            {
                if (subtareas.Contains(PanelCampoSubtarea.transform.GetChild(i).gameObject))
                {
                    return;
                }
            }
        }
        GameObject ToggleSubtarea = Instantiate(PrefabToggleSubtarea, PrefabToggleSubtarea.transform.localPosition, PrefabToggleSubtarea.transform.rotation);
        ToggleSubtarea.transform.SetParent(PanelCampoSubtarea.transform,false);
        ToggleSubtarea.transform.Find("Label").GetComponent<Text>().text = Subtareas.transform.Find("Label").GetComponent<Text>().text;
        subtareas.Add(ToggleSubtarea);
    }
    
    public void EliminarToggleSubtarea()
    {
        GameObject PanelCampoSubtarea = Subtareas.transform.parent.parent.gameObject;
        foreach (GameObject toggle in PanelCampoSubtarea.transform.Find("ToggleSubtarea(Clone)"))
        {
            if (!toggle.GetComponent<Toggle>().isOn)
            {
                subtareas.Remove(toggle);
                DestroyImmediate(toggle,true);
            }
        }
    }

    public void EliminarToggleUsuario()
    {
        GameObject PanelCampoSubtarea = Subtareas.transform.parent.parent.gameObject;
        foreach (GameObject toggle in PanelCampoSubtarea.transform.Find("ToggleSubtarea(Clone)"))
        {
            if (!toggle.GetComponent<Toggle>().isOn)
            {
                usuarios.Remove(toggle);
                var exchangeName = "topic_logs";
                var routingKey = "Tablero";
                var message = "El usuario \""+ toggle.transform.Find("Label").GetComponent<Text>().text+"\" fue removido de la tarea \""+ this.getNombre()+"\"";
                DestroyImmediate(toggle,true);
            }
        }
    }

    public void EditarNombre()
    {
        string nombre = Nombre.GetComponent<Text>().text;
        var exchangeName = "topic_logs";
        var routingKey = "Tablero";
        var message = "Se cambio el nombre de la tarea \""+ this.getNombre() +"\" a \""+ nombre +"\"";
        this.transform.Find("NombreTarea").GetComponent<Text>().text = nombre;
        //aviso al modelo
        tarea.setNombre(nombre);
    }

    public void editarDescripcion()
    {
        string desc = Descripcion.GetComponent<Text>().text;
        var exchangeName = "topic_logs";
        var routingKey = "Tablero";
        var message = "Se cambio la descripcion de la tarea \"" + this.getNombre() + "\" a \"" + desc + "\"";
        //aviso al modelo
        tarea.setDescripcion(desc);
    }

    public int getPrioridad()
    {
        int p = 0;
        string stringprioridad = prioridad.transform.GetComponent<Text>().text;  
        switch (stringprioridad)
        {
            case "": p = 1; break;
            case "Alta": p = 4; break;
            case "Media": p = 3; break;
            case "Baja": p = 2; break;
        }
        return p;
    }

    public GameObject getFecha()
    {
        return Fecha;
    }

    public int extraerAtributoDeFecha(string atrib)
    {
        int atributo = 0;
        string a = Fecha.transform.Find(atrib).Find("Label").gameObject.GetComponent<Text>().text;
        if (!string.Equals(a,atrib))
        {
            atributo = int.Parse(a);
        }
        return atributo;
    }

    public void setNombre(string nombre)
    {
        Nombre.GetComponent<Text>().text = nombre;
        this.transform.Find("NombreTarea").GetComponent<Text>().text = nombre;
    }

    public void setDescripcion(string descripcion)
    {
        Descripcion.GetComponent<Text>().text = descripcion;
    }

    public void setPrioridad(string p)
    {
        prioridad.transform.GetComponent<Text>().text = p;
    }

    public void setVencimiento(DateTime f)
    {
        if (Fecha != null)
        {
            Fecha.transform.Find("Año").Find("Label").gameObject.GetComponent<Text>().text = f.Year.ToString();
            Fecha.transform.Find("Mes").Find("Label").gameObject.GetComponent<Text>().text = f.Month.ToString();
            Fecha.transform.Find("Dia").Find("Label").gameObject.GetComponent<Text>().text = f.Day.ToString();
        }
    }

    public void setUsuarios(string usuario)
    {
        GameObject PanelCampoUsuario = Usuarios.transform.parent.parent.gameObject;
        GameObject ToggleUsuario = Instantiate(PrefabToggleUsuario, PrefabToggleUsuario.transform.localPosition, PrefabToggleUsuario.transform.rotation);
        ToggleUsuario.transform.SetParent(PanelCampoUsuario.transform,false);
        ToggleUsuario.transform.Find("Label").GetComponent<Text>().text = usuario;
        usuarios.Add(ToggleUsuario);
    }

    public void setSubtareas()
    {

    }

    public string getNombre()
    {
        return this.transform.Find("NombreTarea").GetComponent<Text>().text;
    }
}
