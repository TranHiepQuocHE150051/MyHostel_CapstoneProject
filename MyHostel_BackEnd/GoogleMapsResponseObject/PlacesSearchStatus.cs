using GoogleApi.Entities.Common.Converters;
using GoogleApi.Entities.Common.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MyHostel_BackEnd.GoogleMapsResponseObject
{
    [JsonConverter(typeof(EnumConverter<PlacesSearchStatus>))]
    public enum PlacesSearchStatus
    {
        [EnumMember(Value = "OK")]
        Ok,
        [EnumMember(Value = "ZERO_RESULTS")]
        ZeroResults,

        [EnumMember(Value = "OVER_QUERY_LIMIT")]
        OverQueryLimit,

        [EnumMember(Value = "REQUEST_DENIED")]
        RequestDenied,

        [EnumMember(Value = "INVALID_REQUEST")]
        InvalidRequest,

        [EnumMember(Value = "UNKNOWN_ERROR")]
        UnknownError,

        [EnumMember(Value = "OVER_DAILY_LIMIT")]
        OverDailyLimit,

    }
}
