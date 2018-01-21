using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {



    public static class JSONUtlities
    {

        static void ListJson(JSONObject json, List<FBData> fbdata)
        {
            ListJSONObject("", json, fbdata);
        }

        static void ListObject(string parent, JSONObject data, List<FBData> fbdata)
        {
            if (data.IsObject) {
                ListJSONObject(parent, data, fbdata);
            } else if (data.IsArray) {
                ListJSONArray(parent, data, fbdata);
            } else {
                ListPrimitive(parent, data, fbdata);
            }
        }

        static void ListJSONObject(string parent, JSONObject json, List<FBData> fbdata)
        {
            
            var array = json.keys;
            foreach(var key in array)
            {
                var child = json[key];
                string childKey = parent;
                if(!(key.Contains("Name") || key.Contains("Position")))
                    childKey = string.IsNullOrEmpty(parent) ? key : parent + "/" + key;
                ListObject(childKey, child, fbdata);
            }
        }

        static void ListJSONArray(string parent, JSONObject json, List<FBData> fbdata)
        {
            for (int i = 0; i < json.keys.Count; i++)
            {
                var data = json[i];
                ListObject(parent + "[" + i + "]", data, fbdata);
            }
        }

        static void ListPrimitive(string parent, JSONObject obj, List<FBData> fbdata)
        {
            var fb = new FBData();
            fb.path = parent;
            fb.data = obj;
            fbdata.Add(fb);
        }

        public static List<FBData> ReturnPaths(string data)
        {
            JSONObject json = new JSONObject(data);
            var fbdata = new List<FBData>();
            ListJson(json,fbdata);
            return fbdata;
        }

    }
}
