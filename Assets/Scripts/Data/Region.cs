using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace kooltool.Data
{
    public class Region
    {
        public string name;
        public Texture icon;
    }

    public class Regions
    {
        public HashSet<Region> regions = new HashSet<Region>();

        [JsonArray]
        public class TileToRegions : Dictionary<Tile, HashSet<Region>> { };
        [JsonArray]
        public class RegionToTiles : Dictionary<Region, HashSet<Tile>> { };

        public RegionToTiles regionToTiles = new RegionToTiles();
        public TileToRegions tileToRegions = new TileToRegions();

        public bool IsDefault(Tile tile, Region region)
        {
            return regionToTiles[region].Contains(tile);
        }

        public void AddDefault(Tile tile, Region region)
        {
            regionToTiles.GetDefault(region).Add(tile);
            tileToRegions.GetDefault(tile).Add(region);
        }

        public void RemoveDefault(Tile tile, Region region)
        {
            regionToTiles.GetDefault(region).Remove(tile);
            tileToRegions.GetDefault(tile).Remove(region);
        }

        public void Add(Region region)
        {
            regions.Add(region);
        }

        public void Remove(Region region)
        {
            foreach (Tile tile in regionToTiles.GetDefault(region))
            {
                tileToRegions[tile].Remove(region);
            }

            regionToTiles.Remove(region);

            regions.Remove(region);
        }
    }
}

public static class DictTest
{
    public static V GetDefault<K, V>(this IDictionary<K, V> dict, K key)
        where V : new()
    {
        V value;

        if (!dict.TryGetValue(key, out value))
        {
            value = new V();
            dict.Add(key, value);
        }

        return value;
    }
}
