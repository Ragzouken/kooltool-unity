using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Data
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

    public interface ICondition
    {
        bool Check();
    }

    public class Condition
    {
        public Quantifier quantifier;
        public ICollection<Switch> switches;
    }

    public class Action
    {

    }

    public class Card
    {
        public Trigger trigger;
        public Condition condition;
        public IList<Action> actions;
    }
}
