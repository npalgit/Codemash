﻿using System.Collections.Generic;
using Codemash.Phone.Data.Entities;

namespace Codemash.Phone.Shared.Grouping
{
    public interface IGroup
    {
        /// <summary>
        /// Groups the Session List by the implementation
        /// </summary>
        /// <returns></returns>
        IDictionary<string, IList<Session>> Group(IList<Session> sessionList);
    }
}
