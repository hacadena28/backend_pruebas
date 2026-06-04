namespace VehicleManagement.Domain.Common.Wrappers;


    public class Response<T>
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int? StatusCode { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public Response()
        {

        }
        public Response(int statusCode, string message, bool success)
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Success = success;
        }
        public Response(T data, string message = "")
        {
            this.Data = data;
            this.Message = message;
            this.Success = true;
        }

        public Response(int statusCode, string message = "")
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Success = false;
        }

        public Response(int statusCode, string message, bool success, T? data)
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Success = success;
            this.Data = data;
        }

        public Response(int statusCode, T? data)
        {
            this.StatusCode = statusCode;
            this.Success = true;
            if (data is not null)
            {
                this.Data = data;
            }

        }
    }

    public class ApiResponse<T>
    {

        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }

