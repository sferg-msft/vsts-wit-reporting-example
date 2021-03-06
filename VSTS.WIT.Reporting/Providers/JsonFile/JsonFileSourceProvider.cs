﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Newtonsoft.Json;

namespace VSTS.WIT.Reporting.Providers.JsonFile
{
    public class JsonFileSourceProvider : ISourceProvider
    {
        private string path;
        private IEnumerator<string> filesEnumerator;
        private bool hasMoreWorkItems = true;

        public JsonFileSourceProvider(JsonFileSourceProviderOptions providerOptions)
        {
            this.path = providerOptions.Path;
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItems()
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("Path does not exist");
            }

            var workItems = new List<WorkItem>();
            var batchSize = 200;
            var count = 0;
            var filesEnumerator = GetFilesEnumerator();
            while(count++ < batchSize && (hasMoreWorkItems = filesEnumerator.MoveNext()))
            {
                var file = filesEnumerator.Current;
                using (FileStream stream = new FileStream(file, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    var workItem = JsonConvert.DeserializeObject<WorkItem>(json);
                    workItems.Add(workItem);
                }
            }

            return workItems;
        }

        public bool HasMoreWorkItems()
        {
            return hasMoreWorkItems;
        }

        private IEnumerator<string> GetFilesEnumerator()
        {
            if (filesEnumerator == null)
            {
                this.filesEnumerator = Directory.EnumerateFiles($@"{this.path}").GetEnumerator();
            }

            return filesEnumerator;
        }
    }
}
