using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using System;
using CymaticLabs.Unity3D.Amqp;

public class ScriptAbrirMenu : MonoBehaviour
{
    public bool isPaused;
    public bool isInfoPaused;
    public Canvas pauseMenu;
    public Canvas menuTablero;
    public Canvas menuGraficos;
    public Canvas menuDiagramas;
	public Canvas GraficoDeLineas;
    public Canvas crearReunion;
    public Canvas infoProyecto;
    public GameObject Panel;

    [SerializeField]
    PhotonView pv;


    // Start is called before the first frame update
    void Start()
    {
        
        pauseMenu.gameObject.SetActive(false);
        menuTablero.gameObject.SetActive(false);
        menuGraficos.gameObject.SetActive(false);
        menuDiagramas.gameObject.SetActive(false);
		GraficoDeLineas.gameObject.SetActive(false);
        crearReunion.gameObject.SetActive(false);
        infoProyecto.gameObject.SetActive(false);
        pv = GetComponent<PhotonView>();
        Debug.Log("I know who is :  " + pv);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneManager.LoadScene("EscenaMENU");
            if(isPaused)
            {
                pv.RPC("CerrarMenu", RpcTarget.AllViaServer);
                CerrarMenu();
            }
            else
            {
                pv.RPC("AbrirMenu", RpcTarget.AllViaServer);
                AbrirMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //SceneManager.LoadScene("EscenaMENU");
            if(isInfoPaused)
            {
                pv.RPC("CerrarInfoProyecto", RpcTarget.AllViaServer);
                CerrarInfoProyecto();
            }
            else
            {
                pv.RPC("AbrirInfoProyecto", RpcTarget.AllViaServer);
                AbrirInfoProyecto();
            }
        }
    }

    [PunRPC]
    public void AbrirInfoProyecto()
    {
        infoProyecto.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        isInfoPaused = true;
        infoProyecto.gameObject.SetActive(true);
        Panel.GetComponent<CargarInformacionProyecto>().updateArtefactos();
    }

    [PunRPC]
    public void CerrarInfoProyecto()
    {
        isInfoPaused = false;
        infoProyecto.gameObject.SetActive(false);
        Guardar();
    }

    [PunRPC]
    public void AbrirMenu(){
        pauseMenu.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        isPaused = true;
        pauseMenu.gameObject.SetActive(true);
    }

    [PunRPC]
    public void CerrarMenu(){
        isPaused = false;
        pauseMenu.gameObject.SetActive(false);
        Guardar();
    }

    [PunRPC]
    public void PVAbrirMenuTablero(){
        pv.RPC("AbrirMenuTablero", RpcTarget.AllViaServer);
        String message= String.Format("{{\"user_id\": \"{0}\", \"value\": \"{1}\"}}", Actor.actual.getId(), "Tablero");
        AmqpClient.Publish("topic_logs", "Recurso.Utilizado", message);
        AbrirMenuTablero();
    }

    [PunRPC]
    public void AbrirMenuTablero()
    {
        menuTablero.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        menuTablero.gameObject.SetActive(true);
        isPaused = false;
        pauseMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void PVCerrarMenuTablero(){
        pv.RPC("CerrarMenuTablero", RpcTarget.AllViaServer);
        CerrarMenuTablero();
    }

    [PunRPC]
    public void CerrarMenuTablero()
    {
        menuTablero.gameObject.SetActive(false);
        isPaused = true;
        pauseMenu.gameObject.SetActive(true);
        Guardar();
    }

    [PunRPC]
    public void PVAbrirMenuGraficos(){
        pv.RPC("AbrirMenuGraficos", RpcTarget.AllViaServer);
        AbrirMenuGraficos();
    }

    [PunRPC]
    public void AbrirMenuGraficos()
    {
        menuGraficos.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        menuGraficos.gameObject.SetActive(true);
        isPaused = false;
        pauseMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void PVCerrarMenuGraficos(){
        pv.RPC("CerrarMenuGraficos", RpcTarget.AllViaServer);
        CerrarMenuGraficos();
    }

    [PunRPC]
    public void CerrarMenuGraficos()
    {
        menuGraficos.gameObject.SetActive(false);
        isPaused = true;
        pauseMenu.gameObject.SetActive(true);
        Guardar();
    }

    [PunRPC]
    public void PVAbrirMenuDiagramas(){
        pv.RPC("AbrirMenuDiagramas", RpcTarget.AllViaServer);
        String message= String.Format("{{\"user_id\": \"{0}\", \"value\": \"{1}\"}}", Actor.actual.getId(), "Diagrama");
        AmqpClient.Publish("topic_logs", "Recurso.Utilizado", message);
        AbrirMenuDiagramas();
    }

    [PunRPC]
    public void AbrirMenuDiagramas()
    {
        menuDiagramas.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        menuDiagramas.gameObject.SetActive(true);
        isPaused = false;
        pauseMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void PVCerrarMenuDiagramas(){
        pv.RPC("CerrarMenuDiagramas", RpcTarget.AllViaServer);
        CerrarMenuDiagramas();
    }

    [PunRPC]
    public void CerrarMenuDiagramas()
    {
        menuDiagramas.gameObject.SetActive(false);
        isPaused = true;
        pauseMenu.gameObject.SetActive(true);
        Guardar();
    }

    [PunRPC]
	 public void PVAbrirGraficoDeLineas(){
        pv.RPC("AbrirGraficoDeLineas", RpcTarget.AllViaServer);
        String message= String.Format("{{\"user_id\": \"{0}\", \"value\": \"{1}\"}}", Actor.actual.getId(), "Grafico de lineas");
        AmqpClient.Publish("topic_logs", "Recurso.Utilizado", message);
        AbrirGraficoDeLineas();
     }
	
    [PunRPC]
	 public void AbrirGraficoDeLineas()
    {
        GraficoDeLineas.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        GraficoDeLineas.gameObject.SetActive(true);
        menuGraficos.gameObject.SetActive(false);
    }

    [PunRPC]
    public void PVCerrarGraficoDeLineas(){
        pv.RPC("CerrarGraficoDeLineas", RpcTarget.AllViaServer);
        CerrarGraficoDeLineas();
    }

    [PunRPC]
    public void CerrarGraficoDeLineas()
    {
        GraficoDeLineas.gameObject.SetActive(false);
        menuGraficos.gameObject.SetActive(true);
        Guardar();
    }
    
    [PunRPC]
    public void PVAbrirCrearReunion(){
        pv.RPC("AbrirCrearReunion",RpcTarget.AllViaServer);
        AbrirCrearReunion();
    }

    [PunRPC]
    public void AbrirCrearReunion(){
        crearReunion.worldCamera = GameObject.FindWithTag("cameraPlayer").GetComponent<Camera>();
        crearReunion.gameObject.SetActive(true);
        isPaused = false;
        pauseMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void PVCerrarCrearReunion(){
        pv.RPC("CerrarCrearReunion", RpcTarget.AllViaServer);
        CerrarCrearReunion();
        Guardar();
    }

    [PunRPC]
    public void CerrarCrearReunion()
    {
        crearReunion.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        Guardar();
    }


    public void Guardar()
    {
        Proyecto.ProyectoActual.Actualizar();
    }
}