﻿using System.Net;

namespace CoarUtils.models.commands
{
    public class ResponseStatusModel
    {
        public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.BadRequest;
        public string status { get; set; }
    }
}
