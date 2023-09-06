using System;
using System.Collections.Generic;

namespace AMRC.FactoryPlus.ServiceClient.Constants
{

    public enum AppSubcomponents
    {
        Registration,
        Info,
        SparkplugAddress
    }

    public enum ClassTypes
    {
        Class,
        Device,
        Schema,
        App,
        Service
    }

    public enum SchemaTypes
    {
        Device_Information,
        Service
    }

    public enum ServiceTypes
    {
        Directory,
        ConfigDB,
        Registry,
        Authentication,
        CommandEscalation,
        MQTT
    }

    /// <summary>
    /// A helper class of Guids
    /// </summary>
    public static class UUIDs
    {
        /// <summary>
        /// The global FactoryPlus identifier
        /// </summary>
        const string FactoryPlus = "11ad7b32-1d32-4c4a-b0c9-fa049208939a";

        /// <summary>
        /// A Null Guid, likely deprecated. Use Guid.Empty
        /// </summary>
        const string Null = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// App Guids
        /// </summary>
        public static Dictionary<AppSubcomponents, Guid> App = new Dictionary<AppSubcomponents, Guid>()
        {
            {AppSubcomponents.Registration, new Guid("cb40bed5-49ad-4443-a7f5-08c75009da8f")},
            {AppSubcomponents.Info, new Guid("64a8bfa9-7772-45c4-9d1a-9e6290690957")},
            {AppSubcomponents.SparkplugAddress, new Guid("8e32801b-f35a-4cbf-a5c3-2af64d3debd7")},
        };

        /// <summary>
        /// Class Guids
        /// </summary>
        public static Dictionary<ClassTypes, Guid> Class = new Dictionary<ClassTypes, Guid>()
        {
            {ClassTypes.Class, new Guid("04a1c90d-2295-4cbe-b33a-74eded62cbf1")},
            {ClassTypes.Device, new Guid("18773d6d-a70d-443a-b29a-3f1583195290")},
            {ClassTypes.Schema, new Guid("83ee28d4-023e-4c2c-ab86-12c24e86372c")},
            {ClassTypes.App, new Guid("d319bd87-f42b-4b66-be4f-f82ff48b93f0")},
            {ClassTypes.Service, new Guid("265d481f-87a7-4f93-8fc6-53fa64dc11bb")},
        };

        /// <summary>
        /// Schema Guids
        /// </summary>
        public static Dictionary<SchemaTypes, Guid> Schema = new Dictionary<SchemaTypes, Guid>()
        {
            {SchemaTypes.Device_Information, new Guid("2dd093e9-1450-44c5-be8c-c0d78e48219b")},
            {SchemaTypes.Service, new Guid("05688a03-730e-4cda-9932-172e2c62e45c")}
        };

        /// <summary>
        /// Service Guids
        /// </summary>
        public static Dictionary<ServiceTypes, Guid> Service = new Dictionary<ServiceTypes, Guid>()
        {
            {ServiceTypes.Directory, new Guid("af4a1d66-e6f7-43c4-8a67-0fa3be2b1cf9")},
            {ServiceTypes.ConfigDB, new Guid("af15f175-78a0-4e05-97c0-2a0bb82b9f3b")},
            {ServiceTypes.Registry, new Guid("af15f175-78a0-4e05-97c0-2a0bb82b9f3b")},
            {ServiceTypes.Authentication, new Guid("cab2642a-f7d9-42e5-8845-8f35affe1fd4")},
            {ServiceTypes.CommandEscalation, new Guid("78ea7071-24ac-4916-8351-aa3e549d8ccd")},
            {ServiceTypes.MQTT, new Guid("feb27ba3-bd2c-4916-9269-79a61ebc4a47")},
        };
    }
}
