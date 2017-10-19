using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Extensions
{
    internal static class DictionaryExtensions
    {
        public static Dictionary<string, string> WithErrorCode(this Dictionary<string, string> additionalInfo, string errorCode)
        {
            if (additionalInfo == null)
                additionalInfo = new Dictionary<string, string>();

            additionalInfo["ErrorCode"] = errorCode;
            return additionalInfo;
        }

        public static Dictionary<string, string> With(this Dictionary<string, string> additionalInfo, string key, string value)
        {
            if (additionalInfo == null)
                additionalInfo = new Dictionary<string, string>();

            additionalInfo[key] = value;
            return additionalInfo;
        }

        public static Dictionary<string, string> Copy(this Dictionary<string, string> additionalInfo)
        {
            if (additionalInfo == null)
                return null;

            return new Dictionary<string,string>(additionalInfo);
        }
    }
}
