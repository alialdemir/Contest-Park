using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Core.Service.Model
{
    public class MessageModel
    {
        #region Constructor

        public MessageModel(string message, Dictionary<string, string[]> modelState = null)
        {
            Message = message;
            ModelState = modelState;
        }

        #endregion Constructor

        #region Properties

        [JsonProperty(Order = 0)]
        public string Message { get; set; }

        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string[]> ModelState { get; set; }

        #endregion Properties

        #region ToString

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion ToString
    }
}