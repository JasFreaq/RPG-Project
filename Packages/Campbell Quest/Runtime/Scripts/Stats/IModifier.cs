using System.Collections;
using System.Collections.Generic;

namespace Campbell.Stats 
{
    public interface IModifier
    {
        IEnumerable<float> GetAdditive(Stat stat);

        IEnumerable<float> GetMultiplicative(Stat stat);
    }
}