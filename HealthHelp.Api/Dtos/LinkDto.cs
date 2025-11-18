namespace HealthHelp.Api.Dtos
{
    public class LinkDto
    {
        public string Href { get; set; }   // O link (URL)
        public string Rel { get; set; }    // A relação (ex: "self", "update")
        public string Method { get; set; } // O método HTTP (GET, PUT, DELETE)

        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}