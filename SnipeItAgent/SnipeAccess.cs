using System;
using System.Collections.Generic;
using System.Linq;
using SnipeSharp;
using SnipeSharp.Common;
using SnipeSharp.Endpoints;
using SnipeSharp.Endpoints.Models;
using SnipeSharp.Endpoints.SearchFilters;

namespace SnipeItAgent
{
    public class SnipeAccess : ISnipeAccess
    {
        private readonly Invalidatable<SnipeItApi> _lazyRemote;

        private Uri _uri;
        
        private string _apiToken;

        public SnipeAccess()
        {
            this._lazyRemote = new Invalidatable<SnipeItApi>(
                () =>
                {
                    var snipe = new SnipeItApi
                    {
                        ApiSettings =
                        {
                            ApiToken = this.ApiToken,
                            BaseUrl = new Uri(this.Uri, "/api/v1")
                        }
                    };

                    return snipe;
                });
        }

        public Uri Uri
        {
            get { return this._uri; }
            
            set
            {
                this._uri = value;
                this._lazyRemote.Invalidate();
            }
        }

        public string ApiToken
        {
            get { return this._apiToken; }

            set
            {
                this._apiToken = value;
                this._lazyRemote.Invalidate();
            }
        }

        public IEnumerable<T> Get<T>() where T : CommonEndpointModel
        {
            var manager = this._lazyRemote.Value.GetEndpointManager<T>();

            return manager.GetAll()?.Rows ?? new List<T>();
        }

        public T Get<T>(long id) where T : CommonEndpointModel
        {
            // This has to be fixed in SnipeSharp. IDs are longs, not ints.
            return this._lazyRemote.Value.GetEndpointManager<T>().Get((int) id);
        }

        public T Get<T>(string name)
            where T : CommonEndpointModel
        {
            var manager = this._lazyRemote.Value.GetEndpointManager<T>();

            var response = manager.FindAll(new SearchFilter {Search = name});

            if (response.Total == 0)
            {
                return null;
            }

            return response.Rows.FirstOrDefault(r =>
                string.Equals(r?.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public T Update<T>(T item) where T : CommonEndpointModel
        {
            var manager = this._lazyRemote.Value.GetEndpointManager<T>();

            var response = manager.Update(item);

            return ReloadItem(manager, response);
        }

        public T Create<T>(T item)
            where T : CommonEndpointModel
        {
            var manager = this._lazyRemote.Value.GetEndpointManager<T>();
            var response = manager.Create(item);

            return ReloadItem(manager, response);
        }

        private static T ReloadItem<T>(IEndpointManager<T> manager, IRequestResponse response)
            where T : CommonEndpointModel
        {
            if (response.Status == "success")
            {
                return manager.Get((int) response.Payload.Id);
            }

            // FIXME - an error occured. Handle appropriately
            return null;
        }
    }
}