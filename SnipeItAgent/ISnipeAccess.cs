using System.Collections.Generic;
using SnipeSharp.Endpoints.Models;

namespace SnipeItAgent
{
    public interface ISnipeAccess
    {
        T Create<T>(T item) where T : CommonEndpointModel;

        IEnumerable<T> Get<T>() where T : CommonEndpointModel;
        
        T Get<T>(long id) where T : CommonEndpointModel;

        T Get<T>(string name) where T : CommonEndpointModel;

        T Update<T>(T item) where T : CommonEndpointModel;
    }
}