namespace Infra.CrossCutting.UoW.Models
{
    public class CommandResponse
    {
        public CommandResponse(bool success = false) {
            Success = success;
        }

        public bool Success { get; }
    }
}
