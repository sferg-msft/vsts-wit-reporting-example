﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace VSTS.WIT.Reporting.Providers
{
    public interface ISourceProvider
    {
        Task<IEnumerable<WorkItem>> GetWorkItems();

        bool HasMoreWorkItems();
    }
}
