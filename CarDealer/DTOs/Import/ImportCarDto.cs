namespace CarDealer.DTOs.Import;

using Newtonsoft.Json;

public class ImportCarDto
{
    [JsonProperty("make")]

    public string Make { get; set; } = null!;

    [JsonProperty("model")]

    public string Model { get; set; } = null!;

    [JsonProperty("traveledDistance")]
    
    public long TraveledDistance { get; set; }

    [JsonProperty("partsId")]

    public HashSet<int> PartsId { get; set; } = null!;
}
