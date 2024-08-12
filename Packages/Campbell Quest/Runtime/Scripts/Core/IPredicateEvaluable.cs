using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Core
{
    public interface IPredicateEvaluable
    {
        bool? CheckCondition(Condition.PredicateType predicate, string[] parameters);
    }
}
