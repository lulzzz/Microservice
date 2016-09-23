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

namespace Xigadee
{
    /// <summary>
    /// This is the combined interface that allow specific components to centralise all the logging capabilities in to single class.
    /// </summary>
    public interface IDataCollector: IBoundaryLoggerComponent, ILogger, IEventSource, ITelemetry, IServiceOriginator
    {
        void DispatcherPayloadException(TransmissionPayload payload, Exception pex);

        void DispatcherPayloadUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason);

        void DispatcherPayloadIncoming(TransmissionPayload payload);

        void DispatcherPayloadComplete(TransmissionPayload payload, int delta, bool isSuccess);

        void MicroserviceStatisticsIssued(MicroserviceStatistics statistics);
    }

    public interface IDataCollectorComponent: IDataCollector
    {
        DataCollectionSupport Support { get; }

        bool IsSupported(DataCollectionSupport support);
    }
}
