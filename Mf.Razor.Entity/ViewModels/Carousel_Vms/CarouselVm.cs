using System.ComponentModel.DataAnnotations.Schema;

namespace Mf.Razor.Entity.ViewModels;

[NotMapped]
public class CarouselVm
{
    public int Height { get; set; }

    public int Width { get; set; }

    public List<CarouselItemVm>? Items { get; set; }
}