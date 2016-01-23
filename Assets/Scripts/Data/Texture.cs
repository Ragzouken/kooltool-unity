using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using Newtonsoft.Json;

namespace kooltool.Data
{
    public class Frame
    {
        public Texture texture;
        public Rect rect;
        public int pivotX, pivotY;

        public static implicit operator Sprite(Frame frame)
        {
            Sprite sprite = Sprite.Create(frame.texture,
                                          frame.rect,
                                          new Vector2(frame.pivotX / (float) frame.texture.texture.width, 
                                                      frame.pivotY / (float) frame.texture.texture.height),
                                          1,
                                          0U,
                                          SpriteMeshType.FullRect);

            return sprite;
        }

        public Frame BadClone(Texture texture)
        {
            texture.texture.SetPixels32(this.texture.texture.GetPixels32());
            texture.texture.Apply();

            return new Frame
            {
                texture = texture,
                rect = rect,
                pivotX = pivotX,
                pivotY = pivotY,
            };
        }
    }

    public class Flipbook
    {
        public string name;
        public string tag;
        public float period = 0.5f;
        public List<Frame> frames;
    }

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

        void IResource.Initialise(Context.IResourceIndex index)
        {
            texture = index.GetResource<Texture2DResource>(file.path).texture;
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
