using System.Collections;
using System.Collections.Generic;

namespace RPG.Stats 
{
    public interface IModifier
    {
        IEnumerable<float> GetAdditive(Stat stat);

        IEnumerable<float> GetMultiplicative(Stat stat);
    }
}