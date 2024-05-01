using UnityEngine;

namespace GraphRewriting.Rules
{
    public abstract class Rule : ScriptableObject
    {
        public abstract bool IsEnforced(GraphCreator gc);
        public abstract void EnforceRule(GraphCreator gc);
    }
}