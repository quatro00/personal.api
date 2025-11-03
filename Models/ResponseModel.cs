namespace personal.api.Models
{
    public class ResponseModel
    {
        public dynamic result { get; set; }
        public bool response { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public ResponseModel()
        {
            this.response = false;
            this.message = "Ocurrio un error inesperado";
        }

        public void SetResponse(bool r, string m = "")
        {
            this.response = r;
            this.message = m;

            /*
            if (m == "")
            {
                switch (r)
                {
                    case true:
                        this.message = "Datos guardados con éxito.";
                        break;
                    case false:
                        this.message = "Ocurrio un error inesperado";
                        break;
                }
            }
            */
        }
    }

    public class ResponseModel_2<T>
    {
        public T Result { get; set; }
        public bool Response { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public ResponseModel_2()
        {
            this.Response = false;
            this.Message = "Ocurrió un error inesperado";
        }

        public void SetResponse(bool r, string m = "")
        {
            this.Response = r;
            this.Message = m;

            //if (string.IsNullOrEmpty(m))
            //{
            //    this.Message = r ? "Datos guardados con éxito." : "Ocurrió un error inesperado";
            //}
        }
    }
}
