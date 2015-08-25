using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using Newtonsoft.Json;

namespace kooltool.Serialization
{
    public class Texture : IResource
    {
        [JsonIgnore]
        public UnityEngine.Texture2D texture;

        public Index.File_ file;

        private byte[] data
        {
            get
            {
                Assert.IsNotNull(texture, "No texture created yet!");

                return texture.EncodeToPNG();
            }

            set
            {
                Assert.IsNull(texture, "Texture already created!");

                texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.LoadImage(value);
                texture.filterMode = FilterMode.Point;
            }
        }

        public Texture() 
        {

        }

        public Texture(Index index)
        {
            file = index.MFile_("texture.png");
        }

        void IResource.Save(Index index)
        {
            file.Write(data);
        }

        void IResource.Load(Index index)
        {
            data = file.Read();
        }

        public static implicit operator UnityEngine.Texture2D(Texture texture)
        {
            return texture.texture;
        }
    }
}
