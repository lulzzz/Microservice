﻿#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        public static MicroservicePipeline AddApplicationInsightsDataCollector(this MicroservicePipeline pipeline, ApplicationInsightsDataCollector collector)
        {
            pipeline.Service.RegisterDataCollector(collector);

            return pipeline;
        }

        public static MicroservicePipeline AddApplicationInsightsDataCollector<L>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, L> creator, Action<L> action = null)
            where L : ApplicationInsightsDataCollector
        {
            var collector = creator(pipeline.Configuration);

            action?.Invoke(collector);

            pipeline.Service.RegisterDataCollector(collector);

            return pipeline;
        }

        public static MicroservicePipeline AddApplicationInsightsDataCollector<L>(this MicroservicePipeline pipeline, Action<L> action = null)
            where L : ApplicationInsightsDataCollector, new()
        {
            var collector = new L();

            action?.Invoke(collector);

            pipeline.Service.RegisterDataCollector(collector);

            return pipeline;
        }
    }
}
