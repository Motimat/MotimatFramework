using Mf.Razor.Entity.ViewModels;

namespace Mf.Razor.Entity.Interfaces;

public interface ICarouselService
{
    public Task<CarouselVm> GetCarousel(int? carouselId = 0);

    public Task<bool> ExecuteCarousel(CarouselVm carousel);

}