﻿#region using
using System;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base class used to populate and set the correct configuration for the service.
    /// </summary>
    /// <typeparam name="M">The Microservice type.</typeparam>
    /// <typeparam name="C">The Configuration type.</typeparam>
    public abstract class PopulatorBase<M, C>: IPopulator
        where M : Microservice, new()
        where C : ConfigBase, new()
    {
        #region Declarations
        protected IList<ResourceProfile> mResourceProfiles = new List<ResourceProfile>();
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor that creates.
        /// </summary>
        protected PopulatorBase()
        {
            Service = new M();
            Service.StartRequested += ServiceStartRequested;
            Service.StartCompleted += ServiceStartCompleted;
            Service.StopRequested += ServiceStopRequested;
            Service.StopCompleted += ServiceStopCompleted;
            Service.StatisticsIssued += ServiceStatisticsIssued;
        }
        #endregion

        /// <summary>
        /// This method is called when the service issues new statistics.
        /// You can use this method to log specific events or metrics.
        /// </summary>
        /// <param name="service">The issuing service.</param>
        /// <param name="statistics">The statistics.</param>
        protected virtual void ServiceStatisticsProcess(M service, MicroserviceStatistics statistics) { }

        protected virtual void ServiceStartRequested(object sender, StartEventArgs e) { }

        protected virtual void ServiceStartCompleted(object sender, StartEventArgs e) { }

        protected virtual void ServiceStopCompleted(object sender, StopEventArgs e) { }

        protected virtual void ServiceStopRequested(object sender, StopEventArgs e) { }

        #region ServiceStatisticsIssued(object sender, StatisticsEventArgs e)
        /// <summary>
        /// This method is called when the service statistics are issued by the underlying service. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceStatisticsIssued(object sender, StatisticsEventArgs e)
        {
            try
            {
                ServiceStatisticsProcess(sender as M, e.Statistics);
            }
            catch (Exception)
            {
                //We do not throw exceptions here
            }
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// This method starts the underlying Microservice.
        /// </summary>
        public virtual void Start()
        {
            Service.Start();
        }
        /// <summary>
        /// This method stops the underlying Microservice.
        /// </summary>
        public virtual void Stop()
        {
            Service.Stop();
        }
        #endregion



        #region Service
        /// <summary>
        /// This is the Microservice.
        /// </summary>
        public Microservice Service { get; private set; } 
        #endregion
        /// <summary>
        /// This method is used to set the core Microservice settings.
        /// </summary>
        protected virtual void ServiceConfigure()
        {
        }

        #region Config
        /// <summary>
        /// This is the system configuration.
        /// </summary>
        public C Config { get; private set; } 
        #endregion
        #region ConfigInitiate(Func<string, string, string> resolver, bool resolverFirst)
        /// <summary>
        /// This method can be overriden to customise the config class.
        /// </summary>
        /// <param name="resolver">The resolver function used to set the key values from the appropriate store.</param>
        /// <param name="resolverFirst">A boolean property that determines whether the resolver is called first before falling back to the settings classes.</param>
        protected virtual void ConfigInitiate(Func<string, string, string> resolver, bool resolverFirst)
        {
            Config = new C();
            if (resolver != null)
            {
                Config.Resolver = resolver;
                Config.ResolverFirst = resolverFirst;
            }
        }
        #endregion

        #region Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        /// <summary>
        /// This is the main method used to populate.
        /// </summary>
        /// <param name="resolver">The settings resolver.</param>
        /// <param name="resolverFirst">A boolean property that determines whether the resolver is called first.</param>
        public virtual void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        {
            ConfigInitiate(resolver, resolverFirst);

            ServiceConfigure();

            RegisterBoundaryLogger();
            RegisterResourceProfiles();
            RegisterSerializers();
            RegisterTelemetry();
            RegisterLogging();
            RegisterEventSources();

            RegisterCommands();

            RegisterCommunication();
        } 
        #endregion


        protected virtual void RegisterBoundaryLogger()
        {

        }

        protected virtual void RegisterResourceProfiles()
        {

        }

        /// <summary>
        /// This method registers the serializers. By default the JsonContractSerializer is registered.
        /// Overload this method if you do not want to use this serializer.
        /// </summary>
        protected virtual void RegisterSerializers()
        {
            Service.RegisterPayloadSerializer(new JsonContractSerializer());
        }

        protected virtual void RegisterCommunication()
        {

        }

        protected virtual void RegisterCommands()
        {

        }

        protected virtual void RegisterTelemetry()
        {
        }

        protected virtual void RegisterLogging()
        {
        }

        protected virtual void RegisterEventSources()
        {

        }

    }
}
