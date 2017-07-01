using System.Collections.Generic;

public interface IWieghtedNode<T>
{
    IEnumerable<T> GetNeighboursList(bool flag);
    float WalkingCost(T current);
}