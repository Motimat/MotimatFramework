using System.ComponentModel.DataAnnotations.Schema;

namespace Mf.Razor.Entity.ViewModels
{
    [NotMapped]
    public class Modal_Vm
    {
        public string? Title { get; set; }
        public Guid Guid { get; set; }
    }
}
