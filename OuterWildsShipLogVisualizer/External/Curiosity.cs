using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuterWildsShipLogVisualizer.External;

public class Curiosity
{
    [JsonProperty]
    public string id;

    [JsonProperty]
    public MColor color;

    [JsonProperty]
    public MColor highlightColor;
}
