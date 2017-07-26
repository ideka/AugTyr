using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UserConfigHolder))]
public class GameDatabaseHolder : MonoBehaviour
{
    public static string Path { get { return Application.streamingAssetsPath + "/GameDatabase.json"; } }
    public static string URL {  get { return "https://api.guildwars2.com/v2"; } }

    public GameDatabase GameDatabase = new GameDatabase();

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }

    public UserConfigHolder UserConfigHolder { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        this.UserConfigHolder = this.GetComponent<UserConfigHolder>();

        this.StartCoroutine(this.Loading());
    }

    private IEnumerator Loading()
    {
        try
        {
            this.GameDatabase = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText(Path));
        }
        catch (FileNotFoundException)
        {
        }

        if (this.UserConfig.AutoUpdateGameDatabase)
            yield return this.UpdatingDatabase();

        SceneManager.LoadScene("Route");
    }

    private IEnumerator UpdatingDatabase()
    {
        using (APICall call = new APICall("build"))
        {
            yield return call.Requesting();

            // Bail early if the database is up-to-date.
            int build = (int)call.Data["id"];
            if (this.GameDatabase.BuildId == build)
                yield break;

            // Create a new database.
            this.GameDatabase = new GameDatabase()
            {
                BuildId = build
            };
        }

        using (APICall pMaps = new APICall("maps?ids=all"))
        using (APICall continents = new APICall("continents"))
        {
            Coroutine cMaps = this.StartCoroutine(pMaps.Requesting());
            yield return continents.Requesting();

            foreach (string continentId in continents.Data.Skip(0))
            {
                using (APICall floors = new APICall("continents", continentId, "floors?ids=all"))
                {
                    yield return floors.Requesting();

                    Dictionary<int, Map> maps = new Dictionary<int, Map>();
                    foreach (JContainer mapData in (from floor in floors.Data
                                                    from regions in floor["regions"]
                                                    from region in regions
                                                    from mapList in region["maps"]
                                                    select mapList).Values())
                    {
                        int floor = (int)mapData.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent["id"];
                        int mapId = (int)mapData["id"];
                        Map map;
                        if (!maps.TryGetValue(mapId, out map))
                        {
                            yield return cMaps;
                            string type = (string)pMaps.Data.First(m => (int)m["id"] == mapId)["type"];
                            map = new Map()
                            {
                                Name = (string)mapData["name"],

                                IsInstance = type == "Instance" || type == "Tutorial",
                                Rect = new Map.ContinentRect()
                                {
                                    ContinentId = int.Parse(continentId),
                                    Floors = new HashSet<int>() { floor },

                                    Rect1X = (int)mapData["continent_rect"][0][0],
                                    Rect1Y = (int)mapData["continent_rect"][0][1],
                                    Rect2X = (int)mapData["continent_rect"][1][0],
                                    Rect2Y = (int)mapData["continent_rect"][1][1]
                                }
                            };
                            maps[mapId] = map;
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

                    // Add all non-instances first.
                    foreach (KeyValuePair<int, Map> map in maps.Where(m => !m.Value.IsInstance).Concat(maps.Where(m => m.Value.IsInstance)))
                        this.GameDatabase.AddMap(map.Key, map.Value);
                }
            }
        }

        File.WriteAllText(Path, JsonConvert.SerializeObject(this.GameDatabase, Formatting.Indented));
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

        public IEnumerator Requesting()
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