using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Data
{
    public class Card
    {
        public class Trigger
        {
            public string name;
        }

        public class Switch
        {
            public string name;
        }

        public enum Quantifier
        {
            Any,
            All,
        }

        public Trigger trigger;
        public Quantifier quantifier;
        //public ICollection<Switch> 
    }
}
