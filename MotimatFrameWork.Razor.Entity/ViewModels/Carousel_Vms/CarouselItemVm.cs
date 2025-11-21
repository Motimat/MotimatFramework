using System.ComponentModel.DataAnnotations.Schema;

namespace MotimatFrameWork.Razor.Entity.ViewModels;

[NotMapped]
public class CarouselItemVm
{
    public int Index { get; set; }

    public string? Title { get; set; }

    public string? ImageAddress { get; set; }

    public string? LinkAddress  { get; set; }

    public CarouselVm? Carousel{ get; set; }
}