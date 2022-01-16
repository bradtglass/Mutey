using System.Text.Json.Serialization;
using Q42.HueApi.ColorConverters;

namespace HueMicIndicator.Hue;

public record HueLightSetting(bool? On, [property: JsonConverter(typeof(RgbConverter))] RGBColor? Color);