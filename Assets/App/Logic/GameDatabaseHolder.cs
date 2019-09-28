using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(UserConfigHolder))]
public class GameDatabaseHolder : MonoBehaviour
{
    public static string Path { get { return Application.streamingAssetsPath + "/GameDatabase.json"; } }
    public const string URL = "https://api.guildwars2.com/v2";

    public GameDatabase GameDatabase = new GameDatabase();

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }
    public Console Console { get { return this.UserConfigHolder.Console; } }

    public UserConfigHolder UserConfigHolder { get; private set; }

    private void Awake()
    {
        this.UserConfigHolder = this.GetComponent<UserConfigHolder>();

        this.StartCoroutine(this.Loading());
    }

    private IEnumerator Loading()
    {
        if (File.Exists(Path))
            this.GameDatabase = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText(Path));

        if (this.UserConfig.AutoUpdateGameDatabase)
            yield return this.UpdatingDatabase();
        else
            this.Console.InfoFade("Skipping game database update check.");
    }

    private IEnumerator UpdatingDatabase()
    {
        var replacement = new GameDatabase();

        using (var call = new APICall("build"))
        {
            yield return call.Requesting();

            // Bail early if the database is up-to-date.
            int build = (int)call.Data["id"];
            if (this.GameDatabase.BuildId == build)
            {
                this.Console.InfoFade("Game database already up-to-date ({0}).", this.GameDatabase.BuildId);
                yield break;
            }

            this.Console.Info("Updating game database ({0} => {1}).", this.GameDatabase.BuildId, build);

            // Create a new database.
            replacement = new GameDatabase()
            {
                BuildId = build
            };
        }

        using (var pMaps = new APICall("maps?ids=all"))
        using (var continents = new APICall("continents"))
        {
            Coroutine cMaps = this.StartCoroutine(pMaps.Requesting());
            yield return continents.Requesting();

            foreach (string continentId in continents.Data.Skip(0))
            {
                using (var floors = new APICall("continents", continentId, "floors?ids=all"))
                {
                    yield return floors.Requesting();

                    var maps = new Dictionary<int, Map>();
                    foreach (JContainer mapData in (from floor in floors.Data
                                                    from regions in floor["regions"]
                                                    from region in regions
                                                    from mapList in region["maps"]
                                                    select mapList).Values())
                    {
                        int floor = (int)mapData.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent["id"];
                        int mapId = (int)mapData["id"];
                        if (!maps.TryGetValue(mapId, out Map map))
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

                        yield return null;
                    }

                    // Add all non-instances first.
                    foreach (KeyValuePair<int, Map> map in maps.OrderBy(m => m.Value.IsInstance))
                        replacement.AddMap(map.Key, map.Value);
                }
            }
        }

        this.GameDatabase = replacement;
        File.WriteAllText(Path, JsonConvert.SerializeObject(this.GameDatabase, Formatting.Indented));
        this.Console.Info("Game database updated.");
    }

    private class APICall : IDisposable
    {
        public JContainer Data;

        private readonly string _url;

        public const float CallAttemptInterval = 5;

        public APICall(params string[] path)
        {
            this._url = string.Join("/", URL, string.Join("/", path));
        }

        public IEnumerator Requesting()
        {
            while (true)
            {
                using (var request = UnityWebRequest.Get(this._url))
                {
                    yield return request.SendWebRequest();
                    if (request.error == null)
                    {
                        this.Data = JsonConvert.DeserializeObject<JContainer>(request.downloadHandler.text);
                        yield break;
                    }
                    Debug.LogWarning(request.error);
                    Debug.LogFormat("Reattempting in {0} seconds...", CallAttemptInterval);
                    yield return new WaitForSeconds(CallAttemptInterval);
                }
            }
        }

        public void Dispose()
        {
            this.Data = null;
        }
    }
}