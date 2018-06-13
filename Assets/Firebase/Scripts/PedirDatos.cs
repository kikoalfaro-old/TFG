using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;

public class PedirDatos : MonoBehaviour {

    string URL = "https://firestore.googleapis.com/v1beta1/projects/tfg-kiko-web/databases/(default)/documents/Areas";
    ArrayList areas = new ArrayList();
    Text textoPantalla;

    IEnumerator Start()
    {
        using (WWW www = new WWW(URL))
        {
            yield return www;
            var content = JSON.Parse(www.text);
            for(int i = 0; i<content["documents"].Count; i++)
            {
                string name = content["documents"][i]["fields"]["nombre"]["stringValue"];
                double lat = content["documents"][i]["fields"]["latitud"]["doubleValue"];
                double lon = content["documents"][i]["fields"]["longitud"]["doubleValue"];
                double rad = content["documents"][i]["fields"]["radio"]["doubleValue"];
                Area a = new Area(name, lat, lon, rad);
                areas.Add(a);
            }
            setUIText();
        }
    }

    private void setUIText()
    {
        textoPantalla = GetComponent<Text>();
        string texto = "";
        
        foreach(Area a in areas)
        {
            texto += a + "\n\n";
        }

        textoPantalla.text = texto;
    }

    
}
