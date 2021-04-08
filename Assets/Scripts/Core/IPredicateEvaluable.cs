using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IPredicateEvaluable
    {
        bool? CheckCondition(Condition.PredicateType predicate, string[] parameters);
    }
}
