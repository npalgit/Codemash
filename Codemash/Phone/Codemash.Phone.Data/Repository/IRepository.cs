﻿using System;
using Codemash.Phone.Data.Entities;

namespace Codemash.Phone.Data.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Loads the repository from whatever backing data store is most appropriate
        /// </summary>
        void Load();

        /// <summary>
        /// Event to indicate the load of the repository data is complete
        /// </summary>
        event EventHandler LoadCompleted;

        /// <summary>
        /// Get an item from the Repository based on its int key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(long id);

        /// <summary>
        /// Add an item to the repository manually
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);
    }
}
