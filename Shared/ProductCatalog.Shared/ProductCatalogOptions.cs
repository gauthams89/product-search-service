namespace ProductCatalog.Shared
{
    public class ProductCatalogOptions
    {
        public ConnectionStringsOptions ConnectionStrings { get; set; } = new();
        public RedisOptions Redis { get; set; } = new();
        public ElasticOptions Elasticsearch { get; set; } = new();
        public KafkaOptions Kafka { get; set; } = new();
    }

    public class ConnectionStringsOptions
    {
        public string Postgres { get; set; } = string.Empty;
    }

    public class RedisOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class ElasticOptions
    {
        public string Uri { get; set; } = string.Empty;
        public string DefaultIndex { get; set; } = string.Empty;
        public string ProductSearchIndex { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class KafkaOptions
    {
        public string Brokers { get; set; } = string.Empty;
        public string ProductSaveTopic { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Auth Mechanisms (e.g., Plain, ScramSha256, Gssapi)
        public string SaslMechanism { get; set; } = "Plain";

        // Protocol (e.g., Plaintext, SaslPlaintext, SaslSsl)
        public string SecurityProtocol { get; set; } = "SaslPlaintext";
    }
}
