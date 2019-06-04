﻿using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;

namespace ContestPark.Core.CosmosDb
{
    public class DocumentDbInitializer : IDocumentDbInitializer
    {
        /// <summary>
        /// Create new instance from documentClient
        /// </summary>
        /// <param name="dbConnection">Cosmos db connection infos</param>
        /// <param name="connectionPolicy"></param>
        /// <returns>New document client instance</returns>
        public IDocumentClient GetClient(DocumentDbConnection dbConnection, ConnectionPolicy connectionPolicy = null)
        {
            if (dbConnection == null)
                throw new ArgumentNullException(nameof(dbConnection));

            if (string.IsNullOrWhiteSpace(dbConnection.CosmosDbAuthKeyOrResourceToken))
                throw new ArgumentNullException(nameof(dbConnection.CosmosDbAuthKeyOrResourceToken));

            if (string.IsNullOrWhiteSpace(dbConnection.CosmosDbServiceEndpoint))
                throw new ArgumentNullException(nameof(dbConnection.CosmosDbServiceEndpoint));

            if (connectionPolicy == null)
            {
                connectionPolicy = new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Https,
                };
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = { new StringEnumConverter() }
            };

            var documentClient = new DocumentClient(new Uri(dbConnection.CosmosDbServiceEndpoint),
                                                    dbConnection.CosmosDbAuthKeyOrResourceToken,
                                                    serializerSettings,
                                                    connectionPolicy);

            return documentClient;
        }
    }
}