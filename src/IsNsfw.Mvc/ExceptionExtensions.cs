using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Web;

namespace IsNsfw.Mvc
{
    public static class ExceptionExtensions
    {
        public static void ToModelStateError(this Exception ex, Controller c)
        {
            var responseStatus = ex.GetResponseStatus();
            var httpError = ex as IHttpError;

            if(responseStatus != null)
            {
                var errors = responseStatus.Errors;

                foreach(var error in errors)
                    c.ModelState.AddModelError(error.FieldName, error.Message);

                if(c.ModelState.IsValid)
                {
                    c.ModelState.AddModelError("", responseStatus.Message);
                }
            }
            else if(httpError != null)
            {
                c.ModelState.AddModelError("", httpError.Message);
            }
        }

        public static HttpError ForField(this HttpError ex, string fieldName, string message = null)
        {
            ex.Response = new ErrorResponse
            {
                ResponseStatus = new ResponseStatus
                {
                    ErrorCode = ex.ErrorCode,
                    Message = ex.Message,
                    Errors = new List<ResponseError> {
                        new ResponseError {
                            FieldName = fieldName,
                            Message = message.IsNullOrEmpty() ? ex.Message : message
                        }
                    }
                }
            };

            return ex;
        }
    }
}