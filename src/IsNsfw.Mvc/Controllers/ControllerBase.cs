using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using ServiceStack.Mvc;
using ServiceStack.Web;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IsNsfw.Mvc.Controllers
{
    public abstract class ControllerBase : ServiceStackController
    {
        protected bool DoIfValid(Action action)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    action();

                    return ModelState.IsValid;
                }
                catch(Exception ex)
                {
                    if (ex is AggregateException)
                        ex = ex.InnerException;

                    var httpError      = ex as IHttpError;
                    var responseStatus = ex.GetResponseStatus();

                    if(httpError != null || responseStatus != null)
                    {
                        ex.ToModelStateError(this);
                    }
                    else
                        throw;
                }
            }

            return false;
        }

        protected T DoIfValid<T>(Func<T> ifValid, Func<T> failure)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var ret = ifValid();

                    if(ModelState.IsValid)
                        return ret;
                }
                catch(Exception ex)
                {
                    if (ex is AggregateException)
                        ex = ex.InnerException;

                    var httpError      = ex as IHttpError;
                    var responseStatus = ex.GetResponseStatus();

                    if(httpError != null || responseStatus != null)
                    {
                        ex.ToModelStateError(this);
                    }
                    else
                        throw;
                }

                return failure();
            }
            
            return failure();
        }

        protected void HandleExcepetion(Exception ex)
        {
            
        }

        //protected Task<T> DoIfValidAsync<T>(Func<Task<T>> ifValid, Func<Task<T>> failure)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var ret = ifValid();

        //            if(ModelState.IsValid)
        //                return ret;
        //        }
        //        catch(Exception ex)
        //        {
        //            var httpError      = ex as IHttpError;
        //            var responseStatus = ex.GetResponseStatus();

        //            if(httpError != null || responseStatus != null)
        //            {
        //                ex.ToModelStateError(this);
        //            }
        //            else
        //                throw;
        //        }

        //        return failure();
        //    }
            
        //    return failure();
        //}
    }
}
