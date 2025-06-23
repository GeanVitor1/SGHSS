namespace SGHSSVidaPlus.MVC.Models // Namespace atualizado
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}