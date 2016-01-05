using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace kooltool.Data
{
    public class Costume
    {
        public string name;

        [JsonArray]
        public class States : Dictionary<string, Flipbook> { }

        public States flipbooks = new States();
        
        public Flipbook GetFlipbook(string id, string tag="")
        {
            string full = id + "." + tag;

            var real = flipbooks.Where(p => p.Key == full).Select(p => p.Value);
            var fall = flipbooks.Where(p => p.Key == id).Select(p => p.Value);

            return real.Count() > 0 ? real.First() : fall.FirstOrDefault();
        }

        public void SetFlipbook(string id, 
                                string tag, 
                                Flipbook flipbook)
        {
            string full = tag.Length > 0 ? id + "." + tag : id;

            flipbooks[full] = flipbook;
        }

        public void TestInit()
        {
        }
    }
}
