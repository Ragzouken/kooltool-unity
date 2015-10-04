using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;
using kooltool.Editor;

namespace kooltool
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private RectTransform layerParent;
        [SerializeField] private LayerView layerPrefab;

        public MonoBehaviourPooler<Layer, LayerView> layers;

        private void Awake()
        {
            layers = new MonoBehaviourPooler<Layer, LayerView>(layerPrefab,
                                                               layerParent,
                                                               InitialiseLayer);
        }

        private void InitialiseLayer(Layer layer, LayerView view)
        {
            view.SetLayer(layer);
        }

        public void SetWorld(World world)
        {
            layers.SetActive(world.layers);
        }
    }
}

