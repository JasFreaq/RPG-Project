using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Core
{
    [System.Serializable]
    public class Condition
    {
        #region Ancillary Data Types

        public enum PredicateType
        {
            None,
            HasQuest,
            CompletedObjective,
            CompletedQuest,
            HasItem
        }

        [System.Serializable]
        public struct Predicate
        {
            [SerializeField] private PredicateType _predicateType;
            [SerializeField] private bool _negate;
            [SerializeField] private string[] _parameters;

            public PredicateType PredicateType
            {
                get => _predicateType;
                set => _predicateType = value;
            }

            public bool Negate
            {
                get => _negate;
                set => _negate = value;
            }

            public string[] Parameters
            {
                get => _parameters;
                set => _parameters = value;
            }

            public bool Evaluate(IEnumerable<IPredicateEvaluable> evaluables)
            {
                foreach (IPredicateEvaluable evaluable in evaluables)
                {
                    bool? result = evaluable.CheckCondition(_predicateType, _parameters);

                    if (result != null)
                    {
                        if (result == _negate)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        [System.Serializable]
        public class Disjunction
        {
            [SerializeField] private List<Predicate> _or = new List<Predicate>();

            public List<Predicate> Or { get => _or; }

            public bool Evaluate(IEnumerable<IPredicateEvaluable> evaluables)
            {
                foreach (Predicate predicate in _or)
                {
                    if (predicate.Evaluate(evaluables))
                        return true;
                }

                return false;
            }
        }

        #endregion

        [SerializeField] private List<Disjunction> _and = new List<Disjunction>();

        public List<Disjunction> And { get => _and; }

        public bool Evaluate(IEnumerable<IPredicateEvaluable> evaluables)
        {
            foreach (Disjunction disjunction in _and)
            {
                if (!disjunction.Evaluate(evaluables))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            List<string> andParts = new List<string>();
            
            foreach (Disjunction disjunction in _and)
            {
                List<string> orParts = new List<string>();
                
                foreach (Predicate predicate in disjunction.Or)
                {
                    string predicateFunction = predicate.PredicateType switch
                    {
                        PredicateType.HasQuest => "has_quest",
                        PredicateType.CompletedObjective => "completed_objective",
                        PredicateType.CompletedQuest => "completed_quest",
                        PredicateType.HasItem => "has_item",
                        _ => string.Empty,
                    };
                    
                    string predicateString = $"{predicateFunction}('{string.Join("', '", predicate.Parameters)}')";
                    
                    if (predicate.Negate)
                    {
                        predicateString = $"not {predicateString}";
                    }

                    orParts.Add(predicateString);
                }
                
                andParts.Add(string.Join(" or ", orParts));
            }
            
            return string.Join(" and ", andParts);
        }
    }
}