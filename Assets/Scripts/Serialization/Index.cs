using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using Newtonsoft.Json;

namespace kooltool.Serialization
{
    public interface IResource
    {
        void Save(Index index);
        void Load(Index index);
    }

    public class Index
    {
        public class File_
        {
            public Index index;
            public string path;

            public void Write(byte[] data)
            {
                index.Write(this, data);
            }

            public byte[] Read()
            {
                return index.Read(this);
            }
        }

        public List<IResource> resources = new List<IResource>();

        public void Add(IResource resource)
        {
            resources.Add(resource);
        }

        public File_ MFile_(string hint)
        {
            string path = Application.persistentDataPath;
            int id = Random.Range(0, 255);
            string local = string.Format("{0}-{1}", id, hint);

            return new File_ { index = this, path = local, };
        }

        [JsonIgnore]
        public Dictionary<Texture2D, Texture> textures
            = new Dictionary<Texture2D, Texture>();

        public Texture CreateTexture(int width, int height)
        {
            var texture = new Texture(this);

            texture.texture = BlankTexture.New(width, height, new Color(0, 0, 0, 0));

            Add(texture);

            return texture;
        }

        public Texture RefTexture(Texture2D texture, string suggestion)
        {
            if (textures.ContainsKey(texture))
            {
                return textures[texture];
            }
            else
            {
                var t = new Texture(this);
                t.texture = texture;

                textures.Add(texture, t);

                return t;
            }
        }

        public void Save(object data)
        {
            string path = Application.persistentDataPath;

            foreach (IResource resource in resources)
            {
                resource.Save(this);
            }

            File.WriteAllText(path + "/project.json", JsonWrapper.Serialise(data));
        }

        public void Write(File_ file, byte[] data)
        {
            string path = Application.persistentDataPath;

            File.WriteAllBytes(path+"/"+file.path, data);
        }

        public byte[] Read(File_ file)
        {
            string path = Application.persistentDataPath;

            return File.ReadAllBytes(path + "/" + file.path);
        }
    }
}
