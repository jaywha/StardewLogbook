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
        private static string DBName;
        private static string DBCollection;

        public static void Init(string conn_string = MONGODB_CONNECTION_STRING, string owner = "DEFAULT",
            string db_name = "Rabren-Home-DB", string collection_name = "stardew_search_history") {
            Client = new MongoClient(conn_string);
            Owner = owner;
            DBName = db_name;
            DBCollection = collection_name;
        }

        public static List<SearchTerm> Read() => Client
            .GetDatabase(DBName)
            .GetCollection<SearchTerm>(DBCollection)
            .Find(term => term.Owner == Owner)
            .ToList();


        public static void Write(SearchTerm record)
        {
            Client
                .GetDatabase(DBName)
                .GetCollection<SearchTerm>(DBCollection)
                .ReplaceOne(
                    filter: x => x.Id.Equals(record!.Id),
                    replacement: record!,
                    options: new ReplaceOptions()
                    {
                        IsUpsert = true
                    }
                );
        }

        public static void Delete(SearchTerm record)
        {
            Client
                .GetDatabase(DBName)
                .GetCollection<SearchTerm>(DBCollection)
                .DeleteOne(
                    filter: x => x.Id.Equals(record!.Id)
                );
        }

        public static void WriteAll(List<SearchTerm> records)
        {
            foreach (var record in records)
            {
                Write(record);
            }
        }
    }
}
