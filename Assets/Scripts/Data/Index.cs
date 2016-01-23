using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using Newtonsoft.Json;

namespace kooltool.Data
{
    public interface IResource
    {
        void Initialise(Context.IResourceIndex index);

        void Save(Index index);
        void Load(Index index);
    }

    public class Index
    {
        public int id;
        public Context context;

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

        public void Remove(IResource resource)
        {
            resources.Remove(resource);
        }

        public File_ MFile_(string hint)
        {
            string local = string.Format("{0:x4}-{1}", id++, hint);

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
            context.SetResource(texture.file.path, new Texture2DResource { texture = texture.texture, });

            return texture;
        }

        [JsonIgnore]
        public string folder;
        [JsonIgnore]
        public string root;
        [JsonIgnore]
        public string path
        {
            get
            {
                return (root ?? Application.persistentDataPath) + "/" + folder;
            }
        }

        public void Save(object data)
        {
            Directory.CreateDirectory(path);

            foreach (IResource resource in resources)
            {
                resource.Save(this);
            }

            File.WriteAllText(path + "/project.json", JsonWrapper.Serialise(data));
        }

        public IEnumerator SaveCO(object data, float chunk=0.01f)
        {
            Directory.CreateDirectory(path);

            foreach (IResource resource in resources)
            {
                resource.Save(this);

                //yield return null;
            }

            File.WriteAllText(path + "/project.json", JsonWrapper.Serialise(data));

            yield break;
        }

        public void Load()
        {
            foreach (IResource resource in resources)
            {
                resource.Load(this);
            }
        }

        public void Write(File_ file, byte[] data)
        {
            File.WriteAllBytes(path+"/"+file.path, data);
        }

        public byte[] Read(File_ file)
        {
            return File.ReadAllBytes(path + "/" + file.path);
        }


    }
}
