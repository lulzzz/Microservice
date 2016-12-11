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
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Unity.WebApi;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static IMicroservice sExtensionService = null;

        static void ExtensionMicroserviceStart()
        {
            try
            {
                var pipeline = new MicroservicePipeline();

                IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
                IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;
                PersistenceInternalService<Guid, Blah> persistence = null;

                pipeline
                    .Inspect(msInspect: (s) => sExtensionService = s)
                    .AddLogger(new TraceEventLogger())
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn")
                        .AttachResourceProfile(new ResourceProfile("TrackIt"))
                        //.AppendBoundaryLogger(new MemoryBoundaryLogger(), (p,bl) => bLogger = bl)
                        .AttachPriorityPartition(0, 1)
                        .AttachAzureServiceBusQueueListener("Myqueue")
                        .AttachCommand(new PersistenceBlahMemory())
                        .AttachCommand(new PersistenceInternalService<Guid, Blah>(), assign:(c) => persistence = c, channelResponse: cpipeOut)
                        .CallOut((c) => cpipeIn = c)
                        .Revert()
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .AttachPriorityPartition(0, 1)
                        //.AppendBoundaryLogger(bLogger)
                        .CallOut((c) => cpipeOut = c)
                        .Revert();

                pipeline.Start();

                Guid cId = Guid.NewGuid();
                var result = persistence.Create(new Blah() { ContentId = cId, Message = "Hello", VersionId = Guid.NewGuid() }).Result;
                //Assert.IsTrue(result.IsSuccess);

                var result2 = persistence.Read(cId).Result;
                //Assert.IsTrue(result2.IsSuccess);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        static void ExtensionMicroserviceStop()
        {
            sExtensionService.Stop();
            sExtensionService = null;
        }
    }
}
