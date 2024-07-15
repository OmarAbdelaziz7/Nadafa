using Domain.Results;

namespace Domain.Wrappers
{
    public class ResponseHandler
    {
        public Response<T> Deleted<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Deleted Successfully"
            };
        }
        public Response<T> EmailVerified<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Email Verified Successfully"
            };
        }
        public Response<T> EmailSent<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Email Sent Successfully"
            };
        }
        public Response<T> PasswordUpdated<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Password Updated Successfully"
            };
        }
        public Response<T> Success<T>(T entity)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true
            };
        }
        public Response<T> Uploaded<T>(T entity)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Uploaded Successfully"
            };
        }
        public Response<T> Updated<T>(T entity, string Message = null!)
        {
            return new Response<T>()
            {
                Data = entity,
                Message = Message == null! ? "Updated Successfully" : Message,
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true
            };
        }
        public Response<T> Unauthorized<T>()
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Succeeded = true,
                Message = "UnAuthorized",
            };
        }
        public Response<T> BadRequest<T>(string Message = null!)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = Message == null! ? "Bad Request" : Message
            };
        }
        public Response<T> BadRequest<T>(List<string> Errors)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Errors = Errors
            };
        }
        public Response<T> NotFound<T>(string Message = null!)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = Message == null! ? "Not Found" : Message
            };
        }
        public Response<T> Created<T>(T entity, object Meta = null!)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.Created,
                Succeeded = true
            };
        }
    }
}
