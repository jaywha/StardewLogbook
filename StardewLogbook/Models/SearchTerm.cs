using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewLogbook.Models
{
    public record SearchTerm
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("type")]
        public SearchTermTypes TermType { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("owner")]
        public string Owner { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
