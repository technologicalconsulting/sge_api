namespace sge_api.Models.Requests
{
    public class VerifyCodeRequest
    {
        public string NumeroIdentificacion { get; set; } // Número de identificación del usuario
        public string Codigo { get; set; } // Código ingresado por el usuario
    }
}
