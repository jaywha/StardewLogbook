using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using StardewLogbook.Models;

namespace StardewLogbook.Controllers
{
    public static class MongoDBController
    {
        private const string MONGODB_CONNECTION_STRING 
            = "mongodb+srv://gcadmin:groovy4all@rhythm-gnome-db.5tddy.mongodb.net/test?authSource=admin&replicaSet=atlas-ehyha0-shard-0&readPreference=primary&ssl=true";
        private static MongoClient Client;
        private static string Owner;

        public static void Init(string conn_string = MONGODB_CONNECTION_STRING, string owner = "DEFAULT") {
            Client = new MongoClient(conn_string);
            Owner = owner;
        }

        public static IEnumerable<SearchTerm> Read(string db_name, string collection_name)
        {
            var results = Client
                .GetDatabase(db_name)
                .GetCollection<SearchTerm>(collection_name)
                .Find(term => term.Owner == Owner)
                .ToList();

            foreach (var result in results)
            {
                yield return result;
            }
        }

        public static void Write(string db_name, string collection_name, List<SearchTerm> records) {
            foreach (var record in records)
            {
                Client
                    .GetDatabase(db_name)
                    .GetCollection<SearchTerm>(collection_name)
                    .ReplaceOne(
                        filter: x => x.Id.Equals(record!.Id),
                        replacement: record!,
                        options: new ReplaceOptions()
                        {
                            IsUpsert = true
                        }
                    );
            }
        }
    }
}
