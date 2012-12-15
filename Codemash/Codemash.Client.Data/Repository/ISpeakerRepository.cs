﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemash.Client.Data.Entities;

namespace Codemash.Client.Data.Repository
{
    public interface ISpeakerRepository : IRepository<Speaker>
    {
        /// <summary>
        /// Save the dirty speakers
        /// </summary>
        void Save();
    }
}
