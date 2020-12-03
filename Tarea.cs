using System;
using CymaticLabs.Unity3D.Amqp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tarea : ArtefactoContenedor
{
    public struct Estado{
        public ArtefactoContenedor contenedor;
        public bool estadoActual;
        public DateTime comienzo;
        public TimeSpan tiempoEstado;

        public Estado(ArtefactoContenedor t, bool e, DateTime c, TimeSpan ts){
            contenedor=t;
            estadoActual=e;
            comienzo=c;
            tiempoEstado=ts;
        }
    };    
    public static List<Estado> estados = new List<Estado>();
    public Dictionary<string, object> atributos;

    public Tarea()
    {
        atributos = new Dictionary<string, object>();
        atributos["Estados"] = new List<Estado>();
    }

   

    public void addAtributo(string clave, object valor)
    {
        atributos[clave] = valor;
    }

    public object getValorAtributo(string clave)
    {
        return atributos[clave];
    }

    public IReadOnlyCollection<string> getKeys()
    {
        return atributos.Keys;
    }

    public override void getNombres(List<string> listaResultado)
    {
        listaResultado.Add(this.getNombre());
        for (int i =0; i<artefactos.Count; i++)
        {
            artefactos[i].getNombres(listaResultado);
        }
    }

    public static void addEstado(ArtefactoContenedor contenedor){
        estados.Add(new Estado(contenedor, false, System.DateTime.MinValue, TimeSpan.Zero));
    }

    public void setEstado(ArtefactoContenedor contenedor){
        
        for(int i=0; i<estados.Count; i++){
            Estado e = estados[i];
            if(e.contenedor.getNombre().Equals(contenedor.getNombre())){ // Estado nuevo de la tarea
                e.comienzo=DateTime.Now;
                e.estadoActual=true;
                string actores_ids = "[";
                foreach(Actor a in actores){
                    actores_ids+= "\"" + a.getId() + "\", ";
                }
                if(actores_ids.Length-2 > 0)
                    actores_ids.Remove(actores_ids.Length - 2);
                actores_ids+="]";
                String message= String.Format("{{\"user_id\": \"{0}\", \"tarea_id\": \"{1}\", \"estado\": \"{2}\", \"participantes\": {3}}}", Actor.actual.getId(), this.getNombre(), e.contenedor.getNombre(), actores_ids);
                AmqpClient.Publish("topic_logs", "Tarea.Cambio.Estado", message);
            } else if(e.estadoActual==true){ // Estado viejo. Disparo evento para registrar el tiempo que lleva en dicho estado la tarea
                e.tiempoEstado= e.tiempoEstado + (DateTime.Now - e.comienzo);
                e.estadoActual=false;
                //Disparar evento de cambio de estado
            }
            estados[i]=e;
        }
    }

}
