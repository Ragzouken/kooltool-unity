using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace kooltool.Serialization
{
    public class Costume : IResource
    {
        public string name;
        public Serialization.Texture texture;

        [JsonIgnore]
        public Sprite sprite;

        void IResource.Load(Index index) { }
        void IResource.Save(Index index) { }

        public void TestInit()
        {
            if (sprite == null) 
            {
                sprite = Sprite.Create(texture.texture, 
                                       new UnityEngine.Rect(0, 0, 
                                                            texture.texture.width,
                                                            texture.texture.height), 
                                       Vector2.one * 0.5f, 1);
            }
        }
    }
}
