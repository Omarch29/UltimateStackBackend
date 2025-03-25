namespace REPM.Application.Filters;

public record PropertyFilters(string? Street,
    string? City,
    string? State,
    string? ZipCode,
    decimal? MinRent,
    decimal? MaxRent,
    int? MinBedrooms,
    int? MaxBedrooms,
    int? MinBathrooms,
    int? MaxBathrooms, 
    int? MinSquareFootage,
    int? MaxSquareFootage): IFilter
{
    
}