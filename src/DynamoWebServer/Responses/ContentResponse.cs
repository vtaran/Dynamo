﻿using Newtonsoft.Json;

namespace DynamoWebServer.Responses
{
    public class ContentResponse : Response
    {
        public string Message { get; set; }

        public override string GetResponse()
        {
            return JsonConvert.SerializeObject(Message);
        }
    }
}
