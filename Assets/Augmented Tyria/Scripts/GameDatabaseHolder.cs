using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameDatabaseHolder : MonoBehaviour
{
    public static string Path { get { return Application.streamingAssetsPath + "/GameDatabase.json"; } }
    public static string URL {  get { return "https://api.guildwars2.com/v2"; } }

    public GameDatabase GameDatabase = new GameDatabase();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        try
        {
            this.GameDatabase = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText(Path));
        }
        catch (FileNotFoundException)
        {
        }

        this.StartCoroutine(this.UpdateDatabase());
    }

    private IEnumerator UpdateDatabase()
    {
        using (APICall call = new APICall("build"))
        {
            yield return call.Request();

            // Bail early if the database is up-to-date.
            int build = (int)call.Data["id"];
            if (this.GameDatabase.BuildID == build)
                yield break;

            // Create a new database.
            this.GameDatabase = new GameDatabase()
            {
                BuildID = build
            };
        }

        using (APICall call = new APICall("continents"))
        {
            yield return call.Request();
            foreach (string continentID in call.Data.Skip(0))
            {
                using (APICall cCall = new APICall("continents", continentID, "floors?ids=all"))
                {
                    yield return cCall.Request();

                    Dictionary<int, Map> maps = new Dictionary<int, Map>();
                    foreach (JContainer mapData in (from floor in cCall.Data
                                                    from regions in floor["regions"]
                                                    from region in regions
                                                    from mapList in region["maps"]
                                                    select mapList).Values())
                    {
                        int floor = (int)mapData.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent["id"];
                        Map map;
                        if (!maps.TryGetValue((int)mapData["id"], out map))
                        {
                            map = new Map()
                            {
                                ID = (int)mapData["id"],
                                Name = (string)mapData["name"],

                                Rect = new Map.ContinentRect()
                                {
                                    ContinentID = int.Parse(continentID),
                                    Floors = new HashSet<int>() { floor },

                                    Rect1X = (int)mapData["continent_rect"][0][0],
                                    Rect1Y = (int)mapData["continent_rect"][0][1],
                                    Rect2X = (int)mapData["continent_rect"][1][0],
                                    Rect2Y = (int)mapData["continent_rect"][1][1]
                                }
                            };
                            maps[map.ID] = map;
                        }
                        else
                        {
                            map.Rect.Floors.Add(floor);
                        }

                        foreach (JToken wpData in mapData["points_of_interest"].Values().Where(p => (string)p["type"] == "waypoint"))
                            map.Waypoints[(string)wpData["chat_link"]] = (string)wpData["name"];
                        foreach (JToken sectorData in mapData["sectors"].Values())
                            map.Sectors[(int)sectorData["id"]] = (string)sectorData["name"];
                    }

                    foreach (Map map in maps.Values)
                        this.GameDatabase.AddMap(map);
                }
            }
        }

        File.WriteAllText(Path, JsonConvert.SerializeObject(this.GameDatabase));
        Debug.Log("DONE!");
    }

    private class APICall : IDisposable
    {
        public JContainer Data;

        private string[] path;

        public APICall(params string[] path)
        {
            this.path = path;
        }

        public IEnumerator Request()
        {
            while (true)
            {
                WWW www = new WWW(string.Join("/", new string[] { URL, string.Join("/", this.path) }));
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    this.Data = JsonConvert.DeserializeObject<JContainer>(www.text);
                    yield break;
                }
                Debug.LogWarning(www.error);
            }
        }

        public void Dispose()
        {
            this.Data = null;
            this.path = null;
        }
    }
}