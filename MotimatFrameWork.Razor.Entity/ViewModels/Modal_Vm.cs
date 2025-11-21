using System.ComponentModel.DataAnnotations.Schema;

namespace MotimatFrameWork.Razor.Entity.ViewModels
{
    [NotMapped]
    public class Modal_Vm
    {
        public string? Title { get; set; }
        public Guid Guid { get; set; }
    }
}
