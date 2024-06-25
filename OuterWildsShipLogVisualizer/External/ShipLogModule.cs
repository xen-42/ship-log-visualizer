using Newtonsoft.Json;
using System.ComponentModel;

namespace OuterWildsShipLogVisualizer.External;

[JsonObject]
public class ShipLogModule
{
    /// <summary>
    /// Describe what this planet looks and like in map mode
    /// </summary>
    public MapModeInfo mapMode;

    /// <summary>
    /// A path to the folder where entry sprites are stored.
    /// </summary>
    public string spriteFolder;

    /// <summary>
    /// The relative path to the xml file to load ship log entries from.
    /// </summary>
    public string xmlFile;

    [JsonObject]
    public class MapModeInfo
    {
        /// <summary>
        /// Place non-selectable objects in map mode (like sand funnels).
        /// </summary>
        public ShipLogDetailInfo[] details;

        /// <summary>
        /// Hide the planet completely if unexplored instead of showing an outline.
        /// </summary>
        public bool invisibleWhenHidden;

        /// <summary>
        /// Specify where this planet is in terms of navigation.
        /// </summary>
        public MVector2 manualNavigationPosition;

        /// <summary>
        /// Manually place this planet at the specified position.
        /// </summary>
        public MVector2 manualPosition;

        /// <summary>
        /// Extra distance to apply to this object in map mode.
        /// </summary>
        public float offset;

        /// <summary>
        /// The path to the sprite to show when the planet is unexplored in map mode.
        /// </summary>
        public string outlineSprite;

        /// <summary>
        /// Completely remove this planet (and it's children) from map mode.
        /// </summary>
        public bool remove;

        /// <summary>
        /// The path to the sprite to show when the planet is revealed in map mode.
        /// </summary>
        public string revealedSprite;

        /// <summary>
        /// Scale to apply to the planet in map mode.
        /// </summary>
        [DefaultValue(1f)] public float scale = 1f;
    }

    [JsonObject]
    public class ShipLogDetailInfo
    {
        /// <summary>
        /// Whether to completely hide this detail when the parent AstroBody is unexplored.
        /// </summary>
        public bool invisibleWhenHidden;

        /// <summary>
        /// The sprite to show when the parent AstroBody is rumored/unexplored.
        /// </summary>
        public string outlineSprite;

        /// <summary>
        /// The position (relative to the parent) to place the detail.
        /// </summary>
        public MVector2 position;

        /// <summary>
        /// The sprite to show when the parent AstroBody is revealed.
        /// </summary>
        public string revealedSprite;

        /// <summary>
        /// The angle in degrees to rotate the detail.
        /// </summary>
        public float rotation;

        /// <summary>
        /// The amount to scale the x and y-axis of the detail by.
        /// </summary>
        public MVector2 scale;
    }
}
