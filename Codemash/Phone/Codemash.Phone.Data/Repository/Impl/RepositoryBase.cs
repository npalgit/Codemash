﻿using System;
using System.Collections.Generic;
using System.Linq;
using Codemash.Phone.Data.Common;
using Codemash.Phone.Data.Context;
using Codemash.Phone.Data.Entities;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Codemash.Phone.Data.Repository.Impl
{
    public abstract class RepositoryBase<T> where T : EntityBase
    {
        private const string ConnectionString = @"isostore:/Codemash.sdf";

        private IList<T> _repository;
        protected IList<T> Repository
        {
            get { return _repository ?? (_repository = new List<T>()); }
        }

        public event EventHandler LoadCompleted;

        protected RepositoryBase()
        {
            // if the database does not exist, create it
            using (var db = new CodemashDataContext(ConnectionString))
            {
                if (!db.DatabaseExists())
                    db.CreateDatabase();
            }
        }

        protected CodemashDataContext GetContext()
        {
            return new CodemashDataContext(ConnectionString);
        }

        public void Load()
        {
            using (var db = new CodemashDataContext(ConnectionString))
            {
                _repository = db.GetTable<T>().ToList();
                if (_repository.Count == 0)
                {
                    var client = new RestClient();
                    var request = new RestRequest(DownloadUrl, Method.GET);

                    client.ExecuteAsync(request, LoadCompleteCallback);
                }
                else
                {
                    CleanRepository();
                    if (LoadCompleted != null)
                        LoadCompleted(this, new EventArgs());
                }
            }
        }

        private void LoadCompleteCallback(IRestResponse restResponse)
        {
            var jsonString = restResponse.Content;
            var jsonArray = JArray.Parse(jsonString);
            _repository = (from ja in jsonArray.AsJEnumerable()
                           select CreateObject(ja)).ToList();
            SaveChanges();

            if (LoadCompleted != null)
                LoadCompleted(this, new EventArgs());

        }

        public void SaveChanges()
        {
            Save();     // Repository specific Save method

            // Cleanup
            CleanRepository();
        }

        private void CleanRepository()
        {
            // remove any items that are marked as removed
            var removeList = _repository.Where(i => i.EntityState == EntityState.Removed).ToList();
            removeList.ForEach(l => _repository.Remove(l));

            // make sure we reset the flag for all items coming from the database
            foreach (var item in _repository)
                item.MarkAsClean();
        }

        // abstract properties
        protected abstract string DownloadUrl { get; }

        // abstract methods
        protected abstract T CreateObject(JToken jToken);
        protected abstract void Save();
    }
}
